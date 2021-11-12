using System;
using System.Collections.Generic;
using System.Text;

namespace Chess;

public class Board
{
    private readonly int[] Squares;
    public readonly List<Move> Moves;

    public int this[int index]
    {
        get
        {
            if (index < 0 || index > 63)
                return 0;

            return Squares[index];
        }
    }

    public int this[int file, int rank]
    {
        get
        {
            if (file < 1 || file > 8)
                return 0;

            if (rank < 1 || rank > 8)
                return 0;

            return Squares[((rank - 1) * 8) + (file - 1)];
        }
    }

    public int MoveCount
    {
        get
        {
            lock (Moves)
            {
                return Moves.Count;
            }
        }
    }

    public int ColourToMove
    {
        get
        {
            return ((MoveCount % 2) == 0) ? Piece.White : Piece.Black;
        }
    }

    public int OpponentColour
    {
        get
        {
            return (ColourToMove == Piece.White) ? Piece.Black : Piece.White;
        }
    }

    public string AsString
    {
        get
        {
            lock (Squares)
            {
                string boardString = string.Empty;
                for (int rank = 0; rank < 8; rank++)
                {
                    for (int file = 0; file < 8; file++)
                    {
                        int index = (7 - rank) * 8 + file;
                        boardString += Piece.PieceSymbol(Squares[index]) + " ";
                    }
                    boardString += "\b\n";
                }
                return boardString[..^1];
            }
        }
    }

    public Board()
    {
        Squares = new int[64];
        Moves = new List<Move>();
    }

    public List<Move> CalculateLegalMoves()
    {
        var moves = new List<Move>();
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
        {
            if (!Piece.IsEmpty(Squares[squareIndex]))
            {
                if (Piece.IsColour(Squares[squareIndex], ColourToMove))
                {
                    if (Piece.IsSlidingPiece(Squares[squareIndex]))
                    {
                        moves.AddRange(GenerateSlidingMoves(squareIndex));
                    }

                    else if (Piece.IsType(Squares[squareIndex], Piece.Knight))
                    {
                        moves.AddRange(GenerateKnightMoves(squareIndex));
                    }

                    else if (Piece.IsType(Squares[squareIndex], Piece.King))
                    {
                        moves.AddRange(GenerateKingMoves(squareIndex));
                    }

                    else
                    {
                        moves.AddRange(GeneratePawnMoves(squareIndex));
                    }
                }
            }
        }
        return moves;
    }

    private List<Move> GenerateSlidingMoves(int squareIndex)
    {
        var moves = new List<Move>();
        int startDirectionIndex = Piece.IsType(Squares[squareIndex], Piece.Bishop) ? 4 : 0;
        int endDirectionIndex = Piece.IsType(Squares[squareIndex], Piece.Rook) ? 4 : 8;
        for (int directionIndex = startDirectionIndex; directionIndex < endDirectionIndex; directionIndex++)
        {
            for (int n = 0; n < PrecomputedMoveData.NumSquaresToEdge[squareIndex][directionIndex]; n++)
            {
                int targetSquare = squareIndex + PrecomputedMoveData.DirectionOffsets[directionIndex] * (n + 1);

                /* Blocked by friendly piece */
                if (Piece.IsColour(Squares[targetSquare], ColourToMove))
                    break;

                moves.Add(new Move(squareIndex, targetSquare));

                /* Capturing opponent piece */
                if (Piece.IsColour(Squares[targetSquare], OpponentColour))
                    break;
            }
        }
        return moves;
    }

    private List<Move> GenerateKnightMoves(int squareIndex)
    {
        var moves = new List<Move>();
        for (int knightSquareIndex = 0; knightSquareIndex < 8; knightSquareIndex++)
        {
            int targetSquare = PrecomputedMoveData.KnightSquares[squareIndex][knightSquareIndex];

            /* targetSquare is out of bounds */
            if (targetSquare == -1)
                continue;

            /* Square blocked by friendly piece */
            if (Piece.IsColour(Squares[targetSquare], ColourToMove))
                continue;

            moves.Add(new Move(squareIndex, targetSquare));
        }
        return moves;
    }

