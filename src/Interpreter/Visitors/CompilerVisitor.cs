/*using System;
using System.Collections.Generic;

namespace Interpreter
{
  public class IntegerChecker
  {
    public int Value { get; set; }
    public bool Valid { get; set; }

    public IntegerChecker(int v, bool valid)
    {
      this.Value = v;
      this.Valid = valid;
    }
  }

  public class CompilerVisitor : Visitor
  {
    private SymbolTable symbolTable;
    private List<Error> errors;

    public CompilerVisitor()
    {
      this.symbolTable = new SymbolTable();
      this.errors = new List<Error>();
    }

    public void VisitProgram(BlockNode prog)
    {
      this.symbolTable.PushBlock();
      VisitStatements(prog);
      this.symbolTable.PopBlock();
      PrintErrors();
    }

    private void PrintErrors()
    {
      foreach (Error e in this.errors)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e);
        Console.ResetColor();
      }
    }

    private void VisitStatements(BlockNode node)
    {
      foreach (Node stmt in node.Statements)
      {
        stmt.Visit(this);
      }
    }

    public UnaryOperandNode VisitDeclaration(DeclarationNode dn)
    {
      string defaultValue = "";
      switch (dn.Type)
      {
        case SymbolType.IntegerValue: defaultValue = "0"; break;
        default: defaultValue = ""; break;
      }
      if (!this.symbolTable.AddEntry(new SymbolTableEntry(dn.Identifier, dn.Type, defaultValue)))
      {
        this.errors.Add(new DeclarationError($"Variable {dn.Identifier} has already been declared", dn.Token));
      }
      return new UnaryOperandNode("", SymbolType.Invalid);
    }

    public UnaryOperandNode VisitPrint(PrintNode prn)
    {
      UnaryOperandNode expr = prn.Expression.Visit(this);
      if (expr.Type != SymbolType.Invalid)
      {
        Console.WriteLine(expr.Value);
      }
      return expr;
    }

    public UnaryOperandNode VisitUnaryExpression(UnaryExpressionNode uen)
    {
      UnaryOperandNode node = uen.Left.Visit(this);
      return node;
    }

    public UnaryOperandNode VisitBinaryExpression(BinaryExpressionNode ben)
    {
      UnaryOperandNode left = ben.Left.Visit(this);
      UnaryOperandNode right = ben.Right.Visit(this);

      if (left.Type == SymbolType.Invalid || right.Type == SymbolType.Invalid)
      {
        // Do not show meaningless error messages
        return new UnaryOperandNode("", SymbolType.Invalid);
      }
      UnaryOperandNode result = HandleOperation(left, right, ben.Operator);
  
      // Does not matter here what the value is. Only Type matters.
      // return new UnaryOperandNode("", left.Type, left.Token);
      return result;
    }

    private UnaryOperandNode HandleOperation(UnaryOperandNode left, UnaryOperandNode right, string operation)
    {
      if (left.Type == right.Type && left.Type == SymbolType.StringValue)
      {
        return HandleStringOperation(left, right, operation);
      }
      if (left.Type == right.Type && left.Type == SymbolType.IntegerValue)
      {
        return HandleIntegerOperation(left, right, operation);
      }
      if (left.Type == right.Type && left.Type == SymbolType.Boolean)
      {
        return HandleBooleanOperation(left, right, operation);
      }
      if (left.Type != right.Type)
      {
        this.errors.Add(new Error($"Illegal Operation: Can not do operation between {left.Type} and {right.Type}", left.Token));
        return new UnaryOperandNode("", SymbolType.Invalid);
      }
      return new UnaryOperandNode("", SymbolType.Invalid);
    }

    private UnaryOperandNode HandleBooleanOperation(UnaryOperandNode left, UnaryOperandNode right, string operation)
    {
      return new UnaryOperandNode("", SymbolType.Invalid);
    }

    private UnaryOperandNode HandleStringOperation(UnaryOperandNode left, UnaryOperandNode right, string operation)
    {
      switch (operation)
      {
        case "+":
          // This operation is ok (String concat)
          return new UnaryOperandNode(left.Value + right.Value, SymbolType.StringValue, left.Token);
        default:
          this.errors.Add(new Error($"Illegal Operation: Strings can only be concatenated with + operator", left.Token));
          // TODO: Maybe token here should be the operator token.
          return new UnaryOperandNode("", SymbolType.Invalid, left.Token);
      }
    }
    private UnaryOperandNode HandleIntegerOperation(UnaryOperandNode left, UnaryOperandNode right, string operation)
    {
      // TODO: These need to be converted, but no need to check again..
      IntegerChecker leftCheck = ConvertStringToInteger(left.Value, left.Token);
      IntegerChecker rightCheck = ConvertStringToInteger(right.Value, right.Token);
      if (!leftCheck.Valid) return new UnaryOperandNode("", SymbolType.Invalid, left.Token);
      if (!rightCheck.Valid) return new UnaryOperandNode("", SymbolType.Invalid, right.Token);
      int leftValue = leftCheck.Value;
      int rightValue = rightCheck.Value;
      int result;
      switch (operation)
      {
        case "+":
          if (rightValue > Int32.MaxValue - leftValue)
          {
            // The result will not fit 32 Bits
            this.errors.Add(new Error($"Max Integer value reached ({Int32.MaxValue})", left.Token));
            return new UnaryOperandNode("", SymbolType.Invalid, left.Token);
          }
          result = leftValue + rightValue;
          return new UnaryOperandNode(result.ToString(), SymbolType.IntegerValue, left.Token);
        case "-":
          if (leftValue < Int32.MinValue + rightValue)
          {
            // The result will not fit 32 Bits
            this.errors.Add(new Error($"Min Integer value reached ({Int32.MinValue})", left.Token));
            return new UnaryOperandNode("", SymbolType.Invalid, left.Token);
          }
          result = leftValue - rightValue;
          return new UnaryOperandNode(result.ToString(), SymbolType.IntegerValue, left.Token);
        default:
          return new UnaryOperandNode("", SymbolType.Invalid);
      }
    }

    private IntegerChecker ConvertStringToInteger(string value, Token token)
    {
      int result;
      try
      {
        result = Int32.Parse(value);
        return new IntegerChecker(result, true);
      }
      catch (Exception e)
      {
        // Value is too big
        this.errors.Add(new Error($"{value} is too big to be an Integer (Max {Int32.MaxValue})", token));
        return new IntegerChecker(0, false);
      }
    }

    private UnaryOperandNode HandleUnaryOperand(UnaryOperandNode uon)
    {
      if (uon.Type == SymbolType.Identifier)
      {
        SymbolTableEntry entry = CheckThatIdentifierIsDeclaredAndGetEntry(uon.Value, uon.Token);
        if (entry.Type != SymbolType.Invalid)
        {
          return new UnaryOperandNode(entry.Value, entry.Type, uon.Token);
        }
        else
        {
          return new UnaryOperandNode("", SymbolType.Invalid, uon.Token);
        }
      }
      else if (uon.Type == SymbolType.IntegerValue)
      {
        IntegerChecker check = ConvertStringToInteger(uon.Value, uon.Token);
        if (check.Valid) return uon;
        // Could add a SymbolType.NaN and handle Type == NaN in assignments
        else return new UnaryOperandNode("", SymbolType.Invalid);
      }
      return uon;
    }

    public UnaryOperandNode VisitUnaryOperand(UnaryOperandNode uon)
    {
      return HandleUnaryOperand(uon);
    }

    public UnaryOperandNode VisitExpressionOperand(ExpressionOperandNode eon)
    {
      return eon.Expression.Visit(this);
    }

    public UnaryOperandNode VisitAssignment(AssignmentNode an)
    {
      UnaryOperandNode node = an.Expression.Visit(this);

      bool expressionIsValid = node.Type != SymbolType.Invalid;

      // Could add UnaryOperand to AssignmentNode, then this would not need to be done
      SymbolType type = CheckThatIdentifierIsDeclaredAndGetEntry(an.Identifier, an.Token).Type;

      bool identifierIsValid = type != SymbolType.Invalid;

      if (type != node.Type && expressionIsValid && identifierIsValid)
      {
        this.errors.Add(new Error($"Illegal Assignment: Can not assign {node.Type} to variable {an.Identifier}, because it is a {type}", node.Token));
        return new UnaryOperandNode("", SymbolType.Invalid, an.Token);
      }
      if (type == node.Type && expressionIsValid && identifierIsValid)
      {
        // Everything ok
        this.symbolTable.ModifyEntry(an.Identifier, node.Value);
        return node;
      }
      return new UnaryOperandNode("", SymbolType.Invalid, an.Token);
    }

    public UnaryOperandNode VisitForLoop(ForLoopNode fln)
    {
      UnaryOperandNode assignment = fln.Assignment.Visit(this);
      if (
        assignment.Type == SymbolType.Invalid &&
        this.errors[this.errors.Count - 1].Token == assignment.Token &&
        this.errors[this.errors.Count - 1].Type == "DECLARATION ERROR"
        )
      {
        Console.WriteLine("Deleting the last error");
        this.errors.RemoveAt(this.errors.Count - 1);
      }
      fln.Condition.Visit(this);
      this.symbolTable.PushBlock();
      VisitStatements(fln);
      this.symbolTable.PopBlock();
      return new UnaryOperandNode("", SymbolType.Invalid);
    }

    public UnaryOperandNode VisitAssert(AssertNode an)
    {
      return an.Expression.Visit(this);
    }

    public UnaryOperandNode VisitRead(ReadNode rn)
    {
      // Check that Identifier is declared
      SymbolType type = CheckThatIdentifierIsDeclaredAndGetEntry(rn.Identifier, rn.Token).Type;
      // Check that the type of identifier is same as inputs when interpreting
      return new UnaryOperandNode("", SymbolType.Invalid);
    }

    private SymbolTableEntry CheckThatIdentifierIsDeclaredAndGetEntry(string id, Token token)
    {
      SymbolTableEntry entry = this.symbolTable.GetEntry(id);
      if (entry.Type == SymbolType.Invalid)
      {
        this.errors.Add(new DeclarationError($"Variable {id} has not been declared.", token));
      }
      return entry;
    }
  }
}*/