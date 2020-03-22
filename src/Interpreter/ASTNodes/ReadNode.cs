using System;

namespace Interpreter
{
  public class ReadNode : Statement
  {
    public string Identifier { get; set; }
    public string Value { get; set; }
    public Token Token { get; set; }

    public ReadNode(string id, string value, Token token)
    {
      this.Identifier = id;
      this.Value = value;
      this.Token = token;
    }
    public void Visit(Visitor visitor)
    {
      visitor.VisitRead(this);
    }
  }
}