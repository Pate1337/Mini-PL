namespace Interpreter
{
  public class ScanBuffer
  {
    private string input;
    private int pos;
    private bool empty = false;
    public ScanBuffer(string text)
    {
      this.input = text;
      this.pos = 0;
    }
    public char readChar()
    {
      int index = this.pos;
      if (index == this.input.Length)
      {
        this.empty = true;
        return '#';
      }
      this.pos++;
      return this.input[index];
    }
    public int currLength()
    {
      return this.pos;
    }
    public string getLexeme(bool redoLast)
    {
      int i = 0;
      if (redoLast) i = 1;
      string lexeme = this.input.Substring(0, this.pos - i);
      this.input = this.input.Substring(this.pos - i);
      this.pos = 0;
      return lexeme;
    }
    public bool IsEmpty()
    {
      return this.empty;
    }
  }
}