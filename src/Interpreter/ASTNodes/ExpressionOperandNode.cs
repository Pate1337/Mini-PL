using System;

namespace Interpreter
{
  public class ExpressionOperandNode : Operand
  {
    public Expression Expression { get; set; }
    public bool Negative { get; set; }
    public bool Not { get; set; }

    public ExpressionOperandNode(Expression expr)
    {
      this.Expression = expr;
      this.Negative = false;
      this.Not = false;
    }
    public ExpressionOperandNode(Expression expr, bool negative)
    {
      this.Expression = expr;
      this.Negative = negative;
      this.Not = false;
    }
    public ExpressionOperandNode(Expression expr, bool negative, bool not)
    {
      this.Expression = expr;
      this.Negative = false;
      this.Not = not;
    }
    public UnaryOperandNode Visit(Visitor visitor)
    {
      return visitor.VisitExpressionOperand(this);
    }
  }
}