bits 32

; 外部関数の宣言（Windows API）
; 32ビットAPIは先頭にアンダースコア `_` がつく場合が多いです
extern _MessageBoxA@16
extern _ExitProcess@4

section .data
    title_text db 'Hello', 0
    msg_text   db 'Hello World!', 0

section .text
    global _main

_main:
    ; --- MessageBoxA(hWnd, lpText, lpCaption, uType) の呼び出し ---
    ; 引数は右から順（逆順）に push します
    push    0               ; uType = MB_OK (0)
    push    title_text      ; lpCaption = "Hello"
    push    msg_text        ; lpText = "Hello World!"
    push    0               ; hWnd = NULL (0)
    call    _MessageBoxA@16

    ; --- ExitProcess(uExitCode) の呼び出し ---
    push    0               ; uExitCode = 0
    call    _ExitProcess@4