grammar Hello;
@parser::header {#pragma warning disable 3021}
@lexer::jeader {#pragma warning disable 3021}




expr	:	expr op=('*' | '/') expr	#mulDiv
		|	expr op=('+' | '-') expr	#addSub
		|	INT							#int
		|	ID							#id
		|	'(' expr ')'				#parens
		;

MUL		:	'*';
DIV		:	'/';
ADD		:	'+';
SUB		:	'-';

ID		:	[a-zA-Z][a-zA-Z0-9]* ;
INT		:	'0'|([1-9][0-9]*) ;

WS	: [ \t\r\n]+ -> skip ;