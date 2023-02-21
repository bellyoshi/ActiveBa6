// ExeCreator.cpp : コンソール アプリケーションの
//                  エントリ ポイントを定義します。

#include <stdio.h>
#include <time.h>
#include <windows.h>
#include <stdlib.h>
#include <malloc.h>
#include <tchar.h>

#define IMAGE_SIZEOF_NT_OPTIONAL32_HEADER 224

#define ALIGNMENT        0x1000 //ファイルおよびメモリにおけるアライメント
#define EXE_HEADER_SIZE  0x1000 //EXEヘッダの総合サイズ（アライメント考慮）

//ネイティブコード（コードセクションに保存されるバイナリ）
BYTE NativeBuffer[] = {
    0x6A,0x00,                         //push 0
    0x68,  0x0D,0x30,0x40,0x00,        //push dword ptr["exetest"]
    0x68,  0x00,0x30,0x40,0x00,        //push dword ptr["Hello World!"]
    0x6A,0x00,                         //push 0
    0xFF,0x15,    0x4E,0x20,0x40,0x00, //call MessageBoxA
    0xC3                               //ret 0
};

//インポートセクションに保存されるデータ
#define USE_DLL 1
IMAGE_IMPORT_DESCRIPTOR ImportDesc[USE_DLL + 1];
char szDllName[16] = "user32.dll";
DWORD LookupTable[2];
char HintTable[] = {
    0,0,
    'M','e','s','s','a','g','e','B','o','x','A',0
};

//初期済みデータ（データセクションに保存されるバイナリ）
BYTE InitBuffer[] = "Hello World!\0exe test\0\0";

//NULL空間
char szNullSpace[ALIGNMENT];

