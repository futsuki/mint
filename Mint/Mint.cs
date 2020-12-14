using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Mint.Internal;

namespace Mint
{
    public static class Expression
    {
        public static object ParseExpression(string str)
        {
            var inputStream = new AntlrInputStream(str);
            var lex = new CustomExpressionLexer(inputStream);
            lex.PushMode(lex.ModeNames.ToList().IndexOf("Expression"));
            var tokens = new CommonTokenStream(lex);
            var p = new CustomExpressionParser(tokens);
            var a = new ExprAggregater();
            var ret = a.Visit(p.exp());
            return ret;
        }

        public static string ToJson(string expr)
        {
            return MiniJson.Serialize(ParseExpression(expr));
        }

        public static string JsonToExpressionString(string json)
        {
            var t = MiniJson.Deserialize(json);
            return ToExpressionString(t);
        }
        public static string ToExpressionString(object tree)
        {
            var sb = new StringBuilder();
            ToExpressionStringProc(tree, sb, int.MaxValue);
            return sb.ToString();
        }
        static bool triop(StringBuilder sb, object[] vs, string op1, string op2, int parentPriority, int nextPriority)
        {
            if (string.Equals(vs[0], op1))
            {
                if (parentPriority < nextPriority)
                    sb.Append("(");
                ToExpressionStringProc(vs[1], sb, nextPriority);
                sb.Append(op1);
                ToExpressionStringProc(vs[2], sb, nextPriority);
                sb.Append(op2);
                ToExpressionStringProc(vs[3], sb, nextPriority);
                if (parentPriority < nextPriority)
                    sb.Append(")");
                return true;
            }
            return false;
        }
        static bool biop(StringBuilder sb, object[] vs, string op, int parentPriority, int nextPriority)
        {
            if (string.Equals(vs[0], op))
            {
                if (parentPriority < nextPriority)
                    sb.Append("(");
                ToExpressionStringProc(vs[1], sb, nextPriority);
                sb.Append(op);
                ToExpressionStringProc(vs[2], sb, nextPriority);
                if (parentPriority < nextPriority)
                    sb.Append(")");
                return true;
            }
            return false;
        }
        static bool unop(StringBuilder sb, object[] vs, string op, int parentPriority, int nextPriority)
        {
            if (string.Equals(vs[0], op))
            {
                var hit = parentPriority < nextPriority;
                if (sb.Length > 0 && op.Length == 1 && sb.ToString(sb.Length - 1, 1) == op)
                    hit = true;
                if (hit)
                    sb.Append("(");
                sb.Append(op);
                ToExpressionStringProc(vs[1], sb, nextPriority);
                if (hit)
                    sb.Append(")");
                return true;
            }
            return false;
        }
        static void ToExpressionStringProc(object obj, StringBuilder sb, int parentPriority)
        {
            if (obj is object[] vs)
            {
                if (vs.Length == 0)
                {
                    sb.Append("<INVALID>");
                }
                else if (vs.Length == 2)
                {
                    if (string.Equals(vs[0], "id"))
                    {
                        sb.Append(vs[1]);
                    }
                    else if (unop(sb, vs, "+", parentPriority, 50)) { }
                    else if (unop(sb, vs, "-", parentPriority, 50)) { }
                    else if (unop(sb, vs, "!", parentPriority, 50)) { }
                }
                else if (vs.Length == 3)
                {
                    if (biop(sb, vs, ".", parentPriority, 10)) { }
                    else if (string.Equals(vs[0], "call"))
                    {
                        var CALL_PRIORITY = 10;
                        if (parentPriority < CALL_PRIORITY)
                            sb.Append("(");
                        var args = vs[2] as object[];
                        var fun = vs[1];
                        ToExpressionStringProc(fun, sb, CALL_PRIORITY);
                        sb.Append("(");
                        for (var i = 0; i < args.Length; i++)
                        {
                            ToExpressionStringProc(args[i], sb, CALL_PRIORITY);
                            if (i > 0)
                            {
                                sb.Append(", ");
                            }
                        }
                        sb.Append(")");
                        if (parentPriority < CALL_PRIORITY)
                            sb.Append(")");
                    }
                    else if (biop(sb, vs, "*", parentPriority, 110)) { }
                    else if (biop(sb, vs, "/", parentPriority, 110)) { }
                    else if (biop(sb, vs, "%", parentPriority, 110)) { }
                    else if (biop(sb, vs, "+", parentPriority, 150)) { }
                    else if (biop(sb, vs, "-", parentPriority, 150)) { }
                    else if (biop(sb, vs, "==", parentPriority, 200)) { }
                    else if (biop(sb, vs, "!=", parentPriority, 200)) { }
                    else if (biop(sb, vs, ">", parentPriority, 200)) { }
                    else if (biop(sb, vs, "<", parentPriority, 200)) { }
                    else if (biop(sb, vs, ">=", parentPriority, 200)) { }
                    else if (biop(sb, vs, "<=", parentPriority, 200)) { }
                    else if (biop(sb, vs, "&&", parentPriority, 300)) { }
                    else if (biop(sb, vs, "||", parentPriority, 400)) { }
                    else
                    {
                        sb.Append("<INVALID>");
                    }
                }
                else if (vs.Length == 4)
                {
                    if (triop(sb, vs, "?", ":", parentPriority, 500)) { }
                    else
                    {
                        sb.Append("<INVALID>");
                    }
                }
                else
                {
                    sb.Append("<INVALID>");
                }
            }
            else if (obj is string s)
            {
                sb.Append("'");
                sb.Append(s.Replace("\\", "\\\\")
                    .Replace("'", "\\'")
                    .Replace("\n", "\\n")
                    .Replace("\t", "\\t")
                    .Replace("\r", "\\r")
                );
                sb.Append("'");
            }
            else
            {
                sb.Append(obj);
            }
        }
    }


