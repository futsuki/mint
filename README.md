# Mint
埋め込み用のちいさなインタプリタ言語。

.NETで動きます。

数式ベースの言語です。関数定義などはできませんのでホスト側ですべて設定します。

その場で`b+a*3`とか書かれているのを計算させるのに向いてます

ちょっとしたおまけとして行末までのプレーンテキスト形式の引数指定ができます。

* `func#abc`は`func("abc")`と同じ
* `func2(10)#abc`は`func2(10, "abc")`と同じ

くわしい使い方はこちら → [UnitTestProject1/UnitTest1.cs](UnitTestProject1/UnitTest1.cs)
