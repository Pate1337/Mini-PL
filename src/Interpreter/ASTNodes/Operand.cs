using System;

namespace Interpreter
{
  public interface Operand
  {
    UnaryOperandNode Visit(Visitor v);
  }
}