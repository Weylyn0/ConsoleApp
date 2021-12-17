using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chess;

namespace ConsoleApp;

class Program
{
    public const int A1 = 0;
    public const int B1 = 1;
    public const int C1 = 2;
    public const int D1 = 3;
    public const int E1 = 4;
    public const int F1 = 5;
    public const int G1 = 6;
    public const int H1 = 7;
    public const int A2 = 8;
    public const int B2 = 9;
    public const int C2 = 10;
    public const int D2 = 11;
    public const int E2 = 12;
    public const int F2 = 13;
    public const int G2 = 14;
    public const int H2 = 15;
    public const int A3 = 16;
    public const int B3 = 17;
    public const int C3 = 18;
    public const int D3 = 19;
    public const int E3 = 20;
    public const int F3 = 21;
    public const int G3 = 22;
    public const int H3 = 23;
    public const int A4 = 24;
    public const int B4 = 25;
    public const int C4 = 26;
    public const int D4 = 27;
    public const int E4 = 28;
    public const int F4 = 29;
    public const int G4 = 30;
    public const int H4 = 31;
    public const int A5 = 32;
    public const int B5 = 33;
    public const int C5 = 34;
    public const int D5 = 35;
    public const int E5 = 36;
    public const int F5 = 37;
    public const int G5 = 38;
    public const int H5 = 39;
    public const int A6 = 40;
    public const int B6 = 41;
    public const int C6 = 42;
    public const int D6 = 43;
    public const int E6 = 44;
    public const int F6 = 45;
    public const int G6 = 46;
    public const int H6 = 47;
    public const int A7 = 48;
    public const int B7 = 49;
    public const int C7 = 50;
    public const int D7 = 51;
    public const int E7 = 52;
    public const int F7 = 53;
    public const int G7 = 54;
    public const int H7 = 55;
    public const int A8 = 56;
    public const int B8 = 57;
    public const int C8 = 58;
    public const int D8 = 59;
    public const int E8 = 60;
    public const int F8 = 61;
    public const int G8 = 62;
    public const int H8 = 63;

    static readonly string startingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    static readonly string fen = "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1";
    static readonly string fen2 = "8/1p6/8/P2p4/2P2p2/8/4P3/8 w - - 0 1";
    static readonly Board board = Board.FromFen(startingFen);
    const int d = 6;
    static int lastPos = 0;
    static Move lastMove = default;

    static void Main(string[] args)
    {
        /* Chess Programming: https://www.chessprogramming.org/Main_Page
         *                    https://www.youtube.com/channel/UCB9-prLkPwgvlKKqDgXhsMQ/playlists
         *                    https://www.youtube.com/watch?v=g1b80b8DGJM&list=PLmN0neTso3JzhJP35hwPHJi4FZgw5Ior0&index=6
         * 
         *  AI:
         *      Quiescence Search: https://www.chessprogramming.org/Quiescence_Search
         *      
         *  https://www.chessprogramming.org/Perft_Results
         *  https://lichess.org/editor?fen=8%2F8%2F8%2F3N4%2F8%2F8%2F8%2F8+w+-+-+0+1
         */

        PerformanceTest();
    }

    public static void PerformanceTest()
    {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        Console.WriteLine($"Calculated in: {stopwatch.ElapsedMilliseconds} MS");
    }

    private static void MoveTest()
    {
        Console.WriteLine($"Legal Move Count: {board.LegalMoves.Length}\n");
        foreach (var item in board.LegalMoves)
        {
            Console.WriteLine($" {item.Uci}");
        }
    }

    static int Perft(int depth = d)
    {
        if (depth == 0)
            return 1;

        int positions = 0;
        //var moves = board.GenerateLegalMoves();
        foreach (var move in board.LegalMoves)
        {
            board.MakeMove(move);
            positions += Perft(depth - 1);
            if (depth == d)
            {
                Console.WriteLine($"{move.Uci}: {positions - lastPos}");
                lastPos = positions;
                lastMove = move;
            }
            board.Takeback();
        }
        return positions;
    }

    static int PerftTest(int depth)
    {
        if (depth == 0)
            return 1;

        int positions = 0;
        foreach (var move in board.LegalMoves)
        {
            board.MakeMove(move);
            positions += PerftTest(depth - 1);
            board.Takeback();
        }
        return positions;
    }
}
