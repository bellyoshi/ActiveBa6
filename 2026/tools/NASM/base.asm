global _main
extern _ExitProcess@4

section .text
_main:
    push 0
    call _ExitProcess@4