using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Interfaces;

namespace SwarmSimVisu
{
    
    /// <summary>
    /// Interaction logic for BrainSelectionWindow.xaml
    /// </summary>
    public partial class BrainSelectionWindow : Window
    {
        public bool BrainChoosing;
        public bool ExperimentPrepared = false;
        public IExperiment Experiment = null;
        public List<RobotModel> preparedRobots = new List<RobotModel>();
        public List<BrainModel<IRobotBrain>> preparedBrains = new List<BrainModel<IRobotBrain>>();
        public Map SelectedMap;
        public BrainSelectionWindow()
        {
            
            InitializeComponent();
            MapComboBox.Items.Add("WoodMap");
            MapComboBox.Items.Add("MineralMap");
            string name = "";
            MapComboBox.SelectionChanged += (sender, args) =>
            {
                switch (MapComboBox.SelectedIndex)
                {
                    case 0:
                    {
                        name = "Wood map";
                        WoodScene.AmountOfTrees = 200;
                        StringBuilder mapInfo = new StringBuilder(name + "\n");
                        mapInfo.AppendLine("Max amount of robots: " + WoodScene.MaxOfAmountRobots);
                        mapInfo.Append("Map heigth: ");
                        mapInfo.Append(WoodScene.MapHeight.ToString());
                        mapInfo.Append("Map width: ");
                        mapInfo.AppendLine(WoodScene.MapWidth.ToString());
                        mapInfo.Append("Trees in map: ");
                        mapInfo.Append(WoodScene.AmountOfTrees);
                        mapInfo.Append(" Woods in map: ");
                        mapInfo.AppendLine(WoodScene.AmountOfWoods.ToString());
                        MapText.Text = mapInfo.ToString();
                            break; 
                    }
                    case 1:
                    {
                        name = "Mineral map";
                        MineralScene.AmountOfFreeFuel = 10;
                        MineralScene.AmountOfMineral = 10;
                        MineralScene.AmountOfObstacles = 10;
                        StringBuilder mapInfo = new StringBuilder(name + "\n");
                        mapInfo.AppendLine("Max amount of robots: " + MineralScene.MaxOfAmountRobots);
                        mapInfo.Append("Map heigth: ");
                        mapInfo.Append(MineralScene.MapHeight.ToString());
                        mapInfo.Append("Map width: ");
                        mapInfo.AppendLine(MineralScene.MapWidth.ToString());
                        mapInfo.Append("Free fuel in map: ");
                        mapInfo.Append(MineralScene.AmountOfFreeFuel);
                        mapInfo.Append("Minerals in map: ");
                        mapInfo.AppendLine(MineralScene.AmountOfMineral.ToString());
                        mapInfo.Append("Obstacles in map: ");
                        mapInfo.AppendLine(MineralScene.AmountOfObstacles.ToString());
                        MapText.Text = mapInfo.ToString();
                        break;
                    }
                    default:
                    {
                        SelectedMap = null;
                        MapText.Text = "None";
                        return;
                    }
                }


            };
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (BrainChoosing) return;
            BrainChoosing = true;
            var w = new BrainRobotConnectionWindow();
            w.ShowDialog();
            if (w.preparedRobots != null && w.preparedRobots.Count != 0)
            {
                foreach (var pR in w.preparedRobots)
                {
                    preparedRobots.Add(new RobotModel(){amount=pR.amount,model =pR.model});
                    preparedBrains.Add(new BrainModel<IRobotBrain>() {Brain = pR.brain, Robot = pR.model});
                }
                BrainText.Text = w.PreparedModels.Text;
            }
            else
            {
                MessageBox.Show("Unknown setting of brains");
            }
            BrainChoosing = false;

        }

        private void PrepareExperiment_Click(object sender, RoutedEventArgs e)
        {
            if (preparedRobots == null || preparedRobots.Count == 0)
            {
                MessageBox.Show("Brain has to be selected!");
                return;
            }


            switch (MapComboBox.SelectedIndex)
            {
                case 0:
                {
                    SelectedMap = WoodScene.MakeMap(preparedRobots.ToArray());
                    break;
                }
                case 1:
                {
                    SelectedMap = MineralScene.MakeMap(preparedRobots.ToArray()); 
                    break;
                }
            }
            if (SelectedMap == null)
            {
                MessageBox.Show("Map has to be selected!");
                return;
            }


            Experiment = new TestingBrain(SelectedMap,preparedBrains.ToArray(),1000);
            this.Close();
        }
    }
}
