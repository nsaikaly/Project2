using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Cecs475.BoardGames;
using Cecs475.BoardGames.View;
using System;

namespace Cecs475.BoardGames.Chess.View {
    public class ChessSquare : INotifyPropertyChanged {
        public ChessViewModel ViewModel { get; set; }
        private int mPlayer;
        public int CurrentPlayer {
            get { return mPlayer; }
            set {
                if (value != mPlayer) {
                    mPlayer = value;
                    OnPropertyChanged(nameof(CurrentPlayer));
                }
            }
        }

        private bool mIsHovered;
        public bool IsHovered {
            get { return mIsHovered; }
            set {
                if (value != mIsHovered) {
                    mIsHovered = value;
                    OnPropertyChanged(nameof(IsHovered));
                }
            }
        }

        private bool mIsSelected;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set
            {
                if (value != mIsSelected)
                {
                    mIsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        private ChessPiecePosition mPiece;
        public ChessPiecePosition Piece
        {
            get { return mPiece; }
            set
            {
                if (value.Player == mPiece.Player && value.PieceType == mPiece.PieceType && value.PieceType != ChessPieceType.King)
                    return;
                mPiece = value;
                OnPropertyChanged("Piece");
            }
        }


        public BoardPosition Position {
            get; set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class ChessViewModel : INotifyPropertyChanged, IGameViewModel {
        private ChessBoard mBoard;
        private ObservableCollection<ChessSquare> mSquares;
        private bool mPawnPromotion;
        private ObservableCollection<ChessSquare> mPromotionMoves;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler GameFinished;

        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ChessViewModel()
        {
            mBoard = new ChessBoard();
            mSquares = new ObservableCollection<ChessSquare>(
                from pos in (
                    from r in Enumerable.Range(0, 8)
                    from c in Enumerable.Range(0, 8)
                    select new BoardPosition(r, c)
                )
                select new ChessSquare()
                {
                    Position = pos,
                    CurrentPlayer = mBoard.GetPieceAtPosition(pos).Player,
                    Piece = mBoard.GetPieceAtPosition(pos),
                    //ViewModel = ChessViewModel
                }
            );

            PossibleMoves = new HashSet<ChessMove>(
                from ChessMove m in mBoard.GetPossibleMoves()
                select m
            );
        }

        public void UndoMove()
        {
            if(CanUndo)
            {
                //var move = mBoard.MoveHistory.Last();
                mBoard.UndoLastMove();

                PossibleMoves = new HashSet<ChessMove>(
                    from ChessMove m in mBoard.GetPossibleMoves()
                    select m
                );

                var newSquares =
                    from r in Enumerable.Range(0, 8)
                    from c in Enumerable.Range(0, 8)
                    select new BoardPosition(r, c);
                int i = 0;
                foreach (var pos in newSquares)
                {
                    mSquares[i].CurrentPlayer = mBoard.GetPieceAtPosition(pos).Player;
                    mSquares[i].Piece = mBoard.GetPieceAtPosition(pos);
                    mSquares[i].IsSelected = false;
                    i++;
                }

                OnPropertyChanged(nameof(BoardValue));
                OnPropertyChanged(nameof(CurrentPlayer));
                OnPropertyChanged(nameof(CanUndo));

            }
        }

        public void ApplyMove(ChessMove cMove)
        {
            if (mBoard.IsCheckmate)
            {
                GameFinished?.Invoke(this, new EventArgs());
            }
            else
            {
                //Need to account for pawn promotion
                var possMoves = mBoard.GetPossibleMoves() as IEnumerable<ChessMove>;
                foreach (var move in possMoves)
                {
                    if (move.Equals(cMove))
                    {
                        mBoard.ApplyMove(move);
                        break;
                    }                 
                }

                //ChessMove promotionMove = (ChessMove)mBoard.MoveHistory.Last();

                //if (promotionMove.Piece.PieceType == ChessPieceType.Pawn && (promotionMove.EndPosition.Row == 0 || promotionMove.EndPosition.Row == 7))
                //{
                //    IsPawnPromotion = true;
                //    mPromotionMoves = new ObservableCollection<ChessSquare>(
                //    from pos in (
                //        from r in PossibleMoves
                //        select new BoardPosition(r.StartPosition.Row, r.StartPosition.Col)
                //    )
                //    select new ChessSquare()
                //    {
                //        Position = pos,
                //        CurrentPlayer = mBoard.GetPieceAtPosition(pos).Player,
                //        Piece = mBoard.GetPieceAtPosition(pos),
                //            //ViewModel = ChessViewModel
                //        }
                //    );

                //}

                PossibleMoves = new HashSet<ChessMove>(
                    from ChessMove m in mBoard.GetPossibleMoves()
                    select m
                );
                var newSquares =
                    from r in Enumerable.Range(0, 8)
                    from c in Enumerable.Range(0, 8)
                    select new BoardPosition(r, c);
                int i = 0;
                foreach (var pos in newSquares)
                {
                    mSquares[i].CurrentPlayer = mBoard.GetPieceAtPosition(pos).Player;
                    mSquares[i].Piece = mBoard.GetPieceAtPosition(pos);
                    mSquares[i].IsSelected = false;
                    i++;
                }
            }
           
            OnPropertyChanged(nameof(BoardValue));
            OnPropertyChanged(nameof(CurrentPlayer));
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(IsPawnPromotion));
        }

        //public ChessViewModel()
        //{
        //    mBoard = new ChessBoard();
        //    mPromotionMoves = new ObservableCollection<ChessSquare>();
        //    PossibleMoves = new HashSet<BoardPosition>(mBoard.GetPossibleMoves().Cast<ChessMove>().Select<ChessMove, BoardPosition>((Func<ChessMove, BoardPosition>)(chessMove_0 => chessMove_0.StartPosition)));
        //    IEnumerable<int> source = Enumerable.Range(0, 8);
        //    Func<int, IEnumerable<int>> func = (Func<int, IEnumerable<int>>)(int_0 => Enumerable.Range(0, 8));
        //    Func<int, IEnumerable<int>> collectionSelector;
        //    mSquares = new ObservableCollection<ChessSquare>(source.SelectMany<int, int, BoardPosition>(collectionSelector, (Func<int, int, BoardPosition>)((int_0, int_1) => new BoardPosition(int_0, int_1))).Select<BoardPosition, ChessSquare>((Func<BoardPosition, ChessSquare>)(boardPosition_0 =>
        //    {
        //        ChessSquare chessSquare = new ChessSquare();
        //        chessSquare.Position = boardPosition_0;
        //        ChessPiecePosition pieceAtPosition = mBoard.GetPieceAtPosition(boardPosition_0);
        //        chessSquare.Piece = pieceAtPosition;
        //        chessSquare.ViewModel = this;
        //        return chessSquare;
        //    })));
        //}

        //public void UndoMove()
        //{
        //    if (mBoard.MoveHistory.Count > 0 && ((ChessMove)mBoard.MoveHistory.Last<IGameMove>()).MoveType == ChessMoveType.PawnPromote)
        //        mBoard.UndoLastMove();
        //    mBoard.UndoLastMove();
        //    this.method_0();
        //}

        //public void ApplyMove(BoardPosition startPosition, BoardPosition endPosition)
        //{
        //    foreach (ChessMove possibleMove in mBoard.GetPossibleMoves())
        //    {
        //        if (possibleMove.StartPosition.Equals(startPosition) && possibleMove.EndPosition.Equals(endPosition))
        //        {
        //            mBoard.ApplyMove((IGameMove)possibleMove);
        //            if (possibleMove.Piece.PieceType == ChessPieceType.Pawn && (possibleMove.EndPosition.Row == 0 || possibleMove.EndPosition.Row == 7))
        //            {
        //                this.IsPawnPromotion = true;
        //                this.PromotionMoves.Clear();
        //                mBoard.GetPossibleMoves().Cast<ChessMove>().OrderBy<ChessMove, int>((Func<ChessMove, int>)(chessMove_0 => mBoard.GetPieceValue((ChessPieceType)chessMove_0.EndPosition.Col))).Select<ChessMove, ChessSquare>((Func<ChessMove, ChessSquare>)(chessMove_0 => new ChessSquare()
        //                {
        //                    Piece = new ChessPiecePosition((ChessPieceType)chessMove_0.EndPosition.Col, mBoard.CurrentPlayer),
        //                    Position = chessMove_0.StartPosition
        //                })).ToList<ChessSquare>().ForEach((Action<ChessSquare>)(chessSquare_0 => this.PromotionMoves.Add(chessSquare_0)));
        //                break;
        //            }
        //            this.IsPawnPromotion = false;
        //            this.PromotionMoves.Clear();
        //            break;
        //        }
        //    }
        //    this.method_0();
        //}

        //private void method_0()
        //{
        //    IEnumerable<int> source = Enumerable.Range(0, 8);
        //    Func<int, IEnumerable<int>> func = (Func<int, IEnumerable<int>>)(int_0 => Enumerable.Range(0, 8));
        //    Func<int, IEnumerable<int>> collectionSelector;
        //    IEnumerable<BoardPosition> boardPositions = source.SelectMany<int, int, BoardPosition>(collectionSelector, (Func<int, int, BoardPosition>)((int_0, int_1) => new BoardPosition(int_0, int_1)));
        //    int index = 0;
        //    foreach (BoardPosition position in boardPositions)
        //    {
        //        mSquares[index].Piece = mBoard.GetPieceAtPosition(position);
        //        ++index;
        //    }
        //    PossibleMoves = new HashSet<BoardPosition>(mBoard.GetPossibleMoves().Cast<ChessMove>().Select<ChessMove, BoardPosition>((Func<ChessMove, BoardPosition>)(chessMove_0 => chessMove_0.StartPosition)));
        //    this.method_1("BoardValue");
        //    this.method_1("CurrentPlayer");
        //    this.method_1("CanUndo");
        //}

        //public HashSet<BoardPosition> GetPossibleMovesAtPosition(BoardPosition position)
        //{
        //    // ISSUE: object of a compiler-generated type is created
        //    // ISSUE: reference to a compiler-generated method
        //    return new HashSet<BoardPosition>(((IEnumerable<ChessMove>)mBoard.GetPossibleMoves()).Where<ChessMove>(new Func<ChessMove, bool>(new ChessViewModel.Class18()
        //    {
        //        boardPosition_0 = position
        //    }.method_0)).Select<ChessMove, BoardPosition>((Func<ChessMove, BoardPosition>)(chessMove_0 => chessMove_0.EndPosition)));
        //}

        //private void method_1(string string_0)
        //{
        //    // ISSUE: reference to a compiler-generated field
        //    PropertyChangedEventHandler changedEventHandler0 = PropertyChanged;
        //    if (changedEventHandler0 == null)
        //        return;
        //    PropertyChangedEventArgs e = new PropertyChangedEventArgs(string_0);
        //    changedEventHandler0((object)this, e);
        //}
        public ObservableCollection<ChessSquare> Squares {
            get { return mSquares; }
        }

        public ObservableCollection<ChessSquare> PromotionMoves
        {
            get {
                return mPromotionMoves;
            }

            set {
                mPromotionMoves = value;
                OnPropertyChanged("PromotionMoves");
            }
        }

        public HashSet<ChessMove> PossibleMoves {
            get; private set;
        }

        public int BoardValue { get { return mBoard.Value; } }

        public int CurrentPlayer { get { return mBoard.CurrentPlayer; } }

        public bool IsCheck { get { return mBoard.IsCheck; } }

        public bool IsPawnPromotion {
            get {
                return mPawnPromotion;
            }
            set {
                mPawnPromotion = value;
                OnPropertyChanged("IsPawnPromotion");
            }
        }

        public bool CanUndo {
            get {
                return mBoard.MoveHistory.Count > 0;
            }
        }
    }
}
