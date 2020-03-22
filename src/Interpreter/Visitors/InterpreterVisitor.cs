using System;
using System.Collections.Generic;

namespace Interpreter
{

  public class InterpreterVisitor : Visitor
  {
    private SymbolTable symbolTable;
    private IOHandler io;

    public InterpreterVisitor(IOHandler io)
    {
      this.symbolTable = new SymbolTable();
      this.io = io;
    }

    public void VisitProgram(BlockNode prog)
    {
      this.symbolTable.PushBlock();
      VisitStatements(prog);
      this.symbolTable.PopBlock();
    }

    private void VisitStatements(BlockNode node)
    {
      foreach (Statement stmt in node.Statements) stmt.Visit(this);
    }

    public void VisitDeclaration(DeclarationNode dn)
    {
      string defaultValue = "";
      switch (dn.Type)
      {
        case SymbolType.IntegerValue: defaultValue = "0"; break;
        default: defaultValue = ""; break;
      }
      if (!this.symbolTable.AddEntry(new SymbolTableEntry(dn.Identifier, dn.Type, defaultValue)))
      {
        throw new DeclarationError($"Variable {dn.Identifier} has already been declared", dn.Token);
      }
    }

    public void VisitPrint(PrintNode prn)
    {
      UnaryOperandNode expr = prn.Expression.Visit(this);
      this.io.WriteLine(expr.Value);
    }

    public UnaryOperandNode VisitUnaryExpression(UnaryExpressionNode uen)
    {
      return uen.Left.Visit(this);
    }

    public UnaryOperandNode VisitBinaryExpression(BinaryExpressionNode ben)
    {
      UnaryOperandNode left = ben.Left.Visit(this);
      UnaryOperandNode right = ben.Right.Visit(this);
      return HandleOperation(left, right, ben.Operator);
    }

    public UnaryOperandNode VisitUnaryOperand(UnaryOperandNode uon)
    {
      if (uon.Type == SymbolType.Identifier)
      {
        SymbolTableEntry entry = CheckThatIdentifierIsDeclaredAndGetEntry(uon.Value, uon.Token);
        if (entry.Type == SymbolType.IntegerValue && uon.Negative)
        {
          int v = ConvertStringToInteger(entry.Value, uon.Token);
          return new UnaryOperandNode(OppositeOfInteger(v), entry.Type, uon.Token);
        }
        else if (entry.Type == SymbolType.Boolean && uon.Not)
        {
          string newValue;
          if (entry.Value == "true") newValue = "false";
          else newValue = "true";
          return new UnaryOperandNode(newValue, SymbolType.Boolean, uon.Token);
        }
        return new UnaryOperandNode(entry.Value, entry.Type, uon.Token);
      }
      else if (uon.Type == SymbolType.IntegerValue)
      {
        int value = ConvertStringToInteger(uon.Value, uon.Token);
        return new UnaryOperandNode(value.ToString(), uon.Type, uon.Token, uon.Negative);
      }
      // Do not return the AST node. Create a copy
      return new UnaryOperandNode(uon.Value, uon.Type, uon.Token, uon.Negative);
    }

    public UnaryOperandNode VisitExpressionOperand(ExpressionOperandNode eon)
    {
      UnaryOperandNode node = eon.Expression.Visit(this);
      if (node.Type == SymbolType.IntegerValue)
      {
        if (eon.Not) throw new Error($"Can not have ! in front of an expression that returns {node.Type}", node.Token);
        if (eon.Negative)
        {
          int v = ConvertStringToInteger(node.Value, node.Token);
          node.Value = OppositeOfInteger(v);
        }
      }
      else if (node.Type == SymbolType.Boolean)
      {
        if (eon.Negative) throw new Error($"Can not have a - in front of an expression that returns {node.Type}", node.Token);
        if (eon.Not)
        {
          if (node.Value == "true") node.Value = "false";
          else node.Value = "true";
        }
      }
      else
      {
        if (eon.Negative) throw new Error($"Can not have a - in front of an expression that returns {node.Type}", node.Token);
        if (eon.Not) throw new Error($"Can not have ! in front of an expression that returns {node.Type}", node.Token);
      }
      return node;
    }