int _tmain(int argc, _TCHAR* argv[])
{
    /////////////////////////////
    // 必要なデータを初期化
    /////////////////////////////

    //コードセッションのサイズ
    int SizeOf_CodeSection;
    if (sizeof(NativeBuffer) % ALIGNMENT)
        SizeOf_CodeSection = sizeof(NativeBuffer)
        + (ALIGNMENT - sizeof(NativeBuffer) % ALIGNMENT);
    else SizeOf_CodeSection = sizeof(NativeBuffer);

    //インポートセクションのサイズ
    int SizeOf_ImportSection;
    SizeOf_ImportSection = ALIGNMENT;

    //データセクションのサイズ
    int SizeOf_DataSection;
    if (sizeof(InitBuffer) % ALIGNMENT)
        SizeOf_DataSection = sizeof(InitBuffer)
        + (ALIGNMENT - sizeof(InitBuffer) % ALIGNMENT);
    else SizeOf_DataSection = sizeof(InitBuffer);

    //コードセクションの開始位置
    int Pos_CodeSection = EXE_HEADER_SIZE;

    //インポートセクションの開始位置
    int Pos_ImportSection;
    Pos_ImportSection = Pos_CodeSection +
        SizeOf_CodeSection;

    //データセクションの開始位置
    int Pos_DataSection;
    Pos_DataSection = Pos_CodeSection +
        SizeOf_CodeSection +
        SizeOf_ImportSection;

    //インポートディレクトリテーブルを初期化
    ImportDesc[0].OriginalFirstThunk =
        Pos_ImportSection +
        (USE_DLL + 1) * sizeof(IMAGE_IMPORT_DESCRIPTOR) +
        sizeof(szDllName);
    ImportDesc[0].TimeDateStamp = 0;
    ImportDesc[0].ForwarderChain = 0;
    ImportDesc[0].Name =
        Pos_ImportSection +
        (USE_DLL + 1) * sizeof(IMAGE_IMPORT_DESCRIPTOR);
    ImportDesc[0].FirstThunk =
        ImportDesc[0].OriginalFirstThunk +
        sizeof(LookupTable) +
        sizeof(HintTable);

    //ルックアップテーブルを初期化
    LookupTable[0] = Pos_ImportSection +
        (USE_DLL + 1) * sizeof(IMAGE_IMPORT_DESCRIPTOR) +
        sizeof(szDllName) +
        sizeof(LookupTable);
    ////////////////////////////
// EXEファイルのヘッダ情報
////////////////////////////

    IMAGE_DOS_HEADER ImageDosHeader;
    ImageDosHeader.e_magic = 0x5A4D;
    ImageDosHeader.e_cblp = 0x0090;
    ImageDosHeader.e_cp = 0x0003;
    ImageDosHeader.e_crlc = 0;
    ImageDosHeader.e_cparhdr = 4;
    ImageDosHeader.e_minalloc = 0x0000;
    ImageDosHeader.e_maxalloc = 0xFFFF;
    ImageDosHeader.e_ss = 0x0000;
    ImageDosHeader.e_sp = 0x00B8;
    ImageDosHeader.e_csum = 0x0000;
    ImageDosHeader.e_ip = 0x0000;
    ImageDosHeader.e_cs = 0x0000;
    ImageDosHeader.e_lfarlc = 0x0040;
    ImageDosHeader.e_ovno = 0x0000;
    ImageDosHeader.e_res[0] = 0;
    ImageDosHeader.e_res[1] = 0;
    ImageDosHeader.e_res[2] = 0;
    ImageDosHeader.e_res[3] = 0;
    ImageDosHeader.e_oemid = 0x0000;
    ImageDosHeader.e_oeminfo = 0x0000;
    ImageDosHeader.e_res2[0] = 0;
    ImageDosHeader.e_res2[1] = 0;
    ImageDosHeader.e_res2[2] = 0;
    ImageDosHeader.e_res2[3] = 0;
    ImageDosHeader.e_res2[4] = 0;
    ImageDosHeader.e_res2[5] = 0;
    ImageDosHeader.e_res2[6] = 0;
    ImageDosHeader.e_res2[7] = 0;
    ImageDosHeader.e_res2[8] = 0;
    ImageDosHeader.e_res2[9] = 0;
    ImageDosHeader.e_lfanew = 0x0100; //PEヘッダの位置
    /////////////////////////////////////////////
// PEヘッダ
/////////////////////////////////////////////

    IMAGE_NT_HEADERS ImagePeHdr;
    ImagePeHdr.Signature = IMAGE_NT_SIGNATURE;

    //マシンタイプ
    ImagePeHdr.FileHeader.Machine = IMAGE_FILE_MACHINE_I386;

    ImagePeHdr.FileHeader.NumberOfSections = 3; //セクション数
    ImagePeHdr.FileHeader.TimeDateStamp = (DWORD)time(NULL);
    ImagePeHdr.FileHeader.PointerToSymbolTable = 0x00000000;
    ImagePeHdr.FileHeader.NumberOfSymbols = 0x00000000;
    ImagePeHdr.FileHeader.SizeOfOptionalHeader = 
        IMAGE_SIZEOF_NT_OPTIONAL32_HEADER;
    ImagePeHdr.FileHeader.Characteristics = IMAGE_FILE_EXECUTABLE_IMAGE |
        IMAGE_FILE_32BIT_MACHINE |
        IMAGE_FILE_LINE_NUMS_STRIPPED |
        IMAGE_FILE_LOCAL_SYMS_STRIPPED;
    ImagePeHdr.OptionalHeader.Magic = 0x010B;
    ImagePeHdr.OptionalHeader.MajorLinkerVersion = 1;
    ImagePeHdr.OptionalHeader.MinorLinkerVersion = 0;
    //コードサイズ（.textのセッションサイズ）
    ImagePeHdr.OptionalHeader.SizeOfCode = sizeof(NativeBuffer);
    //データサイズ（.dataのセッションサイズ）
    ImagePeHdr.OptionalHeader.SizeOfInitializedData = sizeof(InitBuffer);
    //未初期化データのサイズ（なし）
    ImagePeHdr.OptionalHeader.SizeOfUninitializedData = 0;
    ImagePeHdr.OptionalHeader.AddressOfEntryPoint = Pos_CodeSection;
    //.textのアドレス
    ImagePeHdr.OptionalHeader.BaseOfCode = Pos_CodeSection;
    //.dataのアドレス
    ImagePeHdr.OptionalHeader.BaseOfData = Pos_DataSection;

    //イメージベース
    ImagePeHdr.OptionalHeader.ImageBase = 0x00400000;
    //セクションアラインメント
    ImagePeHdr.OptionalHeader.SectionAlignment = ALIGNMENT;
    ImagePeHdr.OptionalHeader.FileAlignment = ALIGNMENT;
    ImagePeHdr.OptionalHeader.MajorOperatingSystemVersion = 4;
    ImagePeHdr.OptionalHeader.MinorOperatingSystemVersion = 0;
    ImagePeHdr.OptionalHeader.MajorImageVersion = 0;
    ImagePeHdr.OptionalHeader.MinorImageVersion = 0;
    ImagePeHdr.OptionalHeader.MajorSubsystemVersion = 4;
    ImagePeHdr.OptionalHeader.MinorSubsystemVersion = 0;
    ImagePeHdr.OptionalHeader.Win32VersionValue = 0;
    //すべてのイメージサイズ
    ImagePeHdr.OptionalHeader.SizeOfImage = EXE_HEADER_SIZE +
        SizeOf_CodeSection +
        SizeOf_ImportSection +
        SizeOf_DataSection;
    //ヘッダサイズ
    ImagePeHdr.OptionalHeader.SizeOfHeaders = EXE_HEADER_SIZE;
    ImagePeHdr.OptionalHeader.CheckSum = 0;
    ImagePeHdr.OptionalHeader.Subsystem = IMAGE_SUBSYSTEM_WINDOWS_GUI;
    ImagePeHdr.OptionalHeader.DllCharacteristics = 0;
    ImagePeHdr.OptionalHeader.SizeOfStackReserve = 0x00100000;
    ImagePeHdr.OptionalHeader.SizeOfStackCommit = 0x00001000;
    ImagePeHdr.OptionalHeader.SizeOfHeapReserve = 0x00100000;
    ImagePeHdr.OptionalHeader.SizeOfHeapCommit = 0x00001000;
    ImagePeHdr.OptionalHeader.LoaderFlags = 0;
    ImagePeHdr.OptionalHeader.NumberOfRvaAndSizes =
        IMAGE_NUMBEROF_DIRECTORY_ENTRIES;

    //データ ディクショナリ
    ImagePeHdr.OptionalHeader.DataDirectory[0].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[0].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[1].VirtualAddress =
        Pos_ImportSection; //インポートテーブル
    ImagePeHdr.OptionalHeader.DataDirectory[1].Size = SizeOf_ImportSection;
    ImagePeHdr.OptionalHeader.DataDirectory[2].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[2].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[3].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[3].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[4].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[4].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[5].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[5].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[6].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[6].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[7].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[7].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[8].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[8].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[9].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[9].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[10].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[10].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[11].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[11].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[12].VirtualAddress =
        Pos_ImportSection +
        (USE_DLL + 1) * sizeof(IMAGE_IMPORT_DESCRIPTOR) +
        16 * USE_DLL +           //DLL名
        sizeof(LookupTable) +  //ルックアップテーブル
        sizeof(HintTable);    //ヒント名（関数名）テーブル
    ImagePeHdr.OptionalHeader.DataDirectory[12].Size = sizeof(LookupTable);
    ImagePeHdr.OptionalHeader.DataDirectory[13].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[13].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[14].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[14].Size = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[15].VirtualAddress = 0;
    ImagePeHdr.OptionalHeader.DataDirectory[15].Size = 0;

    //コードセクションヘッダ
    IMAGE_SECTION_HEADER CodeSectionHeader;
    memset((char*)CodeSectionHeader.Name, 0, IMAGE_SIZEOF_SHORT_NAME);
    lstrcpy((LPWSTR)CodeSectionHeader.Name, _T(".text"));
    //メモリ上のサイズ
    CodeSectionHeader.Misc.VirtualSize = SizeOf_CodeSection;
    //メモリ上の開始アドレス
    CodeSectionHeader.VirtualAddress = Pos_CodeSection;
    //ファイル上のサイズ
    CodeSectionHeader.SizeOfRawData = sizeof(NativeBuffer);
    //ファイル上の開始アドレス
    CodeSectionHeader.PointerToRawData = Pos_CodeSection;
    CodeSectionHeader.PointerToRelocations = 0;
    CodeSectionHeader.PointerToLinenumbers = 0;
    CodeSectionHeader.NumberOfRelocations = 0;
    CodeSectionHeader.NumberOfLinenumbers = 0;
    CodeSectionHeader.Characteristics = IMAGE_SCN_MEM_EXECUTE |
        IMAGE_SCN_MEM_READ |
        IMAGE_SCN_CNT_CODE;

    //インポートセクションヘッダ
    IMAGE_SECTION_HEADER ImportSectionHeader;
    memset((char*)ImportSectionHeader.Name, 0, IMAGE_SIZEOF_SHORT_NAME);
    lstrcpy((LPWSTR)ImportSectionHeader.Name, _T(".idata"));
    ImportSectionHeader.Misc.VirtualSize = SizeOf_ImportSection;
    ImportSectionHeader.VirtualAddress = Pos_ImportSection;
    ImportSectionHeader.SizeOfRawData = SizeOf_ImportSection;
    ImportSectionHeader.PointerToRawData = Pos_ImportSection;
    ImportSectionHeader.PointerToRelocations = 0;
    ImportSectionHeader.PointerToLinenumbers = 0;
    ImportSectionHeader.NumberOfRelocations = 0;
    ImportSectionHeader.NumberOfLinenumbers = 0;
    ImportSectionHeader.Characteristics = IMAGE_SCN_CNT_INITIALIZED_DATA |
        IMAGE_SCN_MEM_READ;

    //データセクションヘッダ
    IMAGE_SECTION_HEADER DataSectionHeader;
    memset((char*)DataSectionHeader.Name, 0, IMAGE_SIZEOF_SHORT_NAME);
    lstrcpy((LPWSTR)DataSectionHeader.Name, _T(".sdata"));
    DataSectionHeader.Misc.VirtualSize = SizeOf_DataSection;
    DataSectionHeader.VirtualAddress = Pos_DataSection;
    DataSectionHeader.SizeOfRawData = SizeOf_DataSection;
    DataSectionHeader.PointerToRawData = Pos_DataSection;
    DataSectionHeader.PointerToRelocations = 0;
    DataSectionHeader.PointerToLinenumbers = 0;
    DataSectionHeader.NumberOfRelocations = 0;
    DataSectionHeader.NumberOfLinenumbers = 0;
    DataSectionHeader.Characteristics = IMAGE_SCN_CNT_INITIALIZED_DATA |
        IMAGE_SCN_MEM_READ |
        IMAGE_SCN_MEM_WRITE;

    HANDLE hFile;
    DWORD dwAccessBytes;
    hFile = CreateFile(_T(".\\test.exe"), GENERIC_WRITE, 0, NULL, CREATE_ALWAYS,
        FILE_ATTRIBUTE_NORMAL, NULL);
    if (hFile == INVALID_HANDLE_VALUE) {
        printf("ファイルの保存に失敗", "エラー");
        return 0;
    }

    //ヘッダ
    int iFileOffset;
    WriteFile(hFile, (void*)&ImageDosHeader, sizeof(IMAGE_DOS_HEADER),
        &dwAccessBytes, NULL);
    iFileOffset = dwAccessBytes;

    //MS-DOSスタブは省略

    //0x0100までNULLを並べる
    WriteFile(hFile, szNullSpace, 0x0100 - iFileOffset, &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //PEヘッダ
    WriteFile(hFile, &ImagePeHdr, sizeof(IMAGE_NT_HEADERS),
        &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //コード セクション ヘッダ
    WriteFile(hFile, &CodeSectionHeader, sizeof(IMAGE_SECTION_HEADER),
        &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //インポート セクション ヘッダ
    WriteFile(hFile, &ImportSectionHeader, sizeof(IMAGE_SECTION_HEADER),
        &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //データ セクション ヘッダ
    WriteFile(hFile, &DataSectionHeader, sizeof(IMAGE_SECTION_HEADER),
        &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //EXE_HEADER_SIZEまでNULLを並べる
    WriteFile(hFile, szNullSpace, EXE_HEADER_SIZE - iFileOffset,
        &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //コード
    WriteFile(hFile, NativeBuffer, sizeof(NativeBuffer),
        &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //Pos_ImportSectionまでNULLを並べる
    WriteFile(hFile, szNullSpace, Pos_ImportSection - iFileOffset,
        &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //インポート ディレクトリ テーブル（Nullディレクトリ テーブルを含む）
    WriteFile(hFile, ImportDesc, (USE_DLL + 1) * sizeof(
        IMAGE_IMPORT_DESCRIPTOR), &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //DLL名
    WriteFile(hFile, szDllName, 16, &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //ルックアップ テーブル
    WriteFile(hFile, LookupTable, sizeof(LookupTable), &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //ヒント テーブル
    WriteFile(hFile, HintTable, sizeof(HintTable), &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //インポート アドレス テーブル
    WriteFile(hFile, LookupTable, sizeof(LookupTable), &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //Pos_DataSectionまでNULLを並べる
    WriteFile(hFile, szNullSpace, Pos_DataSection - iFileOffset,
        &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //データ テーブル
    WriteFile(hFile, InitBuffer, sizeof(InitBuffer), &dwAccessBytes, NULL);
    iFileOffset += dwAccessBytes;

    //ファイルアラインメントを考慮
    if (iFileOffset % ALIGNMENT) {
        WriteFile(hFile, szNullSpace, ALIGNMENT - iFileOffset % ALIGNMENT,
            &dwAccessBytes, NULL);
        iFileOffset += dwAccessBytes;
    }

    //書き込み終了
    CloseHandle(hFile);

    printf("EXEファイルの生成が無事に完了しました。");

    return 0;
}

