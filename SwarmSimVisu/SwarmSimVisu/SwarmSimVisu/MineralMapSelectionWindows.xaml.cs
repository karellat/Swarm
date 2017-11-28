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
using SwarmSimFramework.Classes.Map;

namespace SwarmSimVisu
{
    /// <summary>
    /// Interaction logic for MineralMapSelectionWindows.xaml
    /// </summary>
    public partial class MineralMapSelectionWindows : Window
    {
        public MineralMapSelectionWindows()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;

            AmountOfFreeFuel.TextChanged += (sender, args) =>
            {
                int i;
                if (int.TryParse(AmountOfFreeFuel.Text, out i))
                    MineralScene.AmountOfFreeFuel = i;
            };

            AmountOfMineralTextBox.TextChanged += (sender, args) =>
            {
                int i;
                if (int.TryParse(AmountOfMineralTextBox.Text, out i))
                    MineralScene.AmountOfMineral = i;
            };

            AmountOfObstaclesTextBox.TextChanged += (sender, args) =>
            {
                int i;
                if (int.TryParse(AmountOfObstaclesTextBox.Text, out i))
                    MineralScene.AmountOfObstacles = i;
            };
        }

        public void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
