grammar Hello;
@parser::header {#pragma warning disable 3021}
@lexer::jeader {#pragma warning disable 3021}

prog	:	stat+;

stat	:	'PRINT' expr NEWLINE		#printExpr
		|	ID '=' expr NEWLINE			#assign
		|	NEWLINE						#blank
		;


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
NEWLINE	:	'\n';


ID		:	[a-zA-Z][a-zA-Z0-9]* ;
INT		:	'0'|([1-9][0-9]*) ;

WS	: [ \t\r]+ -> skip ;