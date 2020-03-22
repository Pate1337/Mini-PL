using System;

namespace Interpreter
{
  public class UnaryExpressionNode : Expression
  {
    public Operand Left { get; set; }
    public Operand Right { get; set; }
    public string Operator { get; set; }

    public UnaryExpressionNode(Operand operand)
    {
      this.Left = operand;
      this.Right = new UnaryOperandNode("", SymbolType.Invalid);
      this.Operator = "";
    }
    public UnaryOperandNode Visit(Visitor visitor)
    {
      return visitor.VisitUnaryExpression(this);
    }
  }
}