    private List<Move> GenerateKingMoves(int squareIndex)
    {
        var moves = new List<Move>();
        for (int directionIndex = 0; directionIndex < 8; directionIndex++)
        {
            int targetSquare = squareIndex + PrecomputedMoveData.DirectionOffsets[directionIndex];

            /* Square blocked by friendly piece */
            if (Piece.IsColour(Squares[targetSquare], ColourToMove))
                continue;

            moves.Add(new Move(squareIndex, targetSquare));
        }
        return moves;
    }

    private List<Move> GeneratePawnMoves(int squareIndex)
    {
        var moves = new List<Move>();
        int rank = squareIndex / 8;
        int file = squareIndex % 8;
        int startingRank = Piece.IsColour(Squares[squareIndex], Piece.White) ? 1 : 6;
        int promotionRank = 7 - startingRank;
        int direction = Piece.IsColour(Squares[squareIndex], Piece.White) ? 1 : -1;

        /* Move forward */
        int targetSquare = squareIndex + direction * 8;
        if (Piece.IsEmpty(Squares[targetSquare]))
        {
            if (rank == promotionRank)
            {
                for (int i = 0; i < 4; i++)
                {
                    moves.Add(new Move(squareIndex, targetSquare, 1 << i));
                }
            }

            else
            {
                moves.Add(new Move(squareIndex, targetSquare));
            }

            /* Can play two squares on starting rank */
            if (rank == startingRank)
            {
                targetSquare += direction * 8;
                if (Piece.IsEmpty(Squares[targetSquare]))
                {
                    moves.Add(new Move(squareIndex, targetSquare, Move.Flags.DoublePawnPush));
                }
            }
        }

        /* Move with capture */
        if (file > 0)
        {
            targetSquare = squareIndex + direction * 8 - 1;
            if (Piece.IsColour(targetSquare, OpponentColour))
            {
                moves.Add(new Move(squareIndex, targetSquare));
            }

            else if (MoveCount > 0)
            {
                var lastMove = Moves[^1];
                if (lastMove.HasFlag(Move.Flags.DoublePawnPush) && lastMove.TargetIndex == squareIndex - 1)
                {
                    moves.Add(new Move(squareIndex, targetSquare, Move.Flags.EnpassantCapture));
                }
            }
        }

        if (file < 8)
        {
            targetSquare = squareIndex + direction * 8 + 1;
            if (Piece.IsColour(targetSquare, OpponentColour))
            {
                moves.Add(new Move(squareIndex, targetSquare));
            }

            else if (MoveCount > 0)
            {
                var lastMove = Moves[^1];
                if (lastMove.HasFlag(Move.Flags.DoublePawnPush) && lastMove.TargetIndex == squareIndex + 1)
                {
                    moves.Add(new Move(squareIndex, targetSquare, Move.Flags.EnpassantCapture));
                }
            }
        }

        return moves;
    }

    public bool MakeMove(int currentIndex, int targetIndex)
    {
        return MakeMove(currentIndex, targetIndex, Move.Flags.None);
    }

    public bool MakeMove(int currentIndex, int targetIndex, int flag)
    {
        if (currentIndex < 0 || currentIndex > 63)
            return false;

        if (targetIndex < 0 || targetIndex > 63)
            return false;

        var legalMoves = CalculateLegalMoves();

        foreach (var legalMove in legalMoves)
        {
            if (legalMove.Index == currentIndex && legalMove.TargetIndex == targetIndex && legalMove.Flag == flag)
            {
                legalMove.WithPieces(Squares[currentIndex], Squares[targetIndex]);
                Squares[currentIndex] = 0;

                if (Piece.IsType(legalMove.Piece, Piece.Pawn))
                {
                    if (legalMove.HasPromotionFlag())
                    {
                        Squares[targetIndex] = Piece.Promote(legalMove.Piece, legalMove.GetPromotionType());
                    }

                    else if (legalMove.HasFlag(Move.Flags.EnpassantCapture))
                    {
                        Squares[targetIndex + (Piece.IsColour(ColourToMove, Piece.White) ? -8 : 8)] = 0;
                        Squares[targetIndex] = legalMove.Piece;
                    }

                    else
                    {
                        Squares[targetIndex] = legalMove.Piece;
                    }
                }

                else if (Piece.IsType(legalMove.Piece, Piece.King))
                {

                }

                else
                {
                    Squares[targetIndex] = legalMove.Piece;
                }

                Moves.Add(legalMove);
                return true;
            }
        }

        return false;
    }

