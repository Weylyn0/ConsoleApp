using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess;

public static class Piece
{
    public const int None = 0;
    public const int Pawn = 1;
    public const int King = 2;
    public const int Knight = 3;
    public const int Bishop = 5;
    public const int Rook = 6;
    public const int Queen = 7;

    public const int White = 0;
    public const int Black = 8;

    public const int WhitePawn = 1;
    public const int WhiteKing = 2;
    public const int WhiteKnight = 3;
    public const int WhiteBishop = 5;
    public const int WhiteRook = 6;
    public const int WhiteQueen = 7;

    public const int BlackPawn = 9;
    public const int BlackKing = 10;
    public const int BlackKnight = 11;
    public const int BlackBishop = 13;
    public const int BlackRook = 14;
    public const int BlackQueen = 15;

    public static readonly int[] Values =
    {
        0,
        100,
        10000,
        300,
        0,
        320,
        500,
        800
    };

    private const string Chars = "_PKN_BRQ_pkn_brq";

    public static int Type(int piece)
    {
        return piece & 0b111;
    }

    public static int Colour(int piece)
    {
        return piece & 0b1000;
    }

    public static int ColourIndex(int piece)
    {
        return (piece >> 3) & 0b1;
    }

    public static bool IsEmpty(int piece)
    {
        return (piece & 0b111) == None;
    }

    public static bool IsBishopOrQueen(int piece)
    {
        return (piece & 0b101) == 0b101;
    }

    public static bool IsRookOrQueen(int piece)
    {
        return (piece & 0b110) == 0b110;
    }

    public static bool IsSlidingPiece(int piece)
    {
        return (piece & 0b100) == 0b100;
    }

    public static int GetPiece(char c)
    {
        return Chars.IndexOf(c);
    }

    public static char GetChar(int piece)
    {
        return Chars[piece];
    }
}
