namespace Chess;

public static class ForsythEdwardsNotation
{
    public const string StartingPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public const string FullCastlings = "KQkq";

    public static void LoadFen(Board board, string fen)
    {
        if (fen.Length < 15)
            return;

        string[] splittedFen = fen.Split(' ');
        string[] position = splittedFen[0].Split('/');

        if (position.Length != 8)
            return;

        board.Reset();

        if (splittedFen.Length > 1)
        {
            board.WhiteToMove = splittedFen[1] != "b";
            board.ColourToMove = board.WhiteToMove ? Piece.White : Piece.Black;
            board.FriendlyColourIndex = board.ColourToMove >> 3;
        }

        if (splittedFen.Length > 2)
        {
            board.Castlings = Flags.None;
            for (int castling = 0; castling < 4; castling++)
                if (splittedFen[2].Contains(FullCastlings[castling]))
                    board.Castlings |= 1 << castling;
        }

        if (splittedFen.Length > 3)
        {
            board.EnpassantSquare = Coordinates.SquareFromUci(splittedFen[3]);
        }

        if (splittedFen.Length > 4 && int.TryParse(splittedFen[4], out int halfMoveCount))
        {
            board.HalfMoveCount = halfMoveCount;
        }

        if (splittedFen.Length > 5 && int.TryParse(splittedFen[5], out int fullMoveCount))
        {
            board.FullMoveCount = fullMoveCount;
        }

        for (int rank = 0; rank < 8; rank++)
        {
            int file = 0;
            foreach (char c in position[rank])
            {
                if (char.IsDigit(c))
                {
                    file += c - '0';
                }

                else
                {
                    int square = ((7 - rank) << 3) | file;
                    int piece = Piece.GetPiece(c);
                    int type = Piece.Type(piece);
                    int colourIndex = Piece.ColourIndex(piece);
                    board.Squares[square] = piece;
                    board.Materials[colourIndex] += Piece.Values[type];
                    var bitboard = 1UL << square;
                    board.Pieces[piece] |= bitboard;
                    board.PiecesByColour[colourIndex] |= bitboard;
                    file++;
                    if (type == Piece.King)
                        board.Kings[colourIndex] = square;
                }
            }
        }

        board.OccupiedSquares = board.PiecesByColour[0] | board.PiecesByColour[1];
        board.OpponentAttacks = MoveGenerator.GenerateOpponentAttacks(board);
        board.PositionKey = ZobristKey.Create(board);
    }
}
