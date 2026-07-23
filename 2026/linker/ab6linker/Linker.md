リンカー（Linker）を実際に作る具体的なアプローチを、「設計」「最小実装の手順」「C言語やPascal/ActiveBasicなどの擬似コード」を交えて徹底解説します。

リンカー作りのゴールは、「アセンブラが出力した `.obj`（COFF形式）を読み込み、メモリ配置を決定し、アドレスを書き換えて、Windowsが実行できる `.exe`（PE形式）を書き出すこと」です。

---

## 最小リンカーの全体処理フロー

自作リンカーが実行する処理は、大きく分けて**5つのフェーズ**に分かれます。

```
[ .obj ファイル ]
       │
       ▼
 1. COFFヘッダーの解析      │ .obj からデータとリロケーション情報を抽出
       │
       ▼
 2. メモリマップの決定      │ .text などのセクションの配置アドレス(RVA)を決める
       │
       ▼
 3. シンボル解決            │ _main や _ExitProcess の最終アドレスを決定
       │
       ▼
 4. リロケーション（補正）  │ マシン語内の仮アドレス(00 00 00 00)を本物に書き換える
       │
       ▼
 5. PEヘッダー付加と保存    │ DOS/PEヘッダーを先頭にくっつけて .exe として出力

```

---

## 段階的実装ステップ（最小のEXEを作るまで）

まずは「`ExitProcess(0)` だけを呼ぶ1つの `.obj` から `.exe` を作る」最小構成を目指します。

### ステップ1：必要な構造体（ヘッダー）を定義する

Windowsのバイナリ（COFF / PE）は、構造体の集合体です。まずはこれらをコード上で定義します。

```basic
' --- 1. COFF Header (OBJの先頭) ---
Type COFF_HEADER
    Machine              As Word   ' x86なら 0x014C
    NumberOfSections     As Word   ' セクション数 (.text など)
    TimeDateStamp        As DWord
    PointerToSymbolTable As DWord  ' シンボルテーブルの位置
    NumberOfSymbols      As DWord  ' シンボル数
    SizeOfOptionalHeader As Word
    Characteristics      As Word
End Type

' --- 2. Section Header (各セクションの情報) ---
Type SECTION_HEADER
    Name(7)               As Byte   ' ".text\0\0\0" など
    VirtualSize           As DWord  ' メモリ上のサイズ
    VirtualAddress        As DWord  ' RVA (相対仮想アドレス)
    SizeOfRawData         As DWord  ' ファイル上のサイズ
    PointerToRawData      As DWord  ' ファイル上の位置
    PointerToRelocations  As DWord  ' リロケーション位置
    PointerToLinenumbers  As DWord
    NumberOfRelocations   As Word   ' リロケーション数
    NumberOfLinenumbers  As Word
    Characteristics       As DWord  ' 実行/読み/書き 権限
End Type

' --- 3. COFF Relocation (書き換え指定情報) ---
Type COFF_RELOCATION
    VirtualAddress   As DWord  ' 補正対象のオフセット
    SymbolTableIndex As DWord  ' どのシンボルを参照しているか
    Type             As Word   ' REL32(相対) か DIR32(絶対) か
End Type

```

---

### ステップ2：メモリレイアウト（RVA / ImageBase）の決定

WindowsのEXEは、デフォルトで仮想メモリの **`0x00400000` (`ImageBase`)** にロードされます。
リンカーは、各セクションがメモリ上のどこに位置するか（RVA: 相対仮想アドレス）を計算します。

* **PE Header群:** RVA `0x00000000` 〜 (通常 `0x1000` バイト確保)
* **`.text` (コード):** RVA `0x00001000` に配置 $\rightarrow$ 実際の絶対アドレスは **`0x00401000`**
* **`.data` (データ):** RVA `0x00002000` に配置 $\rightarrow$ 実際の絶対アドレスは **`0x00402000`**

---

### ステップ3：アドレス解決と「リロケーション」の実装（最重要）

