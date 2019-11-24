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
	|	ifstat
	|	ifstatMulti
	|	forstat
	;

ifstatMulti: IF expr THEN NEWLINE
		 linestat+
		(INT? ELSE NEWLINE
		 linestat*)?
		INT? END IF 
	  ;
ifstat: IF expr THEN stat (ELSE? stat)?;

forstat: FOR ID '=' INT TO INT NEWLINE
		 linestat+
		 NEXT
		;
print :PRINT expr;
dimenition : DIM ID 'As' TYPE;
assign :	LET? ID '=' expr;
label :	INT;

expr	:	expr op=(MUL | DIV) expr	#mulDiv
		|	expr op=(ADD | SUB) expr	#addSub
		|	INT							#int
		|	ID							#id
		|	'(' expr ')'				#parens
		;








