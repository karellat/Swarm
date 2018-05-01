using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Robots.CompetitiveRobots;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;
using CompetitiveSceneBridge = SwarmSimFramework.Classes.Map.CompetitiveScene<SwarmSimFramework.Classes.RobotBrains.SingleLayerNeuronNetwork>;

namespace SwarmSimVisu
{

    /// <summary>
    /// Interaction logic for CompetitiveMapSelectionWindows.xaml
    /// </summary>
    public partial class CompetitiveMapSelectionWindows : Window
    {
        private SingleLayerNeuronNetwork fighterBrain = null;
        private SingleLayerNeuronNetwork fighterScoutBrain = null;

        public CompetitiveMapSelectionWindows()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;

            heightLabel.Text = CompetitiveSceneBridge.MapHeight.ToString();
            widthLabel.Text = CompetitiveSceneBridge.MapWidth.ToString();

            obstaclesLabel.Text = CompetitiveSceneBridge.AmountOfObstacles.ToString();
            obstaclesLabel.TextChanged += (sender, args) =>
            {
                if (int.TryParse(obstaclesLabel.Text, out var i))
                    CompetitiveSceneBridge.AmountOfObstacles = i;
            };

            scoutBrainButton.Click += (sender, args) =>
            {
                try
                {
                    string t = "";
                    Microsoft.Win32.OpenFileDialog openFileDialog =
                        new Microsoft.Win32.OpenFileDialog { InitialDirectory = System.Environment.CurrentDirectory };
                    if (openFileDialog.ShowDialog() == true)
                    {
                        t = File.ReadAllText(openFileDialog.FileName);
                        fighterScoutBrain = (SingleLayerNeuronNetwork)BrainSerializer.DeserializeBrain(t);
                        scoutBrainLabel.Content = "was read";

                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Reading failed, new brain will be generated:" + exp.Message);
                }

            };

            brainButton.Click += (sender, args) =>
            {
                try
                {
                    string t = "";
                    Microsoft.Win32.OpenFileDialog openFileDialog =
                        new Microsoft.Win32.OpenFileDialog { InitialDirectory = System.Environment.CurrentDirectory };
                    if (openFileDialog.ShowDialog() == true)
                    {
                        t = File.ReadAllText(openFileDialog.FileName);
                        fighterBrain = (SingleLayerNeuronNetwork)BrainSerializer.DeserializeBrain(t);
                        brainLabel.Content = "was read";
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Reading failed, new brain will be generated");
                }
            };

        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            //Set brains to the map model
            var scoutModel = new FightScoutRobotMem();
            var robotModel = new FighterRobotMem();
            var enemyModels = new[]
            {
                new RobotModel()
                {
                    amount = (int) scoutCount.Value,
                    model = scoutModel
                },
                new RobotModel()
                {
                    amount = (int) fighterCount.Value,
                    model = robotModel
                }

            };
            //GENERATE NEW IF NOT ANY
            fighterScoutBrain = fighterScoutBrain ?? SingleLayerNeuronNetwork.GenerateNewRandomNetwork(
                                    new IODimension()
                                    {
                                        Input = scoutModel.SensorsDimension,
                                        Output = scoutModel.EffectorsDimension
                                    });
            
            fighterBrain = fighterBrain ?? SingleLayerNeuronNetwork.GenerateNewRandomNetwork(new IODimension()
            {
                Input = robotModel.SensorsDimension,
                Output = robotModel.EffectorsDimension
            });

            var enemyBrainModels = new[]
            {
                new BrainModel<SingleLayerNeuronNetwork>()
                {
                    Brain = fighterScoutBrain,
                    Robot = new FightScoutRobotMem()
                },
                new BrainModel<SingleLayerNeuronNetwork>()
                {
                    Brain = fighterBrain,
                    Robot = new FighterRobotMem()
                }
            };

            CompetitiveSceneBridge.SetUpEnemies(enemyModels, enemyBrainModels);

            this.Close();
        }
    }
}

