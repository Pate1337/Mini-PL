using System;

namespace Interpreter
{
  public class SymbolTableEntry
  {
    public string Identifier { get; set; }
    public SymbolType Type { get; set; }
    public string Value { get; set; }
    public bool Locked { get; set; }

    public SymbolTableEntry(string id, SymbolType type, string value)
    {
      this.Identifier = id;
      this.Type = type;
      this.Value = value;
      this.Locked = false;
    }
    public void PrintEntry()
    {
      Console.WriteLine($"(Identifier: {this.Identifier}, Type: {this.Type}, Value: {this.Value}, Locked: {this.Locked})");
    }
  }
}