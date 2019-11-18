grammar Graphics;
@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}

file : command+ ; // fileはcommandのリストです
command : 'line' 'from' point 'to' point ;
point  : INT ',' INT ; // "0,10"など


INT : '0'..'9' + ; //一桁以上の数字列を照合するための字句規則です
WS	: [ \t\r\n]+ -> skip ;