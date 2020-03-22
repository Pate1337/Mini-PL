using System;

namespace Interpreter
{
  public class AssertNode : Statement
  {
    public Expression Expression { get; set; }

    public AssertNode(Expression expr)
    {
      this.Expression = expr;
    }
    public void Visit(Visitor visitor)
    {
      visitor.VisitAssert(this);
    }
  }
}