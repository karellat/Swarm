using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
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
  
    public class IntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = (string)value;
            int i;
            if (int.TryParse(s, out i))
            {
                return i;
            }
            else
                return null;
        }
    }

    /// <summary>
    /// Interaction logic for WoodMapSelectionWindows.xaml
    /// </summary>
    public partial class WoodMapSelectionWindows : Window
    {
        public static IValueConverter IntConverter = new IntConverter();
        public WoodMapSelectionWindows()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;

            AmountOfTreesTextBox.TextChanged += (sender, args) =>
            {
                int i;
                if (int.TryParse(AmountOfTreesTextBox.Text, out i))
                    WoodScene.AmountOfTrees = i;
            }; 

            AmountOfWoodsTextBox.TextChanged += (sender,args) =>
            {
                int i;
                if (int.TryParse(AmountOfWoodsTextBox.Text, out i))
                    WoodScene.AmountOfWoods = i;
            };
        }

        public void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }


 

}
