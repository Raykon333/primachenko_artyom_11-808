using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public static void RunTests()
        {
            Test("hello world", new[] { "hello", "world" });
            Test(@"a ""bcd ef"" 'x y'", new[] { "a", "bcd ef", "x y" });
            Test(@"""a 'b'""", new[] { "a 'b'" });
            Test(@"a""bcde""f", new[] { "a", "bcde", "f" });
            Test(@"abc ""defgh", new[] { "abc", "defgh" });
            Test(@"""a \""c\""""", new[] { @"a ""c""" });
            Test(@"""\\""", new[] { @"\" });
            Test(@"\\", new[] { @"\\" });
            Test(@"\""ab""", new[] { @"\", @"ab" });
            Test("hello  world", new[] { "hello", "world" });
            Test("  hello world  ", new[] { "hello", "world" });
            Test(@"""""", new[] { "" });
            Test(@"'hello ""world""'", new[] { @"hello ""world""" });
            Test(@"'\''", new[] { @"'" });
            Test("", new string[0]);
            Test(@"""hello world ", new[] { "hello world " });
        }

        public static void Test(string input, string[] expectedOutput)
        {
            Assert.AreEqual(TableParser.FieldsParserTask.ParseLine(input), expectedOutput);
        }
    }
}
