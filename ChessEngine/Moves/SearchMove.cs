using System;
using System.Collections.Generic;

namespace MonoChess
{
    internal static class SearchMove
    {
        internal struct Position
        {
            internal byte SrcPosition;
            internal byte DstPosition;
            internal int Score;
        }

        internal struct ResultBoards
        {
            internal List<ChessBoard> Positions;
        }

        private static byte nodesSearched;

        private static int Sort(Position s2, Position s1)
        {
            return (s1.Score).CompareTo(s2.Score);
        }

        private static int SideToMoveScore(int score, ChessPieceColor color)
        {
            if (color == ChessPieceColor.Black)
                return -score;
            else
                return score;
        }

        internal static MoveContent AlphaBetaRoot(ChessBoard examineBoard, byte depth)
        {
            nodesSearched = 0;

            int alpha = -400000000;
            const int beta = 400000000;

            ChessBoard bestBoard = new ChessBoard(short.MinValue);

            //We are going to store our result boards here           
            ResultBoards succ = new ResultBoards();
            succ.Positions = new List<ChessBoard>();

            for (byte x = 0; x < 64; ++x)
            {
                ChessPiece piece = examineBoard.pieces[x];

                //Make sure there is a piece on the square
                //Make sure the color is the same color as the one we are moving.
                if (piece == null || piece.PieceColor != examineBoard.WhoseMove)
                    continue;

                //For each valid move for this piece
                foreach (byte dst in piece.ValidMoves)
                {
                    //We make copies of the board and move so that we can move it without effecting the parent board
                    ChessBoard board = new ChessBoard(examineBoard);

                    //Make move so we can examine it
                    ChessEngine.MoveContent(board, x, dst, ChessPieceType.Queen);

                    //We Generate Valid Moves for Board
                    PieceValidMoves.GenerateValidMoves(board);

                    //Invalid Move
                    if (board.whiteInCheck && examineBoard.WhoseMove == ChessPieceColor.White)
                    {
                        continue;
                    }
                    //Invalid Move
                    if (board.blackInCheck && examineBoard.WhoseMove == ChessPieceColor.Black)
                    {
                        continue;
                    }

                    //We calculate the board score
                    Evaluation.EvaluateBoardScore(board);

                    //Invert Score to support Negamax
                    board.Score = SideToMoveScore(board.Score, board.WhoseMove);

                    succ.Positions.Add(board);
                }
            }

            //Why we need sorting?
            succ.Positions.Sort(ChessBoard.Sort);

            //Can I make an instant mate?
            foreach (ChessBoard pos in succ.Positions)
            {
                int value = -AlphaBeta(pos, 1, -beta, -alpha);
                if (value >= Constants.GameOverValue)
                {
                    return pos.LastMove;
                }
            }
            --depth;

            byte plyDepthReached = ModifyDepth(depth, succ.Positions.Count);

            int currentBoard = 0;
            alpha = -400000000;

            //Why we need sorting?
            succ.Positions.Sort(ChessBoard.Sort);

            for(int i = 0; i < succ.Positions.Count; ++i)
            {
                ++currentBoard;
                ChessBoard board = succ.Positions[i];
                
                int value = -AlphaBeta(board, plyDepthReached, -beta, -alpha);
                board.Score = value;

                //If value is greater then alpha this is the best board
                if (value > alpha)
                {
                    alpha = value;
                    bestBoard = new ChessBoard(board);
                }
            }

            System.Diagnostics.Debug.WriteLine("Node Searched: " + nodesSearched);

            return bestBoard.LastMove;
        }

        private static byte ModifyDepth(byte depth, int possibleMoves)
        {
            if (possibleMoves <= 15)
            {
                ++depth;
            }
            return depth;
        }