アセンブラが出力した `.obj` の時点では、例えば `call _ExitProcess` の呼び出し先アドレスは **`E8 00 00 00 00`** （ダミーの0）になっています。

リンカーは `.obj` 内の `COFF_RELOCATION` テーブルを見て、ここを本物のアドレスに上書き補正します。

#### x86の `call` 命令（REL32: 相対アドレス）の計算公式

`call` 命令のアドレス指定は、「現在の命令の**次の位置**から、目的地までの差分オフセット」で計算します。

$$\text{相対オフセット} = \text{目的地の絶対アドレス} - (\text{書き換え対象のアドレス} + 4)$$

```basic
' 擬似コード：リロケーション処理のコア部分
Sub ApplyRelocation(codeBuffer() As Byte, reloc As COFF_RELOCATION, targetAddr As DWord, currentRVA As DWord)
    Dim patchOffset As DWord
    Dim relativeOffset As Long
    
    ' 書き換えるべき場所（.text内の位置）
    patchOffset = reloc.VirtualAddress
    
    If reloc.Type = IMAGE_REL_I386_REL32 Then
        ' Relative 32-bit (call / jmp 命令など)
        ' 公式: 目的地 - (現在の書き換え位置のアドレス + 4バイト)
        relativeOffset = targetAddr - (IMAGE_BASE + currentRVA + patchOffset + 4)
        
        ' バッファのマシン語(4バイト)を計算結果で上書き
        MemoryCopy(VarPtr(codeBuffer(patchOffset)), VarPtr(relativeOffset), 4)
        
    ElseIf reloc.Type = IMAGE_REL_I386_DIR32 Then
        ' Absolute 32-bit (変数へのポインタなど)
        ' 公式: 目的地の絶対アドレスをそのまま書き込む
        MemoryCopy(VarPtr(codeBuffer(patchOffset)), VarPtr(targetAddr), 4)
    End If
End Sub

```

---

### ステップ4：PEヘッダーを構築して `.exe` に書き出す

補正が完了した `.text` バイナリの頭に、Windowsが実行に必要な **DOSヘッダー** と **PEヘッダー** をガッチャンコしてファイルに書き出します。

#### 生成する `.exe` ファイルの全体の並び

```
[ DOS Header ] (64 bytes)  ── "MZ" から始まるヘッダー
[ DOS Stub ]   (64 bytes)  ── "This program cannot be run in DOS mode"
[ PE Signature ] (4 bytes) ── "PE\0\0"
[ PE File Header ] (20 bytes)
[ PE Optional Header ] (224 bytes) ── エントリポイント(0x1000)や ImageBase(0x00400000) を指定
[ Section Headers ] (40 bytes × セクション数)
[ パディング (0埋め) ]      ── ファイルアライメント(512バイト)の倍数に揃える
───────────────────────────────────────────────────────────── (ここから RVA 0x1000)
[ .text セクションデータ ]  ── リロケーション補正済みのマシン語

```

---

## 開発を安全に進めるテクニック・裏技

リンカーの自作でハマりがちなポイント（デバッグ手法）です。

### 1. `dumpbin` や `x64dbg` をフル活用する

自分で吐き出した `.exe` が動かない（Windowsに拒否される）ときは、以下のツールで正常なEXEと比較します。

* **`dumpbin /headers test.exe`** （Visual Studio付属のツール。ヘッダーの崩れを指摘してくれる）
* **`x64dbg` / `OllyDbg**` （デバッガで開いて、リロケーション後の `call` アドレスが正しい場所を指しているか確認する）

### 2. 最初は Import Address Table (IAT) をサボる裏技

`ExitProcess` などの External API（DLL関数）を正しくリンクするには、本来 **.idata (インポートセクション)** という複雑なテーブルを自作する必要があります。

しかし、最初のテスト段階では、**`ExitProcess` のアドレスを仮で固定値にして直接 call する** か、**DLLを使わずに無限ループ (`jmp $`) だけのコード** を動かしてみると、IATの実装を後回しにしてリンカーの基本骨格（リロケーションとPE書き出し）を完成させることができます。

---

## リンカー自作でおすすめの学習ロードマップ

