bits 32

; Phase1 assembler smoke: mov / jmp / ret (after rebuild)
extern _ExitProcess@4

section .text
    global _main

_main:
    mov eax, 0
    jmp do_exit
    ret
do_exit:
    push eax
    call _ExitProcess@4