    public class CalcContext
    {
        public Func<object, object, object> dotOperator;
        public Func<string, object> idResolver;


        public Func<object, object, object> addOp;
        public Func<object, object, object> subOp;
        public Func<object, object, object> mulOp;
        public Func<object, object, object> divOp;
        public Func<object, object, object> modOp;
        public Func<object, object, int> cmpOp;
        public Func<object, object, bool> equalOp;
    }

    public class Calc
    {

        public object Execute(string s, CalcContext setup)
        {
            return Execute(Expression.ParseExpression(s), setup);
        }

        public object Execute(object s, CalcContext setup)
        {
            var f = Compile(s);
            var v = f(setup);
            return v;
        }

        public Func<CalcContext, object> Compile(string s)
        {
            return Compile(Expression.ParseExpression(s));
        }
        public Func<CalcContext, object> Compile(object o)
        {
            var f = CompileSemiResolved(o);
            return (setup) =>
            {
                object v;
                if (f is Func<CalcContext, object> fv)
                    v = fv(setup);
                else
                    v = f;
                if (v is object[] vs)
                {
                    if (vs.Length == 2 && Convert.ToString(vs[0]) == "id")
                        return setup.idResolver(Convert.ToString(vs[1]));
                }
                return v;
            };
        }

        static bool IsNumericType(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Decimal:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        object CompileSemiResolved(object o)
        {
            if (o is object[] os)
            {
                if (os.Length == 0)
                {
                    throw new ArgumentException("empty operator array");
                }
                var op = os[0];
                if (string.Equals(op, "==") && os.Length == 3)
                {
                    var f1 = Compile(os[1]);
                    var f2 = Compile(os[2]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        var v1 = f1(setup);
                        var v2 = f2(setup);
                        if (v1 == null && v2 == null)
                            return true;
                        if (v1 == null)
                            return v2.Equals(v1);
                        return v1.Equals(v1);
                    };
                    return f;
                }
                if (string.Equals(op, "!=") && os.Length == 3)
                {
                    var f1 = Compile(os[1]);
                    var f2 = Compile(os[2]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        var v1 = f1(setup);
                        var v2 = f2(setup);
                        if (v1 == null && v2 == null)
                            return false;
                        if (v1 == null)
                            return !v2.Equals(v1);
                        return !v1.Equals(v1);
                    };
                    return f;
                }
                if (string.Equals(op, ">=") && os.Length == 3)
                {
                    var f1 = Compile(os[1]);
                    var f2 = Compile(os[2]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        return Convert.ToDouble(f1(setup)) >= Convert.ToDouble(f2(setup));
                    };
                    return f;
                }
                if (string.Equals(op, "<=") && os.Length == 3)
                {
                    var f1 = Compile(os[1]);
                    var f2 = Compile(os[2]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        return Convert.ToDouble(f1(setup)) <= Convert.ToDouble(f2(setup));
                    };
                    return f;
                }
                if (string.Equals(op, ">") && os.Length == 3)
                {
                    var f1 = Compile(os[1]);
                    var f2 = Compile(os[2]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        return Convert.ToDouble(f1(setup)) > Convert.ToDouble(f2(setup));
                    };
                    return f;
                }
                if (string.Equals(op, "<") && os.Length == 3)
                {
                    var f1 = Compile(os[1]);
                    var f2 = Compile(os[2]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        return Convert.ToDouble(f1(setup)) < Convert.ToDouble(f2(setup));
                    };
                    return f;
                }
                if (string.Equals(op, "+") && os.Length == 2)
                {
                    var f1 = Compile(os[1]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        var v1 = f1(setup);
                        return Convert.ToDouble(v1);
                    };
                    return f;
                }
                if (string.Equals(op, "+") && os.Length == 3)
                {
                    var f1 = Compile(os[1]);
                    var f2 = Compile(os[2]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        var v1 = f1(setup);
                        var v2 = f2(setup);
                        var t1 = v1?.GetType();
                        var t2 = v2?.GetType();
                        if (t1 == typeof(string) || t2 == typeof(string))
                        {
                            //string
                            return Convert.ToString(v1) + Convert.ToString(v2);
                        }
                        else if (IsNumericType(t1) && IsNumericType(t2))
                        {
                            return Convert.ToDouble(v1) + Convert.ToDouble(v2);
                        }
                        throw new ArgumentException("invalid type add");
                    };
                    return f;
                }
                if (string.Equals(op, "-") && os.Length == 2)
                {
                    var f1 = Compile(os[1]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        return -Convert.ToDouble(f1(setup));
                    };
                    return f;
                }
                if (string.Equals(op, "-") && os.Length == 3)
                {
                    var f1 = Compile(os[1]);
                    var f2 = Compile(os[2]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        return Convert.ToDouble(f1(setup)) - Convert.ToDouble(f2(setup));
                    };
                    return f;
                }
                if (string.Equals(op, "*") && os.Length == 3)
                {
                    var f1 = Compile(os[1]);
                    var f2 = Compile(os[2]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        return Convert.ToDouble(f1(setup)) * Convert.ToDouble(f2(setup));
                    };
                    return f;
                }
                if (string.Equals(op, "/") && os.Length == 3)
                {
                    var f1 = Compile(os[1]);
                    var f2 = Compile(os[2]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        return Convert.ToDouble(f1(setup)) / Convert.ToDouble(f2(setup));
                    };
                    return f;
                }
                if (string.Equals(op, ".") && os.Length == 3)
                {
                    var f1 = Compile(os[1]);
                    var f2 = CompileSemiResolved(os[2]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        object v2;
                        if (f2 is Func<CalcContext, object> f2b)
                            v2 = f2b(setup);
                        else
                            v2 = f2;
                        return setup.dotOperator(f1(setup), v2);
                    };
                    return f;
                }
                if (string.Equals(op, "!") && os.Length == 2)
                {
                    var f1 = Compile(os[1]);
                    Func<CalcContext, object> f = (setup) =>
                    {
                        return !Convert.ToBoolean(f1(setup));
                    };
                    return f;
                }
                if (string.Equals(op, "id"))
                {
                    return os;
                }
                if (string.Equals(op, "call"))
                {
                    var dgfun = Compile(os[1]);
                    var args = (object[])os[2];
                    var argfuns = args.Select(x => Compile(x)).ToArray();
                    Func<CalcContext, object> f = (setup) =>
                    {
                        var dg = dgfun(setup) as Delegate;
                        if (argfuns.Length == 0 && dg is Func<object> f0)
                        {
                            return f0();
                        }
                        else if (argfuns.Length == 1 && dg is Func<object, object> f1)
                        {
                            var v1 = argfuns[0](setup);
                            return f1(v1);
                        }
                        else if (argfuns.Length == 2 && dg is Func<object, object, object> f2)
                        {
                            var v1 = argfuns[0](setup);
                            var v2 = argfuns[1](setup);
                            return f2(v1, v2);
                        }
                        else if (argfuns.Length == 3 && dg is Func<object, object, object, object> f3)
                        {
                            var v1 = argfuns[0](setup);
                            var v2 = argfuns[1](setup);
                            var v3 = argfuns[2](setup);
                            return f3(v1, v2, v3);
                        }
                        else if (argfuns.Length == 4 && dg is Func<object, object, object, object, object> f4)
                        {
                            var v1 = argfuns[0](setup);
                            var v2 = argfuns[1](setup);
                            var v3 = argfuns[2](setup);
                            var v4 = argfuns[3](setup);
                            return f4(v1, v2, v3, v4);
                        }
                        else
                        {
                            var vs = new object[argfuns.Length];
                            for (var i = 0; i < argfuns.Length; i++)
                            {
                                vs[i] = argfuns[i](setup);
                            }
                            return dg.DynamicInvoke(vs);
                        }
                    };
                    return f;
                }
                if (string.Equals(op, "concat"))
                {
                    //Console.WriteLine(os[1]);
                    //Console.WriteLine(((object[])os[1]).Length);
                    var argfunsrcs = os[1] as object[] ?? Array.Empty<object>();
                    var argfuns = argfunsrcs.Select(x => Compile(x)).ToArray();
                    //foreach (var ssss in argfunsrcs)
                    //{
                        //Console.WriteLine($"x: {ssss}");
                    //}
                    Func<CalcContext, object> f = (setup) =>
                    {
                        var ss = new object[argfuns.Length];
                        for (var i = 0; i < argfuns.Length; i++)
                        {
                            ss[i] = argfuns[i](setup);
                            //Console.WriteLine($"ss[{i}]: {ss[i]}");
                        }
                        return string.Concat(ss);
                    };
                    return f;
                }
                throw new ArgumentException("invalid operator "+Convert.ToString(op));
            }
            else
            {
                return o;
            }
        }
    }

