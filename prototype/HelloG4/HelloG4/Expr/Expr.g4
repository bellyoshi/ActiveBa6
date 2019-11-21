grammar Expr;
import CommonLexerRules;

@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}
/*
 * Parser Rules
 */

prog	:	stat+;

stat	:	expr NEWLINE				#printExpr
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


