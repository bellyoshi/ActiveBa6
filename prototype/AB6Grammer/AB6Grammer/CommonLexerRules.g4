lexer grammar CommonLexerRules;

@lexer::header {#pragma warning disable 3021}

MUL		:	'*';
DIV		:	'/';
ADD		:	'+';
SUB		:	'-';

/*
 * Upper Lower
 */

fragment A:('a'|'A');
fragment B:('b'|'B');
fragment C:('c'|'C');
fragment D:('d'|'D');
fragment E:('e'|'E');
fragment F:('f'|'F');
fragment G:('g'|'G');
fragment H:('h'|'H');
fragment I:('i'|'I');
fragment J:('j'|'J');
fragment K:('k'|'K');
fragment L:('l'|'L');
fragment M:('m'|'M');
fragment N:('n'|'N');
fragment O:('o'|'O');
fragment P:('p'|'P');
fragment Q:('q'|'Q');
fragment R:('r'|'R');
fragment S:('s'|'S');
fragment T:('t'|'T');
fragment U:('u'|'U');
fragment V:('v'|'V');
fragment W:('w'|'W');
fragment X:('x'|'X');
fragment Y:('y'|'Y');
fragment Z:('z'|'Z');

/*
 * AB Lexer
 */

STRING :S T R I N G;
INTEGER:I N T E G E R;
TYPE	: STRING
		| INTEGER
		;

PRINT : P R I N T ;
DIM: D I M;
LET: L E T;
IF : I F;
ELSE : E L S E;
THEN : T H E N;
END : E N D;
WHILE : W H I L E;
FOR : F O R;
TO : T O;
NEXT : N E X T;
STEP : S T E P;

/*
 * Lexer Rules
 */
ID		:	[a-zA-Z][a-zA-Z0-9]* ;
INT		:	[0-9]+ ;
NEWLINE	:	'\r'?'\n';
WS		:	[ \t]+ -> skip ;