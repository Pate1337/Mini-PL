using System;

namespace Interpreter
{
  public enum SymbolType
  {
    Colon,
    Assignment,
    SemiColon,
    IntegerValue,
    StringValue,
    Identifier,
    WhiteSpace,
    NewLine,
    Variable,
    Integer,
    String,
    Boolean,
    Operator,
    Exclamation,
    Minus,
    Range,
    LeftParenthesis,
    RightParenthesis,
    Comment,
    MultilineComment,
    For,
    End,
    In,
    Do,
    Read,
    Print,
    Assert,
    Invalid,
    EOF
  }
}