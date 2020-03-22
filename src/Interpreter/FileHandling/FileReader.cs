using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpreter
{
  public static class FileReader
  {
    private static string file;
    private static List<string> lines = new List<string>();

    public static void SetFile(string fileName)
    {
      file = fileName;
    }
    public static void AddInputLine(string text)
    {
      lines.Add(text);
    }
    public static string ReadAllText()
    {
      lines = System.IO.File.ReadLines(file).Cast<string>().ToList();
      return System.IO.File.ReadAllText(file);
    }
    public static string ReadLine(int line)
    {
      int lineNumber = 1;
      foreach (string l in lines)
      {
        if (lineNumber == line) return l;
        lineNumber++;
      }
      return "";
    }
    public static void ClearInput()
    {
      file = "";
      lines.Clear();
    }
  }
}