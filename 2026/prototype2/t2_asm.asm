bits 32

; Phase2 assembler smoke (hand-written)
extern _ExitProcess@4

section .data
    dummy db 0

section .text
    global _main

_main:
    push ebp
    mov ebp, esp
    sub esp, 8
    mov [ebp-4], 1
    mov [ebp-8], 2
    mov eax, [ebp-8]
    mov ecx, 3
    imul eax, ecx
    mov ecx, eax
    mov eax, [ebp-4]
    add eax, ecx
    push eax
    call _ExitProcess@4
