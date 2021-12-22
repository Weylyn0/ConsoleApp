using System;
using System.Diagnostics;

namespace Chess;

public static class Engine
{
    private static Stopwatch stopwatch;
    private static ulong leafNodes = 0;

    static Engine()
    {
        stopwatch = new Stopwatch();
    }

    public static void RunUci()
    {
        var board = new Board();
        ForsythEdwardsNotation.LoadFen(board, ForsythEdwardsNotation.StartingPositionFen);
        int depth = -1;
        string command = string.Empty;
        while (true)
        {
            command = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(command))
                continue;

            if (command.StartsWith("position "))
            {
                string position = command[9..];
                if (position.StartsWith("startpos"))
                {
                    ForsythEdwardsNotation.LoadFen(board, ForsythEdwardsNotation.StartingPositionFen);
                    if (position.Length > 14 && position[9..14].Equals("moves"))
                    {
                        foreach (var move in position[15..].Split(' '))
                        {
                            board.MakeMove(move);
                        }
                    }
                    continue;
                }

                else if (position.StartsWith("fen "))
                {
                    ForsythEdwardsNotation.LoadFen(board, position[4..]);
                    int index = position.IndexOf("moves ");
                    if (index >= 0)
                    {
                        foreach (var move in position[(index + 6)..].Split(' '))
                        {
                            board.MakeMove(move);
                        }
                    }
                    continue;
                }
            }

            else if (command.StartsWith("go"))
            {
                depth = -1;
                if (command.StartsWith("go depth ") && int.TryParse(command[9..].Split(' ')[0], out depth))
                {

                }

                else if (command.StartsWith("go perft ") && int.TryParse(command[9..].Split(' ')[0], out depth))
                {
                    PerftTest(board, depth);
                }
            }

            else if (command.Equals("d"))
            {
                Console.WriteLine(board.Text);
            }

            else if (command.Equals("isready"))
            {
                Console.WriteLine("readyok");
            }

            else if (command.Equals("uci"))
            {
                Console.WriteLine("id name Weywey");
                Console.WriteLine("id author Weylyn");
            }
        }
    }

    public static void PerftTest(Board board, int depth)
    {
        stopwatch.Start();
        leafNodes = 0;
        var moves = MoveGenerator.GenerateMoves(board);
        foreach (var move in moves)
        {
            if (!board.MakeMove(move))
                continue;

            ulong cumNodes = leafNodes;
            Perft(board, depth - 1);
            board.Takeback();
            Console.WriteLine($"{move.Uci}: {leafNodes - cumNodes}");
        }
        stopwatch.Stop();
        Console.WriteLine($"\n > Nodes: {leafNodes}\n > Depth: {depth}\n > Duration: {stopwatch.ElapsedMilliseconds} Milliseconds ({stopwatch.ElapsedTicks} Ticks)");
        stopwatch.Reset();
    }

    private static void Perft(Board board, int depth)
    {
        if (depth == 0)
        {
            leafNodes++;
            return;
        }

        var moves = MoveGenerator.GenerateMoves(board);
        foreach (var move in moves)
        {
            if (!board.MakeMove(move))
                continue;

            Perft(board, depth - 1);
            board.Takeback();
        }
    }

    public static ulong MultipleKnightMoves(ulong knights)
    {
        ulong left1 = (knights >> 1) & 0x7f7f7f7f7f7f7f7f;
        ulong left2 = (knights >> 2) & 0x3f3f3f3f3f3f3f3f;
        ulong right1 = (knights << 1) & 0xfefefefefefefefe;
        ulong right2 = (knights << 2) & 0xfcfcfcfcfcfcfcfc;
        ulong h1 = left1 | right1;
        ulong h2 = left2 | right2;
        return (h1 << 16) | (h1 >> 16) | (h2 << 8) | (h2 >> 8);
    }
}
