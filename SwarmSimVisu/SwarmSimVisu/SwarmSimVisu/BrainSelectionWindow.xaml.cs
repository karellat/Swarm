using System.Collections.Generic;
using System.Text;
using System.Windows;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Experiments.TestingMaps;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Robots.WoodRobots;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

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
        public int LengthOfCycle = 1000;
        public BrainSelectionWindow()
        {
            InitializeComponent();
            MapComboBox.Items.Add("WoodMap");
            MapComboBox.Items.Add("MineralMap");
            MapComboBox.Items.Add("CompetitiveMap");
            string name = "";
            MapComboBox.SelectionChanged += (sender, args) =>
            {
                switch (MapComboBox.SelectedIndex)
                {
                    case 0:
                    {
                        WoodScene.AmountOfTrees = 200;
                        var w = new WoodMapSelectionWindows();
                        w.ShowDialog();
                        name = "Wood map";
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
                        MineralScene.AmountOfFreeFuel = 100;
                        MineralScene.AmountOfMineral = 100;
                        MineralScene.AmountOfObstacles = 100;
                        var w = new MineralMapSelectionWindows();
                        w.ShowDialog();
                        name = "Mineral map";
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
                    case 2:
                    {
                        name = "Mineral map";
                        CompetitiveScene<SingleLayerNeuronNetwork>.AmountOfObstacles = 500;
                        var w = new CompetitiveMapSelectionWindows();
                        w.ShowDialog();
                        StringBuilder mapInfo = new StringBuilder(name + "\n");
                        mapInfo.AppendLine("Max amount of robots: " + CompetitiveScene<SingleLayerNeuronNetwork>.MaxOfAmountRobots);
                        mapInfo.Append("Map heigth: ");
                        mapInfo.Append(CompetitiveScene<SingleLayerNeuronNetwork>.MapHeight.ToString());
                        mapInfo.Append("Map width: ");
                        mapInfo.AppendLine(CompetitiveScene<SingleLayerNeuronNetwork>.MapWidth.ToString());
                        mapInfo.Append("Obstacles in map: ");
                        mapInfo.Append(CompetitiveScene<SingleLayerNeuronNetwork>.AmountOfObstacles);
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
            LengthOfCycleTextBox.TextChanged += (sender, args) =>
            {
                if(!int.TryParse(LengthOfCycleTextBox.Text, out LengthOfCycle))
                    LengthOfCycleTextBox.Text = LengthOfCycle.ToString();
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
                case 2:
                {
                    SelectedMap = CompetitiveScene<SingleLayerNeuronNetwork>.MakeMap(preparedRobots.ToArray());
                    break; 
                }
            }
            if (SelectedMap == null)
            {
                MessageBox.Show("Map has to be selected!");
                return;
            }

            if(MapComboBox.SelectedIndex == 2)
                Experiment = new TestingBrain(SelectedMap,preparedBrains.ToArray(), LengthOfCycle, CompetitiveScene<SingleLayerNeuronNetwork>.EnemyBrainModels);
            else
                Experiment = new TestingBrain(SelectedMap,preparedBrains.ToArray(),LengthOfCycle);
            this.Close();
        }
    }
}
