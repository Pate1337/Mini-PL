using System;
using System.Text.RegularExpressions;

namespace Interpreter
{
  public class Scanner
  {
    private Token[] keywordTable = {
      new Token(SymbolType.Variable, "var"),
      new Token(SymbolType.Integer, "int"),
      new Token(SymbolType.String, "string"),
      new Token(SymbolType.Boolean, "bool"),
      new Token(SymbolType.For, "for"),
      new Token(SymbolType.End, "end"),
      new Token(SymbolType.In, "in"),
      new Token(SymbolType.Do, "do"),
      new Token(SymbolType.Read, "read"),
      new Token(SymbolType.Print, "print"),
      new Token(SymbolType.Assert, "assert")
    };
    private ScanBuffer buffer;
    private DFAutomaton automaton;
    private int lineNumber;
    private int charIndex;
    private int columnIndex;

    public Scanner(string text)
    {
      this.buffer = new ScanBuffer(text);
      this.automaton = new DFAutomaton();
      this.lineNumber = 1;
      this.charIndex = 0;
      this.columnIndex = 0;
    }
    public Token NextToken()
    {
      if (this.buffer.IsEmpty()) return new Token(SymbolType.EOF, "");
      Token token = new Token(SymbolType.WhiteSpace, " ");
      // Skip WhiteSpaces, NewLines, Comments and MultilineComments
      while (token.SymbolType == SymbolType.WhiteSpace
      || token.SymbolType == SymbolType.NewLine
      || token.SymbolType == SymbolType.MultilineComment
      || token.SymbolType == SymbolType.Comment)
      {
        token = RunAutomaton();
        HandleMultilines(token);
      }
      if (token.SymbolType == SymbolType.Identifier)
      {
        foreach (Token t in this.keywordTable)
        {
          if (t.Value == token.Value)
          {
            token.SymbolType = t.SymbolType;
          }
        }
      }
      return token;
    }
    private void HandleMultilines(Token token)
    {
      if (token.SymbolType == SymbolType.MultilineComment || token.SymbolType == SymbolType.StringValue)
      {
        int columnOffsetAfterNewLine = 0;
        int lineOffsetAfterNewLine = 0;
        for (int i = 0; i < token.Value.Length; i++)
        {
          if (Regex.IsMatch(token.Value[i].ToString(), "\n|\r|\r\n"))
          {
            lineOffsetAfterNewLine++;
            columnOffsetAfterNewLine = 0;
          }
          else columnOffsetAfterNewLine++;
        }
        if (lineOffsetAfterNewLine > 0)
        {
          // There was a new line
          this.lineNumber += lineOffsetAfterNewLine;
          this.columnIndex = columnOffsetAfterNewLine;
        }
      }
    }
    private Token RunAutomaton()
    {
      SymbolType prevSymbol = SymbolType.Invalid;
      string lexeme;
      while (true)
      {
        char currChar = this.buffer.readChar();
        prevSymbol = this.automaton.HandleInput(currChar);
        // Also check is buffer.IsEmpty() here
        if (currChar == '#')
        {
          lexeme = this.buffer.getLexeme(false);
          if (lexeme.Length == 0)
          {
            return new Token(SymbolType.EOF, "");
          }
          return new Token(prevSymbol, lexeme, this.lineNumber, this.columnIndex - lexeme.Length);
        }
        this.columnIndex++;
        if (prevSymbol == SymbolType.NewLine)
        {
          this.lineNumber++;
          this.columnIndex = 0;
        }
        this.charIndex++;
        switch (this.automaton.GetAction())
        {
          case AutomatonAction.Move:
            break;
          case AutomatonAction.Recognize:
            lexeme = this.buffer.getLexeme(true);
            this.charIndex--;
            this.columnIndex--;
            return new Token(prevSymbol, lexeme, this.lineNumber, this.columnIndex - lexeme.Length);
          case AutomatonAction.Error:
            // Symbol here is either WhiteSpace, NewLine or Invalid
            lexeme = this.buffer.getLexeme(false);
            return new Token(prevSymbol, lexeme, this.lineNumber, this.columnIndex - lexeme.Length);
        }
      }
    }
  }
}