    public bool Takeback()
    {
        if (Moves.Count == 0)
            return false;

        var move = Moves[^1];
        Squares[move.Index] = move.Piece;
        Squares[move.TargetIndex] = move.TargetPiece;
        Moves.RemoveAt(Moves.Count - 1);
        return true;
    }

    public void PutPiece(int squareIndex, int piece)
    {
        if (squareIndex < 0 || squareIndex > 63)
            return;

        Squares[squareIndex] = piece;
    }
}

public static class Piece
{
    public const int None = 0;      /* 000 */
    public const int Pawn = 1;      /* 001 */
    public const int King = 2;      /* 010 */
    public const int Knight = 3;    /* 011 */
    public const int Bishop = 4;    /* 100 */
    public const int Rook = 5;      /* 101 */
    public const int Queen = 6;     /* 110 */

    public const int White = 8;     /* 01... */
    public const int Black = 16;    /* 10... */

    public static bool IsColour(int piece, int colour)
    {
        return (piece & 0b11000) == colour;
    }

    public static bool IsType(int piece, int type)
    {
        return (piece & 0b111) == type;
    }

    public static bool IsEmpty(int piece)
    {
        return (piece & 0b111) == None;
    }

    public static bool IsSlidingPiece(int piece)
    {
        return (piece & 0b100) == 0b100;
    }

    public static int Promote(int piece, int type)
    {
        return (piece & 0b11000) | type;
    }

    public static string PieceSymbol(int piece)
    {
        string symbol = (piece & 0b111) switch
        {
            None => ".",
            Pawn => "p",
            King => "k",
            Knight => "n",
            Bishop => "b",
            Rook => "r",
            Queen => "q",
            _ => string.Empty
        };
        return IsColour(piece, White) ? symbol.ToUpper() : symbol;
    }

    public static int FromSymbol(char symbol)
    {
        int piece = char.ToLower(symbol) switch
        {
            'p' => Pawn,
            'k' => King,
            'n' => Knight,
            'b' => Bishop,
            'r' => Rook,
            'q' => Queen,
            _ => None
        };
        return piece | (char.IsLower(symbol) ? Black : White);
    }
}

public struct Move
{
    private int Value; /* 000000 00000 000000 00000 */

    private const int indexMask = 0b111111;
    private const int pieceMask = 0b11111;

    public readonly struct Flags
    {
        public static readonly int None = 0;
        public static readonly int PromotionKnight = 1;
        public static readonly int PromotionBishop = 2;
        public static readonly int PromotionRook = 4;
        public static readonly int PromotionQueen = 8;
        public static readonly int DoublePawnPush = 16;
        public static readonly int EnpassantCapture = 32;
        public static readonly int KingCastling = 64;
        public static readonly int QueenCastling = 128;
    }

    public int Index
    {
        get
        {
            return (Value >> 5) & indexMask;
        }
    }

    public int TargetIndex
    {
        get
        {
            return ((Value >> 16) & indexMask);
        }
    }

    public int Piece
    {
        get
        {
            return Value & pieceMask;
        }
    }

    public int TargetPiece
    {
        get
        {
            return ((Value >> 11) & pieceMask);
        }
    }

    public int Flag
    {
        get
        {
            return (Value >> 22);
        }
    }

    public Move(int current, int target)
    {
        Value = ((current + (target << 11)) << 5);
    }

    public Move(int current, int target, int flag) : this(current, target)
    {
        Value += flag << 22;
    }

    public void WithPieces(int piece, int targetPiece)
    {
        Value += piece + (targetPiece << 11);
    }

    public bool HasFlag(int flag)
    {
        return Flag == flag;
    }

    public bool NoFlag()
    {
        return Flag == Flags.None;
    }

    public bool HasPromotionFlag()
    {
        return Flags.PromotionKnight <= Flag && Flag <= Flags.PromotionQueen;
    }

    public int GetPromotionType()
    {
        for (int i = 0; i < 4; i++)
            if ((Flag >> i) == 1)
                return i + 3;

        return 0;
    }
}

