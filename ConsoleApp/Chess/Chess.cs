using System;
using System.Collections.Generic;
using System.Text;

namespace Chess;

#region Objects

/// <summary>
/// Board object
/// </summary>
public class Board
{
    /// <summary>
    /// Stores pieces
    /// </summary>
    private readonly int[] Squares;
    private int Flag;

    /// <summary>
    /// Stores moves that played
    /// </summary>
    internal List<Move> Moves;

    #region Board Indexers

    /// <summary>
    /// Returns piece from given square <paramref name="index"/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns><see cref="int"/></returns>
    public int this[int index]
    {
        get
        {
            if (index < 0 || index > 63)
                return 0;

            return Squares[index];
        }
    }

    /// <summary>
    /// Returns piece from given <paramref name="file"/> and <paramref name="rank"/>
    /// </summary>
    /// <param name="file"></param>
    /// <param name="rank"></param>
    /// <returns><see cref="int"/></returns>
    public int this[int file, int rank]
    {
        get
        {
            if (file < 1 || file > 8)
                return 0;

            if (rank < 1 || rank > 8)
                return 0;

            return Squares[((rank - 1) * 8) + (file - 1)];
        }
    }

    #endregion

    #region Board Properties

    /// <summary>
    /// Returns total move count
    /// </summary>
    public int MoveCount
    {
        get
        {
            lock (Moves)
            {
                return Moves.Count;
            }
        }
    }

    /// <summary>
    /// Returns move turn
    /// </summary>
    public int MoveTurn
    {
        get
        {
            lock (Moves)
            {
                return (MoveCount + 2) / 2;
            }
        }
    }

    /// <summary>
    /// Half move counter
    /// </summary>
    public int HalfMoveCount
    {
        get
        {
            lock (Moves)
            {
                for (int i = 0; i < Moves.Count; i++)
                {
                    var move = Moves[^(i + 1)];
                    if (!Piece.IsEmpty(move.TargetPiece) || Piece.IsType(move.Piece, Piece.Pawn))
                    {
                        return i;
                    }
                }

                return 0;
            }
        }
    }

    /// <summary>
    /// Returns the colour to move
    /// </summary>
    public int ColourToMove
    {
        get
        {
            return (((MoveCount % 2) == 0) ? Piece.White : Piece.Black);
        }
    }

    /// <summary>
    /// Returns the opponent colour
    /// </summary>
    public int OpponentColour
    {
        get
        {
            return Piece.OppositColour(ColourToMove);
        }
    }

    /// <summary>
    /// Returns the king location of current player
    /// </summary>
    public int KingSquare
    {
        get
        {
            return Piece.IsColour(ColourToMove, Piece.White) ? WhiteKing : BlackKing;
        }
    }

    /// <summary>
    /// Returns the white kings location
    /// </summary>
    public int WhiteKing
    {
        get
        {
            lock (Squares)
            {
                for (int i = 0; i < 64; i++)
                    if (Squares[i] == (Piece.King | Piece.White))
                        return i;

                return -1;
            }
        }
    }

    /// <summary>
    /// Returns the black kings location
    /// </summary>
    public int BlackKing
    {
        get
        {
            lock (Squares)
            {
                for (int i = 0; i < 64; i++)
                    if (Squares[i] == (Piece.King | Piece.Black))
                        return i;

                return -1;
            }
        }
    }

    /// <summary>
    /// Returns the total count of occupied
    /// </summary>
    public int PieceCount
    {
        get
        {
            int pieceCount = 0;
            for (int i = 0; i < 64; i++)
                if (!Piece.IsEmpty(Squares[i]))
                    pieceCount++;

            return pieceCount;
        }
    }

    /// <summary>
    /// Returns the enpassant square if last move has double pawn push
    /// </summary>
    public int EnpassantSquare
    {
        get
        {
            lock (Moves)
            {
                if (MoveCount > 0)
                    if (Moves[^1].HasFlag(MoveFlags.DoublePawnPush))
                        return Moves[^1].TargetIndex + (Piece.IsColour(Moves[^1].Piece, Piece.White) ? -8 : 8);

                return -1;
            }
        }
    }

    /// <summary>
    /// Match state flag
    /// </summary>
    public int EndFlag
    {
        get
        {
            return Flag;
        }
    }

    /// <summary>
    /// Winner of the match
    /// </summary>
    public int Winner
    {
        get
        {
            if (Flag == EndFlags.Checkmate)
                return OpponentColour;

            return 0;
        }
    }

    /// <summary>
    /// Using for testing on console
    /// </summary>
    public string AsString
    {
        get
        {
            lock (Squares)
            {
                string boardString = string.Empty;
                for (int rank = 0; rank < 8; rank++)
                {
                    for (int file = 0; file < 8; file++)
                    {
                        int index = (7 - rank) * 8 + file;
                        boardString += Piece.PieceSymbol(Squares[index]) + " ";
                    }
                    boardString += "\b\n";
                }
                return boardString[..^1];
            }
        }
    }

    #endregion

    #region Board Constructers

    /// <summary>
    /// Generates an empty board
    /// </summary>
    public Board()
    {
        Squares = new int[64];
        Moves = new List<Move>();
        Flag = EndFlags.None;
    }

    /// <summary>
    /// Generates board with starting chess position
    /// </summary>
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

