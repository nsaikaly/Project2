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
			

         Type gameType = typeof(IGameType);     
         var path = @"lib";

         DirectoryInfo d = new DirectoryInfo(path);
         FileInfo[] dirs = d.GetFiles(); //Getting Text files

         foreach (var dir in dirs) {
            //Assembly.LoadFrom(path + "/" + dir.ToString());
            string assemblyName = System.IO.Path.GetFileNameWithoutExtension(dir.ToString());
            string test = "Cecs475.BoardGames.Chess.Model";
            Assembly.Load(assemblyName + ", Version=1.0.0.0, Culture=neutral, PublicKeyToken=68e71c13048d452a");
            //FullName = "Cecs475.BoardGames.Chess.View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=68e71c13048d452a"
         }

         var gameTypes = AppDomain.CurrentDomain.GetAssemblies()
                 .SelectMany(a => a.GetTypes())
                 .Where(t => gameType.IsAssignableFrom(t) && t.IsClass);


         var instances = gameTypes
            .Select(o => Activator.CreateInstance(o));

         Resources.Add("GameTypes", instances);
         InitializeComponent();

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
