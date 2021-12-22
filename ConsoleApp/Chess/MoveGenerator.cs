using System.Collections.Generic;

namespace Chess;

public static class MoveGenerator
{
    public static List<Move> GenerateMoves(Board board)
    {
        var moves = new List<Move>();
        foreach (var square in Bitboard.Enumerate(board.PiecesByColour[board.FriendlyColourIndex]))
            moves.AddRange(GenerateMoves(board, square));

        return moves;
    }

    public static List<Move> GenerateMoves(Board board, int startingSquare)
    {
        int piece = board.Squares[startingSquare];
        if ((piece >> 3) != board.FriendlyColourIndex)
            return new List<Move>();

        return Piece.Type(piece) switch
        {
            Piece.Pawn => GeneratePawnMoves(board, startingSquare, piece),
            Piece.King => GenerateKingMoves(board, startingSquare, piece),
            Piece.Knight => GenerateKnightMoves(board, startingSquare, piece),
            Piece.Bishop => GenerateSlidingMoves(board, startingSquare, piece, 4, 8),
            Piece.Rook => GenerateSlidingMoves(board, startingSquare, piece, 0, 4),
            Piece.Queen => GenerateSlidingMoves(board, startingSquare, piece, 0, 8),
            _ => new List<Move>()
        };
    }

    internal static List<Move> GeneratePawnMoves(Board board, int startingSquare, int piece)
    {
        var moves = new List<Move>();
        int rank = startingSquare >> 3;
        (int promotionRank, ulong doublePushRank) = board.WhiteToMove ? (6, PrecomputedMoveData.Rank4) : (1, PrecomputedMoveData.Rank5);

        int singlePush = Bitboard.BitScanForward(PrecomputedMoveData.PawnPushPatterns[startingSquare][board.FriendlyColourIndex] & ~board.OccupiedSquares);

        if (singlePush > -1 && rank == promotionRank)
            for (int promotion = 0; promotion < 4; moves.Add(new Move(startingSquare, singlePush, piece, Piece.None, 1 << promotion)), promotion++) ;

        else if (singlePush > -1)
        {
            moves.Add(new Move(startingSquare, singlePush, piece, Piece.None));
            int doublePush = Bitboard.BitScanForward(PrecomputedMoveData.PawnPushPatterns[singlePush][board.FriendlyColourIndex] & ~board.OccupiedSquares & doublePushRank);
            if (doublePush > -1)
                moves.Add(new Move(startingSquare, doublePush, piece, Piece.None, Flags.DoublePawnPush));
        }

        foreach (var endingSquare in Bitboard.Enumerate(PrecomputedMoveData.PawnCapturePatterns[startingSquare][board.FriendlyColourIndex] & (board.PiecesByColour[board.FriendlyColourIndex ^ 1] | (board.EnpassantSquare != -1 ? 1UL << board.EnpassantSquare : 0UL))))
        {
            if (endingSquare == board.EnpassantSquare)
                moves.Add(new Move(startingSquare, endingSquare, piece, Piece.None, Flags.EnpassantCapture));

            else if (rank == promotionRank)
                for (int promotion = 0; promotion < 4; moves.Add(new Move(startingSquare, endingSquare, piece, board.Squares[endingSquare], 1 << promotion)), promotion++) ;

            else
                moves.Add(new Move(startingSquare, endingSquare, piece, board.Squares[endingSquare]));
        }

        return moves;
    }

    internal static List<Move> GenerateKingMoves(Board board, int startingSquare, int piece)
    {
        var moves = new List<Move>();
        ulong pattern = PrecomputedMoveData.KingPatterns[startingSquare] & ~(board.PiecesByColour[board.FriendlyColourIndex] | board.OpponentAttacks);

        foreach (var endingSquare in Bitboard.Enumerate(pattern))
            moves.Add(new Move(startingSquare, endingSquare, piece, board.Squares[endingSquare]));

        if (((board.OpponentAttacks >> board.Kings[board.FriendlyColourIndex]) & 1) == 0)
        {
            int kingCastling = board.ColourToMove >> 2;
            int queenCastling = kingCastling | 1;

            if ((board.Castlings & (1 << kingCastling)) != 0 && (PrecomputedMoveData.Castlings[kingCastling] & ~(board.OccupiedSquares | board.OpponentAttacks)) == PrecomputedMoveData.Castlings[kingCastling])
                moves.Add(new Move(startingSquare, startingSquare + 2, piece, Piece.None, Flags.KingsideCastling));

            if ((board.Castlings & (1 << queenCastling)) != 0 && (PrecomputedMoveData.Castlings[queenCastling] & ~(board.OccupiedSquares | board.OpponentAttacks)) == PrecomputedMoveData.Castlings[queenCastling] && ((board.OccupiedSquares >> (startingSquare - 3)) & 1) == 0)
                moves.Add(new Move(startingSquare, startingSquare - 2, piece, Piece.None, Flags.QueensideCastling));
        }

        return moves;
    }