        return board;
    }

    #endregion

    #region Move Generate

    /// <summary>
    /// Calculates legal moves
    /// </summary>
    /// <returns><see cref="List{Move}"/></returns>
    private List<Move> CalculateLegalMoves()
    {
        var moves = new List<Move>();
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
        {
            if (!Piece.IsEmpty(Squares[squareIndex]))
            {
                moves.AddRange(CalculateLegalMoves(squareIndex));
            }
        }
        
        if (IsUnderAttack(KingSquare, ColourToMove, out List<int> attackers))
        {
            if (attackers.Count > 1)
            {
                moves.RemoveAll(move => !(move.Index == KingSquare));
            }

            else
            {
                for (int i = 0; i < attackers.Count; i++)
                {
                    if (Piece.IsSlidingPiece(Squares[attackers[i]]))
                    {
                        var squares = SquaresInLine(KingSquare, attackers[i]);
                        moves.RemoveAll(move => !((move.TargetIndex == attackers[i]) || squares.Contains(move.TargetIndex)));
                    }

                    else
                    {
                        moves.RemoveAll(move => !(move.TargetIndex == attackers[i]));
                    }
                }
            }
        }

        return moves;
    }

    /// <summary>
    /// Calculates legal moves for <paramref name="squareIndex"/>
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <returns><see cref="List{Move}"/></returns>
    private List<Move> CalculateLegalMoves(int squareIndex)
    {
        if (Piece.IsColour(Squares[squareIndex], ColourToMove))
        {
            if (Piece.IsSlidingPiece(Squares[squareIndex]))
            {
                return GenerateSlidingMoves(squareIndex);
            }

            else if (Piece.IsType(Squares[squareIndex], Piece.Knight))
            {
                return GenerateKnightMoves(squareIndex);
            }

            else if (Piece.IsType(Squares[squareIndex], Piece.King))
            {
                return GenerateKingMoves(squareIndex);
            }

            else if (Piece.IsType(Squares[squareIndex], Piece.Pawn))
            {
                return GeneratePawnMoves(squareIndex);
            }
        }

        return new List<Move>();
    }

    /// <summary>
    /// Generates queen, rook and bishop moves
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <returns><see cref="List{Move}"/></returns>
    private List<Move> GenerateSlidingMoves(int squareIndex)
    {
        var moves = new List<Move>();
        int startDirectionIndex = Piece.IsType(Squares[squareIndex], Piece.Bishop) ? 4 : 0;
        int endDirectionIndex = Piece.IsType(Squares[squareIndex], Piece.Rook) ? 4 : 8;
        bool pinned = IsPinned(squareIndex, out int pinDirection);
        for (int directionIndex = startDirectionIndex; directionIndex < endDirectionIndex; directionIndex++)
        {
            int direction = PrecomputedMoveData.DirectionOffsets[directionIndex];

            /* Can't move any other direction, because it's pinned by an opponent piece */
            if (pinned && (direction != pinDirection && direction != -pinDirection))
                continue;

            for (int n = 0; n < PrecomputedMoveData.NumSquaresToEdge[squareIndex][directionIndex]; n++)
            {
                int targetSquare = squareIndex + PrecomputedMoveData.DirectionOffsets[directionIndex] * (n + 1);

                /* Blocked by friendly piece */
                if (Piece.IsColour(Squares[targetSquare], ColourToMove))
                    break;

                moves.Add(new Move(squareIndex, targetSquare));

                /* Capturing opponent piece */
                if (Piece.IsColour(Squares[targetSquare], OpponentColour))
                    break;
            }
        }
        return moves;
    }

    /// <summary>
    /// Generates knight moves
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <returns><see cref="List{Move}"/></returns>
    private List<Move> GenerateKnightMoves(int squareIndex)
    {
        var moves = new List<Move>();

        /* Can't move any square if knight is pinned */
        if (IsPinned(squareIndex, out _))
            return moves;

        for (int knightSquareIndex = 0; knightSquareIndex < 8; knightSquareIndex++)
        {
            int targetSquare = PrecomputedMoveData.KnightSquares[squareIndex][knightSquareIndex];

            /* targetSquare is out of bounds */
            if (targetSquare == -1)
                continue;

            /* Square blocked by friendly piece */
            if (Piece.IsColour(Squares[targetSquare], ColourToMove))
                continue;

            moves.Add(new Move(squareIndex, targetSquare));
        }
        return moves;
    }

    /// <summary>
    /// Generates king moves
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <returns><see cref="List{Move}"/></returns>
    private List<Move> GenerateKingMoves(int squareIndex)
    {
        var moves = new List<Move>();
        for (int directionIndex = 0; directionIndex < 8; directionIndex++)
        {
            if (PrecomputedMoveData.NumSquaresToEdge[squareIndex][directionIndex] > 0)
            {
                int targetSquare = squareIndex + PrecomputedMoveData.DirectionOffsets[directionIndex];

                /* Square blocked by friendly piece */
                if (Piece.IsColour(Squares[targetSquare], ColourToMove))
                    continue;

                /* The target square is under attack */
                if (IsUnderAttack(targetSquare, ColourToMove, out _))
                    continue;

                moves.Add(new Move(squareIndex, targetSquare));
            }
        }

        /* Check for castlings */
        int kingSquare = (Piece.IsColour(ColourToMove, Piece.White) ? 4 : 60);
        if (!IsUnderAttack(squareIndex, ColourToMove, out _))
        {
            /* Checking for the king not moved */
            if (Moves.FindAll(x => x.Piece == Squares[squareIndex]).Count == 0 || squareIndex == kingSquare)
            {
                var rookMoves = Moves.FindAll(x => x.Piece == (ColourToMove | Piece.Rook));

                /* Checking for the queen rook not moved */
                if (rookMoves.FindAll(x => x.Index == (kingSquare - 4)).Count == 0 && (Squares[kingSquare - 4] == (Piece.Rook | ColourToMove)))
                {
                    /* Checking for there is no attack between the king and the rook. */
                    bool castlingFailed = false;
                    for (int i = -3; i < 0; i++)
                    {
                        castlingFailed &= IsUnderAttack(squareIndex + i, ColourToMove, out _);
                    }
                    if (!castlingFailed)
                        moves.Add(new Move(squareIndex, squareIndex - 2, MoveFlags.QueenCastling));
                }

                /* Checking for the king rook not moved */
                if (rookMoves.FindAll(x => x.Index == (kingSquare + 3)).Count == 0 && (Squares[kingSquare + 3] == (Piece.Rook | ColourToMove)))
                {
                    /* Checking for there is no attack between the king and the rook. */
                    bool castlingFailed = false;
                    for (int i = 1; i < 2; i++)
                    {
                        castlingFailed &= IsUnderAttack(squareIndex + i, ColourToMove, out _);
                    }
                    if (!castlingFailed)
                        moves.Add(new Move(squareIndex, squareIndex + 2, MoveFlags.KingCastling));
                }
            }
        }

        return moves;
    }

    /// <summary>
    /// Generates pawn move
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <returns><see cref="List{Move}"/></returns>
    private List<Move> GeneratePawnMoves(int squareIndex)
    {
        var moves = new List<Move>();
        int rank = squareIndex / 8;
        int file = squareIndex % 8;
        int startingRank = Piece.IsColour(Squares[squareIndex], Piece.White) ? 1 : 6;
        int promotionRank = 7 - startingRank;
        int direction = Piece.IsColour(Squares[squareIndex], Piece.White) ? 8 : -8;

        bool pinned = IsPinned(squareIndex, out int pinDirection);

        /* Move forward */
        int targetSquare = squareIndex + direction;

        if (!pinned || (pinDirection == direction || pinDirection == -direction))
        {
            if (Piece.IsEmpty(Squares[targetSquare]))
            {
                if (rank == promotionRank)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        moves.Add(new Move(squareIndex, targetSquare, MoveFlags.PromotionKnight << i));
                    }
                }

                else
                {
                    moves.Add(new Move(squareIndex, targetSquare));
                }

                /* Can play two squares on starting rank */
                if (rank == startingRank)
                {
                    targetSquare += direction;
                    if (Piece.IsEmpty(Squares[targetSquare]))
                    {
                        moves.Add(new Move(squareIndex, targetSquare, MoveFlags.DoublePawnPush));
                    }
                }
            }
        }

        /* Move with capture */
        if (!pinned || (pinDirection == (direction - 1)))
        {
            if (file > 0)
            {
                targetSquare = squareIndex + direction - 1;
                if (Piece.IsColour(Squares[targetSquare], OpponentColour))
                {
                    if (rank == promotionRank)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            moves.Add(new Move(squareIndex, targetSquare, MoveFlags.PromotionKnight << i));
                        }
                    }

                    else
                    {
                        moves.Add(new Move(squareIndex, targetSquare));
                    }
                }

                /* Check for en passant */
                else if (EnpassantSquare == targetSquare)
                {
                    moves.Add(new Move(squareIndex, targetSquare, MoveFlags.EnpassantCapture));
                }
            }
        }

        if (!pinned || (pinDirection == (direction + 1)))
        {
            if (file < 8)
            {
                targetSquare = squareIndex + direction + 1;
                if (Piece.IsColour(Squares[targetSquare], OpponentColour))
                {
                    if (rank == promotionRank)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            moves.Add(new Move(squareIndex, targetSquare, MoveFlags.PromotionKnight << i));
                        }
                    }

                    else
                    {
                        moves.Add(new Move(squareIndex, targetSquare));
                    }
                }

                /* Check for en passant */
                else if (EnpassantSquare == targetSquare)
                {
                    moves.Add(new Move(squareIndex, targetSquare, MoveFlags.EnpassantCapture));
                }
            }
        }

        return moves;
    }

    #endregion

    #region Utility

    /// <summary>
    /// Returns the <paramref name="squareIndex"/> is under attack
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <returns><see cref="bool"/></returns>
    public bool IsUnderAttack(int squareIndex, int friendlyColour, out List<int> attackers)
    {
        attackers = new List<int>();

        /* Check for sliding pieces and opponent king */
        for (int directionIndex = 0; directionIndex < 8; directionIndex++)
        {
            for (int n = 0; n < PrecomputedMoveData.NumSquaresToEdge[squareIndex][directionIndex]; n++)
            {
                int targetSquare = squareIndex + PrecomputedMoveData.DirectionOffsets[directionIndex] * (n + 1);

                /* No attack from that square, because it's empty */
                if (Piece.IsEmpty(Squares[targetSquare]))
                    continue;

                /* Opponent can't attack from that direction, because there is a friendly piece blocks the way */
                if (Piece.IsColour(Squares[targetSquare], friendlyColour))
                    break;

                /* Kings can't be touched each other */
                if (n == 0)
                {
                    if (Piece.IsType(Squares[targetSquare], Piece.King))
                        if (!Piece.IsColour(Squares[targetSquare], friendlyColour))
                            attackers.Add(targetSquare);
                }

                /* Queen can attack directly */
                if (Piece.IsType(Squares[targetSquare], Piece.Queen))
                    attackers.Add(targetSquare);

                /* But we must check for the rook and bishop */
                if (Piece.IsType(Squares[targetSquare], Piece.Rook))
                    if (directionIndex < 4)
                        attackers.Add(targetSquare);

                if (Piece.IsType(Squares[targetSquare], Piece.Bishop))
                    if (directionIndex >= 4)
                        attackers.Add(targetSquare);
            }
        }

        /* Check for attacking knights */
        for (int knightSquare = 0; knightSquare < 8; knightSquare++)
        {
            int targetSquare = PrecomputedMoveData.KnightSquares[squareIndex][knightSquare];

            if (targetSquare != -1)
                if (Piece.IsType(Squares[targetSquare], Piece.Knight))
                    if (!Piece.IsColour(Squares[targetSquare], friendlyColour))
                        attackers.Add(targetSquare);
        }

        /* Check for attacking pawns */
        int forward = (Piece.IsColour(friendlyColour, Piece.White) ? 8 : -8);

        if ((squareIndex % 8) > 0)
        {
            int targetSquare = squareIndex + forward - 1;

            if (-1 < targetSquare && targetSquare < 64)
                if (Piece.IsType(Squares[targetSquare], Piece.Pawn))
                    if (!Piece.IsColour(Squares[targetSquare], friendlyColour))
                        attackers.Add(targetSquare);
        }

        if ((squareIndex % 8) < 7)
        {
            int targetSquare = squareIndex + forward + 1;

            if (-1 < targetSquare && targetSquare < 64)
                if (Piece.IsType(Squares[targetSquare], Piece.Pawn))
                    if (!Piece.IsColour(Squares[targetSquare], friendlyColour))
                        attackers.Add(targetSquare);
        }

        return (attackers.Count > 0);
    }

    /// <summary>
    /// Returns the <paramref name="squareIndex"/> is pinned by an opponent queen, rook or bishop
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="direction"></param>
    /// <returns><see cref="bool"/></returns>
    public bool IsPinned(int squareIndex, out int direction)
    {
        /* Can't pin if square is empty */
        direction = 0;
        if (Piece.IsEmpty(Squares[squareIndex]))
            return false;

        int friendlyColour = Piece.Colour(Squares[squareIndex]);
        for (int directionIndex = 0; directionIndex < 8; directionIndex++)
        {
            direction = PrecomputedMoveData.DirectionOffsets[directionIndex];
            for (int n = 0; n < PrecomputedMoveData.NumSquaresToEdge[squareIndex][directionIndex]; n++)
            {
                int targetSquare = squareIndex + direction * (n + 1);

                /* Continue until a piece is found on target square */
                if (!Piece.IsEmpty(Squares[targetSquare]))
                {
                    /* Searching for the friendly king */
                    if (!Piece.IsType(Squares[targetSquare], Piece.King))
                        break;

                    if (!Piece.IsColour(Squares[targetSquare], friendlyColour))
                        break;

                    for (int k = 0; k < PrecomputedMoveData.NumSquaresToEdge[squareIndex][directionIndex + ((directionIndex % 2) == 0 ? 1 : -1)]; k++)
                    {
                        targetSquare = squareIndex - direction * (k + 1);

                        /* Continue until a piece is found on target square */
                        if (!Piece.IsEmpty(Squares[targetSquare]))
                        {
                            if (!Piece.IsColour(Squares[targetSquare], friendlyColour))
                            {
                                /* Queen can pin directly but we must check for the rook and bishop */

                                if (Piece.IsType(Squares[targetSquare], Piece.Queen))
                                    return true;

                                if (Piece.IsType(Squares[targetSquare], Piece.Rook))
                                    if (directionIndex < 4)
                                        return true;

                                if (Piece.IsType(Squares[targetSquare], Piece.Bishop))
                                    if (directionIndex >= 4)
                                        return true;
                            }
                        }
                    }

                    direction = 0;
                    return false;
                }
            }
        }

        direction = 0;
        return false;
    }

    public List<int> SquaresInLine(int firstSquare, int secondSquare)
    {
        var squares = new List<int>();

        for (int directionIndex = 0; directionIndex < 8; directionIndex++)
        {
            for (int n = 0; n < PrecomputedMoveData.NumSquaresToEdge[firstSquare][directionIndex]; n++)
            {
                int targetSquare = firstSquare + PrecomputedMoveData.DirectionOffsets[directionIndex] + (n + 1);

                if (targetSquare == secondSquare)
                    return squares;

                squares.Add(targetSquare);
            }

            squares.Clear();
        }

        return squares;
    }

    /// <summary>
    /// Checks the game has ended or not
    /// </summary>
    public void CheckForEnd()
    {
        var legalMoves = CalculateLegalMoves();
        legalMoves.RemoveAll(move => move.HasCastlingFlag());
        if (legalMoves.Count == 0)
        {
            if (IsUnderAttack(KingSquare, ColourToMove, out _))
            {
                Flag = EndFlags.Checkmate;
            }

            else
            {
                Flag = EndFlags.Stalemate;
            }
        }

        else if (HalfMoveCount == 50)
        {
            Flag = EndFlags.FiftyMoveRule;
        }

        else
        {
            if (MoveTurn > 3)
            {
                bool repetition = true;
                for (int i = 0; i < 2; i++)
                {
                    var firstMove = Moves[^(2 * i + 1)];
                    var secondMove = Moves[^(2 * i + 3)];
                    repetition &= (firstMove.Index == secondMove.TargetIndex) && (firstMove.TargetIndex == secondMove.Index);

                    firstMove = Moves[^(2 * i + 2)];
                    secondMove = Moves[^(2 * i + 4)];
                    repetition &= (firstMove.Index == secondMove.TargetIndex) && (firstMove.TargetIndex == secondMove.Index);
                }
                if (repetition)
                {
                    Flag = EndFlags.ThreefoldRepetition;
                }
            }

            /* TODO Insufficient Material
            if (Flag == EndFlags.None)
            {
                int whiteKnights = 0;
                int blackKnights = 0;
                int whiteBishops = 0;
                int blackBishops = 0;
                for (int i = 0; i < 64; i++)
                {
                    var piece = Squares[i];
                    if (Piece.IsType(piece, Piece.Pawn) || Piece.IsType(piece, Piece.Queen) || Piece.IsType(piece, Piece.Rook))
                        return;

                    if (Piece.IsType(piece, Piece.Knight))
                    {
                        if (Piece.IsColour(piece, Piece.White))
                            whiteKnights++;

                        else
                            blackKnights++;
                    }

                    else if (Piece.IsType(piece, Piece.Bishop))
                    {
                        if (Piece.IsColour(piece, Piece.White))
                            whiteBishops++;

                        else
                            blackBishops++;
                    }
                }

                if ((whiteKnights < 3) || (blackKnights < 3))
                     ?
            } */
        }
    }

    /// <summary>
    /// Put piece into board as illegal
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="piece"></param>
    public void PutPiece(int squareIndex, int piece)
    {
        if (squareIndex < 0 || squareIndex > 63)
            return;

        Squares[squareIndex] = piece;
    }

    #endregion

    #region Execute Move

    /// <summary>
    /// Makes move with given <paramref name="uci"/> string
    /// </summary>
    /// <param name="uci"></param>
    /// <returns><see cref="bool"/></returns>
    /// <exception cref="InvalidUciStringException"></exception>
    public bool PushUci(string uci)
    {
        if (uci.Length != 4)
            throw new InvalidUciStringException(uci);

        var move = CalculateLegalMoves().Find(move => move.Uci == uci.ToLower());
        return MakeMove(move.Index, move.TargetIndex, move.Flag);
    }

    /// <summary>
    /// Makes move without flag
    /// </summary>
    /// <param name="currentIndex"></param>
    /// <param name="targetIndex"></param>
    /// <returns><see cref="bool"/></returns>
    public bool MakeMove(int currentIndex, int targetIndex)
    {
        return MakeMove(currentIndex, targetIndex, MoveFlags.None);
    }

    /// <summary>
    /// Makes move with flag
    /// </summary>
    /// <param name="currentIndex"></param>
    /// <param name="targetIndex"></param>
    /// <param name="flag"></param>
    /// <returns><see cref="bool"/></returns>
    public bool MakeMove(int currentIndex, int targetIndex, int flag)
    {
        if (currentIndex < 0 || currentIndex > 63)
            return false;

        if (targetIndex < 0 || targetIndex > 63)
            return false;

        var legalMoves = CalculateLegalMoves();

        foreach (var legalMove in legalMoves)
        {
            if (legalMove.Index == currentIndex && legalMove.TargetIndex == targetIndex && legalMove.Flag == flag)
            {
                legalMove.WithPieces(Squares[currentIndex], Squares[targetIndex]);
                Squares[currentIndex] = Piece.None;

                if (legalMove.HasPromotionFlag())
                {
                    Squares[targetIndex] = Piece.Promote(legalMove.Piece, legalMove.GetPromotionType());
                }

                else if (legalMove.HasFlag(MoveFlags.EnpassantCapture))
                {
                    legalMove.WithPieces(legalMove.Piece, Squares[targetIndex + (Piece.IsColour(ColourToMove, Piece.White) ? -8 : 8)]);
                    Squares[targetIndex + (Piece.IsColour(ColourToMove, Piece.White) ? -8 : 8)] = Piece.None;
                    Squares[targetIndex] = legalMove.Piece;
                }

                else if (legalMove.HasCastlingFlag())
                {
                    int rookSquare = legalMove.Index + (legalMove.HasFlag(MoveFlags.QueenCastling) ? -4 : 3);
                    Squares[legalMove.TargetIndex] = legalMove.Piece;
                    Squares[(legalMove.Index + legalMove.TargetIndex) / 2] = Squares[rookSquare];
                    Squares[rookSquare] = Piece.None;
                }

                else
                {
                    Squares[targetIndex] = legalMove.Piece;
                }

                Moves.Add(legalMove);
                CheckForEnd();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Takebacks the last move
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public bool Takeback()
    {
        if (Moves.Count == 0)
            return false;

        var move = Moves[^1];
        if (move.GetType() != typeof(Move) || move.HasSystemFlag())
            return false;

        if (move.HasPromotionFlag())
        {
            Squares[move.Index] = Piece.Pawn | Piece.Colour(move.Piece);
            Squares[move.TargetIndex] = move.TargetPiece;
        }

        else if (move.HasFlag(MoveFlags.EnpassantCapture))
        {
            Squares[move.Index] = move.Piece;
            Squares[move.TargetIndex] = Piece.None;
            Squares[move.TargetIndex + ((move.TargetIndex - move.Index) > 0 ? -8 : 8)] = move.TargetPiece;
        }

        else if (move.HasCastlingFlag())
        {
            Squares[move.Index] = move.Piece;
            Squares[move.TargetIndex] = Piece.None;
            Squares[(move.Index + move.TargetIndex) / 2] = Piece.None;
            Squares[(Piece.IsColour(move.Piece, Piece.White) ? 0 : 56) + (move.HasFlag(MoveFlags.QueenCastling) ? 0 : 7)] = Piece.Rook | Piece.Colour(move.Piece);
        }

        else
        {
            Squares[move.Index] = move.Piece;
            Squares[move.TargetIndex] = move.TargetPiece;
        }

        Moves.RemoveAt(Moves.Count - 1);
        return true;
    }

    #endregion
}

/// <summary>
/// Piece object
/// </summary>
public static class Piece
{
    public const int None = 0;      /* 000 */
    public const int Pawn = 1;      /* 001 */
    public const int King = 2;      /* 010 */
    public const int Knight = 3;    /* 011 */
    public const int Bishop = 4;    /* 100 */
    public const int Rook = 5;      /* 101 */
    public const int Queen = 6;     /* 110 */

    public const int White = 8;     /* 01... */
    public const int Black = 16;    /* 10... */

    public const string Symbols = ".PKNBRQ";

    /// <summary>
    /// Returns <paramref name="piece"/> colour
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="int"/></returns>
    public static int Colour(int piece)
    {
        return (piece & 0b11000);
    }

    /// <summary>
    /// Returns opposit of <paramref name="piece"/> colour
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="int"/></returns>
    public static int OppositColour(int piece)
    {
        return ((piece & 0b11000) == White) ? Black : White;
    }

    /// <summary>
    /// Returns <paramref name="piece"/> type
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="int"/></returns>
    public static int Type(int piece)
    {
        return (piece & 0b111);
    }

    /// <summary>
    /// Returns <paramref name="piece"/> has <paramref name="colour"/>
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="colour"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsColour(int piece, int colour)
    {
        return (piece & 0b11000) == colour;
    }

    /// <summary>
    /// Returns <paramref name="piece"/> is <paramref name="type"/>
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="type"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsType(int piece, int type)
    {
        return (piece & 0b111) == type;
    }

    /// <summary>
    /// Returns <paramref name="piece"/> is an empty square
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsEmpty(int piece)
    {
        return (piece & 0b111) == None;
    }

    /// <summary>
    /// Returns <paramref name="piece"/> is queen, rook or bishop
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsSlidingPiece(int piece)
    {
        return (piece & 0b100) == 0b100;
    }

    /// <summary>
    /// Returns <paramref name="piece"/> is a promotion type piece
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsPromotionType(int piece)
    {
        return IsSlidingPiece(piece) || IsType(piece, Knight);
    }

    /// <summary>
    /// Promotes <paramref name="piece"/> to <paramref name="type"/>
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="type"></param>
    /// <returns><see cref="int"/></returns>
    public static int Promote(int piece, int type)
    {
        return (piece & 0b11000) | type;
    }

    /// <summary>
    /// Returns symbol from <paramref name="piece"/>
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="string"/></returns>
    public static string PieceSymbol(int piece)
    {
        string symbol = $"{Symbols[(piece & 0b111)]}";
        return IsColour(piece, Black) ? symbol.ToLower() : symbol;
    }

    /// <summary>
    /// Returns piece from <paramref name="symbol"/>
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns><see cref="int"/></returns>
    public static int FromSymbol(char symbol)
    {
        int piece = Symbols.IndexOf(char.ToUpper(symbol));
        piece = piece == -1 ? 0 : piece;
        return piece | (char.IsUpper(symbol) ? White : Black);
    }
}

/// <summary>
/// Chess move object
/// </summary>
public struct Move
{
    private int Current;
    private int Target;
    private readonly int MoveFlag;
    public bool Check;
    public bool Mate;

    private const int indexMask = 0b111111;
    private const int pieceMask = 0b11111;

    /// <summary>
    /// Move flags
    /// </summary>

    #region Move Properties

    /// <summary>
    /// Move's current index
    /// </summary>
    public int Index
    {
        get
        {
            return ((Current >> 5) & indexMask);
        }
    }

    /// <summary>
    /// Move's target index
    /// </summary>
    public int TargetIndex
    {
        get
        {
            return ((Target >> 5) & indexMask);
        }
    }

    /// <summary>
    /// Move's piece
    /// </summary>
    public int Piece
    {
        get
        {
            return (Current & pieceMask);
        }
    }

    /// <summary>
    /// Move's target piece
    /// </summary>
    public int TargetPiece
    {
        get
        {
            return (Target & pieceMask);
        }
    }

    /// <summary>
    /// Move's flag
    /// </summary>
    public int Flag
    {
        get
        {
            return MoveFlag;
        }
    }

    /// <summary>
    /// Returns move as UCI
    /// </summary>
    public string Uci
    {
        get
        {
            return (ConvertUci(Index) + ConvertUci(TargetIndex));
        }
    }

    /// <summary>
    /// Returns move as SAN
    /// </summary>
    public string San
    {
        get
        {
            string pieceSymbol = Chess.Piece.IsType(Piece, Chess.Piece.Pawn) ? string.Empty : Chess.Piece.PieceSymbol(Piece);
            string promotion = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                if (HasFlag(1 << i))
                {
                    promotion = $"={Chess.Piece.Symbols[i + 3]}";
                    break;
                }
            }
            return (pieceSymbol) + (Chess.Piece.IsEmpty(TargetPiece) ? string.Empty : "x") + ConvertUci(TargetIndex) + (Check ? "+" : string.Empty) + (promotion);
        }
    }

    #endregion

    #region Move Methods

    /// <summary>
    /// Generates move with no flag
    /// </summary>
    /// <param name="current">Current square index</param>
    /// <param name="target">Target square index</param>
    public Move(int current, int target) : this(current, target, MoveFlags.None)
    {

    }

    /// <summary>
    /// Generates move with flag
    /// </summary>
    /// <param name="current">Current square index</param>
    /// <param name="target">Target square index</param>
    /// <param name="flag">Move flag</param>
    public Move(int current, int target, int flag)
    {
        Current = (current << 5);
        Target = (target << 5);
        MoveFlag = flag;
        Check = false;
        Mate = false;
    }

    /// <summary>
    /// Assigns pieces to move
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="targetPiece"></param>
    public void WithPieces(int piece, int targetPiece)
    {
        Current = (Current & indexMask) | piece;
        Target = (Target & indexMask) | targetPiece;
    }

    /// <summary>
    /// Assigns check to true for using in SAN
    /// </summary>
    public void ChecksKing()
    {
        Check = true;
    }

    /// <summary>
    /// Assigns mate to true for using in SAN
    /// </summary>
    public void Mates()
    {
        Mate = true;
    }

    /// <summary>
    /// Returns move has given <paramref name="flag"/>
    /// </summary>
    /// <param name="flag"></param>
    /// <returns><see cref="bool"/></returns>
    public bool HasFlag(int flag)
    {
        return (Flag == flag);
    }

    /// <summary>
    /// Returns move contains no flag
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public bool NoFlag()
    {
        return (Flag == MoveFlags.None);
    }

    /// <summary>
    /// Returns move has any promotion flag
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public bool HasPromotionFlag()
    {
        return (MoveFlags.PromotionKnight <= Flag) && (Flag <= MoveFlags.PromotionQueen);
    }

    /// <summary>
    /// Returns promotion piece type if move has promotion flag
    /// </summary>
    /// <returns><see cref="int"/></returns>
    public int GetPromotionType()
    {
        for (int i = 0; i < 4; i++)
            if ((Flag >> i) == 1)
                return i + 3;

        return 0;
    }

    /// <summary>
    /// Returns move has king or queen side castlings
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public bool HasCastlingFlag()
    {
        return (Flag == MoveFlags.KingCastling) || (Flag == MoveFlags.QueenCastling);
    }

    /// <summary>
    /// Returns move has system flag
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public bool HasSystemFlag()
    {
        return (Flag == MoveFlags.System);
    }

    #endregion

    #region Move Formats

    private static readonly string Files = "abcdefgh";

    /// <summary>
    /// Converts <paramref name="squareIndex"/> to Universal Chess Interface
    /// </summary>
    /// <param name="move"></param>
    /// <returns><see cref="string"/></returns>
    public static string ConvertUci(int squareIndex)
    {
        return string.Empty + Files[squareIndex % 8] + (squareIndex / 8 + 1);
    }

    /// <summary>
    /// Returns the square index from <paramref name="uci"/>
    /// </summary>
    /// <param name="uci"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="InvalidUciStringException"></exception>
    public static int ParseUci(string uci)
    {
        if (uci.Length != 2)
            throw new InvalidUciStringException(uci);

        int file = Files.IndexOf(uci[0]);
        if (file == -1 || !int.TryParse($"{uci[1]}", out int rank))
            throw new InvalidUciStringException(uci);

        if (rank < 1 || rank > 8)
            throw new InvalidUciStringException(uci);

        return ((rank - 1) * 8 + file);
    }

    #endregion
}

#endregion

#region Flags

public readonly struct EndFlags
{
    public static readonly int None = 0;
    public static readonly int Checkmate = 1;
    public static readonly int Stalemate = 2;
    public static readonly int FiftyMoveRule = 4;
    public static readonly int ThreefoldRepetition = 8;
}

public readonly struct MoveFlags
{
    public static readonly int None = 0;
    public static readonly int PromotionKnight = 1;
    public static readonly int PromotionBishop = 2;
    public static readonly int PromotionRook = 4;
    public static readonly int PromotionQueen = 8;
    public static readonly int DoublePawnPush = 16;
    public static readonly int EnpassantCapture = 32;
    public static readonly int KingCastling = 64;
    public static readonly int QueenCastling = 128;
    public static readonly int System = 256;
}

#endregion

#region Utility

/// <summary>
/// Contains precomputed move data for using in move functions
/// </summary>
public static class PrecomputedMoveData
{
    /// <summary>
    /// Direction offsets to North, South, West, East, NorthWest, SouthEast, NorthEast, SouthWest
    /// </summary>
    public static readonly int[] DirectionOffsets = new int[] { 8, -8, -1, 1, 7, -7, 9, -9 };

    /// <summary>
    /// Stores number squares to board edge for each direction
    /// </summary>
    public static readonly int[][] NumSquaresToEdge = new int[64][];

    /// <summary>
    /// Stores square index for knight moves
    /// </summary>
    public static readonly int[][] KnightSquares = new int[64][];

    static PrecomputedMoveData()
    {
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                int numNorth = 7 - rank;
                int numSouth = rank;
                int numWest = file;
                int numEast = 7 - file;

                int squareIndex = rank * 8 + file;

                NumSquaresToEdge[squareIndex] = new int[]
                {
                    numNorth,
                    numSouth,
                    numWest,
                    numEast,
                    Math.Min(numNorth, numWest),
                    Math.Min(numSouth, numEast),
                    Math.Min(numNorth, numEast),
                    Math.Min(numSouth, numWest)
                };

                KnightSquares[squareIndex] = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };

                if (rank > 1)
                {
                    if (file > 0)
                        KnightSquares[squareIndex][0] = squareIndex - 17;

                    if (file < 7)
                        KnightSquares[squareIndex][1] = squareIndex - 15;
                }

                if (rank < 6)
                {
                    if (file > 0)
                        KnightSquares[squareIndex][2] = squareIndex + 15;

                    if (file < 7)
                        KnightSquares[squareIndex][3] = squareIndex + 17;
                }

                if (file > 1)
                {
                    if (rank > 0)
                        KnightSquares[squareIndex][4] = squareIndex - 10;

                    if (rank < 7)
                        KnightSquares[squareIndex][5] = squareIndex + 6;
                }

                if (file < 6)
                {
                    if (rank > 0)
                        KnightSquares[squareIndex][6] = squareIndex - 6;

                    if (rank < 7)
                        KnightSquares[squareIndex][7] = squareIndex + 10;
                }
            }
        }
    }
}

