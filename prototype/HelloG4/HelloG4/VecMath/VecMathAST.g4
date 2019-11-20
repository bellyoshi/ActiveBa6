grammar VecMathAST;
options {
	output = AST;
}
tokens {VEC}
/*
 * Parser Rules
 */

statlist : stat+ ;
stat : ID '=' expr -> ^('=' ID expr)
	| 'print' expr -> ^('print' expr)
	;
expr : multExpr ('+'^ multExpr)*;
multExpr : primary ('*'^|'.'^) primary)*;
primary
	: INT
	| ID
	| '[' expr (',' expr)* ']' ->(VEC expr+)
	;

/*
 * Lexer Rules
 */
ID	:	[a-z];
INT	:	[1-9][0-9]*;
WS
	:	[ \t\r\n] -> skip ;
	;
