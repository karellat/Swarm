using System;
using System.Collections.Generic;
using System.IO;
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
using SwarmSimFramework.Interfaces;

namespace SwarmSimVisu
{
    /// <summary>
    /// Interaction logic for BrainSelectionWindow.xaml
    /// </summary>
    public partial class BrainSelectionWindow : Window
    {
        public IRobotBrain SelectedBrain;
        public Map SelectedMap;

        public BrainSelectionWindow()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            string t = "";
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = System.Environment.CurrentDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                t = File.ReadAllText(openFileDialog.FileName);
            }

            SelectedBrain = BrainSerializer.DeserializeBrain(t);
        }
    }
}
