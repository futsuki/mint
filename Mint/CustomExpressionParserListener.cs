//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.8
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from CustomExpressionParser.g4 by ANTLR 4.8

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="CustomExpressionParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.8")]
[System.CLSCompliant(false)]
public interface ICustomExpressionParserListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomExpressionParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpr([NotNull] CustomExpressionParser.ExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomExpressionParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpr([NotNull] CustomExpressionParser.ExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomExpressionParser.number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumber([NotNull] CustomExpressionParser.NumberContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomExpressionParser.number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumber([NotNull] CustomExpressionParser.NumberContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomExpressionParser.singleQuotedString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSingleQuotedString([NotNull] CustomExpressionParser.SingleQuotedStringContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomExpressionParser.singleQuotedString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSingleQuotedString([NotNull] CustomExpressionParser.SingleQuotedStringContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomExpressionParser.doubleQuotedString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoubleQuotedString([NotNull] CustomExpressionParser.DoubleQuotedStringContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomExpressionParser.doubleQuotedString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoubleQuotedString([NotNull] CustomExpressionParser.DoubleQuotedStringContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomExpressionParser.backQuoteString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBackQuoteString([NotNull] CustomExpressionParser.BackQuoteStringContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomExpressionParser.backQuoteString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBackQuoteString([NotNull] CustomExpressionParser.BackQuoteStringContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomExpressionParser.backQuoteExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBackQuoteExpr([NotNull] CustomExpressionParser.BackQuoteExprContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomExpressionParser.backQuoteExpr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBackQuoteExpr([NotNull] CustomExpressionParser.BackQuoteExprContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomExpressionParser.anyChar"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnyChar([NotNull] CustomExpressionParser.AnyCharContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomExpressionParser.anyChar"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnyChar([NotNull] CustomExpressionParser.AnyCharContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomExpressionParser.char"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterChar([NotNull] CustomExpressionParser.CharContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomExpressionParser.char"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitChar([NotNull] CustomExpressionParser.CharContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CustomExpressionParser.escapedChar"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEscapedChar([NotNull] CustomExpressionParser.EscapedCharContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CustomExpressionParser.escapedChar"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEscapedChar([NotNull] CustomExpressionParser.EscapedCharContext context);
}
