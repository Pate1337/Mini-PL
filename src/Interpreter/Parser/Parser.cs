using System;

namespace Interpreter
{
  public class Parser
  {
    private Scanner scanner;
    private Token token;

    public BlockNode Parse(string text)
    {
      this.scanner = new Scanner(text);
      BlockNode root = Program();
      return root;
    }
    private BlockNode Program()
    {
      ProgramNode pn = new ProgramNode();
      this.token = this.scanner.NextToken();
      Statements(pn);
      if (this.token.SymbolType != SymbolType.EOF) throw new Error($"Unexpected character: {this.token.SymbolType}", this.token);
      return pn;
    }
    private void Statements(BlockNode root)
    {
      while (NewStatement())
      {
        Statement(root);
        Match(SymbolType.SemiColon);
      }
    }
    private bool NewStatement()
    {
      return
      (
        this.token.SymbolType == SymbolType.Variable
        || this.token.SymbolType == SymbolType.Identifier
        || this.token.SymbolType == SymbolType.For
        || this.token.SymbolType == SymbolType.Read
        || this.token.SymbolType == SymbolType.Print
        || this.token.SymbolType == SymbolType.Assert
      );
    }
    private void Statement(BlockNode pn)
    {
      Expression expr;
      Token identifier;
      switch (this.token.SymbolType)
      {
        case SymbolType.Variable:
          // <stmt> ::= "var" <var_ident> ":" <type> [ ":=" <expr> ]
          this.token = this.scanner.NextToken();
          identifier = this.token;
          Match(SymbolType.Identifier);
          Match(SymbolType.Colon);
          SymbolType type = Type();
          pn.AddStatement(new DeclarationNode(identifier.Value, type, identifier));
          if (this.token.SymbolType == SymbolType.Assignment)
          {
            this.token = this.scanner.NextToken();
            expr = Expression();
            pn.AddStatement(new AssignmentNode(identifier.Value, expr, identifier));
          }
          break;
        case SymbolType.Identifier:
          // <stmt> ::= <var_ident> ":=" <expr>
          identifier = this.token;
          this.token = this.scanner.NextToken();
          Match(SymbolType.Assignment);
          expr = Expression();
          pn.AddStatement(new AssignmentNode(identifier.Value, expr, identifier));
          break;
        case SymbolType.For:
          // <stmt> ::= "for" <var_ident> "in" <expr> ".." <expr> "do" <stmts> "end" "for"
          this.token = this.scanner.NextToken();
          identifier = this.token;
          string forLoopVar = identifier.Value;
          Match(SymbolType.Identifier);
          Match(SymbolType.In);
          expr = Expression();
          AssignmentNode assignment = new AssignmentNode(identifier.Value, expr, identifier);
          Match(SymbolType.Range);
          expr = Expression();
          BinaryExpressionNode condition = new BinaryExpressionNode(
            new UnaryOperandNode(identifier.Value, SymbolType.Identifier, identifier),
            "to",
            new ExpressionOperandNode(expr) // BinaryExpression expects Operands
          );
          ForLoopNode fln = new ForLoopNode(assignment, condition, forLoopVar);
          Match(SymbolType.Do);
          Statements(fln);
          Match(SymbolType.End);
          Match(SymbolType.For);
          pn.AddStatement(fln);
          break;
        case SymbolType.Read:
          // <stmt> ::= "read" <var_ident>
          this.token = this.scanner.NextToken();
          identifier = this.token;
          Match(SymbolType.Identifier);
          pn.AddStatement(new ReadNode(identifier.Value, "", identifier));
          break;
        case SymbolType.Print:
          // <stmt> ::= "print" <expr>
          this.token = this.scanner.NextToken();
          expr = Expression();
          pn.AddStatement(new PrintNode(expr));
          break;
        case SymbolType.Assert:
          // <stmt> ::= "assert" "(" <expr> ")"
          this.token = this.scanner.NextToken();
          Match(SymbolType.LeftParenthesis);
          expr = Expression();
          Match(SymbolType.RightParenthesis);
          pn.AddStatement(new AssertNode(expr));
          break;
        default:
          throw new Error($"Statement can not start with {this.token.SymbolType}", this.token);
      }
    }
    private Expression Expression()
    {
      // <expr> ::= <opnd> <op> <opnd> | <opnd>
      Operand left;
      Expression e;
      left = Operand();
      e = new UnaryExpressionNode(left);
      if (this.token.SymbolType == SymbolType.Operator || this.token.SymbolType == SymbolType.Minus)
      {
        string op = this.token.Value;
        this.token = this.scanner.NextToken();
        Operand right = Operand();
        e = new BinaryExpressionNode(left, op, right);
      }
      return e;
    }
    private Operand Operand()
    {
      // <opnd> ::= [Minus] <int> | <string> | [Minus | Exclamation] <var_ident> | [Minus | Exclamation] "(" expr ")"
      bool negative = false;
      bool not = false;
      Token minus = this.token;
      if (this.token.SymbolType == SymbolType.Minus)
      {
        negative = true;
        this.token = this.scanner.NextToken();
      }
      else if (this.token.SymbolType == SymbolType.Exclamation)
      {
        not = true;
        this.token = this.scanner.NextToken();
      }
      Operand o;
      switch (this.token.SymbolType)
      {
        case SymbolType.IntegerValue:
          if (not) throw new Error($"! is not allowed in front of an IntegerValue", minus);
          if (negative) o = new UnaryOperandNode($"{Integer.minus}{this.token.Value}", this.token.SymbolType, minus);
          else o = new UnaryOperandNode(this.token.Value, this.token.SymbolType, this.token);
          this.token = this.scanner.NextToken();
          return o;
        case SymbolType.StringValue:
          if (not) throw new Error($"! is not allowed in front of a StringValue", minus);
          if (negative) throw new Error($"- is not allowed in front of a StringValue", minus);
          o = new UnaryOperandNode(this.token.Value, this.token.SymbolType, this.token);
          this.token = this.scanner.NextToken();
          return o;
        case SymbolType.Identifier:
          if (not) o = new UnaryOperandNode(this.token.Value, this.token.SymbolType, this.token, false, true);
          else if (negative) o = new UnaryOperandNode(this.token.Value, this.token.SymbolType, this.token, true);
          else o = new UnaryOperandNode(this.token.Value, this.token.SymbolType, this.token);
          this.token = this.scanner.NextToken();
          return o;
        case SymbolType.LeftParenthesis:
          this.token = this.scanner.NextToken();
          Expression e = Expression();
          if (negative) o = new ExpressionOperandNode(e, true);
          else if (not) o = new ExpressionOperandNode(e, false, true);
          else o = new ExpressionOperandNode(e);
          Match(SymbolType.RightParenthesis);
          return o;
        default:
          throw new Error($"Unexpected token: {this.token.SymbolType}", this.token);
      }
    }
    private SymbolType Type()
    {
      SymbolType type;
      switch (this.token.SymbolType)
      {
        case SymbolType.Integer: type = SymbolType.IntegerValue; break;
        case SymbolType.String: type = SymbolType.StringValue; break;
        case SymbolType.Boolean: type = SymbolType.Boolean; break;
        default:
          throw new Error($"Expected Integer, String or Boolean, not {this.token.SymbolType}", this.token);
      }
      this.token = this.scanner.NextToken();
      return type;
    }
    private void Match(SymbolType expected)
    {
      if (expected == this.token.SymbolType)
      {
        this.token = this.scanner.NextToken();
      }
      else
      {
        throw new Error($"Expected {expected}, not {this.token.SymbolType}", this.token);
      }
    }
  }
}