    public void VisitAssignment(AssignmentNode an)
    {
      SymbolType typeOfIdentifier = CheckThatIdentifierIsDeclaredAndGetEntry(an.Identifier, an.Token).Type;

      UnaryOperandNode expression = an.Expression.Visit(this);

      if (typeOfIdentifier != expression.Type)
      {
        throw new Error($"Illegal Assignment: Can not assign {expression.Type} to variable {an.Identifier}, because it is a {typeOfIdentifier}", expression.Token);
      }
      if (!this.symbolTable.ModifyEntry(an.Identifier, expression.Value))
      {
        throw new Error($"Illegal Assignment: Can not assign to variable {an.Identifier}, because it is used in For Loop", expression.Token);
      }
    }

    public void VisitForLoop(ForLoopNode fln)
    {
      fln.Assignment.Visit(this);
      string op = CreateOperatorToForLoopCondition(fln);
      UnaryOperandNode cond = fln.Condition.Visit(this);
      UnaryOperandNode assignment = new UnaryOperandNode(
        this.symbolTable.GetEntry(cond.Token.Value).Value,
        SymbolType.IntegerValue,
        cond.Token
        );
      this.symbolTable.LockVariable(cond.Token.Value);
      while (cond.Type == SymbolType.Boolean && cond.Value == "true")
      {
        this.symbolTable.PushBlock();
        VisitStatements(fln);
        this.symbolTable.PopBlock();

        // Left value of BinaryExpression needs to be incremented by one (Has to be Identifier)
        assignment = HandleIntegerOperation(
          assignment,
          new UnaryOperandNode("1", SymbolType.IntegerValue, assignment.Token),
          op
        );
        this.symbolTable.ForceModifyEntry(cond.Token.Value, assignment.Value);
        cond = fln.Condition.Visit(this);
      }
      this.symbolTable.ReleaseVariable(cond.Token.Value);
    }

    public void VisitAssert(AssertNode an)
    {
      UnaryOperandNode node = an.Expression.Visit(this);
      if (node.Type == SymbolType.Boolean && node.Value == "false")
      {
        UnaryOperandNode left = an.Expression.Left.Visit(this);
        UnaryOperandNode right = an.Expression.Right.Visit(this);
        string op;
        string message = "Assert was false.";
        if (right.Type != SymbolType.Invalid)
        {
          // BinaryExpression
          op = an.Expression.Operator;
          message = $"Assert was false. Received assert ({left.Value} {op} {right.Value})";
        }
        throw new Error(message, node.Token);
      }
      if (node.Type != SymbolType.Boolean)
      {
        throw new Error($"Expected Boolean value inside assert (Boolean). Instead found {node.Type}", node.Token);
      }
    }

    public void VisitRead(ReadNode rn)
    {
      SymbolType type = CheckThatIdentifierIsDeclaredAndGetEntry(rn.Identifier, rn.Token).Type;
      string input = this.io.ReadLine();
      if (type == SymbolType.IntegerValue) input = ConvertStringToInteger(input, rn.Token).ToString();
      this.symbolTable.ModifyEntry(rn.Identifier, input);
    }






    /***************** HELPER METHODS *************************/

    private string CreateOperatorToForLoopCondition(ForLoopNode fln)
    {
      UnaryOperandNode left = fln.Condition.Left.Visit(this);
      UnaryOperandNode right = fln.Condition.Right.Visit(this);
      UnaryOperandNode res = HandleIntegerOperation(left, right, ">");
      string op = "+";
      if (res.Value == "true")
      {
        fln.Condition.Operator = ">=";
        op = "-";
      }
      else
      {
        fln.Condition.Operator = "<=";
      }
      return op;
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
        return HandleDifferentTypeOperation(left, right, operation);
      }
      throw new Error($"Unexpected Error occured..", left.Token);
    }

    private UnaryOperandNode HandleDifferentTypeOperation(UnaryOperandNode left, UnaryOperandNode right, string operation)
    {
      switch (operation)
      {
        case "+":
          if (left.Type == SymbolType.StringValue || right.Type == SymbolType.StringValue)
          {
            return new UnaryOperandNode($"{left.Value}{right.Value}", SymbolType.StringValue, left.Token);
          }
          throw new Error($"Illegal Operation: Can not add {left.Type} to {right.Type}", left.Token);
        default:
          throw new Error($"Illegal Operation: Can not do operation {operation} between {left.Type} and {right.Type}", left.Token);
      }
    }

