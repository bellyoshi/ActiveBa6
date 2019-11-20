grammar ArrayInit;

/*
 * Parser Rules
 */

init : '{' value (',' value)* '}' ;

value : init 
		| INT
		;

/*
 * Lexer Rules
 */
INT :	[0-9]+ ;
WS	:	[ \t\r\n] -> skip;
