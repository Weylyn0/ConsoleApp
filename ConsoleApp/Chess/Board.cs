using System;
using System.Collections.Generic;

namespace Chess;

public class Board
{
    private readonly int[] Squares;
    private readonly int[] Kings;
    private readonly List<Move> Moves;
    private List<Move> GeneratedMoves;
    private readonly Dictionary<int, int> Pins;
    private readonly List<byte> CastlingHistory;
    private byte Flag;
    private byte Castling;
    private int ColourToMove;
    private int Enpassant;

    private int AttackingSquare;
    private ulong AttackBitboard;
    private bool IsCheck;
    private bool IsDoubleCheck;
    private bool IsKnightCheck;

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
                return Piece.IsWhite(ColourToMove) ? Kings[0] : Kings[1];
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

    public bool WhiteQueenSideCastling
    {
        get
        {
            return (Castling & Castlings.WhiteQueenSide) == Castlings.WhiteQueenSide;
        }
    }

    public bool WhiteKingSideCastling
    {
        get
        {
            return (Castling & Castlings.WhiteKingSide) == Castlings.WhiteKingSide;
        }
    }

    public bool BlackQueenSideCastling
    {
        get
        {
            return (Castling & Castlings.BlackQueenSide) == Castlings.BlackQueenSide;
        }
    }

    public bool BlackKingSideCastling
    {
        get
        {
            return (Castling & Castlings.BlackKingSide) == Castlings.BlackKingSide;
        }
    }

