using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpDX.Direct3D9;
using SwarmSimFramework.Classes.Experiments;
using SwarmSimFramework.Interfaces;
using System.Threading;
using System.Windows.Threading;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Experiments.WoodCuttingExperiment;

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
            ExperimentComboBox.SelectedIndex = 3;
            ExperimentComboBox.Items.Add("None");
            ExperimentComboBox.Items.Add("TestingExperiment");
            ExperimentComboBox.Items.Add("WalkingExperiment");
            ExperimentComboBox.Items.Add("WoodCuttingExperiment - Walk");

            //wait after draw
            ThreadWaitComboBox.SelectedIndex = 0;
            ThreadWaitComboBox.Items.Add("0");
            ThreadWaitComboBox.Items.Add("100");
            ThreadWaitComboBox.SelectionChanged += ((sender, args) =>
            {
                if ((sender as ComboBox).SelectedIndex == 0)
                    ThreadWait = 0;
                else if ((sender as ComboBox).SelectedIndex == 1)
                    ThreadWait = 100;
            });
            //Drawing state, if draw or not
            VisualCombox.SelectedIndex = 0;
            VisualCombox.Items.Add("On");
            VisualCombox.Items.Add("Off");
            VisualCombox.SelectionChanged += ((sender, args) =>
            {
                if ((sender as ComboBox).SelectedIndex == 0)
                    Visualization = true;
                else
                    Visualization = false;
            });

            //Drawing state, if draw or not
            InfoCombox.SelectedIndex = 0;
            InfoCombox.Items.Add("On");
            InfoCombox.Items.Add("Off");
            InfoCombox.SelectionChanged += ((sender, args) =>
            {
                if ((sender as ComboBox).SelectedIndex == 0)
                    ShowInfo = true;
                else
                    ShowInfo = false;
            });


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
        /// <summary>
        /// All info windows of current  WPF Control
        /// </summary>
        public List<InfoWindow> infoWindows = new List<InfoWindow>();
        /// <summary>
        /// Info lock 
        /// </summary>
        public object infoLock = new object();
        //STATES of simulation
        public bool Running;
        public bool Pausing;
        public bool Stopping;
        public bool Paused;
        public int ThreadWait = 0;
        public bool Visualization = true;
        public bool ShowInfo = true;

        /// <summary>
        /// Loc of the controls 
        /// </summary>
        private object ControlsLock = new object();
        /// <summary>
        /// Infos about specific points 
        /// </summary>
        private static List<MetaInfo> metaInfos = new List<MetaInfo>();
        /// <summary>
        /// Control of experiment
        /// </summary>
        private static Thread ExperimentThread;

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
                case (2): 
                    RunningExperiment = new WalkingExperiment();
                    break;
                case (3):
                    RunningExperiment = new WoodCuttingExperimentWalking();
                    break;
                default:
                    MessageBox.Show("Unknown experiment");
                    return;
            }
            //Maximaze window
            WindowState = WindowState.Maximized;
            RunningExperiment.Init();
            //Hide controls 
            InitExpB.Visibility = Visibility.Hidden;
            
            
            if (!PrepareExperiment(RunningExperiment))
            {
                MessageBox.Show("Small height and width to show whole map");
                return;
            }
            BasicInfo.Visibility = Visibility.Visible;
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
                    //Draw experisiment
                    DrawExperiment();
                    //Wait
                    if(ThreadWait > 0) Thread.Sleep(ThreadWait);
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
            BasicInfo.Visibility = Visibility.Hidden;
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
            metaInfos.Clear();
            foreach (var r in RunningExperiment.Map.Robots)
                metaInfos.Add(new MetaInfo() {Info = r.Log(), Middle = r.Middle, Radius = r.Radius});
            foreach (var p in RunningExperiment.Map.PasiveEntities)
                metaInfos.Add(new MetaInfo() {Info = p.Log(),Middle =p.Middle,Radius = p.Radius});
            foreach (var f in RunningExperiment.Map.FuelEntities)
                metaInfos.Add(new MetaInfo() {Info = f.Log(), Middle = f.Middle,Radius = f.Radius});
        }
        /// <summary>
        /// Draw Map & print experiment info
        /// </summary>
        private void DrawExperiment()
        {
            //Mark Metainfos
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new DrawBasicInfo(() => BasicInfo.Text = RunningExperiment.ExperimentInfo.ToString()));
            //Draw info about generation
            if (RunningExperiment.FinnishedGeneration)
            {
                string text = RunningExperiment.GenerationInfo.ToString();
                if (!ShowInfo) return;
                Dispatcher.Invoke(() =>
                {
                    lock (infoLock)
                    {
                        var w = new InfoWindow(text, this);
                        infoWindows.Add(w);
                        w.Show();
                        return;
                    }
                });

            }
            //If no visualization do not draw robots
            if (!Visualization) return;
            //Draw radio signals
            foreach (var radio in RunningExperiment.Map.RadioEntities)
            {
                DrawCanvas.AddCircle(radio.Middle,radio.Radius,"SIGNAL" + radio.ValueOfSignal);
            }
            //Draw map passive entities
            foreach (var passive in RunningExperiment.Map.PasiveEntities)
            {
                if (passive.Color != Entity.EntityColor.RawMaterialColor)
                    DrawCanvas.AddCircle(passive.Middle, passive.Radius, passive.Color.ToString());
                else
                {
                    if((passive as RawMaterialEntity).Discovered)
                        DrawCanvas.AddCircle(passive.Middle, passive.Radius, passive.Color.ToString()+'D');
                    else
                        DrawCanvas.AddCircle(passive.Middle, passive.Radius, passive.Color.ToString() + 'N');

                }

            }
            //Draw fuel
            foreach (var fuel in RunningExperiment.Map.FuelEntities)
            {
                DrawCanvas.AddCircle(fuel.Middle,fuel.Radius,fuel.Color.ToString());
            }
            //Draw robots 
            foreach (var robot in RunningExperiment.Map.Robots)
            {
                DrawCanvas.AddCircle(robot.Middle,robot.Radius,"ROBOT" + robot.TeamNumber);
                DrawCanvas.AddLine(robot.Middle,robot.FPoint,"HEAD",3);
                foreach (var s in robot.Sensors)
                {
                    if (s is LineEntity)
                    {
                        var l = (LineEntity) s;
                        DrawCanvas.AddLine(l.A,l.B,"LINESENSOR",1);
                    }
                   else if (s is CircleEntity)
                    {
                        var c = (CircleEntity) s;
                        DrawCanvas.AddCircle(c.Middle,c.Radius,"CIRCLESENSOR");
                    }
                }

                foreach (var e in robot.Effectors)
                {
                    if (e is LineEntity)
                    {
                        var l = (LineEntity)e;
                        DrawCanvas.AddLine(l.A, l.B, "LINEEFECTOR", 1);
                    }
                    else if (e is CircleEntity)
                    {
                        var c = (CircleEntity)e;
                        DrawCanvas.AddCircle(c.Middle, c.Radius, "CIRCLEEFFECTOR");
                    }
                }

            }

            //Finish frame
            DrawCanvas.CompleteFrame();


        }
        private  delegate void DrawBasicInfo();


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
                Vector2 v = new Vector2((float) clickPoint.X,(float) clickPoint.Y);
     
                foreach (var i in metaInfos)
                {
                    if (i.Contains(v))
                    {
                        lock (infoLock)
                        {
                            var w = new InfoWindow(i.Info.ToString(), this);
                            infoWindows.Add(w);
                            w.Show();
                            return;
                        }
                    }
                }

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

        protected override void OnClosing(CancelEventArgs e)
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
            lock (infoLock)
            {
                Stack<InfoWindow> copy = new Stack<InfoWindow>(infoWindows.Count);
                foreach (var i in infoWindows)
                {
                    copy.Push(i);
                }

                while (copy.Count != 0)
                {
                    copy.Pop().Close();
                }
            }
            Thread.Sleep(600);
            base.OnClosing(e);
        }
        /// <summary>
        /// Close infoWindow with  metainfo clear from the list 
        /// </summary>
        /// <param name="info"></param>
        public void RemoveInfo(InfoWindow info)
        {
            lock (infoLock)
            {
                infoWindows.Remove(info);
            }
        }

        
    }
    /// <summary>
    /// Meta info about point
    /// </summary>
    public struct MetaInfo
    {
        public Vector2 Middle;
        public float Radius;
        public StringBuilder Info;

        public bool Contains(Vector2 point)
        {
            if (Vector2.DistanceSquared(point, Middle) <= Radius * Radius)
                return true;
            return false;
        }
    }

    
}
