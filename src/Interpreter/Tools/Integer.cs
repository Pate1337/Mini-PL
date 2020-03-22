using System;

namespace Interpreter
{
  public abstract class Integer
  {
    public static string minus = "−"; // This is the minus used by C#
    public static int Parse(string value)
    {
      if (value[0] == '-')
      {
        // Wrong unicode character for C#
        return Int32.Parse(value.Substring(1)) * -1;
      }
      return Int32.Parse(value);
    }
  }
}