    class ExprAggregater : CustomExpressionParserBaseVisitor<object>
    {
        public override object VisitExp([NotNull] CustomExpressionParser.ExpContext context)
        {
            if (context.lit != null)
            {
                return this.VisitLiteral(context.lit);
            }
            if (context.func != null)
            {
                var f = VisitExp(context.func);
                var args = (context.args != null) ? VisitExpList(context.args) : Array.Empty<object>();
                
                if (context.ptArg != null)
                {
                    if (args is object[] o)
                    {
                        var update = new object[o.Length + 1];
                        for (var i = 0; i < o.Length; i++)
                            update[i] = o[i];
                        update[o.Length] = VisitPlainTextArgument(context.ptArg);
                        args = update;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
                return new object[]
                {
                    "call",
                    f,
                    args
                };
            }
            var unop = context.unop?.Text;
            if (!string.IsNullOrEmpty(unop))
            {
                var exps = context.exp();
                switch (unop)
                {
                    case "!":
                    case "-":
                    case "+":
                        return new object[]{
                        unop,
                        VisitExp(exps[0])
                    };
                    default:
                        throw new ArgumentException($"exp '{context.GetText()}', '{unop}'");
                }
            }
            else
            {
                var op = context.op?.Text;
                var exps = context.exp();
                switch (op)
                {
                    case "(":
                        return VisitExp(exps[0]);
                    case ">=":
                    case "<=":
                    case ">":
                    case "<":
                    case "==":
                    case "!=":
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                    case "%":
                    case ".":
                        return new object[]{
                        op,
                        VisitExp(exps[0]),
                        VisitExp(exps[1])
                    };
                    default:
                        throw new ArgumentException($"exp '{context.GetText()}', '{op}'");
                }
            }
        }

        public override object VisitPlainTextArgument([NotNull] CustomExpressionParser.PlainTextArgumentContext context)
        {
            return VisitPlainTextString(context.plainTextString());
        }
        public override object VisitPlainTextString([NotNull] CustomExpressionParser.PlainTextStringContext context)
        {
            var chs = context.PlainTextChar();
            var ss = new string[chs.Length];
            var idx = 0;
            foreach (var c in chs)
            {
                ss[idx++] = c.GetText();
            }
            return string.Concat(ss);
        }

        public override object VisitExpList([NotNull] CustomExpressionParser.ExpListContext context)
        {
            var es = context.exp();
            var ret = new object[es.Length];
            var idx = 0;
            foreach (var e in es)
            {
                ret[idx++] = VisitExp(e);
            }
            return ret;
        }



        public override object VisitLiteral([NotNull] CustomExpressionParser.LiteralContext context)
        {
            var id = context.identifier();
            if (id != null)
                return VisitIdentifier(id);
            var n = context.number();
            if (n != null)
                return VisitNumber(n);
            var s = context.singleQuotedString();
            if (s != null)
                return VisitSingleQuotedString(s);
            var ds = context.doubleQuotedString();
            if (ds != null)
                return VisitDoubleQuotedString(ds);
            var bs = context.backQuoteString();
            if (bs != null)
                return VisitBackQuoteString(bs);
            return null;
        }

        public override object VisitIdentifier([NotNull] CustomExpressionParser.IdentifierContext context)
        {
            return new object[]{
                "id",
                context.Identifier().GetText()
            };
        }

        public override object VisitNumber([NotNull] CustomExpressionParser.NumberContext context)
        {
            return Convert.ToDouble(context.Number().GetText());
        }

        public override object VisitAnyChar([NotNull] CustomExpressionParser.AnyCharContext context)
        {
            var c = context.@char();
            if (c != null) {
                var t = c.GetText();
                return t;
            }
            var ec = context.escapedChar();
            if (ec != null)
            {
                return ec.GetText().Substring(1);
            }
            throw new ArgumentException("AnyChar");
        }

        public override object VisitSingleQuotedString([NotNull] CustomExpressionParser.SingleQuotedStringContext context)
        {
            var a = new object[context.children.Count];
            var idx = 0;
            foreach (var c in context.children)
            {
                a[idx++] = Visit(c);
            }
            return string.Concat(a);
        }
        public override object VisitDoubleQuotedString([NotNull] CustomExpressionParser.DoubleQuotedStringContext context)
        {
            var a = new object[context.children.Count];
            var idx = 0;
            foreach (var c in context.children)
            {
                a[idx++] = Visit(c);
            }
            return string.Concat(a);
        }

        public override object VisitBackQuoteString([NotNull] CustomExpressionParser.BackQuoteStringContext context)
        {
            var a = new object[context.children.Count];
            var idx = 0;
            foreach (var c in context.children)
            {
                a[idx++] = Visit(c);
            }
            return new object[]
            {
                "concat",
                a
            };
        }

        public override object VisitBackQuoteExpr([NotNull] CustomExpressionParser.BackQuoteExprContext context)
        {
            return VisitExp(context.exp());
        }



    }
}
