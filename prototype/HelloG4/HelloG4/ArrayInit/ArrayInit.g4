grammar ArrayInit;
@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}
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
