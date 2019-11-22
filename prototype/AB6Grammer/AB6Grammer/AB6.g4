grammar AB6;
import CommonLexerRules;

@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}

/*
 * Parser Rules
 */

prog:	linestat+ ;

linestat: INT? stat NEWLINE;

stat:	print
	|   dimenition
	|	assign 
	;

print :PRINT expr;
dimenition : DIM ID 'As' TYPE;
assign :	ID '=' expr;
label :	INT;

expr	:	expr op=(MUL | DIV) expr	#mulDiv
		|	expr op=(ADD | SUB) expr	#addSub
		|	INT							#int
		|	ID							#id
		|	'(' expr ')'				#parens
		;








