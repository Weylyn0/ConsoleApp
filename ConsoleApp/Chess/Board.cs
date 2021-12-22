using System.Collections.Generic;

namespace Chess;

public class Board
{
    internal int[] Squares;
    internal int[] Kings;
    internal int[] Materials;

    internal ulong[] Pieces;
    internal ulong[] PiecesByColour;
    internal ulong OccupiedSquares;
    internal ulong OpponentAttacks;

    internal bool WhiteToMove;
    internal int ColourToMove;
    internal int FriendlyColourIndex;

    internal int Castlings;
    internal int EnpassantSquare;
    internal int HalfMoveCount;
    internal int FullMoveCount;

    internal ulong PositionKey;
    internal readonly List<BoardHistory> History;

    public int FriendlyColour
    {
        get
        {
            return ColourToMove;
        }
    }

    public int OpponentColour
    {
        get
        {
            return ColourToMove ^ 8;
        }
    }

    public bool IsCheck
    {
        get
        {
            return ((OpponentAttacks >> Kings[FriendlyColourIndex]) & 1) == 1;
        }
    }

    public string Text
    {
        get
        {
            string text = "\n";
            for (int rank = 7; rank > -1; rank--)
            {
                text += $"  {rank + 1} ";
                for (int file = 0; file < 8; file++)
                {
                    text += $" {Piece.GetChar(Squares[rank << 3 | file])}";
                }
                text += "\n";
            }

            string castlings = string.Empty;
            for (int castling = 0; castling < 4; castling++)
                if ((Castlings & (1 << castling)) > 0)
                    castlings += ForsythEdwardsNotation.FullCastlings[castling];

            text += $"\n     A B C D E F G H\n";
            text += $"\n  > Turn: {(WhiteToMove ? "White" : "Black")}";
            text += $"\n  > Castlings: {(castlings.Length == 0 ? '-' : castlings)}";
            text += $"\n  > Enpassant: {Coordinates.SquareToUci(EnpassantSquare)}";
            text += $"\n  > Half Move: {HalfMoveCount}\n  > Full Move: {FullMoveCount}\n";
            return text;
        }
    }

    public Board()
    {
        Squares = new int[64];
        Kings = new int[2] { -1, -1 };
        Materials = new int[2];

        WhiteToMove = true;
        ColourToMove = Piece.White;
        FriendlyColourIndex = 0;

        Pieces = new ulong[16];
        PiecesByColour = new ulong[2];
        OccupiedSquares = 0UL;
        OpponentAttacks = 0UL;

        Castlings = Flags.None;
        EnpassantSquare = -1;
        HalfMoveCount = 0;
        FullMoveCount = 1;

        PositionKey = ZobristKey.Create(this);
        History = new List<BoardHistory>();
    }

    public void Reset()
    {
        Squares = new int[64];
        Kings = new int[2] { -1, -1 };

        Pieces = new ulong[16];
        PiecesByColour = new ulong[2];
        OccupiedSquares = 0UL;

        WhiteToMove = true;
        ColourToMove = Piece.White;
        FriendlyColourIndex = 0;
        OpponentAttacks = 0UL;

        Castlings = Flags.None;
        EnpassantSquare = -1;
        HalfMoveCount = 0;
        FullMoveCount = 0;

        PositionKey = ZobristKey.Create(this);
        History.Clear();
    }

    public bool MakeMove(string uci)
    {
        var moves = MoveGenerator.GenerateMoves(this);
        foreach (var move in moves)
            if (move.Uci == uci)
                return MakeMove(move);

        return false;
    }

