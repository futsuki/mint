parser grammar CustomExpressionParser;

@parser::header {#pragma warning disable 3021}
@parser::modifier{internal}
@parser::namespace {Mint.Internal}

options { tokenVocab=CustomExpressionLexer; }


expList:
	(exp (Comma exp)*)?
;
exp
	: op=OpenParen exp CloseParen
	| exp op=Dot exp
	| unop=(Add | Sub | Not) exp
	| exp op=(Mod | Mul | Div) exp
	| exp op=(Add | Sub) exp
	| exp op=(EqualEqual | NotEqual | Less | Greater | LessOrEqual | GreaterOrEqual) exp
	| exp op=(And | Or) exp
	| exp op=Condition exp Colon exp
	| func=exp OpenParen args=expList CloseParen (Sharp ptArg=plainTextArgument)?
	| func=exp Sharp ptArg=plainTextArgument
	| lit=literal
;

plainTextArgument:
	chars=plainTextString
;
plainTextString: PlainTextChar* ;

literal: number | singleQuotedString | doubleQuotedString | backQuoteString | identifier ;
identifier: Identifier;

number: Number;
singleQuotedString: SingleQuote anyChar* EndSingleQuote;
doubleQuotedString: DoubleQuote anyChar* EndDoubleQuote;
backQuoteString: BackQuote (anyChar | backQuoteExpr)* EndBackQuote;
backQuoteExpr: BQExprStart exp BQExprEnd ;

anyChar: char | escapedChar ;
char: Char ;
escapedChar: EscapedChar;
