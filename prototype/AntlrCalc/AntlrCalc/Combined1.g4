grammar Combined1;

/*
 * Parser Rules
 */

operation	:	NUMBER + NUMBER;

/*
 * Lexer Rules
 */

NUMBER		: [1-9][0-9]* ;
WHITESPACE	: ' ' -> skip ;
