using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cecs475.BoardGames.Chess.View
{
    public class ChessPieceConverter : IValueConverter
    {
        private static string Player(int player)
        {
            return player != 1 ? "Black" : "White";
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ChessPiecePosition chessPiecePosition = (ChessPiecePosition)value;
            switch (chessPiecePosition.PieceType)
            {
                case ChessPieceType.Empty:
                    return null;
                case ChessPieceType.RookQueen:                    
                    return new BitmapImage(new Uri(string.Format("/Cecs475.BoardGames.Chess.View;component/Resources/{0}Rook.png", Player(chessPiecePosition.Player)), UriKind.Relative));
                case ChessPieceType.RookKing:
                    return new BitmapImage(new Uri(string.Format("/Cecs475.BoardGames.Chess.View;component/Resources/{0}Rook.png", Player(chessPiecePosition.Player)), UriKind.Relative));
                case ChessPieceType.RookPawn:
                    return new BitmapImage(new Uri(string.Format("/Cecs475.BoardGames.Chess.View;component/Resources/{0}Rook.png", Player(chessPiecePosition.Player)), UriKind.Relative));
                case ChessPieceType.Bishop:
                    return new BitmapImage(new Uri(string.Format("/Cecs475.BoardGames.Chess.View;component/Resources/{0}Bishop.png", Player(chessPiecePosition.Player)), UriKind.Relative));
                case ChessPieceType.Knight:
                    return new BitmapImage(new Uri(string.Format("/Cecs475.BoardGames.Chess.View;component/Resources/{0}Knight.png", Player(chessPiecePosition.Player)), UriKind.Relative));
                case ChessPieceType.Queen:
                    return new BitmapImage(new Uri(string.Format("/Cecs475.BoardGames.Chess.View;component/Resources/{0}Queen.png", Player(chessPiecePosition.Player)), UriKind.Relative));
                case ChessPieceType.King:
                    return new BitmapImage(new Uri(string.Format("/Cecs475.BoardGames.Chess.View;component/Resources/{0}King.png", Player(chessPiecePosition.Player)), UriKind.Relative));
                case ChessPieceType.Pawn:
                    return new BitmapImage(new Uri(string.Format("/Cecs475.BoardGames.Chess.View;component/Resources/{0}Pawn.png", Player(chessPiecePosition.Player)), UriKind.Relative));
                default:
                    //return new BitmapImage(new Uri("/Cecs475.BoardGames.Chess.View;component/Resources/BlackRook.png", UriKind.Relative));
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
