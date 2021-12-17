using System;
using System.Collections.Generic;
using System.Text;

namespace Chess;

public static class Bitboard
{
    public static bool Contains(ulong bitboard, int squareIndex)
    {
        return ((bitboard >> squareIndex) & 1) == 1;
    }

    public static List<int> BitboardSquares(ulong bitboard)
    {
        var squares = new List<int>();
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
            if (((bitboard >> squareIndex) & 1) == 1)
                squares.Add(squareIndex);

        return squares;
    }

    public static string BitboardToString(ulong bitboard)
    {
        string map = "";
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                map += (bitboard >> (7 - rank) * 8 + file) & 1;
            }
            map += "\n";
        }
        return map;
    }

    public static ulong GetPieceBitboard(Board board, int colour, int type = 0)
    {
        ulong bitboard = 0UL;
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
            if (Piece.IsColour(board[squareIndex], colour) && (Piece.IsEmpty(type) || Piece.IsType(board[squareIndex], type)))
                bitboard |= 1UL << squareIndex;

        return bitboard;
    }

    public static ulong GetAttackBitboard(Board board)
    {
        ulong bitboard = 0UL;
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
            if (Piece.IsColour(board[squareIndex], board.OpponentColour))
                bitboard |= GetAttackBitboard(board, squareIndex);

        return bitboard;
    }

    public static ulong GetAttackBitboard(Board board, int squareIndex)
    {
        ulong bitboard = 0UL;
        int piece = board[squareIndex];
        
        if (Piece.IsEmpty(piece))
            return bitboard;

        switch (Piece.Type(piece))
        {
            case Piece.Pawn:
                bitboard |= PawnAttackBitboard(board, squareIndex);
                break;

            case Piece.King:
                bitboard |= KingAttackBitboard(board, squareIndex);
                break;

            case Piece.Knight:
                bitboard |= KnightAttackBitboard(board, squareIndex);
                break;

            case Piece.Bishop:
                bitboard |= SlidingAttackBitboard(board, squareIndex, 4, 8);
                break;

            case Piece.Rook:
                bitboard |= SlidingAttackBitboard(board, squareIndex, 0, 4);
                break;

            case Piece.Queen:
                bitboard |= SlidingAttackBitboard(board, squareIndex, 0, 8);
                break;

            default:
                break;
        }
        return bitboard;
    }

    public static ulong PawnAttackBitboard(Board board, int squareIndex)
    {
        ulong bitboard = 0UL;
        int file = squareIndex & 7;
        int forward = PrecomputedMoveData.DirectionsOffsets[Piece.Colour(board[squareIndex]) >> 4];
        if (file > 0)
        {
            bitboard |= 1UL << (squareIndex - 1 + forward);
        }
        if (file < 7)
        {
            bitboard |= 1UL << squareIndex + 1 + forward;
        }
        return bitboard;
    }

    public static ulong KingAttackBitboard(Board board, int squareIndex)
    {
        ulong bitboard = 0UL;
        for (int directionIndex = 0; directionIndex < 8; directionIndex++)
        {
            if (PrecomputedMoveData.Rays[squareIndex][directionIndex] == 0)
                continue;

            int targetSquare = squareIndex + PrecomputedMoveData.DirectionsOffsets[directionIndex];
            bitboard |= 1UL << targetSquare;
        }
        return bitboard;
    }

    public static ulong KnightAttackBitboard(Board board, int squareIndex)
    {
        ulong bitboard = 0UL;
        for (int rank = -2; rank <= 2; rank++)
        {
            for (int file = -2; file <= 2; file++)
            {
                if (Math.Abs(rank) == Math.Abs(file) || rank == 0 || file == 0)
                    continue;

                int newRank = (squareIndex >> 3) + rank;
                int newFile = (squareIndex & 7) + file;

                if ((newRank & 8) != 0 || (newFile & 8) != 0)
                    continue;

                bitboard |= 1UL << (newRank * 8 + newFile);
            }
        }
        return bitboard;
    }

    public static ulong SlidingAttackBitboard(Board board, int squareIndex, int startDirectionIndex, int endDirectionIndex, bool breakOnPiece = true)
    {
        ulong bitboard = 0UL;
        for (int directionIndex = startDirectionIndex; directionIndex < endDirectionIndex; directionIndex++)
        {
            for (int n = 0; n < PrecomputedMoveData.Rays[squareIndex][directionIndex]; n++)
            {
                int targetSquare = squareIndex + PrecomputedMoveData.DirectionsOffsets[directionIndex] * (n + 1);
                bitboard |= 1UL << targetSquare;

                if (breakOnPiece && !Piece.IsEmpty(board[targetSquare]))
                    break;
            }
        }
        return bitboard;
    }

    public static ulong RayBitboard(int startingSquare, int endingSquare, bool beyond)
    {
        ulong bitboard = 0UL;
        int directionIndex = PrecomputedMoveData.Lookup(startingSquare, endingSquare);
        if (beyond)
        {
            for (int n = 0; n <= PrecomputedMoveData.Rays[endingSquare][directionIndex]; n++)
            {
                int squareIndex = endingSquare + PrecomputedMoveData.DirectionsOffsets[directionIndex] * (n + 1);
                bitboard |= 1UL << squareIndex;
            }
        }
        else
        {
            for (int n = 0; n <= PrecomputedMoveData.Rays[startingSquare][directionIndex]; n++)
            {
                int squareIndex = startingSquare + PrecomputedMoveData.DirectionsOffsets[directionIndex] * n;
                if (squareIndex == endingSquare)
                    break;

                bitboard |= 1UL << squareIndex;
            }
        }
        
        return bitboard;
    }
}
