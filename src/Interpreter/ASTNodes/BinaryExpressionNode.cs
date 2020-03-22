using System;

namespace Interpreter
{
  public class BinaryExpressionNode : Expression
  {
    public Operand Left { get; set; }
    public string Operator { get; set; }
    public Operand Right { get; set; }

    public BinaryExpressionNode(Operand left, string op, Operand right)
    {
      this.Left = left;
      this.Operator = op;
      this.Right = right;
    }
    public UnaryOperandNode Visit(Visitor visitor)
    {
      return visitor.VisitBinaryExpression(this);
    }
  }
}