using System;

namespace Interpreter
{
  public interface Expression
  {
    Operand Left { get; set; }
    Operand Right { get; set; }
    string Operator { get; set; }
    UnaryOperandNode Visit (Visitor v);
  }
}