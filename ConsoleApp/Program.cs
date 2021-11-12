using System;
using System.Collections.Generic;
using Chess;

namespace ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        var board = new Board();
        board.PutPiece(48, Piece.Black | Piece.Pawn);
        board.PutPiece(25, Piece.White | Piece.Pawn);
        Console.WriteLine(board.AsString + Environment.NewLine);
        board.MakeMove(25, 33);
        Console.WriteLine(board.AsString + Environment.NewLine);
        board.MakeMove(48, 32, Move.Flags.DoublePawnPush);
        Console.WriteLine(board.AsString + Environment.NewLine);
        board.MakeMove(33, 40, Move.Flags.EnpassantCapture);
        Console.WriteLine(board.AsString + Environment.NewLine);
    }
}