1. **1. COFF Dumpツールの作成:** 仕様の可視化.
NASMやGCCが出力した `.obj` をファイル入力し、COFFヘッダー、セクション名、シンボル名、リロケーション一覧をテキスト表示するツールを作る。（これで構造体の読み込みをマスター）


2. **2. ヘッダーテンプレート方式で EXE 化:** バイナリ合成の基礎.
正常に動く最小の `.exe` から PEヘッダー部分（先頭 0x1000 バイト）をバイナリ抽出しておき、自作プログラムで「抽出したPEヘッダー ＋ `.obj` の `.text` データ」を結合して動かしてみる。


3. **3. PEヘッダーの自前ビルドとリロケーション:** 動的生成.
PEヘッダーを構造体で組み立てるコードを書き、`call` 命令の REL32 相対アドレス計算処理を組み込む。


4. **4. IAT (.idata) の自動生成:** 完成.
`kernel32.dll` や `user32.dll` の関数名を解析し、インポートテーブルを自前で構築して `.exe` に埋め込む。


このロードマップで進めると、バイナリが1バイトずれて動かない時の原因究明が格段にしやすくなります！まずは `.obj` のダンパー（解析表示ツール）あたりから手を付けてみるのがおすすめですよ。

2. ヘッダーテンプレート方式で EXE 化について。どういう意味か詳しく説明

「ヘッダーテンプレート方式」とは、リンカーを自作する際の一番大きな壁である「複雑なPEヘッダーをコードでゼロから計算して組み立てる処理」を一旦スキップ（バイナリの使い回し）して、手軽にEXE化を成功させるための開発テクニック（ショートカット技）です。

自作リンカー開発の最初の一歩として、圧倒的におすすめの手段です。

1. なぜこの方式を使うのか？（背景）
Windowsの実行ファイル（PEフォーマット）は、先頭に非常に多くの設定項目（ヘッダー）を持っています。

┌─────────────────────────────────────────┐
│ 1. DOS Header & DOS Stub                │
├─────────────────────────────────────────┤
│ 2. PE Header / Optional Header          │ 
│    ・Entry Point (実行開始位置)         │ ゼロからコードで計算して
│    ・Image Base  (メモリ配置基準値)     │ 構造体を埋めるのは
│    ・Section Alignment (パディング設定) │ めちゃくちゃ大変！
│    ・Subsystem (Console / GUI) など     │
├─────────────────────────────────────────┤
│ 3. Section Headers (.text, .dataなど)   │
└─────────────────────────────────────────┘
┌─────────────────────────────────────────┐
│ 4. マシン語バイナリ (.text)             │ ← 本当に処理したいのはココだけ！
└─────────────────────────────────────────┘
最初からこれらすべてを構造体で計算して自力でバイナリ出力しようとすると、「たった1個のパラメータ計算ミスやパディングずれ」でWindowsに「有効なWin32アプリケーションではありません」と拒否され、どこが悪いのか原因特定が困難になります。

そこで、「すでに動くことが分かっている既存のEXEから、先頭のヘッダー部分だけを『型紙（テンプレート）』としてそのまま借りてしまおう！」 というのがヘッダーテンプレート方式です。

2. 具体的にどうやるのか？（仕組み）
作業は「① 型紙を作る準備」と「② 自作リンカーでの結合」の2つのステップに分かれます。

① 型紙（header_template.bin）の準備
NASMなどの既存ツールを使って、最小の .exe（例えば ExitProcess(0) を呼ぶだけの512バイト〜4096バイト程度のEXE）を1つ作ります。

バイナリエディタでそのEXEを開き、先頭からマシン語（.text）が始まる手前の区切り（通常は 0x00000200 や 0x00001000 バイト目まで）を切り取って、header_template.bin として保存します。

【既存の正常なEXE】
[ ヘッダー領域 (例: 0x0000〜0x01FF) ] ──＞ これを抽出して header_template.bin にする
[ マシン語領域 (例: 0x0200〜0x03FF) ] ──＞ これは捨てる
② 自作リンカープログラムでの処理
自作リンカーが行う処理は、「ファイルのコピペ＋ちょっとした上書き」だけになります。

