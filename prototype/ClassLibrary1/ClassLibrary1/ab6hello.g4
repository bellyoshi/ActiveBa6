grammar ab6hello;
@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}

prog	:	stat+;

stat	:	'PRINT' expr NEW_LINE		#printExpr
		|	ID '=' expr NEW_LINE			#assign
		|	NEW_LINE						#blank
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
NEW_LINE	:	'\n';


ID		:	[a-zA-Z][a-zA-Z0-9]* ;
INT		:	'0'|([1-9][0-9]*) ;

WS	: [ \t\r]+ -> skip ;