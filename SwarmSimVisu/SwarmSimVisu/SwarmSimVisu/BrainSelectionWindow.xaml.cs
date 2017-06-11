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
using SwarmSimFramework.Classes.Experiments.TestingMaps;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Interfaces;

namespace SwarmSimVisu
{
    /// <summary>
    /// Interaction logic for BrainSelectionWindow.xaml
    /// </summary>
    public partial class BrainSelectionWindow : Window
    {
        public bool ExperimentPrepared = false;
        public IExperiment Experiment = null;
        public IRobotBrain SelectedBrain;
        public Map SelectedMap;
        public BrainSelectionWindow()
        {
            
            InitializeComponent();
            MapComboBox.Items.Add("None");
            MapComboBox.Items.Add("WoodMapCutters");
            MapComboBox.Items.Add("WoodMapCutters with mem");
            string name = "";
            MapComboBox.SelectionChanged += (sender, args) =>
            {
                switch (MapComboBox.SelectedIndex)
                {
                    case 1:
                    {
                        name = "Wood map cutters";
                        SelectedMap = TestingMaps.GetWoodMapCuters();
                        break;
                    }
                    case 2:
                    {
                        name = "Wood map cutters with mem";
                        SelectedMap = TestingMaps.GetWoodMapCutersWithMem();
                        break;
                    }
                    default:
                    {
                        SelectedMap = null;
                        MapText.Text = "None";
                        return;
                    }
                }

                StringBuilder mapInfo = new StringBuilder(name + "\n");
                mapInfo.Append("Height: " + SelectedMap.MaxHeight + " Width: " + SelectedMap.MaxWidth + "\n");
                mapInfo.AppendLine("Amount of robots: " + SelectedMap.Robots.Count);
                mapInfo.AppendLine("Passive entities: " +SelectedMap.PasiveEntities.Count);
                MapText.Text = mapInfo.ToString();
            };
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            string t = "";
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.InitialDirectory = System.Environment.CurrentDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                t = File.ReadAllText(openFileDialog.FileName);
                SelectedBrain = BrainSerializer.DeserializeBrain(t);
            }

            //Text about brain update
            if (SelectedBrain != null)
            {
                StringBuilder brainInfo = new StringBuilder("Brain: " + SelectedBrain.GetType().ToString() + '\n');
                brainInfo.Append(" In Dimension: " + SelectedBrain.IoDimension.Input + '\n');
                brainInfo.Append(" Out Dimension: " + SelectedBrain.IoDimension.Output + '\n');
                brainInfo.Append(" Fitness: " + SelectedBrain.Fitness + '\n');
                BrainText.Text = brainInfo.ToString();
            }

           
        }

        private void PrepareExperiment_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBrain == null)
            {
                MessageBox.Show("Brain has to be selected!");
                return;
            }

            if (SelectedMap == null)
            {
                MessageBox.Show("Map has to be selected!");
                return;
            }


            Experiment = new TestingBrain(SelectedMap,SelectedBrain,1000);
            this.Close();
        }
    }
}