[ header_template.bin ] ─┐
                         ├─＞ ガッチャンコ！ ─＞ [ output.exe ]
[ .obj の .text データ ] ─┘
自作リンカーコードのイメージ（擬似コード）
Basic
' 1. テンプレートヘッダーを読み込んで、そのまま出力ファイルに書く
Dim headerBuf() As Byte
headerBuf = FileReadAllBytes("header_template.bin")

' 2. .obj からマシン語 (.text セクションデータ) を抽出する
Dim objCodeBuf() As Byte
objCodeBuf = ExtractTextSectionFromObj("input.obj")

' 3. (必要に応じて) リロケーション補正をする
' ※ テンプレートの配置アドレスに合わせて call / jmp の差分アドレスだけ書き換える
ApplyRelocation(objCodeBuf)

' 4. ヘッダーとマシン語を結合して .exe を書き出す！
Dim hFile As Long
hFile = FileOpen("output.exe", "w")
FileWrite(hFile, headerBuf)    ' 先頭に型紙を書く
FileWrite(hFile, objCodeBuf)   ' その直後にマシン語を書く
FileClose(hFile)
3. この方式のメリットと制限
メリット
速攻で .exe が動く: ヘッダー構造体の計算ロジックを1行も書かずに、Windows上で実行可能なバイナリが生成できます。

デバッグが極めて容易: もし動かない場合、原因は「ヘッダーの不備」ではなく「マシン語（.text）のデータかリロケーションの計算間違い」だけに絞り込めるため、原因追及が何倍も楽になります。

制限（後で乗り越える壁）
柔軟性がない: コードのサイズがテンプレートで想定していたサイズ（セクションサイズ）を超えると動かなくなります。

セクションの追加ができない: 後から新しい .data や .rsrc（アイコン等）を追加したくなった時、テンプレートのヘッダー書き換えが必要になります。

まとめ
自作リンカー開発における「ヘッダーテンプレート方式」とは、いわば「プラモデルの土台だけ既製品を使って、上のフィギュア（マシン語）だけ自作して載せる」ようなアプローチです。

この方法で「自作の .obj から作ったマシン語がWindows上で実際に動く！」という感動をまず味わい、自信がついた後に、テンプレートの代わりに「ヘッダー構造体をコードで動的生成する処理」へと置き換えていくのが、最も挫折しにくいリンカー自作のベストプラクティスです。

header_template.binの作成方法

header_template.bin の作成は、バイナリエディタか標準コマンド/簡単なスクリプトを使って、正常に動く最小の .exe の先頭部分（ヘッダー領域）だけを切り出すことで作成できます。

具体的な手順と、切り出す範囲の決定方法を詳しく解説します。

1. 切り出す範囲の決定（ファイルアライメント）
PEファイル（.exe）は、ファイル上では 0x200（512バイト） または 0x1000（4096バイト） の倍数でセクションが配置されます。

最小のコンパイラやアセンブラ（NASMなど）が出力する最小EXEの場合、ヘッダー領域（DOSヘッダー、PEヘッダー、セクションヘッダー）は先頭から 0x0000 〜 0x01FF（最初の512バイト） に収まっていることがほとんどです。

そして、0x0200 から実際のコード（.text セクションのマシン語）が始まります。

[ファイルオフセット]
0x0000 ┌──────────────────────────────────────────┐
       │ DOS Header ("MZ...")                     │
       │ DOS Stub                                 │
       │ PE Header ("PE\0\0...")                  │
       │ Section Headers (.text ...)              │
       │ (残りは0埋め/パディング)                 │
0x01FF └──────────────────────────────────────────┘ ─── ここまで切り出す！(512 bytes)
0x0200 ┌──────────────────────────────────────────┐
       │ .text セクション (ここからマシン語開始)  │
       └──────────────────────────────────────────┘
つまり、「正常に動く最小のEXEファイルの先頭 512 バイト（0x200 バイト）」をそのまま保存したものが header_template.bin になります。

