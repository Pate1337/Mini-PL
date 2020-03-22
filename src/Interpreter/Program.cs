using System;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            bool fileDefined = false;
            bool printAst = false;
            string fileName = "";
            foreach (string arg in args)
            {
                if (arg == "-ast") printAst = true;
                if (arg[0] != '-' && fileDefined)
                {
                    Console.WriteLine("Only filename can be defined without '-'. Other arguments must have '-' in front!");
                    fileDefined = false;
                    break;
                }
                if (arg[0] != '-')
                {
                    fileDefined = true;
                    fileName = arg;
                }
            }
            if (!fileDefined)
            {
                Console.WriteLine("No file specified. Create a commandline tool here.");
            }
            else
            {
                if (System.IO.File.Exists(fileName))
                {
                    FileReader.SetFile(fileName);
                    string text = FileReader.ReadAllText();
                    try
                    {
                        BlockNode ast = BuildAST(text);
                        if (printAst) PrintAST(ast);
                        RunInterpreter(ast);
                    }
                    catch (Error e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e);
                        Console.ResetColor();
                    }
                    FileReader.ClearInput();
                }
                else
                {
                    Console.WriteLine($"File {fileName} can not be found! Use absolute path.");
                }
            }
        }
        private static BlockNode BuildAST(string text)
        {
            Parser p = new Parser();
            return p.Parse(text);
        }
        private static void PrintAST(BlockNode ast)
        {
            IOHandler io = new SystemIO();
            Visitor printer = new PrintVisitor(io);
            printer.VisitProgram(ast);
        }
        private static void RunInterpreter(BlockNode ast)
        {
            IOHandler io = new SystemIO();
            Visitor v = new InterpreterVisitor(io);
            v.VisitProgram(ast);
        }
    }
}
