using System.Collections.Generic;

namespace Interpreter
{
  public interface IOHandler
  {
    string ReadLine();
    void WriteLine(string line);
    List<string> GetOutput();
  }
}