using System;
using System.Collections.Generic;
using System.Linq;

namespace Cecs475.BoardGames.Chess {

    public class ChessBoard : IGameBoard {
        /// <summary>
        /// The number of rows and columns on the chess board.
        /// </summary>
        public const int BOARD_SIZE = 8;

        private static BoardPosition[] mBishopDirections = {
            new BoardPosition(1, 1),
            new BoardPosition(-1, 1),
            new BoardPosition(1, -1),
            new BoardPosition(-1, -1)
        };

        private static BoardPosition[] mRookDirections = {
            new BoardPosition(1, 0),
            new BoardPosition(-1, 0),
            new BoardPosition(0, 1),
            new BoardPosition(0, -1)
        };

        private static BoardPosition[] mQueenDirections = {
            new BoardPosition(1, 0),
            new BoardPosition(-1, 0),
            new BoardPosition(0, 1),
            new BoardPosition(0, -1),
            new BoardPosition(1, 1),
            new BoardPosition(-1, 1),
            new BoardPosition(1, -1),
            new BoardPosition(-1, -1),
        };

        private static BoardPosition[] mCastlingShortSquares1 = {            
            new BoardPosition(7,2),
            new BoardPosition(7,3), 
            new BoardPosition(7,4)         
                      
        };

        private static BoardPosition[] mCastlingShortSquares2 = {
            new BoardPosition(0, 2),
            new BoardPosition(0, 3),
            new BoardPosition(0, 4)
        };

        private static BoardPosition[] mCastlingLongSquares1 = {           
            new BoardPosition(7,4),
            new BoardPosition(7,5),  
            new BoardPosition(7,6),                    
        };

        private static BoardPosition[] mCastlingLongSquares2 = { 
            new BoardPosition(0, 4),          
            new BoardPosition(0, 5),
            new BoardPosition(0, 6),
        };