public static class PortableGameNotation
{
    /// <summary>
    /// Generates Portable Game Notation from <paramref name="board"/>
    /// </summary>
    /// <param name="board"></param>
    /// <param name="white">White Player</param>
    /// <param name="black">Black Player</param>
    /// <returns><see cref="string"/></returns>
    public static string GeneratePgn(Board board, string white = "Unknown", string black = "Unknown")
    {
        string pgn = $"[White \"{white}\"]\n[Black \"{black}\"]\n[Date \"{DateTime.UtcNow.ToShortDateString()}\"]\n\n";
        for (int i = 0; i < (board.MoveCount + 1) / 2; i++)
        {
            pgn += $"{(i + 1)}. {board.Moves[i * 2].San} ";
            pgn += ((i * 2 + 1) < board.MoveCount) ? $"{board.Moves[i * 2 + 1].San} " : string.Empty;
        }
        return pgn;
    }

    /// <summary>
    /// Generates <see cref="Board"/> from <paramref name="pgn"/>
    /// </summary>
    /// <param name="pgn">Portable Game Notation to load</param>
    /// <returns><see cref="Board"/></returns>
    public static Board LoadPgn(string pgn)
    {
        var board = new Board();
        /* TODO */
        return board;
    }
}

public static class ForsythEdwardsNotation
{
    /// <summary>
    /// Forsyth-Edwards Notation for starting chess position
    /// </summary>
    public static readonly string StaringPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    /// <summary>
    /// Generates Forsyth-Edwards Notation from <paramref name="board"/>
    /// </summary>
    /// <param name="board"></param>
    /// <returns><see cref="string"/></returns>
    public static string GenerateFen(Board board)
    {
        string fen = string.Empty;
        int empty;
        for (int rank = 0; rank < 8; rank++)
        {
            empty = 0;
            for (int file = 0; file < 8; file++)
            {
                int squareIndex = (7 - rank) * 8 + file;
                if (Piece.IsEmpty(board[squareIndex]))
                {
                    empty++;
                }

                else
                {
                    fen += (empty > 0 ? empty : string.Empty) + Piece.PieceSymbol(board[squareIndex]);
                    empty = 0;
                }
            }

            if (empty > 0)
                fen += empty;

            fen += "/";
        }

        fen += $"\b {(Piece.IsColour(board.ColourToMove, Piece.White) ? 'w' : 'b')}";

        string castlings = string.Empty;
        if (board.Moves.FindAll(x => x.Piece == (Piece.King | Piece.White)).Count == 0)
        {
            if (board.Moves.FindAll(x => x.Index == 7).Count == 0)
                castlings += "K";

            if (board.Moves.FindAll(x => x.Index == 0).Count == 0)
                castlings += "Q";
        }
        if (board.Moves.FindAll(x => x.Piece == (Piece.King | Piece.Black)).Count == 0)
        {
            if (board.Moves.FindAll(x => x.Index == 63).Count == 0)
                castlings += "k";

            if (board.Moves.FindAll(x => x.Index == 56).Count == 0)
                castlings += "q";
        }
        fen += $" {(string.IsNullOrEmpty(castlings) ? '-' : castlings)} {(board.EnpassantSquare == -1 ? '-' : Move.ConvertUci(board.EnpassantSquare))} {board.HalfMoveCount} {board.MoveTurn}";
        return fen;
    }

