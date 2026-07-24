bits 32

extern _ExitProcess@4

global _main
section .text

_main:
    push 0              ; 第1引数: exit code = 0
    call _ExitProcess@4