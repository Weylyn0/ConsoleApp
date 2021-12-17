namespace Chess;

public struct Move
{
    #region Fields

    private readonly int From;
    private readonly int To;
    private readonly byte Flag;

    #endregion


    #region Masks

    private const int squareMask = 0b00000111111;
    private const int pieceMask = 0b11111000000;
    private const int colourMask = 0b11000000000;

    #endregion


    #region Properties

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

    public int FriendlyColour
    {
        get
        {
            return (From & colourMask) >> 6; 
        }
    }

    public int OpponentColour
    {
        get
        {
            return FriendlyColour ^ (colourMask >> 6);
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
            return (Flag & (MoveFlags.QueensideCastling | MoveFlags.KingsideCastling)) > 0;
        }
    }

    public bool IsQueensideCastling
    {
        get
        {
            return Flag == MoveFlags.QueensideCastling;
        }
    }

    public bool IsKingsideCastling
    {
        get
        {
            return Flag == MoveFlags.KingsideCastling;
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

    public int PromotionType
    {
        get => Flag switch
        {
            MoveFlags.PromotionKnight => Piece.Knight,
            MoveFlags.PromotionBishop => Piece.Bishop,
            MoveFlags.PromotionRook => Piece.Rook,
            MoveFlags.PromotionQueen => Piece.Queen,
            _ => Piece.None
        };
    }

    public int CastlingRookSquare
    {
        get => Flag switch
        {
            MoveFlags.QueensideCastling => FriendlyColour == Piece.White ? 0 : 56,
            MoveFlags.KingsideCastling => FriendlyColour == Piece.White ? 7 : 63,
            _ => -1
        };
    }

    public string Uci
    {
        get
        {
            return $"{UniversalChessInterface.GetUci(StartingSquare)}{UniversalChessInterface.GetUci(EndingSquare)}{(IsPromotion ? Piece.GetSymbol(PromotionType) : string.Empty)}";
        }
    }

    #endregion


    #region Constructor

    public Move(int startingSquare, int targetSquare, int friendlyPiece, int opponentPiece) : this(startingSquare, targetSquare, friendlyPiece, opponentPiece, MoveFlags.None)
    {

    }

    public Move(int startingSquare, int targetSquare, int friendlyPiece, int opponentPiece, byte flag)
    {
        From = (friendlyPiece << 6) | startingSquare;
        To = (opponentPiece << 6) | targetSquare;
        Flag = flag;
    }

    #endregion
}
