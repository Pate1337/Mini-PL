using System.Collections.Generic;

namespace Interpreter
{
  public class TestIO : IOHandler
  {
    private string[] inputs;
    private int index;
    List<string> outputs;

    public TestIO(string[] inputs)
    {
      this.inputs = inputs;
      this.index = 0;
      this.outputs = new List<string>();
    }
    public string ReadLine()
    {
      return this.inputs[this.index++];
    }
    public void WriteLine(string line)
    {
      this.outputs.Add(line);
    }
    public List<string> GetOutput()
    {
      return this.outputs;
    }
  }
}