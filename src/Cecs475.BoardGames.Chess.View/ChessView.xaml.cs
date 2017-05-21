using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cecs475.BoardGames.Chess.View {
    /// <summary>
    /// Interaction logic for ChessView.xaml
    /// </summary>
    public partial class ChessView : UserControl {
        public static ChessMove MOVE;
        public static ChessSquare currentSelected = null;
        public static bool SELECTED;

        public ChessView() {           
            InitializeComponent();
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e) {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;
            var selected = square;
            var possMoves = vm.PossibleMoves;
            foreach(var move in possMoves)
            {
                if (SELECTED)
                {
                    if (currentSelected != null)
                    {
                        if(move.StartPosition.Equals(currentSelected.Position))
                        {
                            if(square.Position.Equals(move.EndPosition))
                            {
                                square.IsHovered = true;
                            }
                        }
                    }
                }
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e) {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            square.IsHovered = false;
        }

        public ChessViewModel Model {
            get { return FindResource("vm") as ChessViewModel; }
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e) {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;
            var possMoves = vm.PossibleMoves;

            if (SELECTED && currentSelected != null)
            {
                //var move = new ChessMove(currentSelected.Position, square.Position);
                //bool possible = false;
                //foreach (var posMove in vm.PossibleMoves)
                //{                    
                //    if(posMove.Equals(move))
                //    {
                //        possible = true;
                //    }
                //}
                //if (!possible) {
                //currentSelected.IsSelected = false;
                //currentSelected = null;
                //SELECTED = false;
                //}

                if (!square.IsHovered)
                {
                    currentSelected.IsSelected = false;
                    currentSelected = null;
                    SELECTED = false;
                }
            }

            foreach (var move in possMoves)
            {
                if (move.StartPosition.Equals(square.Position))
                {                    
                    foreach(ChessSquare s in vm.Squares)
                    {
                        if (s.IsSelected)
                        {
                            SELECTED = true;
                            currentSelected = s;
                        }
                    }

                    if (!SELECTED)
                    {
                        square.IsSelected = true;
                        currentSelected = square;
                        SELECTED = true;
                        MOVE = new ChessMove(square.Position, square.Position);                       
                    }


                }

                else if(move.EndPosition.Equals(square.Position) && SELECTED)
                {
                    MOVE.EndPosition = square.Position;
                    

                    if (/*MOVE.Piece.PieceType == ChessPieceType.Pawn && */(MOVE.EndPosition.Row == 0 || MOVE.EndPosition.Row == 7))
                    {
                        PawnPromotionWindow subWin = new PawnPromotionWindow(vm);
                        subWin.Show();
                        //subWin.Close();

                    }

                    vm.ApplyMove(MOVE);
                    SELECTED = false;
                    currentSelected = null;
                    
                }
            }
        }

        private void Border_MouseClick(object sender, MouseButtonEventArgs e) {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;

            //if (vm.PossibleMoves.Contains(square.Position))
            //{
            //    if(!square.IsSelected)
            //        square.IsSelected = true;
            //}

        }
        private void Border_MouseClick_Cancel(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;
            //if (!vm.PossibleMoves.Contains(square.Position)) {
            //    square.IsSelected = false;
            //}
        }
    }
}
