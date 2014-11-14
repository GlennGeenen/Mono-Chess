using System.Collections.Generic;

namespace MonoChess
{
    internal static class PieceValidMoves
    {
        internal static bool[] BlackAttackBoard;
        private static byte blackKingPosition;

        internal static bool[] WhiteAttackBoard;
        private static byte whiteKingPosition;

        private static void AnalyzeMovePawn(ChessBoard board, byte dstPos, ChessPiece movingPiece)
        {
            bool movingPieceIsWhite = movingPiece.PieceColor == ChessPieceColor.White;

            //Because Pawns only kill diagonaly we handle the En Passant scenario specialy
            if (board.EnPassantPosition == dstPos && board.EnPassantPosition > 0 && movingPiece.PieceColor != board.EnPassantColor)
            {
                //We have an En Passant Possible
                movingPiece.ValidMoves.Push(dstPos);

                if (movingPieceIsWhite)
                {
                    WhiteAttackBoard[dstPos] = true;
                }
                else
                {
                    BlackAttackBoard[dstPos] = true;
                }
            }

            ChessPiece pcAttacked = board.pieces[dstPos];

            //If there no piece there I can potentialy kill
            if (pcAttacked == null)
                return;

            //Regardless of what is there I am attacking this square
            if (movingPieceIsWhite)
            {
                WhiteAttackBoard[dstPos] = true;

                //if that piece is the same color
                if (pcAttacked.PieceColor == movingPiece.PieceColor)
                {
                    pcAttacked.DefendedValue += movingPiece.PieceActionValue;
                    return;
                }

                pcAttacked.AttackedValue += movingPiece.PieceActionValue;

                //If this is a king set it in check                   
                if (pcAttacked.Identifier == ChessPieceType.King)
                {
                    board.blackInCheck = true;
                }
                else
                {
                    //Add this as a valid move
                    movingPiece.ValidMoves.Push(dstPos);
                }
            }
            else
            {
                BlackAttackBoard[dstPos] = true;

                //if that piece is the same color
                if (pcAttacked.PieceColor == movingPiece.PieceColor)
                {
                    pcAttacked.DefendedValue += movingPiece.PieceActionValue;
                    return;
                }

                pcAttacked.AttackedValue += movingPiece.PieceActionValue;

                //If this is a king set it in check                   
                if (pcAttacked.Identifier == ChessPieceType.King)
                {
                    board.whiteInCheck = true;
                }
                else
                {
                    //Add this as a valid move
                    movingPiece.ValidMoves.Push(dstPos);
                }
            }

            return;
        }

        private static bool AnalyzeMove(ChessBoard board, byte dstPos, ChessPiece movingPiece)
        {
            //If I am not a pawn everywhere I move I can attack

            bool movingPieceIsWhite = movingPiece.PieceColor == ChessPieceColor.White;
            if (movingPieceIsWhite)
            {
                WhiteAttackBoard[dstPos] = true;
            }
            else
            {
                BlackAttackBoard[dstPos] = true;
            }

            //If there no piece there I can potentialy kill just add the move and exit
            if (board.pieces[dstPos] == null)
            {
                movingPiece.ValidMoves.Push(dstPos);
                return true;
            }

            ChessPiece pcAttacked = board.pieces[dstPos];

            //if that piece is a different color
            if (pcAttacked.PieceColor != movingPiece.PieceColor)
            {
                pcAttacked.AttackedValue += movingPiece.PieceActionValue;

                //If this is a king set it in check                   
                if (pcAttacked.Identifier == ChessPieceType.King)
                {
                    if (movingPieceIsWhite)
                    {
                        board.blackInCheck = true;
                    }
                    else
                    {
                        board.whiteInCheck = true;
                    }
                }
                else
                {
                    //Add this as a valid move
                    movingPiece.ValidMoves.Push(dstPos);
                }

                //We don't continue movement past this piece
                return false;
            }

            //Same Color I am defending
            pcAttacked.DefendedValue += movingPiece.PieceActionValue;

            //Since this piece is of my kind I can't move there
            return false;
        }

        private static void CheckValidMovesPawn(byte[] moves, ChessPiece pcMoving, byte srcPosition,
                                                ChessBoard board, byte count)
        {
            for (byte i = 0; i < count; ++i)
            {
                byte dstPos = moves[i];

                // Piece in capture position
                if (dstPos % 8 != srcPosition % 8)
                {
                    //If there is a piece there I can potentialy kill
                    AnalyzeMovePawn(board, dstPos, pcMoving);

                    if (pcMoving.PieceColor == ChessPieceColor.White)
                    {
                        WhiteAttackBoard[dstPos] = true;
                    }
                    else
                    {
                        BlackAttackBoard[dstPos] = true;
                    }
                }
                    // if there is something in front pawns can't move there
                else if (board.pieces[dstPos] != null)
                {
                    return;
                }
                    //if there is nothing in front of me (blocked == false)
                else
                {
                    pcMoving.ValidMoves.Push(dstPos);
                }
            }
        }

