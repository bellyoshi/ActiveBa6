grammar Hello;
@parser::header {#pragma warning disable 3021}
@lexer::jeader {#pragma warning disable 3021}
r	: NUM '+' NUM;
ID	: [a-z]+ ;
NUM	: [0-9]+ ;
WS	: [ \t\r\n]+ -> skip ;