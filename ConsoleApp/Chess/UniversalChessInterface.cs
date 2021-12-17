namespace Chess;

public static class UniversalChessInterface
{
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

        byte flag = MoveFlags.None;
        if (uci.Length == 4)
        {
            if (startingSquare == board.KingSquare && board.KingSquare - endingSquare == 2)
            {
                flag = MoveFlags.QueensideCastling;
            }

            else if (startingSquare == board.KingSquare && board.KingSquare - endingSquare == -2)
            {
                flag = MoveFlags.KingsideCastling;
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
            return new Move(startingSquare, endingSquare, board[startingSquare], board[endingSquare], Piece.GetPieceType(uci[4]) switch
            {
                Piece.Knight => MoveFlags.PromotionKnight,
                Piece.Bishop => MoveFlags.PromotionBishop,
                Piece.Rook => MoveFlags.PromotionRook,
                Piece.Queen => MoveFlags.PromotionQueen,
                _ => 0
            });
        }

        else
        {
            return default;
        }

        return new Move(startingSquare, endingSquare, board[startingSquare], board[endingSquare], flag);
    }
}
