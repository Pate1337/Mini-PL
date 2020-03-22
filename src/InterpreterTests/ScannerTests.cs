using NUnit.Framework;
using Interpreter;

namespace InterpreterTests
{
    public class ScannerTests
    {
        Scanner scanner;
        /*[SetUp]
        public void Setup()
        {
        }*/
        [Test]
        public void ReturnsEOFTokenOnEmptyInput()
        {
            scanner = new Scanner("");
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }

        [Test]
        public void ReturnsCorrectTokens1()
        {
            scanner = new Scanner("var x : int;");
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Colon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Integer, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }
        [Test]
        public void SkipsSequentialWhiteSpaces()
        {
            scanner = new Scanner("     ");
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }
        [Test]
        public void SkipsSequentialNewLines()
        {
            scanner = new Scanner("\n\n\n\n");
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }
        [Test]
        public void SkipsComments()
        {
            scanner = new Scanner("//comment");
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }
        [Test]
        public void SkipsMultilineComments()
        {
            scanner = new Scanner("/*comment*/");
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }
        [Test]
        public void CommentsOnlyLastOneLine()
        {
            scanner = new Scanner("var a;\n//comment\nvar b;");
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }
        [Test]
        public void Comments1()
        {
            scanner = new Scanner("var a;\n////c/om//ment\nvar b;");
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }
        [Test]
        public void MultilineComments1()
        {
            scanner = new Scanner("var a;\n/*comment*/\nvar b;");
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }
        [Test]
        public void MultilineComments2()
        {
            scanner = new Scanner("var a;\n/*comment\nsecondline\nthirdline*/\nvar b;");
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }[Test]
        public void MultilineComments3()
        {
            scanner = new Scanner("var a;\n/*****comment*****/\nvar b;");
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }
        [Test]
        public void CommentsCanHaveCodeInThemWithoutAffect()
        {
            scanner = new Scanner("var a;\n//var c : int;\nvar b;");
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }
        [Test]
        public void MultilineCommentsCanHaveCodeInThemWithoutAffect()
        {
            scanner = new Scanner("var a;\n/*var c : int;\nvar b : string;*/\nvar b;");
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Variable, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.Identifier, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.SemiColon, scanner.NextToken().SymbolType);
            Assert.AreEqual(SymbolType.EOF, scanner.NextToken().SymbolType);
        }
        [Test]
        public void TokensHaveRightLineNumberAndColumn1()
        {
            scanner = new Scanner("var a : int;");

            // var
            Token token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(0, token.Column);
            
            // a
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(4, token.Column);
            
            // :
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(6, token.Column);

            // int
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(8, token.Column);

            // ;
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(11, token.Column);
        }
        [Test]
        public void TokensHaveRightLineNumberAndColumn2()
        {
            scanner = new Scanner("var a : int;\n\nvar c : string;");

            // var
            Token token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(0, token.Column);
            
            // a
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(4, token.Column);
            
            // :
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(6, token.Column);

            // int
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(8, token.Column);

            // ;
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(11, token.Column);

            // var
            token = scanner.NextToken();
            Assert.AreEqual(3, token.LineNumber);
            Assert.AreEqual(0, token.Column);
            
            // c
            token = scanner.NextToken();
            Assert.AreEqual(3, token.LineNumber);
            Assert.AreEqual(4, token.Column);
            
            // :
            token = scanner.NextToken();
            Assert.AreEqual(3, token.LineNumber);
            Assert.AreEqual(6, token.Column);

            // string
            token = scanner.NextToken();
            Assert.AreEqual(3, token.LineNumber);
            Assert.AreEqual(8, token.Column);

            // ;
            token = scanner.NextToken();
            Assert.AreEqual(3, token.LineNumber);
            Assert.AreEqual(14, token.Column);
        }
        [Test]
        public void TokensHaveRightLineNumberAndColumn3()
        {
            scanner = new Scanner("var a : string := \"multi\nline\nstring\";");

            // var
            Token token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(0, token.Column);

            // a
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(4, token.Column);

            // :
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(6, token.Column);

            // string
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(8, token.Column);

            // :=
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(15, token.Column);

            // "multi\nline\nstring"
            token = scanner.NextToken();
            Assert.AreEqual(1, token.LineNumber);
            Assert.AreEqual(18, token.Column);

            // ;
            token = scanner.NextToken();
            Assert.AreEqual(3, token.LineNumber);
            Assert.AreEqual(7, token.Column);
        }
        [Test]
        public void RecognizesSmaller()
        {
            scanner = new Scanner("x < 1");

            Token token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Identifier, token.SymbolType);

            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Operator, token.SymbolType);

            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.IntegerValue, token.SymbolType);
            
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.EOF, token.SymbolType);
        }
        [Test]
        public void RecognizesLarger()
        {
            scanner = new Scanner("x > 1");

            Token token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Identifier, token.SymbolType);

            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Operator, token.SymbolType);

            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.IntegerValue, token.SymbolType);
            
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.EOF, token.SymbolType);
        }
        [Test]
        public void RecognizesSmallerOrEqual()
        {
            scanner = new Scanner("x <= 2");
            
            Token token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Identifier, token.SymbolType);

            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Operator, token.SymbolType);

            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.IntegerValue, token.SymbolType);

            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.EOF, token.SymbolType);
        }
        [Test]
        public void RecognizesLargerOrEqual()
        {
            scanner = new Scanner("x >= 2");
            
            Token token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Identifier, token.SymbolType);

            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Operator, token.SymbolType);

            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.IntegerValue, token.SymbolType);

            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.EOF, token.SymbolType);
        }
        [Test]
        public void RecognizesStrings1()
        {
            scanner = new Scanner("var a : string := \"string\";");

            Token token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Variable, token.SymbolType);
            Assert.AreEqual(0, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Identifier, token.SymbolType);
            Assert.AreEqual(4, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Colon, token.SymbolType);
            Assert.AreEqual(6, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.String, token.SymbolType);
            Assert.AreEqual(8, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Assignment, token.SymbolType);
            Assert.AreEqual(15, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.StringValue, token.SymbolType);
            Assert.AreEqual(18, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.SemiColon, token.SymbolType);
            Assert.AreEqual(26, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.EOF, token.SymbolType);
        }
        [Test]
        public void RecognizesMultilineComments()
        {
            scanner = new Scanner("var a : string := /*string*/;");

            Token token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Variable, token.SymbolType);
            Assert.AreEqual(0, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Identifier, token.SymbolType);
            Assert.AreEqual(4, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Colon, token.SymbolType);
            Assert.AreEqual(6, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.String, token.SymbolType);
            Assert.AreEqual(8, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.Assignment, token.SymbolType);
            Assert.AreEqual(15, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.SemiColon, token.SymbolType);
            Assert.AreEqual(28, token.Column);
            token = scanner.NextToken();
            Assert.AreEqual(SymbolType.EOF, token.SymbolType);
        }
    }
}