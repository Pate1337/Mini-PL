using NUnit.Framework;
using Interpreter;
using System.Collections.Generic;

namespace InterpreterTests
{
  public class InterpreterTests
  {
    Parser p = new Parser();
    string minus = Integer.minus; // The same that C# uses

    private string ConcatLines(string[] lines)
    {
      string input = "";
      int index = 0;
      foreach(string line in lines)
      {
        input += line;
        if (index < lines.Length - 1) input += "\n";
        index++;
      }
      return input;
    }
    private List<string> GetProgramOutput(string[] lines, string[] inputs)
    {
      FileReader.ClearInput();
      IOHandler io = new TestIO(inputs);
      foreach (string line in lines)
      {
        FileReader.AddInputLine(line);
      }
      string input = ConcatLines(lines);
      BlockNode ast = p.Parse(input);
      Visitor v = new InterpreterVisitor(io);
      v.VisitProgram(ast);
      FileReader.ClearInput();
      return io.GetOutput();
    }

    [Test]
    public void InterpretsInputCorrectly1()
    {
      try
      {
        string[] lines = {
          "var X : int := 4 + (6 * 2);",
          "print X;"
        };
        string[] inputs = {};
        List<string> outputs = GetProgramOutput(lines, inputs);
        Assert.AreEqual("16", outputs[0]);
        Assert.AreEqual(1, outputs.Count);
      }
      catch (Error e)
      {
        Assert.AreEqual("no error should occur", e);
      }
    }
    [Test]
    public void InterpretsInputCorrectly2()
    {
      try
      {
        string[] lines = {
          "var nTimes : int := 0;",
          "read nTimes;",
          "print nTimes;"
        };
        string[] input = {"5"};
        List<string> outputs = GetProgramOutput(lines, input);
        Assert.AreEqual("5", outputs[0]);
        Assert.AreEqual(1, outputs.Count);
      }
      catch (Error e)
      {
        Assert.AreEqual("no error should occur", e);
      }
    }

    [Test]
    public void InterpretsInputCorrectly3()
    {
      try
      {
        string[] lines = {
          "var nTimes : int := 0;",
          "print \"How many times?\";",
          "read nTimes;",
          "var x : int;",
          "for x in 0..nTimes-1 do",
          " print x;",
          " print \" : Hello, World!\n\";",
          "end for;",
          "assert (x = nTimes);"
        };
        string[] input = {"3"};
        List<string> outputs = GetProgramOutput(lines, input);
        Assert.AreEqual("How many times?", outputs[0]);
        Assert.AreEqual("0", outputs[1]);
        Assert.AreEqual(" : Hello, World!\n", outputs[2]);
        Assert.AreEqual("1", outputs[3]);
        Assert.AreEqual(" : Hello, World!\n", outputs[4]);
        Assert.AreEqual("2", outputs[5]);
        Assert.AreEqual(" : Hello, World!\n", outputs[6]);
        Assert.AreEqual(7, outputs.Count);
      }
      catch (Error e)
      {
        Assert.AreEqual("no error should occur", e);
      }
    }
    [Test]
    public void InterpretsInputCorrectly4()
    {
      try
      {
        string[] lines = {
          "print \"Give a number\";",
          "var n : int;",
          "read n;",
          "var v : int := 1;",
          "var i : int;",
          "for i in 1..n do",
          " v := v * i;",
          "end for;",
          "print \"The result is: \";",
          "print v; "
        };
        string[] input = {"4"};
        List<string> outputs = GetProgramOutput(lines, input);
        Assert.AreEqual("Give a number", outputs[0]);
        Assert.AreEqual("The result is: ", outputs[1]);
        Assert.AreEqual("24", outputs[2]);
        Assert.AreEqual(3, outputs.Count);
      }
      catch (Error e)
      {
        Assert.AreEqual("no error should occur", e);
      }
    }
    [Test]
    public void NegativeIntegersAreInterpretedCorrectly()
    {
      try
      {
        string[] lines = {
          "var x : int := -2;",
          "x := x * 1;",
          "print x; // -2",
          "x := x * 1;",
          "print x; // -2",
          "x := -x * 1;",
          "print x; // 2",
          "print -(x / -1); // 2",
          "print -(-x * -1); // -2"
        };
        string[] input = {};
        List<string> outputs = GetProgramOutput(lines, input);
        Assert.AreEqual($"{minus}2", outputs[0]);
        Assert.AreEqual($"{minus}2", outputs[1]);
        Assert.AreEqual("2", outputs[2]);
        Assert.AreEqual("2", outputs[3]);
        Assert.AreEqual($"{minus}2", outputs[4]);
        Assert.AreEqual(5, outputs.Count);
      }
      catch (Error e)
      {
        Assert.AreEqual("no error should occur", e);
      }
    }
    [Test]
    public void NegativeIntegersAreInterpretedCorrectly2()
    {
      try
      {
        string[] lines = {
          "var x : int;",
          "read x;",
          "print x;",
          "read x;",
          "print x;"
        };
        string[] input = {"-5", $"{minus}3"};
        List<string> outputs = GetProgramOutput(lines, input);
        Assert.AreEqual($"{minus}5", outputs[0]);
        Assert.AreEqual($"{minus}3", outputs[1]);
        Assert.AreEqual(2, outputs.Count);
      }
      catch (Error e)
      {
        
        Assert.AreEqual("no error should occur", e);
      }
    }
    [Test]
    public void ForLoopWorksBothWays()
    {
      try
      {
        string[] lines = {
          "var x : int;",
          "for x in 2..-2 do",
          "print x;",
          "end for;"
        };
        string[] input = {};
        List<string> outputs = GetProgramOutput(lines, input);
        Assert.AreEqual("2", outputs[0]);
        Assert.AreEqual("1", outputs[1]);
        Assert.AreEqual("0", outputs[2]);
        Assert.AreEqual($"{minus}1", outputs[3]);
        Assert.AreEqual($"{minus}2", outputs[4]);
        Assert.AreEqual(5, outputs.Count);
      }
      catch (Error e)
      {
        Assert.AreEqual("no error should occur", e);
      }
    }
  }
}