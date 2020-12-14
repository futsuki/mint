lexer grammar CustomExpressionLexer;


@lexer::header {#pragma warning disable 3021}
@lexer::modifier{internal}
@lexer::namespace {Mint.Internal}


Error:.;

mode Expression;
Number: [0-9]+ ('.' [0-9]+)?;

WS: [ \t] -> skip;
Sharp: '#' -> pushMode(PlainTextMode);
Define: ':=';
EqualEqual: '==';
NotEqual: '!=';
Not: '!';
And: '&&';
Or: '||';
Condition: '?';
Colon: ':';
GreaterOrEqual: '>=';
LessOrEqual: '<=';
Greater: '>';
Less: '<';
Equal: '=';
Comma: ',';
Add: '+';
Sub: '-';
Mul: '*';
Div: '/';
Mod: '%';
Dot: '.';
OpenParen: '(';
CloseParen: ')';
BackQuote: '`' -> pushMode(BackQuoteMode);
DoubleQuote: '"' -> pushMode(DoubleQuoteMode);
SingleQuote: '\'' -> pushMode(SingleQuoteMode);
Identifier: [a-zA-Z_\u0100-\uF8FF][a-zA-Z0-9_\u0100-\uF8FF]* ;


mode PlainTextMode;
PlainTextModeNewLine: '\r\n' -> skip, popMode ;
PlainTextModeNewLine2: '\r' -> skip, popMode ;
PlainTextModeNewLine3: '\n' -> skip, popMode ;
PlainTextChar: ~[\r\n] ;


mode SingleQuoteMode;
EscapedChar: '\\\'';
EndSingleQuote: '\'' -> popMode;
Char: . ;

mode DoubleQuoteMode;
EscapedDQ: '\\"' -> type(EscapedChar);
EndDoubleQuote: '"' -> popMode;
DQChar: . -> type(Char) ;

mode BackQuoteMode;
EscapedOpenBrace: '\\{' -> type(EscapedChar);
EscapedBQ: '\\`' -> type(EscapedChar);
BQExprStart: '{' -> popMode;
EndBackQuote: '`' -> popMode;
BQChar: . -> type(Char) ;

mode BackQuoteExpr;
BQExprEnd: '}' -> popMode;
BQAdd: '+' -> type(Add);
BQSub: '-' -> type(Sub);
BQMul: '*' -> type(Mul);
BQDiv: '/' -> type(Div);
BQMod: '%' -> type(Mod);
BQDot: '.' -> type(Dot);
BQOpneParen: '(' -> type(OpenParen);
BQCloseParen: ')' -> type(CloseParen);
BQBackQuote: '`' -> type(BackQuote), pushMode(BackQuoteMode);
BQDoubleQuote: '"' -> type(DoubleQuote), pushMode(DoubleQuoteMode);
BQSingleQuote: '\'' -> type(SingleQuote), pushMode(SingleQuoteMode);
