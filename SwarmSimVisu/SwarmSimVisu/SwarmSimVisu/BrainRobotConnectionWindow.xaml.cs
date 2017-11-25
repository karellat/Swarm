using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
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
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Robots.MineralRobots;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.Classes.Robots.WoodRobots;

namespace SwarmSimVisu
{
    /// <summary>
    /// Interaction logic for BrainRobotConnectionWindow.xaml
    /// </summary>
    public partial class BrainRobotConnectionWindow : Window
    {
        public struct PreparedRobot
        {
            public IRobotBrain brain;
            public RobotEntity model;
            public int amount;
        }

        /// <summary>
        /// Actual read brain
        /// </summary>
        public IRobotBrain SelectedBrain;

        /// <summary>
        /// Actual Robot model
        /// </summary>
        public RobotEntity RobotModel;
        /// <summary>
        /// Prepared robots 
        /// </summary>
        public List<PreparedRobot> preparedRobots = new List<PreparedRobot>();
        /// <summary>
        /// Max amount model 
        /// </summary>
        public int MaxAmountModel = 5;
        /// <summary>
        /// 
        /// </summary>
        public BrainRobotConnectionWindow()
        {
            InitializeComponent();
            modelCombox.Items.Add("Wood Cutter");
            modelCombox.Items.Add("Wood Cutter MEM");
            modelCombox.Items.Add("Wood Worker");
            modelCombox.Items.Add("Wood Worker MEM");
            modelCombox.Items.Add("Mineral Refactor");
            modelCombox.Items.Add("Mineral Scout");
            modelCombox.Items.Add("Mineral Worker");
            preparedRobots = new List<PreparedRobot>();
            RobotModel = null;
            SelectedBrain = null;
            modelCombox.SelectionChanged += CreateModel_Click;
        }

        /// <summary>
        /// Open brain from file 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            string t = "";
            Microsoft.Win32.OpenFileDialog openFileDialog =
                new Microsoft.Win32.OpenFileDialog {InitialDirectory = System.Environment.CurrentDirectory};
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
                BrainInfo.Text = brainInfo.ToString();
            }
        }

        /// <summary>
        /// Create robot model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateModel_Click(object sender, RoutedEventArgs e)
        {
            switch (modelCombox.SelectedIndex)
            {
                case 0:
                {
                    RobotModel = new ScoutCutterRobot(Vector2.Zero);
                    break;
                }
                case 1:
                {
                    RobotModel = new ScoutCutterRobotWithMemory(Vector2.Zero);
                    break;
                }
                case 2:
                {
                    RobotModel = new WoodWorkerRobot(Vector2.Zero);
                    break;
                }
                case 3:
                {
                    RobotModel = new WoodWorkerRobotMem(Vector2.Zero);
                    break;
                }
                case 4:
                    {
                        RobotModel = new RefactorRobot(Vector2.Zero,2000);
                        break;
                    }
                case 5:
                    {
                        RobotModel = new ScoutRobot(Vector2.Zero,2000);
                        break;
                    }
                case 6:
                    {
                        RobotModel = new WorkerRobot(Vector2.Zero,2000);
                        break;
                    }
                default:
                {
                    MessageBox.Show("None robot selecter! ");
                    return;
                }
            }
            StringBuilder s = new StringBuilder("Info:\n");
            s.AppendLine("Name: " + RobotModel.Name);
            s.AppendLine("Sensor dimension: " + RobotModel.SensorsDimension);
            s.AppendLine("Effector dimension: " + RobotModel.EffectorsDimension);
            ModelInfo.Text = s.ToString();
        }

        /// <summary>
        /// Add model & brain of given amount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddRobots(object sender, RoutedEventArgs e)
        {
            //Check compability of brain with model 
            if (RobotModel == null || SelectedBrain == null ||
                RobotModel.SensorsDimension != SelectedBrain.IoDimension.Input
                || RobotModel.EffectorsDimension != SelectedBrain.IoDimension.Output ||
                MaxAmountModel == preparedRobots.Count) 
            {
                MessageBox.Show("Not suitable connection between selected brain and robot model or maximum amount of different models");
                return;
            }

            int amountOfRobots = (int) Amount.Value;
            string infoLine = "\n" + preparedRobots.Count+ ") Robot: " + RobotModel.Name + " Brain Input: " + SelectedBrain.IoDimension.Input +
                              " Brain Output: " + SelectedBrain.IoDimension.Output + " Amount: " + amountOfRobots;
            PreparedModels.Text += infoLine + '\n';

            preparedRobots.Add(new PreparedRobot(){amount = amountOfRobots, brain = SelectedBrain,model = RobotModel});
        }

        private void FinishClick(object sender, RoutedEventArgs e)
        {
            if (preparedRobots.Count <= 0)
            {
                MessageBox.Show("Non robots prepared");
                return;
            }
            this.Close();
        }

        private void Slider_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.AmountLabel == null)
                return;
            var slider = sender as Slider;
            double value = slider.Value;
            // ... Set Window Title.
           this.AmountLabel.Text = value.ToString("0") + "/" + slider.Maximum;
        }



    }
}