    internal bool MakeMove(Move move)
    {
        History.Add(new BoardHistory
        {
            Key = PositionKey,
            LastMove = move,
            Castlings = Castlings,
            EnpassantSquare = EnpassantSquare,
            HalfMoveCount = HalfMoveCount,
            FullMoveCount = FullMoveCount
        });

        _ = move.IsHalfmove ? HalfMoveCount++ : HalfMoveCount = 0;
        _ = !WhiteToMove ? FullMoveCount++ : default;

        Squares[move.StartingSquare] = Piece.None;
        Squares[move.EndingSquare] = move.FriendlyPiece;

        ulong bitboard = (1UL << move.StartingSquare) | (1UL << move.EndingSquare);
        Pieces[move.FriendlyPiece] ^= bitboard;
        PiecesByColour[FriendlyColourIndex] ^= bitboard;

        if (move.IsCapture)
        {
            int type = move.OpponentPiece & 0b111;
            Materials[FriendlyColourIndex ^ 1] -= Piece.Values[type];

            bitboard = 1UL << move.EndingSquare;
            Pieces[move.OpponentPiece] ^= bitboard;
            PiecesByColour[FriendlyColourIndex ^ 1] ^= bitboard;

            if (type == Piece.Rook)
            {
                Castlings &= Flags.AllCastlings ^ ((move.EndingSquare >> 3, move.EndingSquare & 7) switch
                {
                    (0, 0) => Flags.WhiteQueensideCastling,
                    (0, 7) => Flags.WhiteKingsideCastling,
                    (7, 0) => Flags.BlackQueensideCastling,
                    (7, 7) => Flags.BlackKingsideCastling,
                    _ => Flags.None
                });
            }
        }

        if (move.IsPromotion)
        {
            int promotion = move.PromotionType | ColourToMove;
            Squares[move.EndingSquare] = promotion;

            Materials[FriendlyColourIndex] += Piece.Values[move.PromotionType] - Piece.Values[1];

            bitboard = 1UL << move.EndingSquare;
            Pieces[move.FriendlyPiece] ^= bitboard;
            Pieces[promotion] ^= bitboard;
        }

        else if (move.IsEnpassantCapture)
        {
            int pawnSquare = EnpassantSquare + (WhiteToMove ? -8 : 8);
            Squares[pawnSquare] = Piece.None;

            Materials[FriendlyColourIndex ^ 1] -= Piece.Values[1];

            bitboard = 1UL << pawnSquare;
            Pieces[Piece.Pawn | (ColourToMove ^ 8)] ^= bitboard;
            PiecesByColour[FriendlyColourIndex ^ 1] ^= bitboard;
        }

        else if (move.IsKingsideCastling)
        {
            int friendlyRook = Piece.Rook | ColourToMove;
            int rookStart = move.EndingSquare + 1;
            int rookEnd = move.EndingSquare - 1;
            Squares[rookStart] = Piece.None;
            Squares[rookEnd] = friendlyRook;
            Castlings &= WhiteToMove ? Flags.BlackCastlings : Flags.WhiteCastlings;

            Kings[FriendlyColourIndex] = move.EndingSquare;

            bitboard = 1UL << rookEnd | 1UL << rookStart;
            Pieces[friendlyRook] ^= bitboard;
            PiecesByColour[FriendlyColourIndex] ^= bitboard;
        }

        else if (move.IsQueensideCastling)
        {
            int friendlyRook = Piece.Rook | ColourToMove;
            int rookStart = move.EndingSquare - 1;
            int rookEnd = move.EndingSquare + 1;
            Squares[rookStart] = Piece.None;
            Squares[rookEnd] = friendlyRook;
            Castlings &= WhiteToMove ? Flags.BlackCastlings : Flags.WhiteCastlings;

            Kings[FriendlyColourIndex] = move.EndingSquare;

            bitboard = 1UL << rookEnd | 1UL << rookStart;
            Pieces[friendlyRook] ^= bitboard;
            PiecesByColour[FriendlyColourIndex] ^= bitboard;
        }

        else if (move.StartingSquare == Kings[FriendlyColourIndex])
        {
            Castlings &= WhiteToMove ? Flags.BlackCastlings : Flags.WhiteCastlings;

            Kings[FriendlyColourIndex] = move.EndingSquare;
        }

        else if (Piece.Type(move.FriendlyPiece) == Piece.Rook)
        {
            Castlings &= Flags.AllCastlings ^ ((move.StartingSquare >> 3, move.StartingSquare & 7) switch
            {
                (0, 0) => Flags.WhiteQueensideCastling,
                (0, 7) => Flags.WhiteKingsideCastling,
                (7, 0) => Flags.BlackQueensideCastling,
                (7, 7) => Flags.BlackKingsideCastling,
                _ => Flags.None
            });
        }

        OccupiedSquares = PiecesByColour[0] | PiecesByColour[1];
        EnpassantSquare = move.IsDoublePawnPush ? move.StartingSquare + (WhiteToMove ? 8 : -8) : -1;
        WhiteToMove = !WhiteToMove;
        ColourToMove ^= 8;
        FriendlyColourIndex = ColourToMove >> 3;
        OpponentAttacks = MoveGenerator.GenerateOpponentAttacks(this);
        PositionKey = ZobristKey.Create(this);

        if (Kings[FriendlyColourIndex ^ 1] != -1 && MoveGenerator.IsAttacked(this, ColourToMove, Kings[FriendlyColourIndex ^ 1]))
        {
            Takeback();
            return false;
        }

        return true;
    }

