using System;

namespace Interpreter
{
  public interface Visitor
  {
    void VisitPrint(PrintNode pn);
    void VisitRead(ReadNode rn);
    void VisitAssignment(AssignmentNode an);
    void VisitDeclaration(DeclarationNode dn);
    void VisitForLoop(ForLoopNode fln);
    void VisitAssert(AssertNode an);
    UnaryOperandNode VisitBinaryExpression(BinaryExpressionNode ben);
    UnaryOperandNode VisitUnaryExpression(UnaryExpressionNode uen);
    UnaryOperandNode VisitUnaryOperand(UnaryOperandNode bon);
    UnaryOperandNode VisitExpressionOperand(ExpressionOperandNode eon);
    void VisitProgram(BlockNode bn);
  }
}