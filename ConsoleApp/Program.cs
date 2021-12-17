using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chess;

namespace ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        var board = Board.StartingChessPosition();
        Console.WriteLine(board.Fen);
    }
}
