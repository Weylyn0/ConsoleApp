using System;
using System.Collections.Generic;
using Chess;

namespace ConsoleApp;

class Program
{
    static void PlayChess()
    {
        var board = Board.StartingChessPosition();
        bool moved = true;
        do
        {
            if (moved)
                Console.WriteLine(board.AsString + Environment.NewLine);
            string san = Console.ReadLine().Trim();
            if (san == "resign")
                break;
            try
            {
                moved = board.MakeMove(san);
            }

            catch
            {

            }
        } while (true);
    }

    static void Main(string[] args)
    {
        PlayChess();
    }
}