    /// <summary>
    /// Generates <see cref="Board"/> from given <paramref name="fen"/>
    /// </summary>
    /// <param name="fen">Forsyth-Edwards Notation to load</param>
    /// <returns><see cref="Board"/></returns>
    /// <exception cref="InvalidFenException"></exception>
    public static Board LoadFen(string fen)
    {
        try
        {
            Board board = new();
            string[] splittedFen = fen.Split(' ');
            if (splittedFen.Length != 6)
                throw new InvalidFenException(fen);

            string[] vs = splittedFen[0].Split('/');
            if (vs.Length != 8)
                throw new InvalidFenException(fen);

            int file;
            for (int rank = 0; rank < 8; rank++)
            {
                file = 0;
                foreach (var character in vs[rank])
                {
                    if (char.IsDigit(character))
                        file += int.Parse($"{character}") - 1;

                    else
                        board.PutPiece((7 - rank) * 8 + file, Piece.FromSymbol(character));
                    file++;
                }
            }
            int turn = (splittedFen[1] == "w") ? Piece.White : Piece.Black;

            if (!splittedFen[2].Contains("Q"))
                board.Moves.Add(new Move(0, 0, MoveFlags.System));

            if (!splittedFen[2].Contains("K"))
                board.Moves.Add(new Move(7, 7, MoveFlags.System));

            if (!splittedFen[2].Contains("q"))
                board.Moves.Add(new Move(56, 56, MoveFlags.System));

            if (!splittedFen[2].Contains("k"))
                board.Moves.Add(new Move(63, 63, MoveFlags.System));

            if (!int.TryParse(splittedFen[5], out int moveCount))
                throw new InvalidFenException(fen);

            for (int i = 0; i < ((moveCount - 1) * 2 + ((turn == Piece.White) ? 0 : 1)) - board.MoveCount; i++)
                board.Moves.Add(new Move(-1, -1, MoveFlags.System));

            return board;
        }

        catch
        {
            throw new InvalidFenException(fen);
        }
    }
}

#endregion

#region Exceptions
public class InvalidFenException : Exception
{
    public string Fen { get; set; }

    public InvalidFenException(string fen) : base("Passed FEN is not a valid item.")
    {
        Fen = fen;
    }
}

public class InvalidUciStringException : Exception
{
    public string Uci { get; set; }

    public InvalidUciStringException(string uci) : base("Passed UCI is not a valid item for this.")
    {
        Uci = uci;
    }
}

public class InvalidSanStringException : Exception
{
    public string San { get; set; }

    public InvalidSanStringException(string san) : base("Passed SAN is not a valid item for this.")
    {
        San = san;
    }
}

#endregion