﻿namespace Chess;

public static class Coordinates
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

    private const string UpperFiles = "ABCDEFGH";
    private const string LowerFiles = "abcdefgh";

    public static int GetSquareFromUci(string uci)
    {
        if (uci.Length != 2)
            return -1;

        uci = uci.ToLower().Trim();
        if (LowerFiles.Contains(uci[0]))
            return (uci[1] - 49) * 8 + LowerFiles.IndexOf(uci[0]);

        return -1;
    }

    public static string GetUci(int squareIndex)
    {
        if ((squareIndex & 64) != 0)
            return "-";

        return $"{LowerFiles[squareIndex & 7]}{(squareIndex >> 3) + 1}";
    }

    public static Move GetMoveFromUci(Board board, string uci)
    {
        if (uci.Length < 4)
            return default;

        int startingSquare = GetSquareFromUci(uci[0..2]);
        int endingSquare = GetSquareFromUci(uci[2..4]);

        if ((startingSquare & 64) != 0 || (endingSquare & 64) != 0)
            return default;

        int flag = MoveFlags.None;
        if (uci.Length == 4)
        {
            if (startingSquare == E1 && endingSquare == C1 && startingSquare == board.KingSquare)
            {
                flag = MoveFlags.WhiteQueenCastling;
            }

            else if (startingSquare == E1 && endingSquare == G1 && startingSquare == board.KingSquare)
            {
                flag = MoveFlags.WhiteKingCastling;
            }

            else if (startingSquare == E8 && endingSquare == C8 && startingSquare == board.KingSquare)
            {
                flag = MoveFlags.BlackQueenCastling;
            }

            else if (startingSquare == E8 && endingSquare == G8 && startingSquare == board.KingSquare)
            {
                flag = MoveFlags.BlackKingCastling;
            }

            else if (Piece.IsType(board[startingSquare], Piece.Pawn))
            {
                if (endingSquare == board.EnpassantSquare)
                    flag = MoveFlags.EnpassantCapture;

                else if (System.Math.Abs(startingSquare - endingSquare) == 16)
                    flag = MoveFlags.DoublePawnPush;
            }
        }

        else if (uci.Length == 5)
        {
            return new Move(startingSquare, endingSquare, board[startingSquare], board[endingSquare], Piece.GetPromotionFlag(Piece.GetPieceType(uci[4])));
        }

        else
        {
            return default;
        }

        return new Move(startingSquare, endingSquare, board[startingSquare], board[endingSquare], flag);
    }
}
