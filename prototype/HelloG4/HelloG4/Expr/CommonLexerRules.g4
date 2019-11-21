lexer grammar CommonLexerRules;

/*
 * Lexer Rules
 */
ID		:	[a-zA-Z][a-zA-Z0-9]* ;
INT		:	'0'|([1-9][0-9]*) ;
NEWLINE	:	'\r'?'\n';
WS		:	[ \t]+ -> skip ;