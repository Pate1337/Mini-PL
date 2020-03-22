using System;

namespace Interpreter
{
  public class PrintVisitor : Visitor
  {
    private int depth;
    private IOHandler io;

    public PrintVisitor(IOHandler io)
    {
      this.io = io;
    }

    public void VisitProgram(BlockNode prog)
    {
      this.depth = 0;
      string spaces = HandleDepth();
      this.io.WriteLine($"{spaces}Program: (");
      string spaces2 = IncreaseDepth();
      this.io.WriteLine($"{spaces2}Statements: [");
      VisitStatements(prog);
      this.io.WriteLine($"{spaces2}]");
      this.io.WriteLine($"{spaces})");
    }
    private void VisitStatements(BlockNode node)
    {
      int temp = this.depth;
      string spaces = IncreaseDepth();
      foreach (Statement stmt in node.Statements)
      {
        this.io.WriteLine($"{spaces}(");
        stmt.Visit(this);
        this.io.WriteLine($"{spaces}),");
      }
      this.depth = temp;
    }
    public void VisitPrint(PrintNode prn)
    {
      int temp = this.depth;
      string spaces = IncreaseDepth();
      this.io.WriteLine($"{spaces}Print: (");
      prn.Expression.Visit(this);
      this.io.WriteLine($"{spaces})");
      this.depth = temp;
    }
    public UnaryOperandNode VisitUnaryExpression(UnaryExpressionNode uen)
    {
      int temp = this.depth;
      string spaces = IncreaseDepth();
      this.io.WriteLine($"{spaces}UnaryExpression: (");
      uen.Left.Visit(this);
      this.io.WriteLine($"{spaces})");
      this.depth = temp;
      return new UnaryOperandNode("", SymbolType.Invalid);
    }
    public UnaryOperandNode VisitBinaryExpression(BinaryExpressionNode ben)
    {
      int temp = this.depth;
      string spaces1 = IncreaseDepth();
      this.io.WriteLine($"{spaces1}BinaryExpression: (");
      string spaces2 = IncreaseDepth();
      this.io.WriteLine($"{spaces2}Left: (");
      ben.Left.Visit(this);
      this.io.WriteLine($"{spaces2}),\n{spaces2}Operator: {ben.Operator},\n{spaces2}Right: (");
      ben.Right.Visit(this);
      this.io.WriteLine($"{spaces2})\n{spaces1})");
      this.depth = temp;
      return new UnaryOperandNode("", SymbolType.Invalid);
    }
    public UnaryOperandNode VisitUnaryOperand(UnaryOperandNode uon)
    {
      int temp = this.depth;
      string spaces = IncreaseDepth();
      this.io.WriteLine($"{spaces}UnaryOperand: (\n{spaces}  Value: {uon.Value},\n{spaces}  Type: {uon.Type},\n{spaces}  Token: {uon.Token},\n{spaces}  Negative: {uon.Negative},\n{spaces}  Not: {uon.Not}\n{spaces})");
      this.depth = temp;
      return new UnaryOperandNode("", SymbolType.Invalid);
    }
    public UnaryOperandNode VisitExpressionOperand(ExpressionOperandNode eon)
    {
      int temp = this.depth;
      string spaces = IncreaseDepth();
      this.io.WriteLine($"{spaces}ExpressionOperand: (");
      this.io.WriteLine($"{spaces}  Negative: {eon.Negative},");
      this.io.WriteLine($"{spaces}  Not: {eon.Not},");
      eon.Expression.Visit(this);
      this.io.WriteLine($"{spaces})");
      this.depth = temp;
      return new UnaryOperandNode("", SymbolType.Invalid);
    }
    public void VisitAssignment(AssignmentNode an)
    {
      // TODO: Also has Token
      int temp = this.depth;
      string spaces1 = IncreaseDepth();
      this.io.WriteLine($"{spaces1}Assignment: (");
      string spaces2 = IncreaseDepth();
      this.io.WriteLine($"{spaces2}Identifier: {an.Identifier},\n{spaces2}Token: {an.Token},\n{spaces2}Expression: (");
      an.Expression.Visit(this);
      this.io.WriteLine($"{spaces2})");
      this.io.WriteLine($"{spaces1})");
      this.depth = temp;
    }
    public void VisitDeclaration(DeclarationNode dn)
    {
      int temp = this.depth;
      string spaces = IncreaseDepth();
      this.io.WriteLine($"{spaces}Declaration: (\n{spaces}  Identifier: {dn.Identifier},\n{spaces}  Type: {dn.Type},\n{spaces}  Token: {dn.Token}\n{spaces})");
      this.depth = temp;
    }
    public void VisitForLoop(ForLoopNode fln)
    {
      int temp = this.depth;
      string spaces1 = IncreaseDepth();
      string spaces2 = IncreaseDepth();
      this.io.WriteLine($"{spaces1}ForLoop: (\n{spaces2}Assignment: (");
      fln.Assignment.Visit(this);
      this.io.WriteLine($"{spaces2}),\n{spaces2}Condition: (");
      fln.Condition.Visit(this);
      this.io.WriteLine($"{spaces2}),\n{spaces2}Statements: [");
      VisitStatements(fln);
      /*string spaces3 = IncreaseDepth();
      foreach (Statement stmt in fln.Statements)
      {
        this.io.WriteLine($"{spaces3}(");
        stmt.Visit(this);
        this.io.WriteLine($"{spaces3}),");
      }*/
      this.io.WriteLine($"{spaces2}]\n{spaces1})");
      this.depth = temp;
    }
    public void VisitAssert(AssertNode an)
    {
      int temp = this.depth;
      string spaces = IncreaseDepth();
      this.io.WriteLine($"{spaces}Assert: (");
      an.Expression.Visit(this);
      this.io.WriteLine($"{spaces})");
      this.depth = temp;
    }
    public void VisitRead(ReadNode rn)
    {
      int temp = this.depth;
      string spaces = IncreaseDepth();
      this.io.WriteLine($"{spaces}Read: (");
      this.io.WriteLine($"{spaces}  Identifier: {rn.Identifier},\n{spaces}  Value: {rn.Value},\n{spaces}  Token: {rn.Token}");
      this.io.WriteLine($"{spaces})");
      this.depth = temp;
    }
    private string HandleDepth()
    {
      string spaces = "";
      for (int i = 0; i < this.depth; i++)
      {
        spaces += "  ";
      }
      return spaces;
    }
    private string IncreaseDepth()
    {
      this.depth++;
      return HandleDepth();
    }
  }
}