// Decompiled with JetBrains decompiler
// Type: Cecs475.BoardGames.Chess.View.ChessSquareColorConverter
// Assembly: Cecs475.BoardGames.Chess.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C959CC75-3A18-4FDD-A37C-9C5A8C3ED23D
// Assembly location: C:\Users\Nick\Downloads\Cecs475.BoardGames.Chess.View-cleaned.dll

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Cecs475.BoardGames.Chess.View
{
    public class ChessSquareColorConverter : IMultiValueConverter
    {
        private static SolidColorBrush Black = new SolidColorBrush(Colors.Black);
        private static SolidColorBrush Red = new SolidColorBrush(Colors.Red);
        private static SolidColorBrush White = new SolidColorBrush(Colors.White);
        private static SolidColorBrush Yellow = new SolidColorBrush(Colors.Yellow);
        private static SolidColorBrush LightGreen = new SolidColorBrush(Colors.LightGreen);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            BoardPosition boardPosition = (BoardPosition)values[1];
            ChessPiecePosition chessPiecePosition = (ChessPiecePosition)values[0];
            ChessViewModel chessViewModel = (ChessViewModel)values[2];
            bool isSelected = (bool)values[3];
            bool isHovered = (bool)values[4];
            if (isSelected)
                return Red;
            if (isHovered)
                return Red;
            if (chessViewModel.IsCheck && chessPiecePosition.PieceType == ChessPieceType.King && chessPiecePosition.Player == chessViewModel.CurrentPlayer)
                return (object)ChessSquareColorConverter.Yellow;
            if ((boardPosition.Row + boardPosition.Col) % 2 != 0)
                return White;
            return Black;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