    public string CastlingText
    {
        get
        {
            return $"{(WhiteKingSideCastling ? 'K' : string.Empty)}{(WhiteQueenSideCastling ? 'Q' : string.Empty)}{(BlackKingSideCastling ? 'k' : string.Empty)}{(BlackQueenSideCastling ? 'q' : string.Empty)}{((Castling == Castlings.None) ? '-' : string.Empty)}";
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
                fen += $" {((ColourToMove == Piece.White) ? 'w' : 'b')} {CastlingText} {Coordinates.GetUci(Enpassant)} {HalfMoveCount} {FullMoveCount}";
                return fen;
            }
        }
    }

    public string AttackersMap
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

    public string String
    {
        get
        {
            lock (Squares)
            {
                string text = $"\n  Turn: {(Piece.IsWhite(ColourToMove) ? "White" : "Black")}\n  Castlings: {CastlingText}\n  Enpassant Square: {EnpassantSquare}\n  FEN: {Fen}\n\n";
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
    }

    public override string ToString()
    {
        return String;
    }

    #endregion

    public Board()
    {
        Squares = new int[64];
        Kings = new int[2] { -1, -1 };
        Moves = new List<Move>();
        GeneratedMoves = new List<Move>();
        Pins = new Dictionary<int, int>();
        CastlingHistory = new List<byte>();
        ColourToMove = Piece.White;
        Castling = Castlings.None;
        Enpassant = -1;
        AttackingSquare = -1;
        AttackBitboard = 0UL;
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
            board.Enpassant = Coordinates.GetSquareFromUci(splittedFen[3]);
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

    public void GenerateLegalMoves()
    {
        /* Generate only moves for king on double check */
        if (IsDoubleCheck)
            GeneratedMoves = GenerateLegalMoves(KingSquare);

        /* Generate legal moves for each occupied square */
        var moves = new List<Move>();
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
        {
            if (Piece.IsColour(Squares[squareIndex], ColourToMove))
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
    }

    public List<Move> GenerateLegalMoves(int squareIndex)
    {
        var moves = new List<Move>();
        int piece = Squares[squareIndex];
        int rank = squareIndex >> 3;
        int file = squareIndex & 7;
        bool pinned = IsPinned(squareIndex);

        /* Generate moves for queen, rook and bishop */
        if (Piece.IsSlidingPiece(piece))
        {
            for (int directionIndex = (Piece.IsType(piece, Piece.Bishop) ? 4 : 0); directionIndex < (Piece.IsType(piece, Piece.Rook) ? 4 : 8); directionIndex++)
            {
                if (!pinned || ((Pins[squareIndex] | 1) == (directionIndex | 1)))
                {
                    for (int i = 0; i < PrecomputedMoveData.Rays[squareIndex][directionIndex]; i++)
                    {
                        int targetSquare = squareIndex + PrecomputedMoveData.DirectionsOffsets[directionIndex] * (i + 1);

                        if (Piece.IsColour(Squares[targetSquare], ColourToMove))
                            break;

                        moves.Add(new Move(squareIndex, targetSquare, piece, Squares[targetSquare]));

                        if (!Piece.IsEmpty(Squares[targetSquare]))
                            break;
                    }
                }
            }
        }

        /* Generate moves for knight */
        else if (Piece.IsType(piece, Piece.Knight))
        {
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
                        if (Piece.IsColour(Squares[targetSquare], ColourToMove))
                            continue;

                        moves.Add(new Move(squareIndex, targetSquare, piece, Squares[targetSquare]));
                    }
                }
            }
        }

        /* Generate moves for king */
        else if (Piece.IsType(piece, Piece.King))
        {
            for (int directionIndex = 0; directionIndex < 8; directionIndex++)
            {
                if (PrecomputedMoveData.Rays[squareIndex][directionIndex] > 0)
                {
                    int targetSquare = squareIndex + PrecomputedMoveData.DirectionsOffsets[directionIndex];
                    if (IsAttacked(targetSquare))
                        continue;

                    if (Piece.IsColour(Squares[targetSquare], ColourToMove))
                        continue;

                    moves.Add(new Move(squareIndex, targetSquare, piece, Squares[targetSquare]));
                }
            }

            /* Generate castlings */
            if (file == 4 && !IsAttacked(squareIndex))
            {
                bool failed;
                if (Piece.IsWhite(piece))
                {
                    if (rank == 0)
                    {
                        if (WhiteQueenSideCastling && (Squares[0] == (Piece.Rook | Piece.White)))
                        {
                            failed = false;
                            for (int i = 2; i < 4; i++)
                            {
                                if (!Piece.IsEmpty(Squares[i]) || IsAttacked(i))
                                {
                                    failed = true;
                                    break;
                                }
                            }

                            if (!failed)
                            {
                                moves.Add(new Move(4, 2, piece, Piece.None, MoveFlags.WhiteQueenCastling));
                            }
                        }

                        if (WhiteKingSideCastling && (Squares[7] == (Piece.Rook | Piece.White)))
                        {
                            failed = false;
                            for (int i = 5; i < 7; i++)
                            {
                                if (!Piece.IsEmpty(Squares[i]) || IsAttacked(i))
                                {
                                    failed = true;
                                    break;
                                }
                            }

                            if (!failed)
                            {
                                moves.Add(new Move(4, 6, piece, Piece.None, MoveFlags.WhiteKingCastling));
                            }
                        }
                    }
                }

                else
                {
                    if (rank == 7)
                    {
                        if (BlackQueenSideCastling && (Squares[56] == (Piece.Rook | Piece.Black)))
                        {
                            failed = false;
                            for (int i = 58; i < 60; i++)
                            {
                                if (!Piece.IsEmpty(Squares[i]) || IsAttacked(i))
                                {
                                    failed = true;
                                    break;
                                }
                            }

                            if (!failed)
                            {
                                moves.Add(new Move(60, 58, piece, Piece.None, MoveFlags.BlackQueenCastling));
                            }
                        }

                        if (BlackKingSideCastling && (Squares[63] == (Piece.Rook | Piece.Black)))
                        {
                            failed = false;
                            for (int i = 61; i < 63; i++)
                            {
                                if (!Piece.IsEmpty(Squares[i]) || IsAttacked(i))
                                {
                                    failed = true;
                                    break;
                                }
                            }

                            if (!failed)
                            {
                                moves.Add(new Move(60, 62, piece, Piece.None, MoveFlags.BlackKingCastling));
                            }
                        }
                    }
                }
            }
        }

        /* Generate moves for pawn */
        else if (Piece.IsType(piece, Piece.Pawn))
        {
            int directionIndex = Piece.IsWhite(piece) ? 0 : 1;
            int direction = PrecomputedMoveData.DirectionsOffsets[directionIndex];
            int startingRank = (directionIndex == 0) ? 1 : 6;
            int promotionRank = 7 - startingRank;

            int targetSquare = squareIndex + direction;
            if (!pinned || (Pins[squareIndex] == directionIndex))
            {
                if (Piece.IsEmpty(Squares[targetSquare]))
                {
                    if (rank == promotionRank)
                    {
                        for (int promotion = 0; promotion < 4; promotion++)
                        {
                            moves.Add(new Move(squareIndex, targetSquare, piece, Piece.None, 1 << promotion));
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

            /* Generate pawn capture moves */
            if (file > 0 && (!pinned || (Pins[squareIndex] == (direction == 8 ? 4 : 7))))
            {
                targetSquare = squareIndex + direction - 1;
                if (Piece.IsColour(Squares[targetSquare], OpponentColour))
                {
                    if (rank == promotionRank)
                    {
                        for (int promotion = 0; promotion < 4; promotion++)
                        {
                            moves.Add(new Move(squareIndex, targetSquare, piece, Squares[targetSquare], 1 << promotion));
                        }
                    }

                    else
                    {
                        moves.Add(new Move(squareIndex, targetSquare, piece, Squares[targetSquare]));
                    }
                }

                else if (targetSquare == Enpassant)
                {
                    moves.Add(new Move(squareIndex, targetSquare, piece, Squares[targetSquare], MoveFlags.EnpassantCapture));
                }
            }

            if (file < 7 && (!pinned || (Pins[squareIndex] == (direction == 8 ? 6 : 5))))
            {
                targetSquare = squareIndex + direction + 1;
                if (Piece.IsColour(Squares[targetSquare], OpponentColour))
                {
                    if (rank == promotionRank)
                    {
                        for (int promotion = 0; promotion < 4; promotion++)
                        {
                            moves.Add(new Move(squareIndex, targetSquare, piece, Squares[targetSquare], 1 << promotion));
                        }
                    }

                    else
                    {
                        moves.Add(new Move(squareIndex, targetSquare, piece, Squares[targetSquare]));
                    }
                }

                else if (targetSquare == Enpassant)
                {
                    moves.Add(new Move(squareIndex, targetSquare, piece, Squares[targetSquare], MoveFlags.EnpassantCapture));
                }
            }
        }
        return moves;
    }

    public bool MakeMove(string uci)
    {
        return MakeMove(Coordinates.GetMoveFromUci(this, uci));
    }

    public bool MakeMove(int startingSquare, int endingSquare)
    {
        return MakeMove(startingSquare, endingSquare, MoveFlags.None);
    }

    public bool MakeMove(int startingSquare, int endingSquare, int flag)
    {
        return MakeMove(new Move(startingSquare, endingSquare, Squares[startingSquare], Squares[endingSquare], flag));
    }

    public bool MakeMove(Move move)
    {
        if (!GeneratedMoves.Contains(move))
            return false;

        Squares[move.StartingSquare] = Piece.None;
        Squares[move.EndingSquare] = move.FriendlyPiece;
        CastlingHistory.Add(Castling);
        switch (move.MoveFlag)
        {
            case MoveFlags.None:
                if (Piece.IsType(move.FriendlyPiece, Piece.King))
                {
                    Castling ^= Piece.IsWhite(ColourToMove) ? Castlings.White : Castlings.Black;
                }
                else if (Piece.IsType(move.FriendlyPiece, Piece.Rook))
                {
                    if (Piece.IsWhite(ColourToMove))
                    {
                        if (move.StartingSquare == Coordinates.A1)
                            Castling ^= Castlings.WhiteQueenSide;

                        else if (move.StartingSquare == Coordinates.H1)
                            Castling ^= Castlings.WhiteKingSide;
                    }
                    else
                    {
                        if (move.StartingSquare == Coordinates.A8)
                            Castling ^= Castlings.BlackQueenSide;

                        else if (move.StartingSquare == Coordinates.H8)
                            Castling ^= Castlings.BlackKingSide;
                    }
                }
                break;

            case MoveFlags.PromotionKnight:
                Squares[move.EndingSquare] = Piece.Knight | ColourToMove;
                break;

            case MoveFlags.PromotionBishop:
                Squares[move.EndingSquare] = Piece.Bishop | ColourToMove;
                break;

            case MoveFlags.PromotionRook:
                Squares[move.EndingSquare] = Piece.Rook | ColourToMove;
                break;

            case MoveFlags.PromotionQueen:
                Squares[move.EndingSquare] = Piece.Queen | ColourToMove;
                break;

            case MoveFlags.EnpassantCapture:
                Squares[Enpassant + (Piece.IsWhite(ColourToMove) ? -8 : 8)] = Piece.None;
                break;

            case MoveFlags.WhiteQueenCastling:
                Squares[Coordinates.A1] = Piece.None;
                Squares[Coordinates.D1] = Piece.Rook | Piece.White;
                Castling ^= Castlings.WhiteQueenSide;
                break;

            case MoveFlags.WhiteKingCastling:
                Squares[Coordinates.F1] = Piece.Rook | Piece.White;
                Squares[Coordinates.H1] = Piece.None;
                Castling ^= Castlings.WhiteKingSide;
                break;

            case MoveFlags.BlackQueenCastling:
                Squares[Coordinates.A8] = Piece.None;
                Squares[Coordinates.D8] = Piece.Rook | Piece.Black;
                Castling ^= Castlings.BlackQueenSide;
                break;

            case MoveFlags.BlackKingCastling:
                Squares[Coordinates.F8] = Piece.Rook | Piece.Black;
                Squares[Coordinates.H8] = Piece.None;
                Castling ^= Castlings.BlackKingSide;
                break;
        }

        Enpassant = move.IsDoublePawnPush ? move.StartingSquare + (Piece.IsWhite(ColourToMove) ? 8 : -8) : -1;
        ColourToMove = OpponentColour;
        Moves.Add(move);
        Calculate();
        return true;
    }

    public bool Takeback()
    {
        if (Moves.Count == 0)
            return false;

        var move = Moves[^1];
        Castling = CastlingHistory[^1];
        Moves.RemoveAt(Moves.Count - 1);
        CastlingHistory.RemoveAt(CastlingHistory.Count - 1);
        Squares[move.StartingSquare] = move.FriendlyPiece;
        Squares[move.EndingSquare] = move.OpponentPiece;
        switch (move.MoveFlag)
        {
            case MoveFlags.PromotionKnight:
                Squares[move.StartingSquare] = Piece.Pawn | OpponentColour;
                break;

            case MoveFlags.PromotionBishop:
                Squares[move.StartingSquare] = Piece.Pawn | OpponentColour;
                break;

            case MoveFlags.PromotionRook:
                Squares[move.StartingSquare] = Piece.Pawn | OpponentColour;
                break;

            case MoveFlags.PromotionQueen:
                Squares[move.StartingSquare] = Piece.Pawn | OpponentColour;
                break;

            case MoveFlags.EnpassantCapture:
                Squares[move.EndingSquare + (Piece.IsWhite(OpponentColour) ? -8 : 8)] = Piece.Pawn | FriendlyColour;
                break;

            case MoveFlags.WhiteQueenCastling:
                Squares[0] = Piece.Rook | Piece.White;
                Squares[3] = Piece.None;
                break;

            case MoveFlags.WhiteKingCastling:
                Squares[5] = Piece.None;
                Squares[7] = Piece.Rook | Piece.White;
                break;

            case MoveFlags.BlackQueenCastling:
                Squares[56] = Piece.Rook | Piece.Black;
                Squares[59] = Piece.None;
                break;

            case MoveFlags.BlackKingCastling:
                Squares[61] = Piece.None;
                Squares[63] = Piece.Rook | Piece.Black;
                break;
        }

        if (Moves.Count > 0)
            Enpassant = Moves[^1].IsDoublePawnPush ? Moves[^1].StartingSquare + (Piece.IsWhite(ColourToMove) ? 8 : -8) : -1;
        ColourToMove = OpponentColour;
        Calculate();
        return true;
    }

    private void Calculate()
    {
        /* Finding king squares */
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
            if (Squares[squareIndex] == (Piece.King | Piece.White))
                Kings[0] = squareIndex;

            else if (Squares[squareIndex] == (Piece.King | Piece.Black))
                Kings[1] = squareIndex;

        CalculateAttacks();
        CalculatePins();
        ValidateEnpassantSquare();
        GenerateLegalMoves();
        CalculateGameState();
    }

    public bool IsAttacked(int squareIndex)
    {
        return Bitboard.Contains(AttackBitboard, squareIndex);
    }

    public bool IsPinned(int squareIndex)
    {
        return Pins.ContainsKey(squareIndex);
    }

    private void CalculateAttacks()
    {
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
                if (Bitboard.Contains(bitboard, KingSquare))
                {
                    AttackingSquare = squareIndex;
                    IsDoubleCheck = IsCheck;
                    IsCheck = true;
                    IsKnightCheck = Piece.IsType(Squares[squareIndex], Piece.Knight);
                }
                AttackBitboard |= bitboard;
            }
        }
    }

    private void CalculatePins()
    {
        Pins.Clear();
        if (KingSquare > -1)
        {
            for (int directionIndex = 0; directionIndex < 8; directionIndex++)
            {
                int friendlyPiece = -1;
                for (int i = 0; i < PrecomputedMoveData.Rays[KingSquare][directionIndex]; i++)
                {
                    int targetSquare = KingSquare + PrecomputedMoveData.DirectionsOffsets[directionIndex] * (i + 1);

                    if (Piece.IsEmpty(Squares[targetSquare]))
                        continue;

                    if (friendlyPiece > 0)
                    {
                        if (Piece.IsColour(Squares[targetSquare], ColourToMove))
                            break;

                        if (!((directionIndex < 4) ? Piece.IsRookOrQueen(Squares[targetSquare]) : Piece.IsBishopOrQueen(Squares[targetSquare])))
                            break;

                        Pins.Add(friendlyPiece, directionIndex);
                        break;
                    }

                    else
                    {
                        if (!IsAttacked(targetSquare))
                            break;

                        friendlyPiece = targetSquare;
                    }
                }
            }
        }
    }

    private void ValidateEnpassantSquare()
    {
        if (Enpassant == -1)
            return;

        int opponentPawnSquare = Enpassant + (Piece.IsWhite(OpponentColour) ? 8 : -8);
        int file = opponentPawnSquare & 7;
        int kingSquareFile = KingSquare & 7;

        bool enPassant = false;

        int pawnSquare = opponentPawnSquare - 1;
        if (file > 0 && Squares[pawnSquare] == (Piece.Pawn | ColourToMove))
            if (kingSquareFile != file)
                enPassant = true;

            else if (!IsKingAttackedAfterEnpassant(pawnSquare, opponentPawnSquare))
                enPassant = true;

        pawnSquare += 2;
        if (file < 7 && Squares[pawnSquare] == (Piece.Pawn | ColourToMove))
            if (kingSquareFile != file)
                enPassant = true;

            else if (!IsKingAttackedAfterEnpassant(pawnSquare, opponentPawnSquare))
                enPassant = true;

        Enpassant = enPassant ? Enpassant : -1;
    }

    private bool IsKingAttackedAfterEnpassant(int pawnSquare, int opponentPawnSquare)
    {
        bool kingAttacked = false;
        Squares[opponentPawnSquare] = Piece.None;
        Squares[Enpassant] = Squares[pawnSquare];
        Squares[pawnSquare] = Piece.None;

        if (Bitboard.Contains(Bitboard.GetAttackBitboard(this), KingSquare))
            kingAttacked = true;

        Squares[opponentPawnSquare] = Piece.Pawn | OpponentColour;
        Squares[pawnSquare] = Squares[Enpassant];
        Squares[Enpassant] = Piece.None;

        return kingAttacked;
    }

    private void CalculateGameState()
    {
        if (LegalMoves.Length == 0)
        {
            if (IsCheck)
                Flag = EndFlags.Checkmate;

            else
                Flag = EndFlags.Stalemate;
        }
    }
}
