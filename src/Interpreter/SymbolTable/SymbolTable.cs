using System;
using System.Collections.Generic;

namespace Interpreter
{
  public class SymbolTable
  {
    Stack<SymbolTableBlock> blocks;

    public SymbolTable()
    {
      this.blocks = new Stack<SymbolTableBlock>();
    }
    public void PushBlock()
    {
      this.blocks.Push(new SymbolTableBlock());
    }
    public SymbolTableBlock PopBlock()
    {
      return this.blocks.Pop();
    }
    public bool AddEntry(SymbolTableEntry e)
    {
      SymbolTableBlock current = this.blocks.Peek();
      SymbolTableEntry entry = GetEntry(e.Identifier);
      if (entry.Type != SymbolType.Invalid)
      {
        // Entry already exists in some of the parent blocks
        return false;
      }
      current.AddEntry(e);
      return true;
    }
    /*
    * Used in for loops to assign a new value to the identifier
    */
    public void ForceModifyEntry(string id, string newValue)
    {
      SymbolTableEntry e = GetEntry(id);
      e.Value = newValue;
    }
    public bool ModifyEntry(string id, string newValue)
    {
      SymbolTableEntry e = GetEntry(id);
      if (e.Type != SymbolType.Invalid)
      {
        if (!e.Locked) e.Value = newValue;
        else return false;
      }
      else
      {
        Console.WriteLine("No such entry, Illegal assignment");
        return false;
      }
      return true;
    }
    public SymbolTableEntry GetEntry(string id)
    {
      foreach (SymbolTableBlock stb in this.blocks)
      {
        SymbolTableEntry entry = stb.GetEntry(id);
        if (entry.Type != SymbolType.Invalid) return entry;
      }
      return new SymbolTableEntry("error", SymbolType.Invalid, "error");
    }
    public void PrintTable()
    {
      foreach (SymbolTableBlock b in this.blocks)
      {
        b.PrintBlock();
      }
    }
    public void LockVariable(string id)
    {
      SymbolTableEntry e = GetEntry(id);
      e.Locked = true;
    }
    public void ReleaseVariable(string id)
    {
      SymbolTableEntry e = GetEntry(id);
      e.Locked = false;
    }
  }
}