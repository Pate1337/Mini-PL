using System;
using System.Text.RegularExpressions;

namespace Interpreter
{
  public enum AutomatonAction
  {
    Move,
    Recognize,
    Error
  }
  public class DFAutomaton
  {
    private int state;
    private AutomatonAction action;

    public DFAutomaton()
    {
      this.state = 0;
      this.action = AutomatonAction.Move;
    }
    public SymbolType HandleInput(char c)
    {
      if (this.state == 0) return State0(c.ToString());
      else if (this.state == 1) return State1(c.ToString());
      else if (this.state == 2) return State2();
      else if (this.state == 3) return State3();
      else if (this.state == 4) return State4(c.ToString());
      else if (this.state == 5) return State5(c.ToString());
      else if (this.state == 6) return State6(c.ToString());
      else if (this.state == 7) return State7();
      else if (this.state == 8) return State8();
      else if (this.state == 9) return State9();
      else if (this.state == 10) return State10(c.ToString());
      else if (this.state == 11) return State11();
      else if (this.state == 12) return State12();
      else if (this.state == 13) return State13();
      else if (this.state == 14) return State14(c.ToString());
      else if (this.state == 15) return State15(c.ToString());
      else if (this.state == 16) return State16(c.ToString());
      else if (this.state == 17) return State17(c.ToString());
      else if (this.state == 18) return State18();
      else if (this.state == 19) return State19();
      else if (this.state == 20) return State20(c.ToString());
      else if (this.state == 21) return State21();
      else if (this.state == 22) return State22();
      return SymbolType.Invalid;
    }
    private SymbolType State0(string c)
    {
      if (Regex.IsMatch(c, "\n|\r|\r\n"))
      {
        // NewLine
        this.action = AutomatonAction.Error;
        return SymbolType.NewLine;
      }
      else if (Regex.IsMatch(c, "\\s"))
      {
        // WhiteSpace needs to be returned separately
        this.action = AutomatonAction.Error;
        return SymbolType.WhiteSpace;
      }
      else if (Regex.IsMatch(c, ":"))
      {
        this.action = AutomatonAction.Move;
        this.state = 1;
      }
      else if (Regex.IsMatch(c, ";"))
      {
        this.action = AutomatonAction.Move;
        this.state = 3;
      }
      else if (Regex.IsMatch(c, "[a-zA-Z]"))
      {
        this.action = AutomatonAction.Move;
        this.state = 5;
      }
      else if (Regex.IsMatch(c, "[0-9]"))
      {
        this.action = AutomatonAction.Move;
        this.state = 4;
      }
      else if (Regex.IsMatch(c, "\""))
      {
        this.action = AutomatonAction.Move;
        this.state = 6;
      }
      else if (Regex.IsMatch(c, "\\+|\\*|\\=|\\&"))
      {
        this.action = AutomatonAction.Move;
        this.state = 9;
      }
      else if (Regex.IsMatch(c, "\\."))
      {
        this.action = AutomatonAction.Move;
        this.state = 10;
      }
      else if (Regex.IsMatch(c, "\\("))
      {
        this.action = AutomatonAction.Move;
        this.state = 12;
      }
      else if (Regex.IsMatch(c, "\\)"))
      {
        this.action = AutomatonAction.Move;
        this.state = 13;
      }
      else if (Regex.IsMatch(c, "\\/"))
      {
        this.action = AutomatonAction.Move;
        this.state = 14;
      }
      else if (Regex.IsMatch(c, "!"))
      {
        this.action = AutomatonAction.Move;
        this.state = 19;
      }
      else if (Regex.IsMatch(c, "\\<|\\>"))
      {
        this.action = AutomatonAction.Move;
        this.state = 20;
      }
      else if (Regex.IsMatch(c, "\\-|\\−"))
      {
        this.action = AutomatonAction.Move;
        this.state = 22;
      }
      else
      {
        this.action = AutomatonAction.Error;
      }
      return SymbolType.Invalid;
    }
    private SymbolType State1(string c)
    {
      if (Regex.IsMatch(c, "[^=]"))
      {
        this.action = AutomatonAction.Recognize;
        this.state = 0;
      }
      else
      {
        this.state = 2;
      }
      return SymbolType.Colon;
    }
    private SymbolType State2()
    {
      this.action = AutomatonAction.Recognize;
      this.state = 0;
      return SymbolType.Assignment;
    }
    private SymbolType State3()
    {
      this.action = AutomatonAction.Recognize;
      this.state = 0;
      return SymbolType.SemiColon;
    }
    private SymbolType State4(string c)
    {
      if (Regex.IsMatch(c, "[^0-9]"))
      {
        this.action = AutomatonAction.Recognize;
        this.state = 0;
      }
      return SymbolType.IntegerValue;
    }
    private SymbolType State5(string c)
    {
      if (Regex.IsMatch(c, "[^a-zA-Z_]"))
      {
        this.action = AutomatonAction.Recognize;
        this.state = 0;
      }
      return SymbolType.Identifier;
    }
    private SymbolType State6(string c)
    {
      if (Regex.IsMatch(c, "\""))
      {
        this.action = AutomatonAction.Move;
        this.state = 8;
      }
      else if (Regex.IsMatch(c, "\\\\"))
      {
        this.action = AutomatonAction.Move;
        this.state = 7;
      }
      else
      {
        this.action = AutomatonAction.Move;
        this.state = 6;
      }
      return SymbolType.Invalid;
    }
    private SymbolType State7()
    {
      // Escape
      this.action = AutomatonAction.Move;
      this.state = 6;
      return SymbolType.Invalid;
    }
    private SymbolType State8()
    {
      this.action = AutomatonAction.Recognize;
      this.state = 0;
      return SymbolType.StringValue;
    }
    private SymbolType State9()
    {
      this.action = AutomatonAction.Recognize;
      this.state = 0;
      return SymbolType.Operator;
    }
    private SymbolType State10(string c)
    {
      if (Regex.IsMatch(c, "\\."))
      {
        this.action = AutomatonAction.Move;
        this.state = 11;
      }
      else
      {
        this.action = AutomatonAction.Recognize;
        this.state = 0;
      }
      return SymbolType.Invalid;
    }
    private SymbolType State11()
    {
      this.action = AutomatonAction.Recognize;
      this.state = 0;
      return SymbolType.Range;
    }
    private SymbolType State12()
    {
      this.action = AutomatonAction.Recognize;
      this.state = 0;
      return SymbolType.LeftParenthesis;
    }
    private SymbolType State13()
    {
      this.action = AutomatonAction.Recognize;
      this.state = 0;
      return SymbolType.RightParenthesis;
    }
    private SymbolType State14(string c)
    {
      if (Regex.IsMatch(c, "\\/"))
      {
        this.action = AutomatonAction.Move;
        this.state = 15;
      }
      else if (Regex.IsMatch(c, "\\*"))
      {
        this.action = AutomatonAction.Move;
        this.state = 16;
      }
      else
      {
        this.action = AutomatonAction.Recognize;
        this.state = 0;
      }
      return SymbolType.Operator;
    }
    private SymbolType State15(string c)
    {
      if (Regex.IsMatch(c, "\n|\r|\r\n"))
      {
        this.action = AutomatonAction.Recognize;
        this.state = 0;
      }
      else
      {
        this.action = AutomatonAction.Move;
      }
      return SymbolType.Comment;
    }
    private SymbolType State16(string c)
    {
      if (Regex.IsMatch(c, "\\*"))
      {
        this.action = AutomatonAction.Move;
        this.state = 17;
      }
      else
      {
        this.action = AutomatonAction.Move;
      }
      return SymbolType.Invalid;
    }
    private SymbolType State17(string c)
    {
      if (Regex.IsMatch(c, "\\/"))
      {
        this.action = AutomatonAction.Move;
        this.state = 18;
      }
      else if (Regex.IsMatch(c, "\\*"))
      {
        this.action = AutomatonAction.Move;
      }
      else
      {
        this.action = AutomatonAction.Move;
        this.state = 16;
      }
      return SymbolType.Invalid;
    }
    private SymbolType State18()
    {
      this.action = AutomatonAction.Recognize;
      this.state = 0;
      return SymbolType.MultilineComment;
    }
    private SymbolType State19()
    {
      this.action = AutomatonAction.Recognize;
      this.state = 0;
      return SymbolType.Exclamation;
    }
    private SymbolType State20(string c)
    {
      if (Regex.IsMatch(c, "[^=]"))
      {
        this.action = AutomatonAction.Recognize;
        this.state = 0;
      }
      else
      {
        this.state = 21;
      }
      return SymbolType.Operator;
    }
    private SymbolType State21()
    {
      this.action = AutomatonAction.Recognize;
      this.state = 0;
      return SymbolType.Operator;
    }
    private SymbolType State22()
    {
      this.action = AutomatonAction.Recognize;
      this.state = 0;
      return SymbolType.Minus;
    }
    public AutomatonAction GetAction()
    {
      return this.action;
    }
  }
}