        private static int MinMax(ChessBoard examineBoard, byte depth)
        {
            if (depth == 0)
            {
                //Evaluate Score
                Evaluation.EvaluateBoardScore(examineBoard);
                //Invert Score to support Negamax
                return SideToMoveScore(examineBoard.Score, examineBoard.WhoseMove);
            }

            List<Position> positions = EvaluateMoves(examineBoard, depth);

            if (examineBoard.whiteInCheck || examineBoard.blackInCheck || positions.Count == 0)
            {
                if (SearchForMate(examineBoard.WhoseMove, examineBoard, ref examineBoard.blackInMate, ref examineBoard.whiteInMate, ref examineBoard.staleMate))
                {
                    if (examineBoard.blackInMate)
                    {
                        if (examineBoard.WhoseMove == ChessPieceColor.Black)
                            return -Constants.GameOverValue - depth;

                        return Constants.GameOverValue + depth;
                    }
                    if (examineBoard.whiteInMate)
                    {
                        if (examineBoard.WhoseMove == ChessPieceColor.Black)
                            return Constants.GameOverValue + depth;

                        return -Constants.GameOverValue - depth;
                    }

                    //If Not Mate then StaleMate
                    return 0;
                }
            }

            int bestScore = -Constants.GameOverValue;

            foreach (Position move in positions)
            {
                //Make a copy
                ChessBoard board = new ChessBoard(examineBoard);

                //Move Piece
                ChessEngine.MoveContent(board, move.SrcPosition, move.DstPosition, ChessPieceType.Queen);

                //We Generate Valid Moves for Board
                PieceValidMoves.GenerateValidMoves(board);

                if (board.blackInCheck && examineBoard.WhoseMove == ChessPieceColor.Black)
                {
                    //Invalid Move
                    continue;
                }

                if (board.whiteInCheck && examineBoard.WhoseMove == ChessPieceColor.White)
                {
                    //Invalid Move
                    continue;
                }

                int value = -MinMax(board, (byte)(depth - 1));
                if (value > bestScore)
                {
                    bestScore = value;
                }
            }

            return bestScore;
        }

        internal static List<Position> EvaluateMoves(ChessBoard board, byte depth)
        {
            //We are going to store our result boards here           
            List<Position> positions = new List<Position>();

            for (byte x = 0; x < 64; ++x)
            {
                ChessPiece piece = board.pieces[x];

                //Make sure there is a piece on the square
                //Make sure the color is the same color as the one we are moving.
                if (piece == null || piece.PieceColor != board.WhoseMove) 
                    continue;

                //For each valid move for this piece
                foreach (byte dst in piece.ValidMoves)
                {
                    Position move = new Position();

                    move.SrcPosition = x;
                    move.DstPosition = dst;

                    ChessPiece pieceAttacked = board.pieces[move.DstPosition];

                    //If the move is a capture add it's value to the score
                    if (pieceAttacked != null)
                    {
                        move.Score += pieceAttacked.PieceValue;

                        if (piece.PieceValue < pieceAttacked.PieceValue)
                        {
                            move.Score += pieceAttacked.PieceValue - piece.PieceValue;
                        }
                    }

                    if (!piece.Moved)
                    {
                        move.Score += 10;
                    }

                    move.Score += piece.PieceActionValue;

                    //Add Score for Castling
                    if (!board.whiteCastled && board.WhoseMove == ChessPieceColor.White)
                    {
                        if (piece.Identifier == ChessPieceType.King)
                        {
                            if (move.DstPosition != 62 && move.DstPosition != 58)
                            {
                                move.Score -= 40;
                            }
                            else
                            {
                                move.Score += 40;
                            }
                        }
                        if (piece.Identifier == ChessPieceType.Rook)
                        {
                            move.Score -= 40;
                        }
                    }

                    if (!board.blackCastled && board.WhoseMove == ChessPieceColor.Black)
                    {
                        if (piece.Identifier == ChessPieceType.King)
                        {
                            if (move.DstPosition != 6 && move.DstPosition != 2)
                            {
                                move.Score -= 40;
                            }
                            else
                            {
                                move.Score += 40;
                            }
                        }
                        if (piece.Identifier == ChessPieceType.Rook)
                        {
                            move.Score -= 40;
                        }
                    }

                    positions.Add(move);
                }
            }

            return positions;
        }