public static class PrecomputedMoveData
{
    public static readonly int[] DirectionOffsets = new int[] { 8, -8, -1, 1, 7, -7, 9, -9 };
    public static readonly int[][] NumSquaresToEdge = new int[64][];
    public static readonly int[][] KnightSquares = new int[64][];

    static PrecomputedMoveData()
    {
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                int numNorth = 7 - rank;
                int numSouth = rank;
                int numWest = file;
                int numEast = 7 - file;

                int squareIndex = rank * 8 + file;

                NumSquaresToEdge[squareIndex] = new int[]
                {
                    numNorth,
                    numSouth,
                    numWest,
                    numEast,
                    Math.Min(numNorth, numWest),
                    Math.Min(numSouth, numEast),
                    Math.Min(numNorth, numEast),
                    Math.Min(numSouth, numWest)
                };

                KnightSquares[squareIndex] = new int[8];
                for (int i = 0; i < 4; i++)
                {
                    int targetSquare = squareIndex + DirectionOffsets[i] + DirectionOffsets[(i % 2) + 4];
                    KnightSquares[squareIndex][2 * i] = targetSquare < 0 || targetSquare > 63 ? -1 : targetSquare;

                    targetSquare = squareIndex + DirectionOffsets[i] + DirectionOffsets[(i < 2) ? i + 6 : 9 - i];
                    KnightSquares[squareIndex][2 * i + 1] = targetSquare < 0 || targetSquare > 63 ? -1 : targetSquare;
                }
            }
        }
    }
}

public static class PortableGameNotation
{
    private static readonly string Files = "abcdefgh";

    public static string GeneratePgn(Board board, string white = "Unknown", string black = "Unknown")
    {
        string pgn = $"[White \"{white}\"]\n[Black \"{black}\"]\n[Date \"{DateTime.UtcNow.ToShortDateString()}\"]\n\n";
        for (int i = 0; i < (board.MoveCount + 1) / 2; i++)
        {
            pgn += $"{(i + 1)}. {San(board.Moves[i * 2])} ";
            pgn += ((i * 2 + 1) < board.MoveCount) ? $"{San(board.Moves[i * 2 + 1])} " : string.Empty;
        }
        return pgn;
    }

    public static Board LoadPgn(string pgn)
    {
        var board = new Board();
        /* TODO */
        return board;
    }

    public static string Uci(int squareIndex)
    {
        return string.Empty + Files[squareIndex % 8] + (squareIndex / 8 + 1);
    }

    public static string Uci(Move move)
    {
        return Uci(move.Index) + Uci(move.TargetIndex);
    }

    public static string San(Move move)
    {
        return (Piece.IsType(move.Piece, Piece.Pawn) ? string.Empty : Piece.PieceSymbol(move.Piece)) + (Piece.IsEmpty(move.TargetPiece) ? string.Empty : "x") + Uci(move.TargetIndex);
    }
}

public static class ForsythEdwardsNotation
{
    public static readonly string StaringPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    public static string GenerateFen(Board board)
    {
        string fen = string.Empty;
        int empty;
        for (int rank = 0; rank < 8; rank++)
        {
            empty = 0;
            for (int file = 0; file < 8; file++)
            {
                int squareIndex = (7 - rank) * 8 + file;
                if (Piece.IsEmpty(board[squareIndex]))
                {
                    empty++;
                }

                else
                {
                    fen += (empty > 0 ? empty : string.Empty) + Piece.PieceSymbol(board[squareIndex]);
                    empty = 0;
                }
            }

            if (empty > 0)
                fen += empty;

            fen += "/";
        }

        fen += $"\b {(Piece.IsColour(board.ColourToMove, Piece.White) ? 'w' : 'b')}";
        return fen;
    }

    public static Board LoadFen(string fen)
    {
        Board board = new Board();
        string[] vs = fen.Split(' ')[0].Split('/');
        int file;
        for (int rank = 0; rank < 8; rank++)
        {
            file = 0;
            foreach (var character in vs[rank])
            {
                if (char.IsDigit(character))
                    file += int.Parse(character.ToString()) - 1;

                else
                    board.PutPiece((7 - rank) * 8 + file, Piece.FromSymbol(character));
                file++;
            }
        }
        return board;
    }
}