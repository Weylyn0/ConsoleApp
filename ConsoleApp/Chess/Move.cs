namespace Chess;

public struct Move
{
    private readonly int From;
    private readonly int To;
    private readonly int Flag;

    private const int squareMask = 0b00000111111;
    private const int pieceMask = 0b11111000000;

    public int StartingSquare
    {
        get
        {
            return From & squareMask;
        }
    }

    public int EndingSquare
    {
        get
        {
            return To & squareMask;
        }
    }

    public int FriendlyPiece
    {
        get
        {
            return (From & pieceMask) >> 6;
        }
    }

    public int OpponentPiece
    {
        get
        {
            return (To & pieceMask) >> 6;
        }
    }

    public int MoveFlag
    {
        get
        {
            return Flag;
        }
    }

    public bool NoFlag
    {
        get
        {
            return (Flag == MoveFlags.None);
        }
    }

    public bool IsPromotion
    {
        get
        {
            return (Flag & (MoveFlags.PromotionKnight | MoveFlags.PromotionBishop | MoveFlags.PromotionRook | MoveFlags.PromotionQueen)) > 0;
        }
    }

    public bool IsCastling
    {
        get
        {
            return (Flag & (MoveFlags.BlackKingCastling | MoveFlags.BlackQueenCastling | MoveFlags.WhiteKingCastling | MoveFlags.BlackQueenCastling)) != 0;
        }
    }

    public bool IsDoublePawnPush
    {
        get
        {
            return Flag == MoveFlags.DoublePawnPush;
        }
    }

    public bool IsEnpassant
    {
        get
        {
            return Flag == MoveFlags.EnpassantCapture;
        }
    }

    public string Uci
    {
        get
        {
            return $"{Coordinates.GetUci(StartingSquare)}{Coordinates.GetUci(EndingSquare)}{(IsPromotion ? Piece.GetSymbol(Piece.GetPromotionType(Flag)) : string.Empty)}";
        }
    }

    public override string ToString()
    {
        return Uci;
    }

    public Move(int startingSquare, int targetSquare, int friendlyPiece, int opponentPiece) : this(startingSquare, targetSquare, friendlyPiece, opponentPiece, MoveFlags.None)
    {

    }

    public Move(int startingSquare, int targetSquare, int friendlyPiece, int opponentPiece, int flag)
    {
        From = (friendlyPiece << 6) | startingSquare;
        To = (opponentPiece << 6) | targetSquare;
        Flag = flag;
    }

    public bool HasFlag(int flag)
    {
        return (Flag & flag) == flag;
    }
}
