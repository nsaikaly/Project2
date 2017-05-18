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
        private static SolidColorBrush solidColorBrush_0 = new SolidColorBrush(Colors.Black);
        private static SolidColorBrush solidColorBrush_1 = new SolidColorBrush(Colors.Red);
        private static SolidColorBrush solidColorBrush_2 = new SolidColorBrush(Colors.White);
        private static SolidColorBrush solidColorBrush_3 = new SolidColorBrush(Colors.Yellow);
        private static SolidColorBrush solidColorBrush_4 = new SolidColorBrush(Colors.LightGreen);

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            BoardPosition boardPosition = (BoardPosition)values[1];
            ChessPiecePosition chessPiecePosition = (ChessPiecePosition)values[0];
            ChessViewModel chessViewModel = (ChessViewModel)values[2];
            int isSelected = (bool)values[3] ? 1 : 0;
            bool isHovered = (bool)values[4];
            if (isSelected != 0)
                return solidColorBrush_1;
            if (isHovered)
                return solidColorBrush_1;
            //if (chessViewModel.IsCheck && chessPiecePosition.PieceType == ChessPieceType.King && chessPiecePosition.Player == chessViewModel.CurrentPlayer)
               // return (object)ChessSquareColorConverter.solidColorBrush_3;
            if ((boardPosition.Row + boardPosition.Col) % 2 != 0)
                return solidColorBrush_2;
            return solidColorBrush_0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
