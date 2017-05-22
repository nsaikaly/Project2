using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Cecs475.BoardGames.Chess.View
{
    /// <summary>
    /// Interaction logic for PawnPromotionWindow.xaml
    /// </summary>
    public partial class PawnPromotionWindow : Window {
        public static SolidColorBrush RED_BRUSH = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush GREEN_BRUSH = new SolidColorBrush(Colors.Green);
        public static SolidColorBrush BLUE_BRUSH = new SolidColorBrush(Colors.Blue);

        public PawnPromotionWindow(ChessViewModel ViewModel) {

            //use current viewmodel in here
            //have two stackpanels
            //one for white player and one for black player
            //based on whose turn it is from the viewmodel, display only one stackpanel in the window with the hardcoded 4 moves
            InitializeComponent();
            //var player = ViewModel.CurrentPlayer;
            //ChessPieceConverter converter = new ChessPieceConverter();
            //var moves = ViewModel.GetPromotionSquares();
            //var panel = FindResource("Panel") as StackPanel;
            //var source = new Binding("Piece") { Converter = new ChessPieceConverter() };

            //var binding = new Binding("Piece");
            //binding.Converter = new ChessPieceConverter();



            //Image Img = new Image();

            //Img.Source = new BitmapImage(new Uri("/Cecs475.BoardGames.Chess.View;component/Resources/" + "white" + "rook.png"));
            ////< Image Source = "{Binding Piece, Converter={StaticResource PieceImage}}" />
            //for (var i = 0; i < 5; i++)
            //{


            //    var converted = (BitmapImage)(converter.Convert(moves[i].Piece, null, null, null));
            //    PromotionPanel.Children.Add(new Button());
            //}


            //DataContext = ViewModel;

        }


        private void Border_MouseUp(object sender, MouseButtonEventArgs e) {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;
           
            var startPos = square.Position;
            var piece = square.Piece.PieceType;
            var possMoves = vm.PossibleMoves;

            foreach (var move in possMoves)
            {
                if (move.EndPosition.ToString().Equals(piece.ToString()))
                {
                    b.Background = RED_BRUSH;
                    vm.ApplyMove(move);
                }
            }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e) {
            Border b = sender as Border;
            var square = b.DataContext as ChessSquare;
            var vm = FindResource("vm") as ChessViewModel;
            foreach (var move in vm.PossibleMoves)
            {
                if (square.Position.Equals(move.EndPosition))
                    b.Background = GREEN_BRUSH;

            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e) {
            Border b = sender as Border;
            b.Background = BLUE_BRUSH;
        }


    }
}
