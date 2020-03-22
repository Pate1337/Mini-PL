using System;

namespace Interpreter
{
  public class PrintNode : Statement
  {
    public Expression Expression { get; set; }

    public PrintNode(Expression expr)
    {
      this.Expression = expr;
    }
    public void Visit(Visitor visitor)
    {
      visitor.VisitPrint(this);
    }
  }
}