        private static int AlphaBeta(ChessBoard examineBoard, byte depth, int alpha, int beta)
        {
            if (examineBoard == null || examineBoard.pieces == null)
                return 0;

            ++nodesSearched;

            if (examineBoard.FiftyMove >= 50 || examineBoard.RepeatedMove >= 3)
                return 0;

            if (depth == 0)
            {
                //Evaluate Score
                Evaluation.EvaluateBoardScore(examineBoard);
                //Invert Score to support Negamax
                return SideToMoveScore(examineBoard.Score, examineBoard.WhoseMove);
            }

            List<Position> positions = EvaluateMoves(examineBoard,depth);

            if (examineBoard.whiteInCheck || examineBoard.blackInCheck || positions.Count == 0)
            {
                if (SearchForMate(examineBoard.WhoseMove, examineBoard, ref examineBoard.blackInMate, ref examineBoard.whiteInMate, ref examineBoard.staleMate))
                {
                    if (examineBoard.blackInMate)
                    {
                        if (examineBoard.WhoseMove == ChessPieceColor.Black)
                            return -Constants.GameOverValue - depth;

                        return Constants.GameOverValue + depth;
                    }
                    if (examineBoard.whiteInMate)
                    {
                        if (examineBoard.WhoseMove == ChessPieceColor.Black)
                            return Constants.GameOverValue + depth;

                        return -Constants.GameOverValue - depth;
                    }

                    //If Not Mate then StaleMate
                    return 0;
                }
            }

            //Why we need sorting?
            positions.Sort(Sort);

            foreach (Position move in positions)
            {
                //Make a copy
                ChessBoard board = new ChessBoard(examineBoard);

                //Move Piece
                ChessEngine.MoveContent(board, move.SrcPosition, move.DstPosition, ChessPieceType.Queen);

                //We Generate Valid Moves for Board
                PieceValidMoves.GenerateValidMoves(board);

                if (board.blackInCheck && examineBoard.WhoseMove == ChessPieceColor.Black)
                {
                    //Invalid Move
                    continue;
                }

                if (board.whiteInCheck && examineBoard.WhoseMove == ChessPieceColor.White)
                {
                    //Invalid Move
                    continue;
                }

                int value = -AlphaBeta(board, (byte)(depth - 1), -beta, -alpha);

                if (value >= beta)
                {
                    // Beta cut-off
                    return beta;
                }
                if (value > alpha)
                {
                    alpha = value;
                }
            }

            return alpha;
        }

        internal static bool SearchForMate(ChessPieceColor movingSide, ChessBoard examineBoard, ref bool blackMate, ref bool whiteMate, ref bool staleMate)
        {
            bool foundNonCheckBlack = false;
            bool foundNonCheckWhite = false;

            for (byte x = 0; x < 64; ++x)
            {
                ChessPiece piece = examineBoard.pieces[x];

                //Make sure there is a piece on the square
                //Make sure the color is the same color as the one we are moving.
                if (piece == null || piece.PieceColor != movingSide)
                    continue;

                //For each valid move for this piece
                foreach (byte dst in piece.ValidMoves)
                {
                    //We make copies of the board and move so we don't change the original
                    ChessBoard board = new ChessBoard(examineBoard);

                    //Make move so we can examine it
                    ChessEngine.MoveContent(board, x, dst, ChessPieceType.Queen);

                    //We Generate Valid Moves for Board
                    PieceValidMoves.GenerateValidMoves(board);

                    if (!board.blackInCheck)
                    {
                        foundNonCheckBlack = true;
                    }
                    else if (movingSide == ChessPieceColor.Black)
                    {
                        continue;
                    }

                    if (!board.whiteInCheck)
                    {
                        foundNonCheckWhite = true;
                    }
                    else if (movingSide == ChessPieceColor.White)
                    {
                        continue;
                    }
                }
            }

            if (!foundNonCheckBlack)
            {
                if (examineBoard.blackInCheck)
                {
                    blackMate = true;
                    return true;
                }
                if (!examineBoard.whiteInMate && movingSide != ChessPieceColor.White)
                {
                    staleMate = true;
                    return true;
                }
            }

            if (!foundNonCheckWhite)
            {
                if (examineBoard.whiteInCheck)
                {
                    whiteMate = true;
                    return true;
                }
                if (!examineBoard.blackInMate && movingSide != ChessPieceColor.Black)
                {
                    staleMate = true;
                    return true;
                }
            }
            return false;
        }
    }
}
