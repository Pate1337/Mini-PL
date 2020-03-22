using System;
using System.Collections.Generic;

namespace Interpreter
{
  public interface BlockNode
  {
    void AddStatement(Statement stmt);
    List<Statement> Statements { get; set; }
  }
}