        private static void GenerateValidMovesKing(ChessPiece piece, ChessBoard board, byte srcPosition)
        {
            if (piece == null)
            {
                return;
            }

            for (byte i = 0; i < MoveArrays.KingTotalMoves[srcPosition]; ++i)
            {
                byte dstPos = MoveArrays.KingMoves[srcPosition][i];

                if (piece.PieceColor == ChessPieceColor.White)
                {
                    //I can't move where I am being attacked
                    if (BlackAttackBoard[dstPos])
                    {
                        WhiteAttackBoard[dstPos] = true;
                        continue;
                    }
                }
                else if (WhiteAttackBoard[dstPos])
                {
                    BlackAttackBoard[dstPos] = true;
                    continue;
                }

                AnalyzeMove(board, dstPos, piece);
            }
        }

        private static void GenerateValidMovesKingCastle(ChessBoard board, ChessPiece king)
        {
            if (king == null || king.Moved)
            {
                return;
            }

            if (king.PieceColor == ChessPieceColor.White &&
                board.whiteCastled)
            {
                return;
            }
            if (king.PieceColor == ChessPieceColor.Black &&
                board.blackCastled)
            {
                return;
            }
            if (king.PieceColor == ChessPieceColor.Black &&
                board.blackInCheck)
            {
                return;
            }
            if (king.PieceColor == ChessPieceColor.White &&
                board.whiteInCheck)
            {
                return;
            }


            //This code will add the castleling move to the pieces available moves
            if (king.PieceColor == ChessPieceColor.White)
            {
                if (board.whiteInCheck)
                {
                    return;
                }

                if (board.pieces[63] != null
                    && board.pieces[63].Identifier == ChessPieceType.Rook
                    && board.pieces[63].PieceColor == king.PieceColor
                    && board.pieces[62] == null
                    && board.pieces[61] == null
                    && BlackAttackBoard[61] == false
                    && BlackAttackBoard[62] == false)
                {
                    //Ok looks like move is valid lets add it
                    king.ValidMoves.Push(62);
                    WhiteAttackBoard[62] = true;
                }

                if (board.pieces[56] != null
                    && board.pieces[56].Identifier == ChessPieceType.Rook
                    && board.pieces[56].PieceColor == king.PieceColor
                    && board.pieces[57] == null
                    && board.pieces[58] == null
                    && board.pieces[59] == null
                    && BlackAttackBoard[58] == false
                    && BlackAttackBoard[59] == false)
                {
                    //Ok looks like move is valid lets add it
                    king.ValidMoves.Push(58);
                    WhiteAttackBoard[58] = true;
                }
            }
            else if (king.PieceColor == ChessPieceColor.Black)
            {
                if (board.blackInCheck)
                {
                    return;
                }

                //There are two ways to castle, scenario 1:
                if (board.pieces[7] != null)
                {
                    //Check if the Right Rook is still in the correct position
                    if (board.pieces[7].Identifier == ChessPieceType.Rook
                        && !board.pieces[7].Moved)
                    {
                        if (board.pieces[7].PieceColor == king.PieceColor)
                        {
                            //Move one column to right see if its empty

                            if (board.pieces[6] == null)
                            {
                                if (board.pieces[5] == null)
                                {
                                    if (WhiteAttackBoard[5] == false && WhiteAttackBoard[6] == false)
                                    {
                                        //Ok looks like move is valid lets add it
                                        king.ValidMoves.Push(6);
                                        BlackAttackBoard[6] = true;
                                    }
                                }
                            }
                        }
                    }
                }
                //There are two ways to castle, scenario 2:
                if (board.pieces[0] != null)
                {
                    //Check if the Left Rook is still in the correct position
                    if (board.pieces[0].Identifier == ChessPieceType.Rook &&
                        !board.pieces[0].Moved)
                    {
                        if (board.pieces[0].PieceColor ==
                            king.PieceColor)
                        {
                            //Move one column to right see if its empty
                            if (board.pieces[1] == null)
                            {
                                if (board.pieces[2] == null)
                                {
                                    if (board.pieces[3] == null)
                                    {
                                        if (WhiteAttackBoard[2] == false &&
                                            WhiteAttackBoard[3] == false)
                                        {
                                            //Ok looks like move is valid lets add it
                                            king.ValidMoves.Push(2);
                                            BlackAttackBoard[2] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static void GenerateValidMoves(ChessBoard board)
        {
            // Reset Board
            board.blackInCheck = false;
            board.whiteInCheck = false;

            WhiteAttackBoard = new bool[64];
            BlackAttackBoard = new bool[64];

            //Generate Moves
            for (byte x = 0; x < 64; ++x)
            {
                ChessPiece piece = board.pieces[x];

                if (piece == null)
                    continue;

                piece.ValidMoves = new Stack<byte>(piece.LastValidMoveCount);

                switch (piece.Identifier)
                {
                    case ChessPieceType.Pawn:
                        {
                            if (piece.PieceColor == ChessPieceColor.White)
                            {
                                CheckValidMovesPawn(MoveArrays.WhitePawnMoves[x], piece, x,
                                                    board,
                                                    MoveArrays.WhitePawnTotalMoves[x]);
                                break;
                            }
                            if (piece.PieceColor == ChessPieceColor.Black)
                            {
                                CheckValidMovesPawn(MoveArrays.BlackPawnMoves[x], piece, x,
                                                    board,
                                                    MoveArrays.BlackPawnTotalMoves[x]);
                                break;
                            }

                            break;
                        }
                    case ChessPieceType.Knight:
                        {
                            for (byte i = 0; i < MoveArrays.KnightTotalMoves[x]; ++i)
                            {
                                AnalyzeMove(board, MoveArrays.KnightMoves[x][i], piece);
                            }

                            break;
                        }
                    case ChessPieceType.Bishop:
                        {
                            for (byte i = 0; i < MoveArrays.BishopTotalMoves1[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.BishopMoves1[x][i],
                                                piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.BishopTotalMoves2[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.BishopMoves2[x][i],
                                                piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.BishopTotalMoves3[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.BishopMoves3[x][i],
                                                piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.BishopTotalMoves4[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.BishopMoves4[x][i],
                                                piece) ==
                                    false)
                                {
                                    break;
                                }
                            }

                            break;
                        }
                    case ChessPieceType.Rook:
                        {
                            for (byte i = 0; i < MoveArrays.RookTotalMoves1[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.RookMoves1[x][i], piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.RookTotalMoves2[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.RookMoves2[x][i], piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.RookTotalMoves3[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.RookMoves3[x][i], piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.RookTotalMoves4[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.RookMoves4[x][i], piece) ==
                                    false)
                                {
                                    break;
                                }
                            }

                            break;
                        }
                    case ChessPieceType.Queen:
                        {
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves1[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves1[x][i], piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves2[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves2[x][i], piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves3[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves3[x][i], piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves4[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves4[x][i], piece) ==
                                    false)
                                {
                                    break;
                                }
                            }

                            for (byte i = 0; i < MoveArrays.QueenTotalMoves5[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves5[x][i], piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves6[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves6[x][i], piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves7[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves7[x][i], piece) ==
                                    false)
                                {
                                    break;
                                }
                            }
                            for (byte i = 0; i < MoveArrays.QueenTotalMoves8[x]; ++i)
                            {
                                if (
                                    AnalyzeMove(board, MoveArrays.QueenMoves8[x][i], piece) ==
                                    false)
                                {
                                    break;
                                }
                            }

                            break;
                        }
                    case ChessPieceType.King:
                        {
                            if (piece.PieceColor == ChessPieceColor.White)
                            {
                                whiteKingPosition = x;
                            }
                            else
                            {
                                blackKingPosition = x;
                            }

                            break;
                        }
                }
            }


            if (board.WhoseMove == ChessPieceColor.White)
            {
                GenerateValidMovesKing(board.pieces[blackKingPosition], board,
                                       blackKingPosition);
                GenerateValidMovesKing(board.pieces[whiteKingPosition], board,
                                       whiteKingPosition);
            }
            else
            {
                GenerateValidMovesKing(board.pieces[whiteKingPosition], board,
                                       whiteKingPosition);
                GenerateValidMovesKing(board.pieces[blackKingPosition], board,
                                       blackKingPosition);
            }


            //Now that all the pieces were examined we know if the king is in check
            GenerateValidMovesKingCastle(board, board.pieces[whiteKingPosition]);
            GenerateValidMovesKingCastle(board, board.pieces[blackKingPosition]);
        }
    }
}
