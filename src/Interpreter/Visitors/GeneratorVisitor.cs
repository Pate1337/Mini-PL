/*using System;

namespace Interpreter
{
  public class GeneratorVisitor : Visitor
  {
    private SymbolTable symbolTable;

    public GeneratorVisitor()
    {
      this.symbolTable = new SymbolTable();
    }
    public UnaryOperandNode VisitProgram(BaseNode prog)
    {
      this.symbolTable.PushBlock();
      VisitStatements(prog);
      this.symbolTable.PopBlock();
      return new UnaryOperandNode("", SymbolType.Invalid);
    }
    private void VisitStatements(BaseNode node)
    {
      foreach (Node stmt in node.Statements)
      {
        stmt.Visit(this);
      }
    }
    public UnaryOperandNode VisitPrint(PrintNode prn)
    {
      UnaryOperandNode expr = prn.Expression.Visit(this);
      // TODO: Should know what is the type of expressions value (string, int, bool or ident)
      Console.WriteLine(expr.Value);
      return expr;
    }
    public UnaryOperandNode VisitUnaryExpression(UnaryExpressionNode uen)
    {
      UnaryOperandNode op = uen.Operand.Visit(this);
      return op;
    }
    private UnaryOperandNode HandleIntegerOperation(int left, UnaryOperandNode right, string op)
    {
      if (right.Type == SymbolType.IntegerValue)
      {
        int rv = Int32.Parse(right.Value);
        return HandleIntegerBinaryExpression(left, rv, op);
      }
      else if (right.Type == SymbolType.Identifier)
      {
        SymbolTableEntry e = this.symbolTable.GetEntry(right.Value);
        if (e.Type == SymbolType.IntegerValue)
        {
          int rv = Int32.Parse(e.Value);
          return HandleIntegerBinaryExpression(left, rv, op);
        }
        // Otherwise Type error
      }
      // Type error
      return new UnaryOperandNode("", SymbolType.Invalid);
    }
    private UnaryOperandNode HandleStringOperation(string left, UnaryOperandNode right, string op)
    {
      if (right.Type == SymbolType.StringValue)
      {
        return HandleStringBinaryExpression(left, right.Value, op);
      }
      else if (right.Type == SymbolType.Identifier)
      {
        SymbolTableEntry e = this.symbolTable.GetEntry(right.Value);
        if (e.Type == SymbolType.StringValue)
        {
          return HandleStringBinaryExpression(left, e.Value, op);
        }
        // Otherwise Type error
      }
      // Type error
      return new UnaryOperandNode("", SymbolType.Invalid);
    }
    private UnaryOperandNode ExecuteOperation(UnaryOperandNode left, UnaryOperandNode right, string op)
    {
      if (left.Type == SymbolType.IntegerValue)
      {
        int lv = Int32.Parse(left.Value);
        return HandleIntegerOperation(lv, right, op);
      }
      else if (left.Type == SymbolType.StringValue)
      {
        return HandleStringOperation(left.Value, right, op);
      }
      else if (left.Type == SymbolType.Identifier)
      {
        SymbolTableEntry l = this.symbolTable.GetEntry(left.Value);
        if (l.Type == SymbolType.IntegerValue)
        {
          int lv = Int32.Parse(l.Value);
          return HandleIntegerOperation(lv, right, op);
        }
        else if (l.Type == SymbolType.StringValue)
        {
          return HandleStringOperation(l.Value, right, op);
        }
        // TODO: Handle booleans
      }
      // Type Error: UnaryOperand can't have other than IntegerValue, StringValue or Identifier
      return new UnaryOperandNode("", SymbolType.Invalid);
    }
    public UnaryOperandNode VisitBinaryExpression(BinaryExpressionNode ben)
    {
      UnaryOperandNode left = ben.Left.Visit(this);
      UnaryOperandNode right = ben.Right.Visit(this);
      string op = ben.Operator;
      return ExecuteOperation(left, right, op);
    }
    private UnaryOperandNode HandleStringBinaryExpression(string left, string right, string op)
    {
      string value;
      switch (op)
      {
        case "+":
          value = $"{left}{right}";
          return new UnaryOperandNode(value, SymbolType.StringValue);
        default:
          return new UnaryOperandNode("ERROR", SymbolType.Invalid);
      }
    }
    private UnaryOperandNode HandleIntegerBinaryExpression(int left, int right, string op)
    {
      string value;
      int result;
      bool comparison;
      switch (op)
      {
        case "+":
          result = left + right;
          value = result.ToString();
          return new UnaryOperandNode(value, SymbolType.IntegerValue);
        case "-":
          result = left - right;
          value = result.ToString();
          return new UnaryOperandNode(value, SymbolType.IntegerValue);
        case "<":
          comparison = left < right;
          if (comparison) return new UnaryOperandNode("true", SymbolType.Boolean);
          else return new UnaryOperandNode("false", SymbolType.Boolean);
        case "<=":
          comparison = left <= right;
          if (comparison) return new UnaryOperandNode("true", SymbolType.Boolean);
          else return new UnaryOperandNode("false", SymbolType.Boolean);
        case ">":
          comparison = left > right;
          if (comparison) return new UnaryOperandNode("true", SymbolType.Boolean);
          else return new UnaryOperandNode("false", SymbolType.Boolean);
        case ">=":
          comparison = left >= right;
          if (comparison) return new UnaryOperandNode("true", SymbolType.Boolean);
          else return new UnaryOperandNode("false", SymbolType.Boolean);
        case "=":
          comparison = left == right;
          if (comparison) return new UnaryOperandNode("true", SymbolType.Boolean);
          else return new UnaryOperandNode("false", SymbolType.Boolean);
        default:
          return new UnaryOperandNode("ERROR", SymbolType.Invalid);
      }
    }
    public UnaryOperandNode VisitUnaryOperand(UnaryOperandNode uon)
    {
      if (uon.Type == SymbolType.Identifier)
      {
        // Search the value of identifier from symboltable
        SymbolTableEntry e = this.symbolTable.GetEntry(uon.Value);
        // If no entry, returns { Value: "ERROR", Type: Invalid, Value: "error" }
        return new UnaryOperandNode(e.Value, e.Type);
      }
      return uon;
    }
    public UnaryOperandNode VisitExpressionOperand(ExpressionOperandNode eon)
    {
      UnaryOperandNode value = eon.Expression.Visit(this);
      return value;
    }
    public UnaryOperandNode VisitAssignment(AssignmentNode an)
    {
      UnaryOperandNode o = an.Expression.Visit(this);
      // public void ModifyEntry(string id, string newValue, SymbolType type)
      this.symbolTable.ModifyEntry(an.Identifier, o.Value);
      return o;
    }
    public UnaryOperandNode VisitDeclaration(DeclarationNode dn)
    {
      // Initial value is ""
      SymbolTableEntry e = new SymbolTableEntry(dn.Identifier, dn.Type, "");
      this.symbolTable.AddEntry(e);
      // Console.WriteLine($"{spaces}Declaration: (\n{spaces}  Identifier: {dn.Identifier},\n{spaces}  Type: {dn.Type}\n{spaces})");
      return new UnaryOperandNode("", SymbolType.Invalid);
    }
    public UnaryOperandNode VisitForLoop(ForLoopNode fln)
    {
      this.symbolTable.PrintTable();
      this.symbolTable.PushBlock();
      while (true)
      {
        this.symbolTable.PrintTable();
        UnaryOperandNode ass = fln.Assignment.Visit(this);
        UnaryOperandNode cond = fln.Condition.Visit(this);
        if (cond.Value == "false" && cond.Type == SymbolType.Boolean) break;

        // Execute the code in block
        VisitStatements(fln);

        // Have to increase the value of the identifier in assignment by one.
        UnaryOperandNode res = HandleIntegerBinaryExpression(Int32.Parse(ass.Value), 1, "+");
        ass.Value = res.Value;
        // break;
      }
      this.symbolTable.PopBlock();
      return new UnaryOperandNode("", SymbolType.Invalid);
    }
    public UnaryOperandNode VisitAssert(AssertNode an)
    {
      UnaryOperandNode expr = an.Expression.Visit(this);
      return expr;
    }
    public UnaryOperandNode VisitRead(ReadNode rn)
    {
      rn.Value = Console.ReadLine();
      // public void ModifyEntry(string id, string newValue, SymbolType type)
      this.symbolTable.ModifyEntry(rn.Identifier, rn.Value);
      // Console.WriteLine($"{spaces}  Identifier: {rn.Identifier},\n{spaces}  Value: {rn.Value}");
      return new UnaryOperandNode("", SymbolType.Invalid);
    }
  }
}*/