bits 32

; Phase2 assembler smoke via [ebx]
extern _ExitProcess@4

section .data
    dummy db 0

section .text
    global _main

_main:
    push ebp
    mov ebp, esp
    sub esp, 8
    mov eax, 1
    mov ebx, ebp
    add ebx, -4
    mov [ebx], eax
    mov eax, 2
    mov ebx, ebp
    add ebx, -8
    mov [ebx], eax
    mov ebx, ebp
    add ebx, -8
    mov eax, [ebx]
    mov ecx, 3
    imul eax, ecx
    mov ecx, eax
    mov ebx, ebp
    add ebx, -4
    mov eax, [ebx]
    add eax, ecx
    push eax
    call _ExitProcess@4