    private UnaryOperandNode HandleBooleanOperation(UnaryOperandNode left, UnaryOperandNode right, string operation)
    {
      switch (operation)
      {
        case "=":
          if (left.Value == right.Value) return new UnaryOperandNode("true", SymbolType.Boolean, left.Token);
          else return new UnaryOperandNode("false", SymbolType.Boolean, left.Token);
        case "&":
          if (left.Value == right.Value && left.Value == "true")
          {
            return new UnaryOperandNode("true", SymbolType.Boolean, left.Token);
          }
          else return new UnaryOperandNode("false", SymbolType.Boolean, left.Token);
        default:
          throw new Error($"Illegal Operation: Can not do operation {operation} between Booleans", left.Token);
      }
    }

    private UnaryOperandNode HandleStringOperation(UnaryOperandNode left, UnaryOperandNode right, string operation)
    {
      switch (operation)
      {
        case "+":
          return new UnaryOperandNode(left.Value + right.Value, SymbolType.StringValue, left.Token);
        case "=":
          if (left.Value == right.Value) return new UnaryOperandNode("true", SymbolType.Boolean, left.Token);
          return new UnaryOperandNode("false", SymbolType.Boolean, left.Token);
        default:
          throw new Error($"Illegal Operation: Strings can only be concatenated with + operator", left.Token);
      }
    }

    private UnaryOperandNode HandleIntegerOperation(UnaryOperandNode left, UnaryOperandNode right, string operation)
    {
      int leftValue = ConvertStringToInteger(left.Value, left.Token);
      int rightValue = ConvertStringToInteger(right.Value, right.Token);
      int result;
      switch (operation)
      {
        case "+":
          result = leftValue + rightValue;
          return new UnaryOperandNode(result.ToString(), SymbolType.IntegerValue, left.Token);
        case "-":
          result = leftValue - rightValue;
          return new UnaryOperandNode(result.ToString(), SymbolType.IntegerValue, left.Token);
        case "*":
          result = leftValue * rightValue;
          return new UnaryOperandNode(result.ToString(), SymbolType.IntegerValue, left.Token);
        case "/":
          result = leftValue / rightValue;
          return new UnaryOperandNode(result.ToString(), SymbolType.IntegerValue, left.Token);
        case "<":
          if (leftValue < rightValue) return new UnaryOperandNode("true", SymbolType.Boolean, left.Token);
          return new UnaryOperandNode("false", SymbolType.Boolean, left.Token);
        case ">":
          if (leftValue > rightValue) return new UnaryOperandNode("true", SymbolType.Boolean, left.Token);
          return new UnaryOperandNode("false", SymbolType.Boolean, left.Token);
        case "<=":
          if (leftValue <= rightValue) return new UnaryOperandNode("true", SymbolType.Boolean, left.Token);
          return new UnaryOperandNode("false", SymbolType.Boolean, left.Token);
        case ">=":
          if (leftValue >= rightValue) return new UnaryOperandNode("true", SymbolType.Boolean, left.Token);
          return new UnaryOperandNode("false", SymbolType.Boolean, left.Token);
        case "=":
          if (leftValue == rightValue) return new UnaryOperandNode("true", SymbolType.Boolean, left.Token);
          return new UnaryOperandNode("false", SymbolType.Boolean, left.Token);
        default:
          throw new Error($"Can not do operation {operation} between two Integers", left.Token);
      }
    }

    private int ConvertStringToInteger(string value, Token token)
    {
      try
      {
        return Integer.Parse(value);
      }
      catch (OverflowException)
      {
        throw new Error($"{value} is too big to be an Integer (Max {Int32.MaxValue})", token);
      }
      catch (FormatException)
      {
        throw new Error($"{value} can not be assigned to an Integer variable", token);
      }
      catch (Exception)
      {
        throw new Error($"Something went wrong assigning {value} to an Integer variable", token);
      }
    }

    private string OppositeOfInteger(int value)
    {
      int result = value * -1;
      return result.ToString();
    }

    private SymbolTableEntry CheckThatIdentifierIsDeclaredAndGetEntry(string id, Token token)
    {
      SymbolTableEntry entry = this.symbolTable.GetEntry(id);
      if (entry.Type == SymbolType.Invalid)
      {
        throw new DeclarationError($"Variable {id} has not been declared.", token);
      }
      return entry;
    }
  }
}