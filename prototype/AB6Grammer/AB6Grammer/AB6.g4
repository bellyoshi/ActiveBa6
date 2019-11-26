grammar AB6;
import CommonLexerRules;

@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}

/*
 * Parser Rules
 */

prog:	linestat+ ;

linestat: INT? stat NEWLINE;

declSub :  SUB ID '(' ')' NEWLINE
		    linestat+
		   INT? END SUB 
		;
callSub :  'call'? ID'(' ')';

stat:	print
	|   dimenition
	|	assign 
	|	ifstat
	|	ifstatMulti
	|	forstat
	|	declSub
	|   callSub
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
		|	expr op=(ADD | MINUS) expr	#addSub
		|	INT							#int
		|	ID							#id
		|	'(' expr ')'				#parens
		;








