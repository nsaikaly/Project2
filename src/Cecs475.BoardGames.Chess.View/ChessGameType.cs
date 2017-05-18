using Cecs475.BoardGames.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cecs475.BoardGames.Chess.View {
	public class ChessGameType : IGameType {
		public string GameName {
			get {
				return "Chess";
			}
		}

		public IValueConverter CreateBoardValueConverter() {
			return new ChessValueConverter();
		}

		public IValueConverter CreateCurrentPlayerConverter() {
			return new ChessCurrentPlayerConverter();
		}

		public Tuple<Control, IGameViewModel> CreateViewAndViewModel() {
			var view = new ChessView();
			var model = view.Model;
			return new Tuple<Control, IGameViewModel>(view, model);
		}
	}
}
