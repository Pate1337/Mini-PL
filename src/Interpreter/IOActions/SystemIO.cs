using System;
using System.Collections.Generic;

namespace Interpreter
{
  public class SystemIO : IOHandler
  {
    public string ReadLine()
    {
      return Console.ReadLine();
    }
    public void WriteLine(string line)
    {
      Console.WriteLine(line);
    }
    public List<string> GetOutput()
    {
      return new List<string>();
    }
  }
}