using System;
using System.Collections.Generic;

namespace Interpreter
{
  public class ProgramNode : BlockNode
  {
    public List<Statement> Statements { get; set; }

    public ProgramNode()
    {
      this.Statements = new List<Statement>();
    }

    public void AddStatement(Statement stmt)
    {
      this.Statements.Add(stmt);
    }
  }
}