        // Reminder: there are 3 different types of rooks
        public sbyte[,] mBoard = new sbyte[8, 8] {
            {-2, -4, -5, -6, -7, -5, -4, -3 },
            {-1, -1, -1, -1, -1, -1, -1, -1 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {0, 0, 0, 0, 0, 0, 0, 0 },
            {1, 1, 1, 1, 1, 1, 1, 1 },
            {2, 4, 5, 6, 7, 5, 4, 3 }
        };

        // TODO: DONE
        // You need a way of keeping track of certain game state flags. For example, a rook cannot perform a castling move
        // if either the rook or its king has moved in the game, so you need a way of determining whether those things have 
        // happened. There are several ways to do it and I leave it up to you.

        public bool mAllowCastlingKingSide1;
        public bool mAllowCastlingQueenSide1;
        public bool mAllowCastlingKingSide2;
        public bool mAllowCastlingQueenSide2;
        private bool mAllowEnPassant;
        private bool mAllowPawnTwoSteps;
        private int mPlayer;        

        /// <summary>
        /// Constructs a new chess board with the default starting arrangement.
        /// </summary>
        public ChessBoard() {
            MoveHistory = new List<IGameMove>();

            // TODO: DONE
            // Finish any other one-time setup.
            mPlayer = 1;
            mAllowCastlingKingSide1 = true;
            mAllowCastlingQueenSide1 = true;
            mAllowCastlingKingSide2 = true;
            mAllowCastlingQueenSide2 = true;
            mAllowEnPassant = true;
            mAllowPawnTwoSteps = true;
        }

        /// <summary>
        /// Constructs a new chess board by only placing pieces as specified.
        /// </summary>
        /// <param name="startingPositions">a sequence of tuple pairs, where each pair specifies the starting
        /// position of a particular piece to place on the board</param>
        public ChessBoard(IEnumerable<Tuple<BoardPosition, ChessPiecePosition>> startingPositions)

            : this() { // NOTE THAT THIS CONSTRUCTOR CALLS YOUR DEFAULT CONSTRUCTOR FIRST


            foreach (int i in Enumerable.Range(0, 8)) { // another way of doing for i = 0 to < 8
                foreach (int j in Enumerable.Range(0, 8)) {
                    mBoard[i, j] = 0;
                }
            }
            foreach (var pos in startingPositions) {
                SetPosition(pos.Item1, pos.Item2);
            }
        }

        /// <summary>
        /// A difference in piece values for the pieces still controlled by white vs. black, where
        /// a pawn is value 1, a knight and bishop are value 3, a rook is value 5, and a queen is value 9.
        /// </summary>
        public int Value { get; private set; }



        public int CurrentPlayer {
            //TODO: if player is -1 return 2, otherwise return 1
            get {
                if (mPlayer == 1)
                    return mPlayer;

                else if (mPlayer == -1)
                    return 2;

                else if (mPlayer == -2)
                    return 1;

                else
                    return 1;

            }

            // TODO: implement the CurrentPlayer property.


            /* private set {
                 //TODO: FIX LOGIC
                 if(value == 1 || value == -2) {
                     mPlayer = 1;
                 }

                 else if (value == -1) {
                     mPlayer = 2;
                 }
             }*/

            private set {
                mPlayer = value;
            }

        }

        // TODO: Implement IsCheck and IsCheckMate
        //Checks if Current Player is in check
        public bool IsCheck {
            get {
                CurrentPlayer *= -1;
                int enemyPlayer = CurrentPlayer;
                CurrentPlayer *= -1;

                var enemy = GetThreatenedPositions(enemyPlayer);
                var possMoves = GetPossibleMoves();
                foreach (var pos in enemy) {
                    var cPos = GetPieceAtPosition(pos);
                    if (cPos.PieceType == ChessPieceType.King && cPos.Player == CurrentPlayer) {
                        if (possMoves.Count() > 0)
                            return true;
                    }
                }

                return false;

            }
            
        }

        //Checks if current player is in checkmate
        public bool IsCheckmate {

           get {
                CurrentPlayer *= -1;
                int enemyPlayer = CurrentPlayer;
                CurrentPlayer *= -1;

                var enemy = GetThreatenedPositions(enemyPlayer);
                var possMoves = GetPossibleMoves();
                foreach (var pos in enemy) {
                    var cPos = GetPieceAtPosition(pos);
                    if (cPos.PieceType == ChessPieceType.King && cPos.Player == CurrentPlayer) {
                        if (possMoves.Count() == 0)
                            return true;
                    }
                }

                return false;

            }

        }

        public bool IsStalemate {
            get {
                var moves = GetPossibleMoves();
                if(moves.Count() == 0) {
                    if (!IsCheckmate)
                        return true;
                }

                return false;
            }
                }

        // An auto-property suffices here.
        public IList<IGameMove> MoveHistory {
            get; private set;
        }

        /// <summary>
        /// Returns the piece and player at the given position on the board.
        /// </summary>
        public ChessPiecePosition GetPieceAtPosition(BoardPosition position) {
            var boardVal = mBoard[position.Row, position.Col];
            return new ChessPiecePosition((ChessPieceType)Math.Abs(mBoard[position.Row, position.Col]),
                boardVal > 0 ? 1 : boardVal < 0 ? 2 : 0);
        }


        public void ApplyMove(IGameMove move) {
            // TODO: implement this method. 
            ChessMove chessMove = move as ChessMove;

            int kingRow = chessMove.StartPosition.Row;
            if (CurrentPlayer == 2) {
                kingRow = 0;
            }



            ChessMoveType moveType = chessMove.MoveType;
            BoardPosition startPosition = chessMove.StartPosition;
            BoardPosition endPosition = chessMove.EndPosition;
            

            //Chess Piece Position of start/end position for move
            ChessPiecePosition cStartPosition = GetPieceAtPosition(startPosition);
            ChessPiecePosition cEndPosition = new ChessPiecePosition();
            
            //Piece type at start/end position for move
            ChessPieceType startPieceType = cStartPosition.PieceType;
            ChessPieceType endPieceType = new ChessPieceType();
            chessMove.Piece = cStartPosition;

            if (PositionInBounds(endPosition)) {
                cEndPosition = GetPieceAtPosition(endPosition);
                endPieceType = cEndPosition.PieceType;
            }

            //Player of piece moving
            int startPositionPlayer = cStartPosition.Player;
            sbyte player = (sbyte)startPositionPlayer;
            sbyte piece = (sbyte)startPieceType;
            bool castleKingSide = mAllowCastlingKingSide1;
            bool castleQueenSide = mAllowCastlingQueenSide1;



            //TODO: FIX THIS
            if (player == 2) {
                castleKingSide = mAllowCastlingKingSide2;
                castleQueenSide = mAllowCastlingQueenSide2;
                player = -1;
            }
                        
            switch (moveType) {
                case ChessMoveType.Normal:
                    //Change board values
                    
                    
                    mBoard[endPosition.Row, endPosition.Col] = (sbyte)(player * piece);
                    mBoard[startPosition.Row, startPosition.Col] = 0;

                    //Piece captured
                    chessMove.Captured = cEndPosition;
                   
                    break;

                case ChessMoveType.CastleKingSide:
                    
                        
                    //TODO: Implement castling king side
                    //king moves two right. rook moves two left

                    //set new location of rook from empty to rook 
                    mBoard[kingRow, 5] = mBoard[kingRow, 7];

                    //set rook oringinal position to empty
                    mBoard[kingRow, 7] = 0;

                    //Set new empty position to be King
                    mBoard[kingRow, 6] = mBoard[kingRow, 4];

                    //Set old king position to be empty
                    mBoard[kingRow, 4] = 0;
                        
                    
                    break;

                case ChessMoveType.CastleQueenSide:
                   
                        
                    //TODO: Implement castling queen side
                    //king moves two left
                    //rook moves three right, or one left of the orginal king position

                    //set new location of rook from empty to rook 
                    mBoard[kingRow, 3] = mBoard[kingRow, 0];

                    //set rook oringinal position to empty
                    mBoard[kingRow, 0] = 0;

                    //Set new empty position to be King
                    mBoard[kingRow, 2] = mBoard[kingRow, 4];

                    //Set old king position to be empty
                    mBoard[kingRow, 4] = 0;

                        
                    
                    break;

                case ChessMoveType.PawnTwoSteps:

                    mBoard[endPosition.Row, endPosition.Col] = (sbyte)(player * piece);
                    mBoard[startPosition.Row, startPosition.Col] = 0;

                    //Piece captured
                    chessMove.Captured = cEndPosition;
                    break; 

                case ChessMoveType.EnPassant:
                    mBoard[endPosition.Row, endPosition.Col] = (sbyte)(player * piece);
                    mBoard[startPosition.Row, startPosition.Col] = 0;

                    BoardPosition pos = new BoardPosition(endPosition.Row + player, endPosition.Col);
                    ChessPiecePosition cPos = GetPieceAtPosition(pos);

                    mBoard[pos.Row, pos.Col] = 0;

                    

                    chessMove.Captured = cPos;
                    endPieceType = ChessPieceType.Pawn;

                    break;

                case ChessMoveType.PawnPromote:
                    //TODO: DO THIS
                    //player = (sbyte)player;                    
                    mBoard[startPosition.Row, startPosition.Col] = (sbyte)(endPosition.Col * player);
                    Value += GetPieceValue(GetPieceAtPosition(startPosition).PieceType) * player;
                    Value -= 1 * player;
                    break;
            }


            //Sets castling to false if it is not allowed
            SetCastlingAfterMove(startPieceType);
            SetCastlingAfterMove(endPieceType);

            //Set EnPassant to false if not allowed
            SetEnPassant(startPieceType);            


            //Add move to MoveHistory
            MoveHistory.Add(chessMove);

            //switch players
            if(!CheckForPawnPromote(endPosition, startPieceType))
                CurrentPlayer = CurrentPlayer * -1;

            //Update board value            
            Value += player * GetPieceValue(endPieceType);
            
        }

        private bool CheckForPawnPromote(BoardPosition endPos, ChessPieceType pieceType) {
            if((endPos.Row == 7 || endPos.Row == 0) && pieceType == ChessPieceType.Pawn) {
                //CurrentPlayer *= -1;
                return true;
            }

            return false;
        }

        private void SetCastlingAfterMove(ChessPieceType piece) {
            bool castleKingSide = mAllowCastlingKingSide1;
            bool castleQueenSide = mAllowCastlingQueenSide1;

            if(CurrentPlayer == 2) {
                castleKingSide = mAllowCastlingKingSide2;
                castleQueenSide = mAllowCastlingQueenSide2;
            }

            if (piece == ChessPieceType.RookKing) {
                castleKingSide = false;
            }
            
            if(piece == ChessPieceType.RookQueen) {
                castleQueenSide = false;
            }

            if(piece == ChessPieceType.King) {
                castleKingSide = false;
                castleQueenSide = false;
            }

            if(CurrentPlayer == 1) {
                mAllowCastlingKingSide1 = castleKingSide;
                mAllowCastlingQueenSide1 = castleQueenSide;
            }
            
            else {
                mAllowCastlingKingSide2 = castleKingSide;
                mAllowCastlingQueenSide2 = castleQueenSide;
            }

        }

        private void SetCastlingAfterUndo(ChessPieceType piece) {
            bool castleKingSide = mAllowCastlingKingSide1;
            bool castleQueenSide = mAllowCastlingQueenSide1;

            if (CurrentPlayer == 1) {
                castleKingSide = mAllowCastlingKingSide2;
                castleQueenSide = mAllowCastlingQueenSide2;
            }

            if (piece == ChessPieceType.RookKing) {
                castleKingSide = true;
            }

            if (piece == ChessPieceType.RookQueen) {
                castleQueenSide = true;
            }

            if (piece == ChessPieceType.King) {
                castleKingSide = true;
                castleQueenSide = true;
            }

            if (CurrentPlayer == 2) {
                mAllowCastlingKingSide1 = castleKingSide;
                mAllowCastlingQueenSide1 = castleQueenSide;
            }

            else {
                mAllowCastlingKingSide2 = castleKingSide;
                mAllowCastlingQueenSide2 = castleQueenSide;
            }

        }

        private void SetCastlingThroughCheck(IEnumerable<BoardPosition> list) {
            //mAllowCastlingKingSide = true;
            //mAllowCastlingQueenSide = true;
            BoardPosition[] castlingLongSquares = mCastlingLongSquares1;
            BoardPosition[] castlingShortSquares = mCastlingShortSquares1;
            bool castleKingSide = mAllowCastlingKingSide1;
            bool castleQueenSide = mAllowCastlingQueenSide1;

            if (CurrentPlayer == 2) {
                castlingLongSquares = mCastlingLongSquares2;
                castlingShortSquares = mCastlingShortSquares2;
                castleKingSide = mAllowCastlingKingSide2;
                castleQueenSide = mAllowCastlingQueenSide2;
            }
            


            foreach (var pos in list) {
                foreach(var castlePos in castlingLongSquares) {
                    if (pos.Equals(castlePos)) {
                        castleKingSide = false;
                    }
                }

                foreach(var castlePos in castlingShortSquares) {
                    if (pos.Equals(castlePos)) {
                        castleQueenSide = false;
                    }
                }
            }

            if (CurrentPlayer == 1) {
                mAllowCastlingKingSide1 = castleKingSide;
                mAllowCastlingQueenSide1 = castleQueenSide;
            }

            else {
                mAllowCastlingKingSide2 = castleKingSide;
                mAllowCastlingQueenSide2 = castleQueenSide;
            }
        }

        private void SetEnPassant(ChessPieceType piece) {
            
        }


        public IEnumerable<IGameMove> GetPossibleMoves() {

            ChessMove moveTest = new ChessMove(new BoardPosition(7, 4), new BoardPosition(7, 6));  

            List<ChessMove> possMoves = new List<ChessMove>();
            IEnumerable<BoardPosition> moves = new List<BoardPosition>();
            List<ChessMove> list = new List<ChessMove>();
            IEnumerable<BoardPosition> threatenedPositions = new List<BoardPosition>();           
            
            CurrentPlayer *= -1;
            int enemy = CurrentPlayer;
            CurrentPlayer *= -1;
            
            threatenedPositions = GetThreatenedPositions(enemy);
            SetCastlingThroughCheck(threatenedPositions);

            bool castleKingSide = mAllowCastlingKingSide1;
            bool castleQueenSide = mAllowCastlingQueenSide1;

            //TODO: implement as method
            int kingRow = 7;
            if (CurrentPlayer == 2) {
                castleKingSide = mAllowCastlingKingSide2;
                castleQueenSide = mAllowCastlingQueenSide2;
                kingRow = 0;
            }

            //Hardcoded position of king
            BoardPosition kingPos = new BoardPosition(kingRow, 4);
            ChessPiecePosition cPos = new ChessPiecePosition();
            BoardPosition pos = new BoardPosition();
            ChessPieceType pieceType;
            int player;

            for (int row = 0; row < BOARD_SIZE; row++) {
                for (int col = 0; col < BOARD_SIZE; col++) {
                    pos.Row = row;
                    pos.Col = col;
                    

                    //returns 1 and 2 for players, NOT 1 and -1. Need to adjust logic to account for this
                    player = GetPlayerAtPosition(pos);

                    //GetPlayerAtPosition(pos)
                    if (CurrentPlayer == player) {

                        cPos = GetPieceAtPosition(pos);
                        pieceType = cPos.PieceType;
                        
                        switch (pieceType) {
                                
                            case ChessPieceType.Pawn:
                                //TODO: Pawn Promote
                                if(CurrentPlayer == 1 && row == 0 || CurrentPlayer == 2 && row == 7) {
                                    List<ChessMove> pawnPromote = new List<ChessMove>();
                                    //Rook
                                    pawnPromote.Add(new ChessMove(pos, new BoardPosition(-1,8), ChessMoveType.PawnPromote));
                                    //Queen
                                    pawnPromote.Add(new ChessMove(pos, new BoardPosition(-1, 6), ChessMoveType.PawnPromote));
                                    //Bishop
                                    pawnPromote.Add(new ChessMove(pos, new BoardPosition(-1, 5), ChessMoveType.PawnPromote));
                                    //Knight
                                    pawnPromote.Add(new ChessMove(pos, new BoardPosition(-1, 4), ChessMoveType.PawnPromote));
                                    return pawnPromote;
                                }
                                


                                moves = GetThreatenedPositionsPawn(pos, CurrentPlayer);
                                List<BoardPosition> pawnMoves = moves as List<BoardPosition>;
                                
                                
                                list = PossibleMovesQueryPawn(pawnMoves, pos);
                                
                                possMoves.AddRange(list);

                                break;

                            case ChessPieceType.Knight:
                                moves = GetThreatenedPositionsKnight(pos, CurrentPlayer);
                                list = PossibleMovesQuery(moves, pos, CurrentPlayer);
                                possMoves.AddRange(list);

                                break;

                            case ChessPieceType.King:
                                //TODO: 
                                moves = GetThreatenedPositionsKing(pos, CurrentPlayer);
                                list = PossibleMovesQuery(moves, pos, CurrentPlayer);

                                foreach(var tMove in threatenedPositions) {
                                    ChessMove kingMove = new ChessMove(pos, tMove);
                                    list.Remove(kingMove);
                                }

                                /*filter threatenedPositions list so King cant move to a threatened position, puting him in check
                                var newList = from move in list
                                              where !(from tMove in threatenedPositions
                                                      select tMove)
                                                    .Contains(move)
                                              select move;*/

                                possMoves.AddRange(list);

                                break;

                            case ChessPieceType.RookKing:
                                moves = GetThreatenedPositionsBQR(pos, CurrentPlayer, mRookDirections) as List<BoardPosition>;
                                list = PossibleMovesQuery(moves, pos, CurrentPlayer);
                               
                                if(castleKingSide) {
                                    if(moves.Contains(kingPos)) {
                                        list.Add(new ChessMove(kingPos, new BoardPosition(pos.Row, pos.Col - 1), ChessMoveType.CastleKingSide));
                                    }
                                }



                                possMoves.AddRange(list);

                                break;

                            case ChessPieceType.RookQueen:
                                moves = GetThreatenedPositionsBQR(pos, CurrentPlayer, mRookDirections) as List<BoardPosition>;
                                list = PossibleMovesQuery(moves, pos, CurrentPlayer);

                                if (castleQueenSide) {
                                    if (moves.Contains(kingPos)) {
                                        list.Add(new ChessMove(kingPos, new BoardPosition(pos.Row, pos.Col + 2), ChessMoveType.CastleQueenSide));
                                    }
                                }

                                possMoves.AddRange(list);

                                break;

                            case ChessPieceType.RookPawn:
                                moves = GetThreatenedPositionsBQR(pos, CurrentPlayer, mRookDirections) as List<BoardPosition>;
                                list = PossibleMovesQuery(moves, pos, CurrentPlayer);
                                possMoves.AddRange(list);

                                break;


                            case ChessPieceType.Bishop:
                                moves = GetThreatenedPositionsBQR(pos, CurrentPlayer, mBishopDirections) as List<BoardPosition>;
                                list = PossibleMovesQuery(moves, pos, CurrentPlayer);
                                possMoves.AddRange(list);

                                break;

                            case ChessPieceType.Queen:
                                moves = GetThreatenedPositionsBQR(pos, CurrentPlayer, mQueenDirections) as List<BoardPosition>;
                                list = PossibleMovesQuery(moves, pos, CurrentPlayer);

                                possMoves.AddRange(list);

                                break;


                        }

                    }
                }


                //return possMoves;
            }
          
            var retList =  possMoves.Distinct().ToList();            
            retList = PossibleMovesWhenInCheck(retList, threatenedPositions);           

            return retList;
        }

        private List<ChessMove> PossibleMovesWhenInCheck(List<ChessMove> moves, IEnumerable<BoardPosition> enemy) {            
            List<ChessMove> checkMoves = new List<ChessMove>();

            if (Check()) {                
                foreach (var move in moves) {

                    ApplyMove(move);
                    if (!CheckForPawnPromote(move.EndPosition, GetPieceAtPosition(move.EndPosition).PieceType))
                        CurrentPlayer *= -1;                                       
                    if (!Check())
                        if (move.MoveType != ChessMoveType.CastleKingSide && move.MoveType != ChessMoveType.CastleQueenSide)
                            checkMoves.Add(move);
                    CurrentPlayer *= -1;
                    if (CheckForPawnPromote(move.EndPosition, GetPieceAtPosition(move.EndPosition).PieceType))
                        CurrentPlayer *= -1;
                    UndoLastMove();                    

                }

                return checkMoves;
            }

            else if (!Check()) {
                foreach (var move in moves) {
                    var pieceType = GetPieceAtPosition(move.StartPosition).PieceType;

                    if (move.MoveType == ChessMoveType.Normal) {
                        //CheckForPawnPromote(move.EndPosition, pieceType);
                        ApplyMove(move);                        
                        CurrentPlayer *= -1;
                        if (!Check())
                            checkMoves.Add(move);                        
                        CurrentPlayer *= -1;
                        UndoLastMove();
                    }
                    else
                        checkMoves.Add(move);
                }

                return checkMoves;
            }


            return moves;           
        }

        private bool Check() {
            CurrentPlayer *= -1;
            int enemyPlayer = CurrentPlayer;
            CurrentPlayer *= -1;

            var enemy = GetThreatenedPositions(enemyPlayer);
            bool check = false;
            foreach (var pos in enemy) {
                var cPos = GetPieceAtPosition(pos);
                if (cPos.PieceType == ChessPieceType.King && cPos.Player == CurrentPlayer) {
                    check = true;
                }
            }

            return check;
        }

        private List<ChessMove> PossibleMovesQuery(IEnumerable<BoardPosition> threatenedMoves, BoardPosition startPos, int currentPlayer) {

            var list = new List<ChessMove>();
            foreach (var tMove in threatenedMoves) {
                var movePos = GetPieceAtPosition(tMove);                
                if (movePos.Player != CurrentPlayer) {                   
                    ChessMove cMove = new ChessMove(startPos, tMove);
                    list.Add(cMove);
                }
            }

            /* var list = from tMove in threatenedMoves                       
                        let movePos = GetPieceAtPosition(tMove)                             
                        where movePos.Player != CurrentPlayer
                        select tMove;
                        */
            return list;
        }

        private List<ChessMove> PossibleMovesQueryPawn(List<BoardPosition> threatenedMoves, BoardPosition startPos) {
            var list = new List<ChessMove>();

            //TODO: FIX CURRENTPLAYER LOGIC
            int player = CurrentPlayer;
            int pawnRow = 6;            
            if (CurrentPlayer == 2) {
                pawnRow = 1;
                player = -1;
            }

            CurrentPlayer *= -1;
            int enemyPlayer = CurrentPlayer;
            CurrentPlayer *= -1;

           


            BoardPosition pawnPos = new BoardPosition(startPos.Row - player, startPos.Col);

            if (PositionInBounds(pawnPos) && PositionIsEmpty(pawnPos)) {
                list.Add(new ChessMove(startPos, pawnPos));
                pawnPos.Row += -player;

                if (startPos.Row == pawnRow && PositionInBounds(pawnPos) && PositionIsEmpty(pawnPos)) {
                    list.Add(new ChessMove(startPos, pawnPos, ChessMoveType.PawnTwoSteps));
                }
            }

            
            foreach (var tMove in threatenedMoves) {
                var movePos = GetPieceAtPosition(tMove);

                //if move doesn't have friendly piece on it
                if (movePos.Player != CurrentPlayer) {   
                    //if pawn move is to the left or right column where they threaten                                   
                    if (startPos.Col != tMove.Col) {
                        //if the threatened position actually has an enemy peice
                        if(movePos.Player == enemyPlayer) {
                            ChessMove cMove = new ChessMove(startPos, tMove);
                            list.Add(cMove);
                        }

                        if (MoveHistory.Count - 1 >= 0) {
                            ChessMove lastMove = MoveHistory.ElementAt(MoveHistory.Count - 1) as ChessMove;
                            if (lastMove.MoveType == ChessMoveType.PawnTwoSteps) {
                                ChessMove currentMove = new ChessMove(startPos, tMove, ChessMoveType.EnPassant);
                                BoardPosition endPos = new BoardPosition(currentMove.EndPosition.Row, currentMove.EndPosition.Col);
                                endPos.Row += player;
                                if (lastMove.EndPosition.Equals(endPos)) {
                                    list.Add(currentMove);
                                }
                            }
                        }

                       
                            
                    }

                    //Pawn is just moving forward one and staying in the same column
                    /*else {
                        ChessMove cMove = new ChessMove(startPos, tMove);
                        list.Add(cMove);
                    }*/
                    
                    
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a sequence of all positions on the board that are threatened by the given player. A king
        /// may not move to a square threatened by the opponent.
        /// </summary>
        public IEnumerable<BoardPosition> GetThreatenedPositions(int byPlayer) {
            // TODO: implement this method. Make sure to account for "special" moves.
            List<BoardPosition> directions = new List<BoardPosition>();
            List<IEnumerable<BoardPosition>> threatenedPositions = new List<IEnumerable<BoardPosition>>();
            List<BoardPosition> tPos = new List<BoardPosition>();

            
            ChessPiecePosition cPos = new ChessPiecePosition();
            BoardPosition pos = new BoardPosition();          
            ChessPieceType pieceType;            
            
            for(int row = 0; row < BOARD_SIZE; row++) {
                for (int col = 0; col < BOARD_SIZE; col++) {
                    pos.Row = row;
                    pos.Col = col;
                    
                    if(byPlayer == GetPlayerAtPosition(pos)) {

                        cPos = GetPieceAtPosition(pos);
                        pieceType = cPos.PieceType;
                        
                        switch (pieceType) {
                            case ChessPieceType.Pawn:
                                threatenedPositions.Add(GetThreatenedPositionsPawn(pos, byPlayer));
                                break;

                            case ChessPieceType.Bishop:
                                threatenedPositions.Add(GetThreatenedPositionsBQR(pos, byPlayer, mBishopDirections));                               
                                break;

                            case ChessPieceType.RookKing:
                                IEnumerable<BoardPosition> list = (GetThreatenedPositionsBQR(pos, byPlayer, mRookDirections));
                                threatenedPositions.Add(list);
                                
                                break;

                            case ChessPieceType.RookQueen:                               
                                threatenedPositions.Add(GetThreatenedPositionsBQR(pos, byPlayer, mRookDirections));
                                break;

                            case ChessPieceType.RookPawn:                                
                                threatenedPositions.Add(GetThreatenedPositionsBQR(pos, byPlayer, mRookDirections));
                                break;

                            case ChessPieceType.Queen:                                
                                threatenedPositions.Add(GetThreatenedPositionsBQR(pos, byPlayer, mQueenDirections));
                                break;

                            case ChessPieceType.Knight:                                                               
                                threatenedPositions.Add(GetThreatenedPositionsKnight(pos, byPlayer));
                                break;

                            case ChessPieceType.King:
                                threatenedPositions.Add(GetThreatenedPositionsKing(pos, byPlayer));
                                break;

                            default:
                                break;
                        }
                    }
                        
                    
                }
            }
            
			foreach (IEnumerable<BoardPosition> list in threatenedPositions) {
                foreach(BoardPosition listPosition in list) {
                    tPos.Add(listPosition);
                }
            }

            return tPos;
		}
        
        private IEnumerable<BoardPosition> GetThreatenedPositionsKnight(BoardPosition pos, int byPlayer) {
            List<BoardPosition> threatenedPos = new List<BoardPosition>();
            BoardPosition tPos = new BoardPosition();
            List<BoardPosition> directions = new List<BoardPosition>();
            int enemyPlayer = 0;
            int player = GetPieceAtPosition(pos).Player;

            directions.Add(new BoardPosition(2, 1));
            directions.Add(new BoardPosition(2, -1));
            directions.Add(new BoardPosition(-2, 1));
            directions.Add(new BoardPosition(-2, -1));
            directions.Add(new BoardPosition(1, 2));
            directions.Add(new BoardPosition(-1, 2));
            directions.Add(new BoardPosition(1, -2));
            directions.Add(new BoardPosition(-1, -2));

            foreach (BoardPosition direction in directions) {

                //TODO: FIX
                //tPos.Translate((pos.Row + direction.Row), (pos.Col + direction.Col));
                tPos.Row = pos.Row + direction.Row;
                tPos.Col = pos.Col + direction.Col;
                
                if(PositionInBounds(tPos))
                    enemyPlayer = GetPieceAtPosition(tPos).Player;

                if (PositionInBounds(tPos) && enemyPlayer != player)
                    threatenedPos.Add(tPos);

            }

            return threatenedPos;
        }



        private IEnumerable<BoardPosition> GetThreatenedPositionsPawn(BoardPosition pos, int byPlayer) {
            List<BoardPosition> threatenedPos = new List<BoardPosition>();
            BoardPosition tPos = new BoardPosition();
            //when byPlayer is 2, logic is wrong. I assumed player was -1 and 1, but player is 1 and 2. 
            //TODO: FIX
            if(byPlayer == 2) {
                byPlayer = -1;
            }

            tPos.Row = pos.Row - byPlayer;
            tPos.Col = pos.Col + 1;

            if(PositionInBounds(tPos))
                threatenedPos.Add(tPos);

            tPos.Row = pos.Row - byPlayer;
            tPos.Col = pos.Col - 1;

            if (PositionInBounds(tPos))
                threatenedPos.Add(tPos);

            return threatenedPos;
        }



        private IEnumerable<BoardPosition> GetThreatenedPositionsBQR(BoardPosition pos, int byPlayer, BoardPosition[] directions) {            
            List<BoardPosition> threatenedPos = new List<BoardPosition>();
            BoardPosition tPos = new BoardPosition(pos.Row, pos.Col);
            int direcIndex = 0;

            while (direcIndex < directions.Length) {

                //Use directions array check all four directions in one loop
                //tPos.Translate(directions[direcIndex].Row, directions[direcIndex].Col);               
                tPos.Row += directions[direcIndex].Row;
                tPos.Col += directions[direcIndex].Col;



                //if in bounds and empty, add and keep going
                if (PositionInBounds(tPos) && PositionIsEmpty(tPos))
                    threatenedPos.Add(tPos);


                //if in bounds but space isn't empty, add and switch directions
                else if (PositionInBounds(tPos) && !PositionIsEmpty(tPos)) {
                    threatenedPos.Add(tPos);
                    tPos = pos;                    
                    direcIndex++;
                }

                //if position isnt in bounds, try another direction
                else if (!PositionInBounds(tPos)) {
                    tPos = pos;
                    direcIndex++;
                }

               // else
                    //tPos.Row = pos.Row;
                    //tPos.Col = pos.Col;
                    //direcIndex++;

            }

            return threatenedPos;
        }

        private IEnumerable<BoardPosition> GetThreatenedPositionsKing(BoardPosition pos, int byPlayer) {
            
            List<BoardPosition> list = new List<BoardPosition>();
            List<BoardPosition> movesToRemove = new List<BoardPosition>();
            list.Add(pos.Translate(-1, 0));
            list.Add(pos.Translate(-1, 1));
            list.Add(pos.Translate(-1, -1));
            list.Add(pos.Translate(0, 1));
            list.Add(pos.Translate(0, -1));
            list.Add(pos.Translate(1, 0));
            list.Add(pos.Translate(1, -1));
            list.Add(pos.Translate(1, 1));

            foreach (var move in list) {
                if(!PositionInBounds(move)) {
                    movesToRemove.Add(move);
                }
            }

            foreach (var move in movesToRemove) {
                list.Remove(move);
            }

            return list;
        }


        public void UndoLastMove() {
            // TODO: implement this method. Make sure to account for "special" moves.
            var lastMove = MoveHistory[MoveHistory.Count - 1] as ChessMove;
            var capturedPiece = lastMove.Captured;
            var startPos = lastMove.StartPosition;
            var endPos = lastMove.EndPosition;
            var piece = lastMove.Piece;
            if(PositionInBounds(endPos)) {
                piece = GetPieceAtPosition(endPos);
            }
            var pieceType = piece.PieceType;
            var player = piece.Player;
            var moveType = lastMove.MoveType;
            
            if (player == 2)
                player = -1;

            var enemyPlayer = player * -1;


            switch (moveType) {
                case ChessMoveType.Normal:
                    
                    mBoard[startPos.Row, startPos.Col] = (sbyte)((sbyte)pieceType * player);
                    mBoard[endPos.Row, endPos.Col] = (sbyte)((sbyte)capturedPiece.PieceType * enemyPlayer);
                    break;

                case ChessMoveType.CastleQueenSide:
                    mBoard[startPos.Row, 2] = 0;
                    mBoard[startPos.Row, 3] = 0;
                    mBoard[startPos.Row, 0] = (sbyte)((sbyte)ChessPieceType.RookQueen * player);
                    mBoard[startPos.Row, 4] = (sbyte)((sbyte)ChessPieceType.King * player);
                    if (player == 1) {
                        mAllowCastlingQueenSide1 = true;


                        mAllowCastlingQueenSide2 = true;
                        //mAllowCastlingKingSide1 = true;
                    }

                    else {
                        mAllowCastlingQueenSide2 = true;


                        mAllowCastlingQueenSide1 = true;
                        //mAllowCastlingKingSide2 = true;

                    }                
                    
                    break;

                case ChessMoveType.CastleKingSide:
                    mBoard[startPos.Row, 5] = 0;
                    mBoard[startPos.Row, 6] = 0;
                    mBoard[startPos.Row, 7] = (sbyte)((sbyte)ChessPieceType.RookKing * player);
                    mBoard[startPos.Row, 4] = (sbyte)((sbyte)ChessPieceType.King * player);
                    if (player == 1) {
                        mAllowCastlingKingSide1 = true;

                        
                        mAllowCastlingKingSide2 = true;
                        //mAllowCastlingQueenSide1 = true;
                    }

                    else {
                        mAllowCastlingKingSide2 = true;


                        mAllowCastlingKingSide1 = true;
                        //mAllowCastlingQueenSide2 = true;
                    }
                    break;

                case ChessMoveType.EnPassant:
                    mBoard[endPos.Row, endPos.Col] = 0;
                    mBoard[startPos.Row, startPos.Col] = (sbyte)(player * (int)pieceType);

                    BoardPosition pos = new BoardPosition(endPos.Row + player, endPos.Col);  
                    mBoard[pos.Row, pos.Col] = (sbyte)((int)capturedPiece.PieceType * enemyPlayer);

                   
                    break;

                case ChessMoveType.PawnPromote:
                    player = -1;
                    if (CurrentPlayer == 2)
                        player = 1;
                    Value -= GetPieceValue(GetPieceAtPosition(startPos).PieceType) * player;
                    Value += 1 * player;
                    mBoard[startPos.Row, startPos.Col] = (sbyte)((int)ChessPieceType.Pawn * player);       
                    break;

                case ChessMoveType.PawnTwoSteps:
                    mBoard[endPos.Row, endPos.Col] = 0;
                    mBoard[startPos.Row, startPos.Col] = (sbyte)((int)ChessPieceType.Pawn * player);
                    break;                


            }

            SetCastlingAfterUndo(pieceType);
            SetCastlingAfterUndo(capturedPiece.PieceType);

            //switch players
            if(!CheckForPawnPromote(endPos, pieceType))
                CurrentPlayer = CurrentPlayer * -1;

            //Update Value
            Value += enemyPlayer * GetPieceValue(capturedPiece.PieceType);

            //Remove move from list
            MoveHistory.RemoveAt(MoveHistory.Count - 1);

            
		}

		
		/// <summary>
		/// Returns true if the given position on the board is empty.
		/// </summary>
		/// <remarks>returns false if the position is not in bounds</remarks>
		public bool PositionIsEmpty(BoardPosition pos) {
            // TODO: implement this method, using GetGetPieceAtPosition for convenience.
            return GetPieceAtPosition(pos).Player == 0;            			
		}

		/// <summary>
		/// Returns true if the given position contains a piece that is the enemy of the given player.
		/// </summary>
		/// <remarks>returns false if the position is not in bounds</remarks>
		public bool PositionIsEnemy(BoardPosition pos, int player) {
            // TODO: implement this method.
            return GetPieceAtPosition(pos).Player == -player;			
		}

		/// <summary>
		/// Returns true if the given position is in the bounds of the board.
		/// </summary>
		public static bool PositionInBounds(BoardPosition pos) {
            // TODO: implement this method. 
            return (pos.Row >= 0 && pos.Row < 8 && pos.Col >= 0 && pos.Col < 8);			
		}

		/// <summary>
		/// Returns which player has a piece at the given board position, or 0 if it is empty.
		/// </summary>
		public int GetPlayerAtPosition(BoardPosition pos) {
            // TODO: implement this method, returning 1, 2, or 0.
            return GetPieceAtPosition(pos).Player;			
		}

        public IEnumerable<BoardPosition> GetPositionsOfPiece(ChessPieceType piece, int player) {
            List<BoardPosition> posOfPiece = new List<BoardPosition>();

            for (sbyte row = 0; row < BOARD_SIZE; row++) {
                for (sbyte col = 0; col < BOARD_SIZE; col++) {
                    if (mBoard[row, col] == (sbyte)piece) {                        
                        if (mBoard[row, col] / (sbyte)piece == player) {
                            posOfPiece.Add(new BoardPosition(row, col));
                        }
                    }
                }
            }

            return posOfPiece;

        }

        /// <summary>
        /// Gets the value weight for a piece of the given type.
        /// </summary>
        /*
		 * VALUES:
		 * Pawn: 1
		 * Knight: 3
		 * Bishop: 3
		 * Rook: 5
		 * Queen: 9
		 * King: infinity (maximum integer value)
		 */
        public int GetPieceValue(ChessPieceType pieceType) {
            // TODO: implement this method.
            // Come back to verify this is correct

            switch(pieceType) {
                case ChessPieceType.Pawn:
                    return 1;

                case ChessPieceType.Knight:
                    return 3;

                case ChessPieceType.Bishop:
                    return 3;

                case ChessPieceType.RookKing:
                    return 5;

                case ChessPieceType.RookQueen:
                    return 5;

                case ChessPieceType.RookPawn:
                    return 5;

                case ChessPieceType.Queen:
                    return 9;

                case ChessPieceType.King:
                    return int.MaxValue;

                default:
                    return 0;


            }          
			
		}


		/// <summary>
		/// Manually places the given piece at the given position.
		/// </summary>
		// This is used in the constructor
		private void SetPosition(BoardPosition position, ChessPiecePosition piece) {
			mBoard[position.Row, position.Col] = (sbyte)((int)piece.PieceType * (piece.Player == 2 ? -1 :
				piece.Player));
		}
	}
}
