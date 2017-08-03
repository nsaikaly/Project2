using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Cecs475.BoardGames;
using Cecs475.BoardGames.View;
using Cecs475.BoardGames.ComputerOpponent;
using System;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.Chess.View {
    public class ChessSquare : INotifyPropertyChanged {


        private ChessPiecePosition mPiece;
        private int mPlayer;
        private bool mIsHovered;
        private bool mIsSelected;

        public ChessViewModel ViewModel { get; set; }
        
        public int CurrentPlayer {
            get { return mPlayer; }
            set {
                if (value != mPlayer) {
                    mPlayer = value;
                    OnPropertyChanged(nameof(CurrentPlayer));
                }
            }
        }

        public bool IsHovered {
            get { return mIsHovered; }
            set {
                if (value != mIsHovered) {
                    mIsHovered = value;
                    OnPropertyChanged(nameof(IsHovered));
                }
            }
        }

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
        private const int MAX_AI_DEPTH = 3;
        private ChessBoard mBoard;
        private ObservableCollection<ChessSquare> mSquares;
        private bool mPawnPromotion;
        private ObservableCollection<ChessSquare> mPromotionMoves;
        private IGameAi mGameAi = new MinimaxAi(MAX_AI_DEPTH);

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
                select new ChessSquare() {
                    Position = pos,
                    CurrentPlayer = mBoard.GetPieceAtPosition(pos).Player,
                    Piece = mBoard.GetPieceAtPosition(pos),
                    ViewModel = this
                }
            );
            


            PossibleMoves = new HashSet<ChessMove>(
                from ChessMove m in mBoard.GetPossibleMoves()
                select m
            );

            var possMoves = mBoard.GetPossibleMoves() as IEnumerable<ChessMove>;

            List<ChessPiecePosition> pawnMoves = new List<ChessPiecePosition>();
            var queen = new ChessPiecePosition(ChessPieceType.Queen, CurrentPlayer);
            var knight = new ChessPiecePosition(ChessPieceType.Knight, CurrentPlayer);
            var rookPawn = new ChessPiecePosition(ChessPieceType.RookPawn, CurrentPlayer);
            var bioshop = new ChessPiecePosition(ChessPieceType.Bishop, CurrentPlayer);
            pawnMoves.Add(queen);
            pawnMoves.Add(knight);
            pawnMoves.Add(rookPawn);
            pawnMoves.Add(bioshop);

            mPromotionMoves = new ObservableCollection<ChessSquare>();
            for (int pMoves = 0; pMoves < 4; pMoves++)
            {
                mPromotionMoves.Add(new ChessSquare());
                mPromotionMoves[pMoves].CurrentPlayer = CurrentPlayer;
                mPromotionMoves[pMoves].Piece = pawnMoves[pMoves];
                mPromotionMoves[pMoves].IsSelected = false;

            }
        }

        public void UndoMove()
        {
            if(CanUndo)
            {                
                mBoard.UndoLastMove();

                if (Players == NumberOfPlayers.One) {
                    mBoard.UndoLastMove();
                }

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

        public async Task ApplyMove(ChessMove cMove)
        {
            if (mBoard.IsCheckmate)
            {
                GameFinished?.Invoke(this, new EventArgs());
            }
            else
            {

               if (cMove.MoveType == ChessMoveType.PawnPromote) {
                  IsPawnPromotion = true;
               }

               if (IsPawnPromotion) {
                  mBoard.ApplyMove(cMove);
                  IsPawnPromotion = false;
                  RebindState();
               }

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

                RebindState();
                //CheckPromotionMoves();

                if (Players == NumberOfPlayers.One && !mBoard.IsFinished && mBoard.CurrentPlayer == 2) {

                  var bestMove = await Task.Run(() => mGameAi.FindBestMove(mBoard));
                  if (bestMove != null) {
                     mBoard.ApplyMove(bestMove);
                     RebindState();
                  }
               }

            CheckPromotionMoves();
         }
           
          
        }


        private void RebindState() {
            PossibleMoves = new HashSet<ChessMove>(
                    from ChessMove m in mBoard.GetPossibleMoves()
                    select m
                );

            var newSquares =
                from r in Enumerable.Range(0, 8)
                from c in Enumerable.Range(0, 8)
                select new BoardPosition(r, c);
            int i = 0;
            foreach (var pos in newSquares) {
               mSquares[i].CurrentPlayer = mBoard.GetPieceAtPosition(pos).Player;
               mSquares[i].Piece = mBoard.GetPieceAtPosition(pos);
               mSquares[i].IsSelected = false;
               i++;
            }

            OnPropertyChanged(nameof(BoardValue));
            OnPropertyChanged(nameof(CurrentPlayer));
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(IsPawnPromotion));
        }

        private void RebindState(ChessPiecePosition piece) {
            PossibleMoves = new HashSet<ChessMove>(
                    from ChessMove m in mBoard.GetPossibleMoves()
                    select m
                );

            var newSquares =
                from r in Enumerable.Range(0, 8)
                from c in Enumerable.Range(0, 8)
                select new BoardPosition(r, c);
            int i = 0;
            foreach (var pos in newSquares) {
               mSquares[i].CurrentPlayer = mBoard.GetPieceAtPosition(pos).Player;
               mSquares[i].Piece = piece;
               mSquares[i].IsSelected = false;
               i++;
            }

            OnPropertyChanged(nameof(BoardValue));
            OnPropertyChanged(nameof(CurrentPlayer));
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(IsPawnPromotion));
        }


        private void CheckPromotionMoves() {
            var possMoves = mBoard.GetPossibleMoves() as IEnumerable<ChessMove>;
            ChessMove promotionMove = (ChessMove)mBoard.MoveHistory.Last();
            if (promotionMove.Piece.PieceType == ChessPieceType.Pawn && (promotionMove.EndPosition.Row == 0 || promotionMove.EndPosition.Row == 7)) {
               IsPawnPromotion = true;
               possMoves = mBoard.GetPossibleMoves() as IEnumerable<ChessMove>;

               List<ChessPiecePosition> pawnMoves = new List<ChessPiecePosition>();
               var queen = new ChessPiecePosition(ChessPieceType.Queen, CurrentPlayer);
               var knight = new ChessPiecePosition(ChessPieceType.Knight, CurrentPlayer);
               var rookPawn = new ChessPiecePosition(ChessPieceType.RookPawn, CurrentPlayer);
               var bioshop = new ChessPiecePosition(ChessPieceType.Bishop, CurrentPlayer);
               pawnMoves.Add(queen);
               pawnMoves.Add(knight);
               pawnMoves.Add(rookPawn);
               pawnMoves.Add(bioshop);

               mPromotionMoves = new ObservableCollection<ChessSquare>();
               for (int pMoves = 0; pMoves < 4; pMoves++) {
                  mPromotionMoves.Add(new ChessSquare());
                  mPromotionMoves[pMoves].CurrentPlayer = CurrentPlayer;
                  mPromotionMoves[pMoves].Piece = pawnMoves[pMoves];
                  mPromotionMoves[pMoves].IsSelected = false;

               }

            PossibleMoves = new HashSet<ChessMove>(
                 from ChessMove m in mBoard.GetPossibleMoves()
                 select m
             );
         }
         }
        public List<ChessSquare> GetPromotionSquares() {
            ChessSquare queen = new ChessSquare();
            ChessSquare rook = new ChessSquare();
            ChessSquare knight = new ChessSquare();
            ChessSquare bishop = new ChessSquare();
            queen.Piece = new ChessPiecePosition(ChessPieceType.Queen, CurrentPlayer);
            rook.Piece = new ChessPiecePosition(ChessPieceType.RookPawn, CurrentPlayer);
            knight.Piece = new ChessPiecePosition(ChessPieceType.Knight, CurrentPlayer);
            bishop.Piece = new ChessPiecePosition(ChessPieceType.Bishop, CurrentPlayer);

            List < ChessSquare > promotionMoves = new List<ChessSquare>();
            promotionMoves.Add(queen);
            promotionMoves.Add(knight);
            promotionMoves.Add(rook);
            promotionMoves.Add(bishop);

            foreach(var move in promotionMoves) {
                move.CurrentPlayer = CurrentPlayer;
            }
            return promotionMoves;



        }

       
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
                OnPropertyChanged(nameof(PromotionMoves));
            }
        }

        public HashSet<ChessMove> PossibleMoves {
            get; private set;
        }

        public NumberOfPlayers Players { get; set; }

        public int BoardValue { get { return mBoard.Value; } }

        public int CurrentPlayer { get { return mBoard.CurrentPlayer; } }

        public bool IsCheck { get { return mBoard.IsCheck; } }

        public bool IsPawnPromotion {
            get {
                return mPawnPromotion;
            }
            set {
                mPawnPromotion = value;
                OnPropertyChanged(nameof(IsPawnPromotion));
            }
        }

        public bool CanUndo {
            get {
                return mBoard.MoveHistory.Count > 0;
            }
        }
    }
}
