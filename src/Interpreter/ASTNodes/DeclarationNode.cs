using System;

namespace Interpreter
{
  public class DeclarationNode : Statement
  {
    public string Identifier { get; set; }
    public SymbolType Type { get; set; }
    public Token Token { get; set; }

    public DeclarationNode(string id, SymbolType type, Token token)
    {
      this.Identifier = id;
      this.Type = type;
      this.Token = token;
    }
    public void Visit(Visitor visitor)
    {
      visitor.VisitDeclaration(this);
    }
  }
}