grammar Java;

/*
 * Parser Rules
 */

classDeclaration
	:	'class' Identifier typeParameters? ('extends' type)?
		(implements' typeList)?
		 classBody
	;
methodDeclaration
	:	type Identifier formalParameters ('[' ']')* methodDeclarationRest
	|	'void' Identifier formalParameters methodDeclarationRest
	;
/*
 * Lexer Rules
 */

WS
	:	' ' -> channel(HIDDEN)
	;
