using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        void TestCalc(string s, object d)
        {
            var c = new Mint.Calc();
            var dic2 = new Dictionary<string, object>()
            {
                { "x", 100.0 },
                { "y", 200.0 },
            };
            var dic = new Dictionary<string, object>()
            {
                { "a", 10.0 },
                { "b", 20.0 },
                { "obj", dic2 },
                { "fun", new Func<double>(() => 1000.0) },
                { "pow2", new Func<double, double>(x => x * x) },
                { "con2", new Func<object, object>(x => x.ToString() + x.ToString()) },
            };
            var calcContext = new Mint.CalcContext();
            calcContext.idResolver = (id) =>
            {
                return dic[id];
            };
            calcContext.dotOperator = (a, b) =>
            {
                if (a is Dictionary<string, object> env && b is object[] ba)
                {
                    if (ba.Length == 2 && Convert.ToString(ba[0]) == "id")
                    {
                        var name = Convert.ToString(ba[1]);
                        return env[name];
                    }
                }
                return Convert.ToString(a) + "." + Convert.ToString(b);
            };
            var v = c.Execute(s, calcContext);
            Assert.AreEqual(v, d);
        }
        void TestTree(string s, string t)
        {
            var json = Mint.Expression.ToJson(s);
            Assert.AreEqual(json, t);
        }

        void TestExpr(string s)
        {
            var tree = Mint.Expression.ParseExpression(s);
            var es = Mint.Expression.ToExpressionString(tree);
            Assert.AreEqual(s, es);
        }

        [TestMethod]
        public void Expr1()
        {
            TestCalc("1", 1.0);
            TestExpr("1");
        }

        [TestMethod]
        public void Expr2()
        {
            TestCalc("1+2", 3.0);
            TestExpr("1+2");
        }

        [TestMethod]
        public void Expr3()
        {
            TestCalc("2*4", 8.0);
            TestExpr("2*4");
        }

        [TestMethod]
        public void Expr4()
        {
            TestCalc("2-3", -1.0);
            TestExpr("2-3");
        }

        [TestMethod]
        public void Expr5()
        {
            TestCalc("8/2", 4.0);
            TestExpr("8/2");
        }


        [TestMethod]
        public void Expr6()
        {
            TestCalc("1+2*3", 7.0);
            TestExpr("1+2*3");
        }

        [TestMethod]
        public void Expr7()
        {
            TestCalc("(1+2)*3", 9.0);
            TestExpr("(1+2)*3");
        }

        [TestMethod]
        public void Expr8()
        {
            TestCalc("1+2-3+4-5", -1.0);
            TestExpr("1+2-3+4-5");
        }

        [TestMethod]
        public void Expr9()
        {
            TestCalc("1+2-(3+4)-5", -9.0);
            //TestExpr("1+2-(3+4)-5"); 括弧は無視されるので通らなくて当然
        }

        [TestMethod]
        public void Expr10()
        {
            TestCalc("a+b", 30.0);
            TestExpr("a+b");
        }

        [TestMethod]
        public void Expr11()
        {
            TestCalc("obj.x", 100.0);
            TestExpr("obj.x");
        }

        [TestMethod]
        public void Expr12()
        {
            TestCalc("1.2+3.4", 4.6);
            TestExpr("1.2+3.4");
        }

        [TestMethod]
        public void Expr13()
        {
            TestCalc("a<b", true);
            TestExpr("a<b");
        }


        [TestMethod]
        public void Expr14()
        {
            TestCalc("pow2(10)", 100.0);
            TestExpr("pow2(10)");
        }

        [TestMethod]
        public void Expr23_()
        {
            TestCalc("fun()", 1000.0);
            TestExpr("fun()");
        }


        [TestMethod]
        public void Expr15()
        {
            TestCalc("con2(123)", "123123");
            TestExpr("con2(123)");
        }

        [TestMethod]
        public void Expr16()
        {
            TestCalc("'a'", "a");
            TestExpr("'a'");
        }


        [TestMethod]
        public void Expr17()
        {
            TestCalc("'a'+'bc'", "abc");
            TestExpr("'a'+'bc'");
        }

        [TestMethod]
        public void Expr18()
        {
            TestCalc("`abc{1+2}`", "abc3");
        }

        [TestMethod]
        public void Expr19()
        {
            TestCalc("!(1==1)", false);
            TestExpr("!(1==1)");
        }

        [TestMethod]
        public void Expr20()
        {
            TestCalc("-(10+20)", -30.0);
            TestExpr("-(10+20)");
        }


        [TestMethod]
        public void Expr21()
        {
            TestCalc("1-(-10)", 11.0);
            TestCalc("1- -10", 11.0);
            TestCalc("1--10", 11.0);
            TestExpr("1-(-10)");
        }

        [TestMethod]
        public void Expr22()
        {
            TestCalc("con2()# hoge ", " hoge  hoge ");
        }

        [TestMethod]
        public void Expr23()
        {
            TestCalc("con2# hoge ", " hoge  hoge ");
        }


        [TestMethod]
        public void Tree8()
        {
            TestTree("1+2-3+4-5", @"['-',['+',['-',['+',1,2],3],4],5]".Replace("'", "\""));
            TestExpr("1+2-3+4-5");
        }





        [TestMethod]
        public void Tree1()
        {
            TestTree("(1+2+3)*3", @"[""*"",[""+"",[""+"",1,2],3],3]");
            TestExpr("(1+2+3)*3");
        }

        [TestMethod]
        public void Tree10()
        {
            TestTree("a+b", @"[""+"",[""id"",""a""],[""id"",""b""]]");
        }

        [TestMethod]
        public void Tree11()
        {
            TestTree("obj.x", @"[""."",[""id"",""obj""],[""id"",""x""]]");
        }

        [TestMethod]
        public void Tree12()
        {
            TestTree("obj.x(10)", @"[""call"",[""."",[""id"",""obj""],[""id"",""x""]],[10]]");
        }

        [TestMethod]
        public void Tree22()
        {
            TestTree("con2()#hoge hoge", @"[""call"",[""id"",""con2""],[""hoge hoge""]]");
        }


        [TestMethod]
        public void Tree23()
        {
            TestTree("con2#hoge hoge", @"[""call"",[""id"",""con2""],[""hoge hoge""]]");
        }

        [TestMethod]
        public void Tree24()
        {
            TestTree("a.b#hoge", @"[""call"",[""."",[""id"",""a""],[""id"",""b""]],[""hoge""]]");
        }



        [TestMethod]
        public void ExprStr1()
        {
            TestExpr("1");
        }



        [TestMethod]
        public void ExprStr2()
        {
            TestExpr("1+2");
        }

        [TestMethod]
        public void ExprStr3()
        {
            TestExpr("(1+2+3)*4");
        }

        [TestMethod]
        public void ExprStr4()
        {
            TestExpr("1+2*3");
        }

        [TestMethod]
        public void ExprStr5()
        {
            TestExpr("(1+2)*3");
        }

        [TestMethod]
        public void ExprStr6()
        {
            TestExpr("(1+2)*(3+4)");
        }

        [TestMethod]
        public void ExprStr7()
        {
            TestExpr("1+2*3+4");
        }






        [TestMethod]
        public void ExprStr10()
        {
            TestExpr("obj.x(10)");
        }

    }
}