2. 具体的な作成手順（3パターン）
お好みの環境やツールに合わせて作成してください。

パターンA：NASM ＋ バイナリエディタを使う（直感的）
一番仕組みが分かりやすい方法です。

ベースとなるアセンブリ（base.asm）を用意

コード スニペット
global _main
extern _ExitProcess@4

section .text
_main:
    push 0
    call _ExitProcess@4
NASMとGoLink（またはGCC）で最小EXEを作る

Bash
nasm -f win32 base.asm -o base.obj
golink /console base.obj kernel32.dll
# -> base.exe が生成される
バイナリエディタ（Stirling や Stirling-GUI、xxd など）で base.exe を開く

アドレス 0x00000000 から 0x000001FF までの512バイトを選択・コピーする

新規ファイルに貼り付けて header_template.bin として保存する

パターンB：PowerShell コマンドで作る（Windows標準・高速）
バイナリエディタを使わず、Windows標準の PowerShell だけで一発で切り出すコマンドです。

上記の手順2で base.exe を作成した後、PowerShell で以下のコマンドを実行します。

PowerShell
# base.exe の先頭512バイトを抽出して header_template.bin に保存する
$bytes = [System.IO.File]::ReadAllBytes("base.exe")
[System.IO.File]::WriteAllBytes("header_template.bin", $bytes[0..511])
パターンC：ActiveBasic / Pascal コードで作る
将来的にリンカーを自作する言語（ActiveBasicなど）を使って切り出しツールを作る方法です。

Basic
' base.exe から先頭 512 バイトを切り出して header_template.bin を作成する
Dim hIn As Long, hOut As Long
Dim buf(511) As Byte

hIn = FileOpen("base.exe", "r")
If hIn <> 0 Then
    FileRead(hIn, VarPtr(buf(0)), 512)
    FileClose(hIn)
    
    hOut = FileOpen("header_template.bin", "w")
    If hOut <> 0 Then
        FileWrite(hOut, VarPtr(buf(0)), 512)
        FileClose(hOut)
        Print "header_template.bin の作成に成功しました (512 bytes)"
    End If
End If
3. 作成したテンプレートが正しいか確認する方法
バイナリエディタで header_template.bin を開いたとき、以下の特徴を持っていれば成功です。

先頭 2 バイト が ASCII 文字の MZ（4D 5A）になっている（DOSヘッダーの識別記号）。

0x00000080 〜 0x00000100 付近 に ASCII 文字の PE（50 45 00 00）がある（PEヘッダーの識別記号）。

ファイルサイズ がぴったり 512 バイト（または 0x1000 を採用した場合は 4096 バイト）になっている。

4. 自作リンカーでの合体方法
このテンプレートを用意できたら、自作リンカーが行う処理は以下のように非常にシンプルになります。

Basic
' 自作リンカーの出力処理
Sub OutputEXE(objMachineCode() As Byte, codeSize As Long)
    Dim hExe As Long
    Dim headerBuf(511) As Byte
    Dim hHeader As Long
    
    ' 1. テンプレートヘッダーを読み込む
    hHeader = FileOpen("header_template.bin", "r")
    FileRead(hHeader, VarPtr(headerBuf(0)), 512)
    FileClose(hHeader)
    
    ' 2. output.exe に書き出す
    hExe = FileOpen("output.exe", "w")
    
    ' (a) テンプレートヘッダーを書く (0x0000 - 0x01FF)
    FileWrite(hExe, VarPtr(headerBuf(0)), 512)
    
    ' (b) マシン語を書く (0x0200 - )
    FileWrite(hExe, VarPtr(objMachineCode(0)), codeSize)
    
    FileClose(hExe)
    Print "output.exe の生成が完了しました！"
End Sub
💡 ワンポイントアドバイス
テンプレートの .text セクションのアドレス設定（RVA 0x1000 や VirtualSize など）と、自作コードの読み込みアドレスが一致していれば、これで生成した output.exe がそのまま Windows 上で起動します！