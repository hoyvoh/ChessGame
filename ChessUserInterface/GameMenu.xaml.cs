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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessUserInterface
{
    /// <summary>
    /// Interaction logic for GameMenu.xaml
    /// </summary>
    public partial class GameMenu : UserControl
    {
        public event Action<Options> OptionSelected;
        public GameMenu()
        {
            InitializeComponent();

        }

        //Start game
        private void Newgame_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Options.Start);
        }

        //Exit game
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Options.Exit);
        }
    }
}
