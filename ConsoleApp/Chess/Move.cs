namespace Chess;

public struct Move
{
    private int Value;

    private const int SquareMask = 0b111111;
    private const int PieceMask = 0b1111;

    public int StartingSquare
    {
        get
        {
            return Value & SquareMask;
        }
    }

    public int EndingSquare
    {
        get
        {
            return (Value >> 6) & SquareMask;
        }
    }

    public int FriendlyPiece
    {
        get
        {
            return (Value >> 12) & PieceMask;
        }
    }

    public int OpponentPiece
    {
        get
        {
            return (Value >> 16) & PieceMask;
        }
    }

    public bool IsCapture
    {
        get
        {
            return OpponentPiece != 0;
        }
    }

    public bool IsHalfmove
    {
        get
        {
            return OpponentPiece == 0 && (FriendlyPiece & 0b111) != Piece.Pawn;
        }
    }

    public bool IsPromotion
    {
        get
        {
            return ((Value >> 20) & 0b1111) != 0;
        }
    }

    public bool IsDoublePawnPush
    {
        get
        {
            return ((Value >> 24) & 0b1) != 0;
        }
    }

    public bool IsEnpassantCapture
    {
        get
        {
            return ((Value >> 25) & 0b1) != 0;
        }
    }

    public bool IsKingsideCastling
    {
        get
        {
            return ((Value >> 26) & 0b1) != 0;
        }
    }

    public bool IsQueensideCastling
    {
        get
        {
            return ((Value >> 27) & 0b1) != 0;
        }
    }

    public int PromotionType
    {
        get => ((Value >> 20) & 0b1111) switch
        {
            Flags.PromotionKnight => Piece.Knight,
            Flags.PromotionBishop => Piece.Bishop,
            Flags.PromotionRook => Piece.Rook,
            Flags.PromotionQueen => Piece.Queen,
            _ => Piece.None
        };
    }

    public string Uci
    {
        get
        {
            return $"{Coordinates.SquareToUci(StartingSquare)}{Coordinates.SquareToUci(EndingSquare)}{(IsPromotion ? Piece.GetChar(PromotionType | 8) : string.Empty)}";
        }
    }

    public Move(int startingSquare, int endingSquare, int friendlyPiece, int opponentPiece) : this(startingSquare, endingSquare, friendlyPiece, opponentPiece, Flags.None)
    {

    }

    public Move(int startingSquare, int endingSquare, int friendlyPiece, int opponentPiece, int flag)
    {
        Value = startingSquare | endingSquare << 6 | friendlyPiece << 12 | opponentPiece << 16 | flag << 20;
    }
}
