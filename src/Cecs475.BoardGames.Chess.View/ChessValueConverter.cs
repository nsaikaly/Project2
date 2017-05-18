﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Cecs475.BoardGames.Chess.View {
	public class ChessValueConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			int v = (int)value;
			if (v == 0)
				return "Tie game";
			if (v > 0)
				return $"Player 1 is winning by {v} pieces";
			return $"Player 2 is winning by {v} pieces";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