    internal static List<Move> GenerateKnightMoves(Board board, int startingSquare, int piece)
    {
        var moves = new List<Move>();
        ulong pattern = PrecomputedMoveData.KnightPatterns[startingSquare] & ~board.PiecesByColour[board.FriendlyColourIndex];
        foreach (var endingSquare in Bitboard.Enumerate(pattern))
            moves.Add(new Move(startingSquare, endingSquare, piece, board.Squares[endingSquare]));

        return moves;
    }

    internal static List<Move> GenerateSlidingMoves(Board board, int startingSquare, int piece, int startDirectionIndex, int endDirectionIndex)
    {
        var moves = new List<Move>();
        ulong bitboard = 0UL;
        for (int directionIndex = startDirectionIndex; directionIndex < endDirectionIndex; directionIndex++)
        {
            ulong pattern = PrecomputedMoveData.RayPatterns[startingSquare][directionIndex];
            ulong blocker = pattern & board.OccupiedSquares;
            if (blocker != 0UL)
                pattern ^= PrecomputedMoveData.RayPatterns[((directionIndex & 1) == 0 ? Bitboard.BitScanReverse(blocker) : Bitboard.BitScanForward(blocker))][directionIndex];

            pattern &= ~board.PiecesByColour[board.FriendlyColourIndex];
            bitboard |= pattern;
        }

        foreach (var endingSquare in Bitboard.Enumerate(bitboard))
            moves.Add(new Move(startingSquare, endingSquare, piece, board.Squares[endingSquare]));

        return moves;
    }

    public static ulong GenerateOpponentAttacks(Board board)
    {
        ulong bitboard = 0UL;
        int colour = board.ColourToMove ^ 8;

        foreach (var square in Bitboard.Enumerate(board.Pieces[Piece.Pawn | colour]))
            bitboard |= PrecomputedMoveData.PawnCapturePatterns[square][board.FriendlyColourIndex ^ 1];

        foreach (var square in Bitboard.Enumerate(board.Pieces[Piece.King | colour]))
            bitboard |= PrecomputedMoveData.KingPatterns[square];

        foreach (var square in Bitboard.Enumerate(board.Pieces[Piece.Knight | colour]))
            bitboard |= PrecomputedMoveData.KnightPatterns[square];

        foreach (var square in Bitboard.Enumerate(board.Pieces[Piece.Bishop | colour] | board.Pieces[Piece.Queen | colour]))
            for (int directionIndex = 4; directionIndex < 8; directionIndex++)
            {
                ulong pattern = PrecomputedMoveData.RayPatterns[square][directionIndex];
                ulong blocker = pattern & board.OccupiedSquares;

                if (blocker != 0UL)
                    pattern ^= PrecomputedMoveData.RayPatterns[(directionIndex & 1) == 0 ? Bitboard.BitScanReverse(blocker) : Bitboard.BitScanForward(blocker)][directionIndex];

                bitboard |= pattern;
            }

        foreach (var square in Bitboard.Enumerate(board.Pieces[Piece.Rook | colour] | board.Pieces[Piece.Queen | colour]))
            for (int directionIndex = 0; directionIndex < 4; directionIndex++)
            {
                ulong pattern = PrecomputedMoveData.RayPatterns[square][directionIndex];
                ulong blocker = pattern & board.OccupiedSquares;

                if (blocker != 0UL)
                    pattern ^= PrecomputedMoveData.RayPatterns[(directionIndex & 1) == 0 ? Bitboard.BitScanReverse(blocker) : Bitboard.BitScanForward(blocker)][directionIndex];

                bitboard |= pattern;
            }

        return bitboard;
    }

    public static bool IsAttacked(Board board, int colour, int square)
    {
        if ((PrecomputedMoveData.PawnCapturePatterns[square][(colour >> 3) ^ 1] & board.Pieces[Piece.Pawn | colour]) != 0)
            return true;

        if ((PrecomputedMoveData.KnightPatterns[square] & board.Pieces[Piece.Knight | colour]) != 0)
            return true;

        for (int directionIndex = 0; directionIndex < 8; directionIndex++)
        {
            ulong pattern = PrecomputedMoveData.RayPatterns[square][directionIndex];
            ulong blocker = pattern & board.OccupiedSquares;

            if (blocker != 0UL)
                pattern ^= PrecomputedMoveData.RayPatterns[(directionIndex & 1) == 0 ? Bitboard.BitScanReverse(blocker) : Bitboard.BitScanForward(blocker)][directionIndex];

            if ((pattern & (board.Pieces[(directionIndex < 4 ? Piece.Rook : Piece.Bishop) | colour] | board.Pieces[Piece.Queen | colour])) != 0)
                return true;
        }

        if ((PrecomputedMoveData.KingPatterns[square] & board.Pieces[Piece.King | colour]) != 0)
            return true;

        return false;
    }
}
