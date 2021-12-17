using System;
using System.Collections.Generic;

namespace Chess;

public class Board
{
    #region Fields

    private readonly int[] Squares;
    private readonly int[] Kings;

    private int ColourToMove;
    private int Enpassant;
    private byte Flag;

    private readonly List<Move> Moves;
    private List<Move> GeneratedMoves;

    private readonly List<byte> CastlingHistory;
    private byte Castling;

    private ulong PinBitboard;
    private ulong AttackBitboard;

    private int AttackingSquare;
    private bool IsCheck;
    private bool IsDoubleCheck;
    private bool IsKnightCheck;

    #endregion


    #region Properties

    public int this[int index]
    {
        get
        {
            if ((index & 64) == 0)
                return Squares[index];

            return Piece.None;
        }
    }

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
            return 0b11000 - ColourToMove;
        }
    }

    public Move[] LegalMoves
    {
        get
        {
            lock (GeneratedMoves)
            {
                return GeneratedMoves.ToArray();
            }
        }
    }

    public int PlyCount
    {
        get
        {
            lock (Moves)
            {
                return Moves.Count;
            }
        }
    }

    public int FullMoveCount
    {
        get
        {
            lock (Moves)
            {
                return (Moves.Count + 3) / 2;
            }
        }
    }

    public int HalfMoveCount
    {
        get
        {
            lock (Moves)
            {
                for (int moveIndex = 1; moveIndex <= Moves.Count; moveIndex++)
                {
                    var move = Moves[^moveIndex];
                    if (Piece.IsType(move.FriendlyPiece, Piece.Pawn) || !Piece.IsEmpty(move.OpponentPiece))
                    {
                        return moveIndex - 1;
                    }
                }

                return Moves.Count;
            }
        }
    }

    public int KingSquare
    {
        get
        {
            lock (Squares)
            {
                return ColourToMove == Piece.White ? Kings[0] : Kings[1];
            }
        }
    }

    public int WhiteKing
    {
        get
        {
            lock (Kings)
            {
                return Kings[0];
            }
        }
    }

    public int BlackKing
    {
        get
        {
            lock (Kings)
            {
                return Kings[1];
            }
        }
    }

    public bool Check
    {
        get
        {
            return IsCheck;
        }
    }

    public bool DoubleCheck
    {
        get
        {
            return IsDoubleCheck;
        }
    }

    public int EndFlag
    {
        get
        {
            return Flag;
        }
    }

    public int Winner
    {
        get
        {
            return IsCheckmate ? OpponentColour : 0;
        }
    }

    public bool IsCheckmate
    {
        get
        {
            return EndFlag == EndFlags.Checkmate;
        }
    }

    public bool IsStalemate
    {
        get
        {
            return EndFlag == EndFlags.Stalemate;
        }
    }

    public bool QueenSideCastling
    {
        get
        {
            return (Castling & (ColourToMove == Piece.White ? Castlings.WhiteQueenSide : Castlings.BlackQueenSide)) > 0;
        }
    }

    public bool KingSideCastling
    {
        get
        {
            return (Castling & (ColourToMove == Piece.White ? Castlings.WhiteKingSide : Castlings.BlackKingSide)) > 0;
        }
    }

    public string CastlingText
    {
        get
        {
            return Castling == Castlings.None ? "-" : $"{((Castling & Castlings.WhiteKingSide) > 0 ? "K" : string.Empty)}{((Castling & Castlings.WhiteQueenSide) > 0 ? "Q" : string.Empty)}{((Castling & Castlings.BlackKingSide) > 0 ? "k" : string.Empty)}{((Castling & Castlings.BlackQueenSide) > 0 ? "q" : string.Empty)}";
        }
    }

    public int EnpassantSquare
    {
        get
        {
            return Enpassant;
        }
    }

    public string Pgn
    {
        get
        {
            string pgn = $"[Event \"Weychess\"]\n[UTCDate \"{DateTime.UtcNow:yyyy.MM.dd}\"]\n[UTCTime \"{DateTime.UtcNow:HH:mm:ss}\"]";
            /* TODO */
            return pgn;
        }
    }

    public string Fen
    {
        get
        {
            lock (Squares)
            {
                string fen = string.Empty;
                for (int rank = 0; rank < 8; rank++)
                {
                    int k = 0;
                    for (int file = 0; file < 8; file++)
                    {
                        int piece = Squares[(7 - rank) * 8 + file];
                        if (Piece.IsEmpty(piece))
                            k++;

                        else
                        {
                            fen += $"{((k > 0) ? k : string.Empty)}{Piece.GetSymbol(piece)}";
                            k = 0;
                        }
                    }
                    fen += (k > 0) ? k : string.Empty;
                    fen += (rank < 7) ? "/" : string.Empty;
                }
                fen += $" {((ColourToMove == Piece.White) ? 'w' : 'b')} {CastlingText} {UniversalChessInterface.GetUci(Enpassant)} {HalfMoveCount} {FullMoveCount}";
                return fen;
            }
        }
    }

    public string AttackMap
    {
        get
        {
            lock (Squares)
            {
                string text = string.Empty;
                for (int rank = 0; rank < 8; rank++)
                {
                    text += $"  {8 - rank} ";
                    for (int file = 0; file < 8; file++)
                    {
                        text += $" {(IsAttacked((7 - rank) * 8 + file) ? 'x' : '_')}";
                    }
                    text += "\n";
                }
                text += "\n     A B C D E F G H\n";
                return text;
            }
        }
    }

    public override string ToString()
    {
        lock (Squares)
        {
            string text = $"\n  Turn: {(ColourToMove == Piece.White ? "White" : "Black")}\n  Castlings: {CastlingText}\n  Enpassant Square: {EnpassantSquare}\n  FEN: {Fen}\n\n";
            for (int rank = 0; rank < 8; rank++)
            {
                text += $"  {8 - rank} ";
                for (int file = 0; file < 8; file++)
                {
                    text += $" {Piece.GetSymbol(Squares[(7 - rank) * 8 + file])}";
                }
                text += "\n";
            }
            text += "\n     A B C D E F G H\n";
            return text;
        }
    }

    #endregion


    #region Constructors

    public Board()
    {
        Squares = new int[64];
        Kings = new int[2] { -1, -1 };

        ColourToMove = Piece.White;
        Enpassant = -1;
        Flag = EndFlags.None;

        Moves = new List<Move>();
        GeneratedMoves = new List<Move>();

        CastlingHistory = new List<byte>();
        Castling = Castlings.None;

        PinBitboard = 0UL;
        AttackBitboard = 0UL;

        AttackingSquare = -1;
        IsCheck = false;
        IsDoubleCheck = false;
        IsKnightCheck = false;
    }

    public static Board FromFen(string fen)
    {
        var board = new Board();
        var splittedFen = fen.Split(' ');
        var rows = splittedFen[0].Split('/');
        for (int rank = 0; rank < 8; rank++)
        {
            int file = 0;
            foreach (var c in rows[7 - rank])
            {
                if (char.IsDigit(c))
                {
                    file += c - 48;
                }

                else
                {
                    board.Squares[rank * 8 + file] = Piece.GetPiece(c);
                    file++;
                }
            }
        }

        if (splittedFen.Length > 1)
            board.ColourToMove = (splittedFen[1] == "b" ? Piece.Black : Piece.White);

        if (splittedFen.Length > 2)
        {
            board.Castling = Castlings.None;
            board.Castling |= splittedFen[2].Contains('K') ? Castlings.WhiteKingSide : Castlings.None;
            board.Castling |= splittedFen[2].Contains('Q') ? Castlings.WhiteQueenSide : Castlings.None;
            board.Castling |= splittedFen[2].Contains('k') ? Castlings.BlackKingSide : Castlings.None;
            board.Castling |= splittedFen[2].Contains('q') ? Castlings.BlackQueenSide : Castlings.None;
        }

        if (splittedFen.Length > 3)
        {
            board.Enpassant = UniversalChessInterface.GetSquareFromUci(splittedFen[3]);
        }

        board.Calculate();
        return board;
    }

    public static Board StartingChessPosition()
    {
        var board = new Board();

        /* White Pieces */
        board.Squares[0] = Piece.Rook | Piece.White;
        board.Squares[1] = Piece.Knight | Piece.White;
        board.Squares[2] = Piece.Bishop | Piece.White;
        board.Squares[3] = Piece.Queen | Piece.White;
        board.Squares[4] = Piece.King | Piece.White;
        board.Squares[5] = Piece.Bishop | Piece.White;
        board.Squares[6] = Piece.Knight | Piece.White;
        board.Squares[7] = Piece.Rook | Piece.White;

        /* White Pawns */
        board.Squares[8] = Piece.Pawn | Piece.White;
        board.Squares[9] = Piece.Pawn | Piece.White;
        board.Squares[10] = Piece.Pawn | Piece.White;
        board.Squares[11] = Piece.Pawn | Piece.White;
        board.Squares[12] = Piece.Pawn | Piece.White;
        board.Squares[13] = Piece.Pawn | Piece.White;
        board.Squares[14] = Piece.Pawn | Piece.White;
        board.Squares[15] = Piece.Pawn | Piece.White;

        /* Black Pawns */
        board.Squares[48] = Piece.Pawn | Piece.Black;
        board.Squares[49] = Piece.Pawn | Piece.Black;
        board.Squares[50] = Piece.Pawn | Piece.Black;
        board.Squares[51] = Piece.Pawn | Piece.Black;
        board.Squares[52] = Piece.Pawn | Piece.Black;
        board.Squares[53] = Piece.Pawn | Piece.Black;
        board.Squares[54] = Piece.Pawn | Piece.Black;
        board.Squares[55] = Piece.Pawn | Piece.Black;

        /* Black Pieces */
        board.Squares[56] = Piece.Rook | Piece.Black;
        board.Squares[57] = Piece.Knight | Piece.Black;
        board.Squares[58] = Piece.Bishop | Piece.Black;
        board.Squares[59] = Piece.Queen | Piece.Black;
        board.Squares[60] = Piece.King | Piece.Black;
        board.Squares[61] = Piece.Bishop | Piece.Black;
        board.Squares[62] = Piece.Knight | Piece.Black;
        board.Squares[63] = Piece.Rook | Piece.Black;

        board.Castling = Castlings.All;
        board.Calculate();
        return board;
    }

    #endregion


    #region Move

    public bool Push(string uci)
    {
        return Push(UniversalChessInterface.GetMoveFromUci(this, uci));
    }

    public bool Push(int startingSquare, int endingSquare)
    {
        return Push(startingSquare, endingSquare, MoveFlags.None);
    }

    public bool Push(int startingSquare, int endingSquare, byte flag)
    {
        return Push(new Move(startingSquare, endingSquare, Squares[startingSquare], Squares[endingSquare], flag));
    }

    public bool Push(Move move)
    {
        if (!GeneratedMoves.Contains(move))
            return false;

        MakeMove(move);
        Moves.Add(move);
        ColourToMove = OpponentColour;
        Calculate();
        return true;
    }

    public bool Takeback()
    {
        if (Moves.Count == 0)
            return false;

        var lastMove = Moves[^1];
        Moves.RemoveAt(Moves.Count - 1);
        UnmakeMove(lastMove);
        ColourToMove = OpponentColour;
        Calculate();
        return true;
    }

    private void MakeMove(Move move)
    {
        CastlingHistory.Add(Castling);
        Squares[move.StartingSquare] = Piece.None;
        Squares[move.EndingSquare] = move.FriendlyPiece;
        Enpassant = move.IsDoublePawnPush ? move.EndingSquare + (move.FriendlyColour == Piece.White ? -8 : 8) : -1;

        if (move.IsPromotion)
        {
            Squares[move.EndingSquare] = move.PromotionType | move.FriendlyColour;
        }

        else if (move.IsEnpassant)
        {
            Squares[move.EndingSquare + (move.FriendlyColour == Piece.White ? -8 : 8)] = Piece.None;
        }

        else if (move.IsCastling)
        {
            Squares[move.CastlingRookSquare] = Piece.None;
            Squares[move.CastlingRookSquare + (move.IsQueensideCastling ? 3 : -2)] = Piece.Rook | move.FriendlyColour;
            Castling ^= move.FriendlyColour == Piece.White ? Castlings.White : Castlings.Black;
        }

        else
        {
            if (Piece.IsType(move.FriendlyPiece, Piece.King))
                Castling &= move.FriendlyColour == Piece.White ? Castlings.Black : Castlings.White;

            else if (Piece.IsType(move.FriendlyPiece, Piece.Rook))
                Castling ^= move.StartingSquare switch
                {
                    Coordinates.A1 => (byte)(move.FriendlyColour == Piece.White ? (Castlings.All ^ Castlings.WhiteQueenSide) : Castlings.None),
                    Coordinates.A8 => (byte)(move.FriendlyColour == Piece.Black ? (Castlings.All ^ Castlings.BlackQueenSide) : Castlings.None),
                    Coordinates.H1 => (byte)(move.FriendlyColour == Piece.White ? (Castlings.All ^ Castlings.WhiteKingSide) : Castlings.None),
                    Coordinates.H8 => (byte)(move.FriendlyColour == Piece.Black ? (Castlings.All ^ Castlings.BlackKingSide) : Castlings.None),
                    _ => Castlings.None
                };
        }
    }

    private void UnmakeMove(Move move)
    {
        if (CastlingHistory.Count > 0)
        {
            Castling = CastlingHistory[^1];
            CastlingHistory.RemoveAt(CastlingHistory.Count - 1);
        }

        Squares[move.StartingSquare] = move.FriendlyPiece;
        Squares[move.EndingSquare] = move.OpponentPiece;
        Enpassant = Moves.Count > 0 ? (Moves[^1].IsDoublePawnPush ? Moves[^1].EndingSquare + (Moves[^1].FriendlyColour == Piece.White ? -8 : 8) : -1) : -1;

        if (move.IsPromotion)
        {
            Squares[move.StartingSquare] = Piece.Pawn | move.FriendlyColour;
        }

        else if (move.IsEnpassant)
        {
            Squares[move.EndingSquare + (move.FriendlyColour == Piece.White ? -8 : 8)] = Piece.Pawn | move.OpponentColour;
        }

        else if (move.IsCastling)
        {
            Squares[move.CastlingRookSquare] = Piece.Rook | move.FriendlyColour;
            Squares[move.CastlingRookSquare + (move.IsQueensideCastling ? 3 : -2)] = Piece.None;
        }
    }

    #endregion


    #region Move Generation

    private void Calculate()
    {
        /* Finding king squares */
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
            if (Squares[squareIndex] == (Piece.King | Piece.White))
                Kings[0] = squareIndex;

            else if (Squares[squareIndex] == (Piece.King | Piece.Black))
                Kings[1] = squareIndex;

        /* Regenerating pin and attack bitboard */
        PinBitboard = Bitboard.GetPinBitboard(this);

        AttackingSquare = -1;
        AttackBitboard = 0UL;
        IsCheck = false;
        IsDoubleCheck = false;
        IsKnightCheck = false;
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
        {
            if (Piece.IsColour(Squares[squareIndex], OpponentColour))
            {
                ulong bitboard = Bitboard.GetAttackBitboard(this, squareIndex);
                if (((bitboard >> KingSquare) & 1) == 1)
                {
                    AttackingSquare = squareIndex;
                    IsDoubleCheck = IsCheck;
                    IsCheck = true;
                    IsKnightCheck = Piece.IsType(Squares[squareIndex], Piece.Knight);
                }
                AttackBitboard |= bitboard;
            }
        }

        /* Generate only moves for king on double check */
        if (IsDoubleCheck)
            GeneratedMoves = GenerateLegalMoves(KingSquare);

        /* Generate legal moves for each occupied square */
        var moves = new List<Move>();
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
        {
            if (Piece.IsColour(Squares[squareIndex], FriendlyColour))
            {
                moves.AddRange(GenerateLegalMoves(squareIndex));
            }
        }

        /* Removing illegal moves is king under attack */
        if (IsKnightCheck)
        {
            moves.RemoveAll(move => (move.StartingSquare != KingSquare) && (move.EndingSquare != AttackingSquare));
        }

        else if (IsCheck)
        {
            ulong attackRay = Bitboard.RayBitboard(AttackingSquare, KingSquare, false);
            ulong beyondAttackRay = Bitboard.RayBitboard(AttackingSquare, KingSquare, true);
            for (int moveIndex = 0; moveIndex < moves.Count; moveIndex++)
            {
                if (moves[moveIndex].StartingSquare == KingSquare && ((beyondAttackRay >> moves[moveIndex].EndingSquare) & 1) == 0)
                    continue;

                if (moves[moveIndex].StartingSquare != KingSquare && ((attackRay >> moves[moveIndex].EndingSquare) & 1) == 1)
                    continue;

                moves.RemoveAt(moveIndex);
                moveIndex--;
            }
        }
        GeneratedMoves = moves;

        /* Checking for end after all calculations for current position */
        if (LegalMoves.Length == 0)
        {
            Flag = IsCheck ? EndFlags.Checkmate : EndFlags.Stalemate;
        }
    }

    private List<Move> GenerateLegalMoves(int squareIndex)
    {
        var moves = new List<Move>();
        int piece = Squares[squareIndex];
        int rank = squareIndex >> 3;
        int file = squareIndex & 7;
        bool pinned = IsPinned(squareIndex);
        int pinDirection = pinned ? PrecomputedMoveData.Lookup(KingSquare, squareIndex) : -1;

        /* Generating moves for queen, rook and bishop */
        if (Piece.IsSlidingPiece(piece))
        {
            for (int directionIndex = (Piece.IsType(piece, Piece.Bishop) ? 4 : 0); directionIndex < (Piece.IsType(piece, Piece.Rook) ? 4 : 8); directionIndex++)
            {
                if (!pinned || ((pinDirection | 1) == (directionIndex | 1)))
                {
                    for (int n = 0; n < PrecomputedMoveData.Rays[squareIndex][directionIndex]; n++)
                    {
                        int targetSquare = squareIndex + PrecomputedMoveData.DirectionsOffsets[directionIndex] * (n + 1);
                        int targetPiece = Squares[targetSquare];

                        /* Can't go any further because a friendly piece blocked the way */
                        if (Piece.IsColour(targetPiece, FriendlyColour))
                            break;

                        moves.Add(new Move(squareIndex, targetSquare, piece, targetPiece));

                        /* Can't go any further after capturing opponent piece */
                        if (!Piece.IsEmpty(targetPiece))
                            break;
                    }
                }
            }
        }

        /* Generating moves for knight */
        else if (Piece.IsType(piece, Piece.Knight))
        {
            /* Knights can't move while pinned */
            if (!pinned)
            {
                for (int knightRank = -2; knightRank <= 2; knightRank++)
                {
                    for (int knightFile = -2; knightFile <= 2; knightFile++)
                    {
                        if (Math.Abs(knightRank) == Math.Abs(knightFile) || knightRank == 0 || knightFile == 0)
                            continue;

                        int newRank = rank + knightRank;
                        int newFile = file + knightFile;

                        if ((newRank & 8) != 0 || (newFile & 8) != 0)
                            continue;

                        int targetSquare = newRank * 8 + newFile;
                        int targetPiece = Squares[targetSquare];

                        /* Can't leap on a friendly piece */
                        if (Piece.IsColour(targetPiece, FriendlyColour))
                            continue;

                        moves.Add(new Move(squareIndex, targetSquare, piece, targetPiece));
                    }
                }
            }
        }

        /* Generating moves for king */
        else if (Piece.IsType(piece, Piece.King))
        {
            for (int directionIndex = 0; directionIndex < 8; directionIndex++)
            {
                if (PrecomputedMoveData.Rays[squareIndex][directionIndex] > 0)
                {
                    int targetSquare = squareIndex + PrecomputedMoveData.DirectionsOffsets[directionIndex];

                    /* Can't move to a attacked square */
                    if (IsAttacked(targetSquare))
                        continue;

                    int targetPiece = Squares[targetSquare];

                    /* Can't move over a friendly piece */
                    if (Piece.IsColour(targetPiece, FriendlyColour))
                        continue;

                    moves.Add(new Move(squareIndex, targetSquare, piece, targetPiece));
                }
            }

            /* Generating castlings */
            if ((squareIndex == (FriendlyColour == Piece.White ? Coordinates.E1 : Coordinates.E8)) && !IsAttacked(squareIndex))
            {
                bool failed = false;
                int rookSquare = FriendlyColour == Piece.White ? Coordinates.A1 : Coordinates.A8;
                if (QueenSideCastling && Squares[rookSquare] == (Piece.Rook | FriendlyColour) && Piece.IsEmpty(Squares[rookSquare + 1]))
                {
                    int targetSquare = KingSquare - 2;

                    for (int i = targetSquare; i < KingSquare; i++)
                    {
                        if (!Piece.IsEmpty(Squares[i]) || IsAttacked(i))
                        {
                            failed = true;
                            break;
                        }
                    }

                    if (!failed)
                        moves.Add(new Move(squareIndex, targetSquare, piece, Piece.None, MoveFlags.QueensideCastling));
                }

                failed = false;
                rookSquare += 7;
                if (KingSideCastling && Squares[rookSquare] == (Piece.Rook | FriendlyColour))
                {
                    int targetSquare = KingSquare + 2;

                    for (int i = targetSquare; i > KingSquare; i--)
                    {
                        if (!Piece.IsEmpty(Squares[i]) || IsAttacked(i))
                        {
                            failed = true;
                            break;
                        }
                    }

                    if (!failed)
                        moves.Add(new Move(squareIndex, targetSquare, piece, Piece.None, MoveFlags.KingsideCastling));
                }
            }
        }

        /* Generate moves for pawn */
        else if (Piece.IsType(piece, Piece.Pawn))
        {
            int directionIndex = Piece.Colour(piece) == Piece.White ? 0 : 1;
            int direction = PrecomputedMoveData.DirectionsOffsets[directionIndex];
            int startingRank = (directionIndex == 0) ? 1 : 6;
            int promotionRank = 7 - startingRank;

            /* Generating forward moves */
            int targetSquare = squareIndex + direction;
            if (!pinned || (pinDirection == directionIndex))
            {
                if (Piece.IsEmpty(Squares[targetSquare]))
                {
                    if (rank == promotionRank)
                    {
                        for (int promotion = 0; promotion < 4; promotion++)
                        {
                            moves.Add(new Move(squareIndex, targetSquare, piece, Piece.None, (byte)(MoveFlags.PromotionKnight << promotion)));
                        }
                    }

                    else
                    {
                        moves.Add(new Move(squareIndex, targetSquare, piece, Piece.None));
                        if (rank == startingRank)
                        {
                            targetSquare += direction;
                            if (Piece.IsEmpty(Squares[targetSquare]))
                            {
                                moves.Add(new Move(squareIndex, targetSquare, piece, Piece.None, MoveFlags.DoublePawnPush));
                            }
                        }
                    }
                }
            }

            /* Generating capture moves */
            if (file > 0 && (!pinned || (pinDirection == (direction == 8 ? 4 : 7))))
            {
                targetSquare = squareIndex + direction - 1;
                int targetPiece = Squares[targetSquare];
                if (Piece.IsColour(targetPiece, OpponentColour))
                {
                    if (rank == promotionRank)
                    {
                        for (int promotion = 0; promotion < 4; promotion++)
                        {
                            moves.Add(new Move(squareIndex, targetSquare, piece, targetPiece, (byte)(MoveFlags.PromotionKnight << promotion)));
                        }
                    }

                    else
                    {
                        moves.Add(new Move(squareIndex, targetSquare, piece, targetPiece));
                    }
                }

                else if (targetSquare == Enpassant)
                {
                    var enpassantMove = new Move(squareIndex, targetSquare, piece, Piece.None, MoveFlags.EnpassantCapture);

                    if (KingSquare == -1 || (rank != (KingSquare >> 3)) || !KingAttackedAfterEnpassant(enpassantMove))
                        moves.Add(enpassantMove);
                }
            }

            if (file < 7 && (!pinned || (pinDirection == (direction == 8 ? 6 : 5))))
            {
                targetSquare = squareIndex + direction + 1;
                int targetPiece = Squares[targetSquare];
                if (Piece.IsColour(targetPiece, OpponentColour))
                {
                    if (rank == promotionRank)
                    {
                        for (int promotion = 0; promotion < 4; promotion++)
                        {
                            moves.Add(new Move(squareIndex, targetSquare, piece, targetPiece, (byte)(MoveFlags.PromotionKnight << promotion)));
                        }
                    }

                    else
                    {
                        moves.Add(new Move(squareIndex, targetSquare, piece, targetPiece));
                    }
                }

                else if (targetSquare == Enpassant)
                {
                    var enpassantMove = new Move(squareIndex, targetSquare, piece, Piece.None, MoveFlags.EnpassantCapture);

                    if (KingSquare == -1 || (rank != (KingSquare >> 3)) || !KingAttackedAfterEnpassant(enpassantMove))
                        moves.Add(enpassantMove);
                }
            }
        }
        return moves;
    }

    private bool KingAttackedAfterEnpassant(Move enpassantMove)
    {
        MakeMove(enpassantMove);
        ulong bitboard = 0UL;
        for (int file = 0; file < 8; file++)
        {
            int squareIndex = enpassantMove.StartingSquare & 0b111000 | file;
            if (Piece.IsColour(Squares[squareIndex], OpponentColour) && Piece.IsRookOrQueen(Squares[squareIndex]))
                bitboard |= Bitboard.GetAttackBitboard(this, squareIndex);
        }

        UnmakeMove(enpassantMove);
        return ((bitboard >> KingSquare) & 1) == 1;
    }

    private bool IsAttacked(int squareIndex)
    {
        return ((AttackBitboard >> squareIndex) & 1) == 1;
    }

    private bool IsPinned(int squareIndex)
    {
        return ((PinBitboard >> squareIndex) & 1) == 1;
    }

    #endregion
}
