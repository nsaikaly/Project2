using Cecs475.BoardGames.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace Cecs475.BoardGames.WpfApplication {
	/// <summary>
	/// Interaction logic for GameChoiceWindow.xaml
	/// </summary>
	public partial class GameChoiceWindow : Window {
		public GameChoiceWindow() {
			InitializeComponent();

         //Type gameType = typeof(IGameType);
         //var path = @"bin\Debug\lib";
         //List<string> dirs = new List<string>(Directory.EnumerateDirectories(path, "*.dll"));

         //foreach (var dir in dirs) {
         //   Assembly.LoadFrom(dir);
         //}

         //AppDomain.CurrentDomain.GetAssemblies();

         //foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
         //   foreach(var type in assembly.GetTypes()) {
         //      //filter the list of types to only containt hose types t such that IGameType is assignable from t
         //   }
         //}

      }

		private void Button_Click(object sender, RoutedEventArgs e) {
			Button b = sender as Button;
			IGameType gameType = b.DataContext as IGameType;
			var gameWindow = new MainWindow(gameType, 
				mHumanBtn.IsChecked.Value ? NumberOfPlayers.Two : NumberOfPlayers.One) {
				Title = gameType.GameName
			};
			gameWindow.Closed += GameWindow_Closed;

			gameWindow.Show();
			this.Hide();
		}

		private void GameWindow_Closed(object sender, EventArgs e) {
			this.Show();
		}
	}
}
