using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Chess;

namespace ConsoleApp;

class Program
{
    static void PlayChess()
    {
        var board = Board.StartingChessPosition();
        bool moved = false;
        do
        {
            if (moved)
                Console.WriteLine(board.AsString + Environment.NewLine);
            string uci = Console.ReadLine().Trim();
            if (uci == "resign")
                break;
            moved = board.PushUci(uci);
        } while (board.EndFlag == EndFlags.None);
        Console.WriteLine(board.EndFlag);
        Console.WriteLine(board.Winner);
    }

    static void Main(string[] args)
    {
        PlayChess();
    }
}
