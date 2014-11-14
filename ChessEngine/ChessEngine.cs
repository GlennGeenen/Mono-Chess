namespace MonoChess
{
    public class ChessEngine
    {
        internal static bool CheckForMate(ChessPieceColor whosTurn, ChessBoard chessBoard)
        {
            SearchMove.SearchForMate(whosTurn, chessBoard, ref chessBoard.blackInMate,
                  ref chessBoard.whiteInMate, ref chessBoard.staleMate);

            if (chessBoard.blackInMate || chessBoard.whiteInMate || chessBoard.staleMate)
            {
                return true;
            }

            return false;
        }

        public static void EngineMove(ChessBoard board)
        {
            if (CheckForMate(board.WhoseMove, board))
            {
                return;
            }

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            //If there is no playbook move search for the best move
            MoveContent bestMove = SearchMove.AlphaBetaRoot(board, Constants.ply);
            ChessEngine.MoveContent(board, bestMove.MovingPiecePrimary.SrcPosition, bestMove.MovingPiecePrimary.DstPosition, ChessPieceType.Queen);

            PieceValidMoves.GenerateValidMoves(board);
            Evaluation.EvaluateBoardScore(board);

            System.Diagnostics.Debug.WriteLine("Engine Move Time: " + watch.ElapsedTicks);
        }

        public static bool IsValidMove(ChessBoard board, byte sourceIndex, byte destinationIndex)
        {
            if (board.pieces[sourceIndex] == null)
            {
                return false;
            }
            foreach (byte bs in board.pieces[sourceIndex].ValidMoves)
            {
                if (bs == destinationIndex)
                {
                    return true;
                }
            }
            if (destinationIndex == board.EnPassantPosition)
            {
                return true;
            }
            return false;
        }

        public static bool MovePiece(ChessBoard board, byte sourceIndex, byte destinationIndex)
        {
            ChessPiece piece = board.pieces[sourceIndex];

            //Do the actual move
            ChessEngine.MoveContent(board, sourceIndex, destinationIndex, ChessPieceType.Queen);
            PieceValidMoves.GenerateValidMoves(board);

            //If there is a check in place and still check
            if (piece.PieceColor == ChessPieceColor.White)
            {
                if (board.whiteInCheck)
                {
                    //Invalid Move -> undo last move
                    ChessEngine.MoveContent(board, destinationIndex, sourceIndex, ChessPieceType.Queen);
                    PieceValidMoves.GenerateValidMoves(board);
                    return false;
                }
            }
            else
            {
                if (board.blackInCheck)
                {
                    //Invalid Move -> undo last move
                    ChessEngine.MoveContent(board, destinationIndex, sourceIndex, ChessPieceType.Queen);
                    PieceValidMoves.GenerateValidMoves(board);
                    return false;
                }
            }
            return true;
        }

        internal static MoveContent MoveContent(ChessBoard board, byte srcPosition, byte dstPosition, ChessPieceType promoteToPiece)
        {
            ChessPiece piece = board.pieces[srcPosition];

            //Record my last move
            board.LastMove = new MoveContent();

            //Add One to FiftyMoveCount to check for tie.
            ++board.FiftyMove;

            if (piece.PieceColor == ChessPieceColor.Black)
            {
                ++board.MoveCount;
            }

            //En Passant
            if (board.EnPassantPosition > 0)
            {
                board.LastMove.EnPassantOccured = ChessEngine.SetEnpassantMove(board, dstPosition, piece.PieceColor);
            }

            if (!board.LastMove.EnPassantOccured)
            {
                ChessPiece dstPiece = board.pieces[dstPosition];
                if (dstPiece != null)
                {
                    board.LastMove.TakenPiece = new PieceTaken(dstPiece.PieceColor, dstPiece.Identifier,
                                                               dstPiece.Moved, dstPosition);
                    board.FiftyMove = 0;
                }
                else
                {
                    board.LastMove.TakenPiece = new PieceTaken(ChessPieceColor.White, ChessPieceType.None, false,
                                                               dstPosition);
                }
            }

            board.LastMove.MovingPiecePrimary = new PieceMoving(piece.PieceColor, piece.Identifier, piece.Moved, srcPosition, dstPosition);

            //Delete the piece in its source position
            board.pieces[srcPosition] = null;

            //Add the piece to its new position
            piece.Moved = true;
            board.pieces[dstPosition] = piece;

            //Reset EnPassantPosition
            board.EnPassantPosition = 0;

            //Record En Passant if Pawn Moving
            if (piece.Identifier == ChessPieceType.Pawn)
            {
                board.FiftyMove = 0;
                ChessEngine.RecordEnPassant(board, piece.PieceColor, piece.Identifier, srcPosition, dstPosition);
            }

            board.WhoseMove = board.WhoseMove == ChessPieceColor.White ? ChessPieceColor.Black : ChessPieceColor.White;

            ChessEngine.KingCastle(board, piece, srcPosition, dstPosition);

            //Promote Pawns 
            if (ChessEngine.PromotePawns(board, piece, dstPosition, promoteToPiece))
            {
                board.LastMove.PawnPromoted = true;
            }
            else
            {
                board.LastMove.PawnPromoted = false;
            }

            if (board.FiftyMove >= 50)
            {
                board.staleMate = true;
            }

            return board.LastMove;
        }

        internal static bool PromotePawns(ChessBoard board, ChessPiece piece, byte dstPosition, ChessPieceType promoteToPiece)
        {
            if (piece.Identifier == ChessPieceType.Pawn)
            {
                if (dstPosition < 8)
                {
                    board.pieces[dstPosition].Identifier = promoteToPiece;
                    return true;
                }
                if (dstPosition > 55)
                {
                    board.pieces[dstPosition].Identifier = promoteToPiece;
                    return true;
                }
            }
            return false;
        }

        internal static void RecordEnPassant(ChessBoard board, ChessPieceColor pcColor, ChessPieceType pcType, byte srcPosition, byte dstPosition)
        {
            //Record En Passant if Pawn Moving
            if (pcType == ChessPieceType.Pawn)
            {
                //Reset FiftyMoveCount if pawn moved
                board.FiftyMove = 0;

                int difference = srcPosition - dstPosition;

                if (difference == 16 || difference == -16)
                {
                    board.EnPassantPosition = (byte)(dstPosition + (difference / 2));
                    board.EnPassantColor = pcColor;
                }
            }
        }

        internal static bool SetEnpassantMove(ChessBoard board, byte dstPosition, ChessPieceColor pcColor)
        {
            //En Passant
            if (board.EnPassantPosition == dstPosition && pcColor != board.EnPassantColor)
            {
                int pieceLocationOffset = 8;
                if (board.EnPassantColor == ChessPieceColor.White)
                {
                    pieceLocationOffset = -8;
                }

                dstPosition = (byte)(dstPosition + pieceLocationOffset);

                ChessPiece piece = board.pieces[dstPosition];

                board.LastMove.TakenPiece = new PieceTaken(piece.PieceColor, piece.Identifier, piece.Moved, dstPosition);

                board.pieces[dstPosition] = null;

                //Reset FiftyMoveCount if capture
                board.FiftyMove = 0;

                return true;
            }

            return false;
        }

        internal static void KingCastle(ChessBoard board, ChessPiece piece, byte srcPosition, byte dstPosition)
        {
            if (piece.Identifier != ChessPieceType.King)
            {
                return;
            }

            //Lets see if this is a casteling move.
            if (piece.PieceColor == ChessPieceColor.White && srcPosition == 60)
            {
                //Castle Right
                if (dstPosition == 62 && board.pieces[63] != null)
                {
                    board.pieces[61] = board.pieces[63];
                    board.pieces[63] = null;
                    board.whiteCastled = true;
                    board.LastMove.MovingPieceSecondary = new PieceMoving(board.pieces[61].PieceColor, board.pieces[61].Identifier, board.pieces[61].Moved, 63, 61);
                    board.pieces[61].Moved = true;
                    return;
                }
                //Castle Left
                else if (dstPosition == 58 && board.pieces[56] != null)
                {
                    board.pieces[59] = board.pieces[56];
                    board.pieces[56] = null;
                    board.whiteCastled = true;
                    board.LastMove.MovingPieceSecondary = new PieceMoving(board.pieces[59].PieceColor, board.pieces[59].Identifier, board.pieces[59].Moved, 56, 59);
                    board.pieces[59].Moved = true;
                    return;
                }
            }
            else if (piece.PieceColor == ChessPieceColor.Black && srcPosition == 4)
            {
                if (dstPosition == 6 && board.pieces[7] != null)
                {
                    board.pieces[5] = board.pieces[7];
                    board.pieces[7] = null;
                    board.blackCastled = true;
                    board.LastMove.MovingPieceSecondary = new PieceMoving(board.pieces[5].PieceColor, board.pieces[5].Identifier, board.pieces[5].Moved, 7, 5);
                    board.pieces[5].Moved = true;
                    return;
                }
                //Castle Left
                else if (dstPosition == 2 && board.pieces[0] != null)
                {
                    board.pieces[3] = board.pieces[0];
                    board.pieces[0] = null;
                    board.blackCastled = true;
                    board.LastMove.MovingPieceSecondary = new PieceMoving(board.pieces[3].PieceColor, board.pieces[3].Identifier, board.pieces[3].Moved, 0, 3);
                    board.pieces[3].Moved = true;
                    return;
                }
            }
            return;
        }
    }
}
