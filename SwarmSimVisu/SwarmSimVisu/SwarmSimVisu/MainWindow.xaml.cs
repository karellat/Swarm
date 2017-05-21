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
using SharpDX.Direct3D9;
using SwarmSimFramework.Classes.Experiments;
using SwarmSimFramework.Interfaces;
using System.Threading;

namespace SwarmSimVisu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //ComoBox for choosing experiment
            ExperimentComboBox.SelectedIndex = 0;
            ExperimentComboBox.Items.Add("None");
            ExperimentComboBox.Items.Add("TestingExperiment");

        }
        /// <summary>
        /// Currently running Experiment
        /// </summary>
        public IExperiment RunningExperiment;
        /// <summary>
        /// Grid containing DrawCanvas
        /// </summary>
        public Grid DrawGrid;
        /// <summary>
        /// D2DControl graphics accelerated control for drawing
        /// </summary>
        public MapCanvas DrawCanvas;

        //STATES of simulation
        public bool Running;
        public bool Pausing;
        public bool Stopping;
        public bool Paused;

        /// <summary>
        /// Loc of the controls 
        /// </summary>
        private object ControlsLock = new object();

        /// <summary>
        /// Control of experiment
        /// </summary>
        private Thread ExperimentThread;

        /// <summary>
        /// Init selected Experiment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitExperimentClick(object sender, RoutedEventArgs e)
        {
            //Experiment selection
            switch (ExperimentComboBox.SelectedIndex)
            {
                case (0):
                    MessageBox.Show("None experiment chosen!");
                    return;
                case (1):
                    RunningExperiment = new TestingExperiment();
                    break;
                default:
                    MessageBox.Show("Unknown experiment");
                    return;
            }
            RunningExperiment.Init();
            //Maximaze window
            WindowState = WindowState.Maximized;
            if (!PrepareExperiment(RunningExperiment))
            {
                MessageBox.Show("Small height and width to show whole map");
                return;
            }

            //if successfull disable ExperimentCombox & init button, show experiment control button
            ExperimentComboBox.IsEditable = false;
            ExperimentComboBox.IsHitTestVisible = false;
            ExperimentComboBox.Focusable = false;
            //Set up controls
            InitExpB.Visibility = Visibility.Hidden;
            StartB.Visibility = Visibility.Visible;
            Running = false;
            Stopping = false;
            Paused = false;
            Pausing = false;
        }
        /// <summary>
        /// Prepare Experiment Drawing stuff
        /// </summary>
        /// <param name="experiment"></param>
        /// <returns></returns>
        private bool PrepareExperiment(IExperiment experiment)
        {
            var HeightMargin = MainGrid.ActualHeight - experiment.Map.MaxHeight;
            var WidthMargin = MainGrid.ActualWidth - experiment.Map.MaxWidth;
            if (HeightMargin < 0 || WidthMargin < 0)
                return false;

            //Prepare grid and drawing canvas for map 
            DrawGrid = new Grid();
            MainGrid.Children.Add(DrawGrid);
            DrawGrid.Margin = new Thickness(WidthMargin / 2, HeightMargin / 2, WidthMargin / 2, HeightMargin / 2);
            var b = new Border
            {
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.Black
            };
            DrawGrid.Children.Add(b);
            DrawCanvas = new MapCanvas();
            DrawGrid.Children.Add(DrawCanvas);
            DrawGrid.MouseRightButtonDown += MetaInfoClick;

            //PrepareThread
            ExperimentThread = new Thread(RunExperiment);

            return true;
        }
        /// <summary>
        /// Function,that runs the experiment while not finneshed
        /// </summary>
        private void RunExperiment()
        {
            while (!RunningExperiment.Finnished)
            {
                //Check Controls
                lock (ControlsLock)
                {
                    if (Stopping)
                    {
                       MainGrid.Dispatcher.Invoke(StopExperiment);
                        return;
                    }
                    else if (Pausing)
                    {
                        Running = false;
                        Paused = true;
                        Pausing = false;
                        MarkMetaInfos();
                        Monitor.Wait(ControlsLock);
                        Running = true;
                        Paused = false;
                    }
                    //Make step of Experiment: 
                    RunningExperiment.MakeStep();
                    //Draw experiment
                    DrawExperiment();
                }
            }
            MainGrid.Dispatcher.Invoke(StopExperiment);
        }
        /// <summary>
        /// Stop Experiment, change controls
        /// </summary>
        private void StopExperiment()
        {
            //HIDE BUTTONS
            StartB.Visibility = Visibility.Hidden;
            PauseB.Visibility = Visibility.Hidden;
            StopB.Visibility = Visibility.Hidden;
            //Remove grid
            MainGrid.Children.Remove(DrawGrid);
            DrawGrid = null;
            DrawCanvas = null;
            ExperimentThread = null;
            //newExperiment control show
            ExperimentComboBox.IsEditable = true;
            ExperimentComboBox.IsHitTestVisible = true;
            ExperimentComboBox.Focusable = true;
            InitExpB.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// MarkmetaInfosAboutRobots
        /// </summary>
        private void MarkMetaInfos()
        {
            
        }
        /// <summary>
        /// Draw Map & print experiment info
        /// </summary>
        private void DrawExperiment()
        {
            //Draw radio signals
            foreach (var radio in RunningExperiment.Map.RadioEntities)
            {
                
            }
            //Draw map passive entities
            foreach (var passive in RunningExperiment.Map.PasiveEntities)
            {
                
            }
            //Draw fuel
            foreach (var fuel in RunningExperiment.Map.FuelEntities)
            {
                
            }
            //Draw robots 
            foreach (var robot in RunningExperiment.Map.Robots)
            {
                
            }
        }

        //CLICK CONTROL: 
        /// <summary>
        /// Show metainfo about nearest object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetaInfoClick(object sender, MouseButtonEventArgs e)
        {
            lock (ControlsLock)
            {
                if (!Paused) return;
                Point clickPoint = e.GetPosition(DrawGrid);
                MessageBox.Show("Actual position of click X = " + clickPoint.X + ", Y = " + clickPoint.Y);
            }
        }
        /// <summary>
        /// Start button click implemetation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartClick(object sender, RoutedEventArgs e)
        {
            lock (ControlsLock)
            {
                if (Running || Pausing || Stopping)
                    return;
                if (Paused)
                {
                    Paused = false;
                    Running = true;
                    Monitor.PulseAll(ControlsLock);
                    return;
                }
                if (!Running)
                {
                    Running = true;
                    ExperimentThread.Start();
                    StopB.Visibility = Visibility.Visible;
                    PauseB.Visibility = Visibility.Visible;
                }
            }
        }
        /// <summary>
        /// Pause current Experiment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PauseClick(object sender, RoutedEventArgs e)
        {
            lock (ControlsLock)
            {
                if (!Running)
                {
                    MessageBox.Show("No experiment running");
                    return;
                }
                Pausing = true;
            }
        }
        /// <summary>
        /// Stop current Experiment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StopClick(object sender, RoutedEventArgs e)
        {
            lock (ControlsLock)
            {
                if (!Stopping)
                {
                    if (MessageBox.Show(this, "Cancel Experiment", "Are you sure?", MessageBoxButton.YesNo) ==
                        MessageBoxResult.Yes)
                    {
                        Stopping = true;
                        if (!Running)
                            Monitor.PulseAll(ControlsLock);
                    }
                }

            }
            
        }
    }
}
