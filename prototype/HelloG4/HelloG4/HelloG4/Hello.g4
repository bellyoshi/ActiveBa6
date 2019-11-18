grammar Hello;
@parser::header {#pragma warning disable 3021}
@lexer::jeader {#pragma warning disable 3021}
r	: 'hello' ID ;
ID	: [a-z]+ ;
WS	: [ \t\r\n]+ -> skip ;