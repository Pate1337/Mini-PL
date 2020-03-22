using NUnit.Framework;
using Interpreter;

namespace InterpreterTests
{
    public class ParserTests
    {
      Parser p = new Parser();

      [Test]
      public void ParsesStatementStartingWithVariable1()
      {
        try
        {
          p.Parse("var a : int;");
          Assert.IsTrue(true);
        }
        catch (Error e)
        {
          Assert.AreEqual("no error should occur", e);
        }
      }
      [Test]
      public void ParsesStatementStartingWithVariable2()
      {
        try
        {
          p.Parse("var b : string := \"some text\";");
          Assert.IsTrue(true);
        }
        catch (Error e)
        {
          Assert.AreEqual("no error should occur", e);
        }
      }
      [Test]
      public void ReportsError1()
      {
        FileReader.ClearInput();
        string input = "var a : int;;";
        FileReader.AddInputLine(input);
        try
        {
          p.Parse(input);
          Assert.IsTrue(false);
        }
        catch (Error e)
        {
          Assert.AreEqual(
            $"ERROR (line 1, column 12): Unexpected character: {SymbolType.SemiColon}\n\n\t{input}\n\t            ^",
            e.ToString()
          );
        }
        FileReader.ClearInput();
      }
    }
}