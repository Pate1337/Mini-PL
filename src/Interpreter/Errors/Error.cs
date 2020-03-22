using System;

namespace Interpreter
{
  [Serializable()]
  public class Error : System.Exception
  {
    protected string message;
    public Token Token { get; set; }
    protected string lineContent;
    public string Type { get; set; }
    public Error() : base() { }
    public Error(string message) : base(message) { }
    public Error(string message, System.Exception inner) : base(message, inner) { }

    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client. 
    protected Error(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    
    public Error(string message, Token token)
    {
      this.message = message;
      this.Token = token;
      this.lineContent = FileReader.ReadLine(this.Token.LineNumber);
      this.Type = "ERROR";
    }
    public override string ToString()
    {
      string line = FormLine();
      return this.Type + " (line " + this.Token.LineNumber + ", column " + this.Token.Column + "): " + this.message + "\n\n" + line;
    }
    protected string FormLine()
    {
      string arrow = "";
      int i = 0;
      while (i < this.Token.Column)
      {
        arrow += " ";
        i++;
      }
      arrow += "^";
      return "\t" + this.lineContent + "\n\t" + arrow;
    }
  }
}