using System;

namespace Interpreter
{
  public class Token
  {
    public Token(SymbolType symbolType, string value)
    {
      SymbolType = symbolType;
      Value = value;
      LineNumber = 0;
      Column = 0;
    }
    public Token(SymbolType symbolType, string value, int lineNumber, int column)
    {
      SymbolType = symbolType;
      Value = value;
      LineNumber = lineNumber;
      Column = column;
    }
    public override string ToString()
    {
      return $"(Value: {Value}, SymbolType: {SymbolType}, Line: {LineNumber}, Column: {Column})";
    }

    public SymbolType SymbolType { get; set; }
    public string Value { get; set; }
    public int LineNumber { get; set; }
    public int Column { get; set; }
  }
}