grammar Graphics;
@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}

file : command+ ; // file‚Ícommand‚ÌƒŠƒXƒg‚Å‚·
command : 'line' 'from' point 'to' point ;
point  : INT ',' INT ; // "0,10"‚È‚Ç


INT : '0'..'9' + ; //ˆêŒ…ˆÈã‚Ì”š—ñ‚ğÆ‡‚·‚é‚½‚ß‚Ìš‹å‹K‘¥‚Å‚·
WS	: [ \t\r\n]+ -> skip ;