    internal bool Takeback()
    {
        if (History.Count == 0)
            return false;

        var history = History[^1];
        var move = history.LastMove;
        PositionKey = history.Key;
        Castlings = history.Castlings;
        EnpassantSquare = history.EnpassantSquare;
        HalfMoveCount = history.HalfMoveCount;
        FullMoveCount = history.FullMoveCount;

        WhiteToMove = !WhiteToMove;
        ColourToMove ^= 8;
        FriendlyColourIndex = ColourToMove >> 3;

        Squares[move.StartingSquare] = move.FriendlyPiece;
        Squares[move.EndingSquare] = move.OpponentPiece;

        ulong bitboard = (1UL << move.StartingSquare) | (1UL << move.EndingSquare);
        Pieces[move.FriendlyPiece] ^= bitboard;
        PiecesByColour[FriendlyColourIndex] ^= bitboard;

        if (move.IsCapture)
        {
            Materials[FriendlyColourIndex ^ 1] += Piece.Values[Piece.Type(move.OpponentPiece)];

            bitboard = 1UL << move.EndingSquare;
            Pieces[move.OpponentPiece] ^= bitboard;
            PiecesByColour[FriendlyColourIndex ^ 1] ^= bitboard;
        }

        if (move.IsPromotion)
        {
            Materials[FriendlyColourIndex] += Piece.Values[1] - Piece.Values[move.PromotionType];

            bitboard = 1UL << move.EndingSquare;
            Pieces[move.FriendlyPiece] ^= bitboard;
            Pieces[move.PromotionType | ColourToMove] ^= bitboard;
        }

        else if (move.IsEnpassantCapture)
        {
            int pawnSquare = EnpassantSquare + (WhiteToMove ? -8 : 8);
            int opponentPawn = Piece.Pawn | (ColourToMove ^ 8);
            Squares[pawnSquare] = opponentPawn;

            Materials[FriendlyColourIndex ^ 1] += Piece.Values[1];

            bitboard = 1UL << pawnSquare;
            Pieces[Piece.Pawn | (ColourToMove ^ 8)] ^= bitboard;
            PiecesByColour[FriendlyColourIndex ^ 1] ^= bitboard;
        }

        else if (move.IsKingsideCastling)
        {
            int friendlyRook = Piece.Rook | ColourToMove;
            int rookEnd = move.EndingSquare + 1;
            int rookStart = move.EndingSquare - 1;
            Squares[rookEnd] = friendlyRook;
            Squares[rookStart] = Piece.None;

            Kings[FriendlyColourIndex] = move.StartingSquare;

            bitboard = 1UL << rookEnd | 1UL << rookStart;
            Pieces[friendlyRook] ^= bitboard;
            PiecesByColour[FriendlyColourIndex] ^= bitboard;
        }

        else if (move.IsQueensideCastling)
        {
            int friendlyRook = Piece.Rook | ColourToMove;
            int rookEnd = move.EndingSquare - 2;
            int rookStart = move.EndingSquare + 1;
            Squares[rookEnd] = friendlyRook;
            Squares[rookStart] = Piece.None;

            Kings[FriendlyColourIndex] = move.StartingSquare;

            bitboard = 1UL << rookEnd | 1UL << rookStart;
            Pieces[friendlyRook] ^= bitboard;
            PiecesByColour[FriendlyColourIndex] ^= bitboard;
        }

        else if (move.EndingSquare == Kings[FriendlyColourIndex])
        {
            Kings[FriendlyColourIndex] = move.StartingSquare;
        }

        OccupiedSquares = PiecesByColour[0] | PiecesByColour[1];
        History.RemoveAt(History.Count - 1);
        OpponentAttacks = MoveGenerator.GenerateOpponentAttacks(this);
        return true;
    }
}
