using System;

namespace Interpreter
{
  public class DeclarationError : Error
  {
    public DeclarationError() {}
    public DeclarationError(string message, Token token)
    {
      this.message = message;
      this.Token = token;
      this.lineContent = FileReader.ReadLine(this.Token.LineNumber);
      this.Type = "DECLARATION ERROR";
    }
    public override string ToString()
    {
      string line = FormLine();
      return this.Type + " (line " + this.Token.LineNumber + ", column " + this.Token.Column + "): " + this.message + "\n\n" + line;
    }
  }
}