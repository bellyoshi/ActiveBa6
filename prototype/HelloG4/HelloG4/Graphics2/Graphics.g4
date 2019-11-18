grammar Graphics;
@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}

file : command+ ; // file��command�̃��X�g�ł�
command : 'line' 'from' point 'to' point ;
point  : INT ',' INT ; // "0,10"�Ȃ�


INT : '0'..'9' + ; //�ꌅ�ȏ�̐�������ƍ����邽�߂̎���K���ł�
WS	: [ \t\r\n]+ -> skip ;