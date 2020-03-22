using System;
using System.Collections.Generic;

namespace Interpreter
{
  public class SymbolTableBlock
  {
    List<SymbolTableEntry> entries;

    public SymbolTableBlock()
    {
      this.entries = new List<SymbolTableEntry>();
    }
    public void AddEntry(SymbolTableEntry entry)
    {
      this.entries.Add(entry);
    }
    public SymbolTableEntry GetEntry(string identifier)
    {
      foreach (SymbolTableEntry e in this.entries)
      {
        if (e.Identifier == identifier) return e;
      }
      return new SymbolTableEntry("error", SymbolType.Invalid, "error");
    }
    public void PrintBlock()
    {
      Console.WriteLine("Block:");
      foreach (SymbolTableEntry e in this.entries)
      {
        e.PrintEntry();
      }
    }
  }
}