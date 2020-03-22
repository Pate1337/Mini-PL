using System;

namespace Interpreter
{
  public interface Statement
  {
    void Visit(Visitor v);
  }
}