using System;

namespace Interpreter
{
  public class AssignmentNode : Statement
  {
    public string Identifier { get; set; }
    public Expression Expression { get; set; }
    public Token Token { get; set; }

    public AssignmentNode(string id, Expression expr, Token token)
    {
      this.Identifier = id;
      this.Expression = expr;
      this.Token = token;
    }
    public void Visit(Visitor visitor)
    {
      visitor.VisitAssignment(this);
    }
  }
}