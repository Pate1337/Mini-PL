using System;
using System.Collections.Generic;

namespace Interpreter
{
  public class ForLoopNode : Statement, BlockNode
  {
    public AssignmentNode Assignment { get; set; }
    public BinaryExpressionNode Condition { get; set; }
    public List<Statement> Statements { get; set; }
    public string Identifier { get; set; }

    public ForLoopNode(AssignmentNode ass, BinaryExpressionNode be, string id)
    {
      this.Assignment = ass;
      this.Condition = be;
      this.Statements = new List<Statement>();
      this.Identifier = id;
    }
    public void AddStatement(Statement stmt)
    {
      this.Statements.Add(stmt);
    }
    public void Visit(Visitor visitor)
    {
      visitor.VisitForLoop(this);
    }
  }
}