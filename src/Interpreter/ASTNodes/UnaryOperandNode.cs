using System;

namespace Interpreter
{
  public class UnaryOperandNode : Operand
  {
    public string Value { get; set; }
    public SymbolType Type { get; set; }
    public Token Token { get; set; }
    public bool Negative { get; set; } // For Identifiers only
    public bool Not { get; set; }

    public UnaryOperandNode()
    {
      this.Value = "Default";
      this.Type = SymbolType.Invalid;
      this.Token = new Token(SymbolType.Invalid, "EMPTY");
      this.Negative = false;
      this.Not = false;
    }
    public UnaryOperandNode(string value, SymbolType type)
    {
      this.Value = RemoveSurroundingQuotes(value);
      this.Type = type;
      this.Token = new Token(SymbolType.Invalid, "EMPTY");
      this.Negative = false;
      this.Not = false;
    }
    public UnaryOperandNode(string value, SymbolType type, Token token)
    {
      this.Value = RemoveSurroundingQuotes(value);
      this.Type = type;
      this.Token = token;
      this.Negative = false;
      this.Not = false;
    }
    public UnaryOperandNode(string value, SymbolType type, Token token, bool negative)
    {
      this.Value = RemoveSurroundingQuotes(value);
      this.Type = type;
      this.Token = token;
      this.Negative = negative;
      this.Not = false;
    }
    public UnaryOperandNode(string value, SymbolType type, Token token, bool negative, bool not)
    {
      this.Value = RemoveSurroundingQuotes(value);
      this.Type = type;
      this.Token = token;
      this.Negative = false;
      this.Not = not;
    }
    private string RemoveSurroundingQuotes(string value)
    {
      if (value.Length > 1 && value[0] == '"' && value[value.Length - 1] == '"') return value.Substring(1, value.Length - 2);
      return value;
    }
    public UnaryOperandNode Visit(Visitor visitor)
    {
      return visitor.VisitUnaryOperand(this);
    }
    public override string ToString()
    {
      return $"(Value: {this.Value}, Type: {this.Type}, Token: {this.Token}, Negative: {this.Negative}, Not: {this.Not})";
    }
  }
}