using System;
using System.Numerics;
using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Effectors
{
    /// <summary>
    /// Refactor entity on top of container, if mineral entity
    /// </summary>
    public class MineralRefactor : CircleEntity,IEffector
    { 
        /// <summary>
        /// Cycles left to complete refactor from mineral to fuel 
        /// </summary>
        public int CyclesToEnd { get; protected set; }
        /// <summary>
        /// Amount of fuel created by refactor 
        /// </summary>
        public float FuelToRefactor { get; protected set; }
        /// <summary>
        /// curently refactoring mineral or waiting for empty stack 
        /// </summary>
        public bool Refactoring { get; protected set; }
        /// <summary>
        /// Local bounds of setting refactor
        /// </summary>
        public Bounds[] LocalBounds { get; }
        /// <summary>
        /// Normalization funcs from robot values to local bounds
        /// </summary>
        public NormalizeFunc[] NormalizeFuncs { get; protected set;  }
        /// <summary>
        /// Dimension of setting
        /// </summary>
        public int Dimension { get; }
        /// <summary>
        /// Create new Mineral Refactor 
        /// </summary>
        /// <param name="robot"></param>
        public MineralRefactor(RobotEntity robot) : base(robot.Middle,0,"Mineral Refactor Effector")
        {
            //Create normalization fncs
            Dimension = 1;
            LocalBounds = new Bounds[1];
            LocalBounds[0] = new Bounds() {Max = 2,Min=0};
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound,LocalBounds);
        }
        /// <summary>
        /// Create clone of refactor 
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            var r =  (MineralRefactor) this.MemberwiseClone();
            r.Refactoring = false;
            FuelToRefactor = 0;
            CyclesToEnd = 0;
            return r;
        }
        /// <summary>
        /// Connect to robot
        /// </summary>
        /// <param name="robot"></param>
        public void ConnectToRobot(RobotEntity robot)
        {
            this.MoveTo(robot.Middle);
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound, LocalBounds);
        }
        /// <summary>
        /// Make refactor step if (0,1) - take Mineral Entity from stack or make refactor cycle if currently refactoring
        /// if 1,2 idle
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            //Update position 
            this.MoveTo(robot.Middle);
            lastSettigs = settings;
            var s = settings.Normalize(NormalizeFuncs)[0];
            //Refactor
            if (s >= 0 && s <= 1)
            {
                //refactor currently running 
                if (Refactoring)
                {
                    if (CyclesToEnd == 0)
                    {
                        if (robot.ActualContainerCount < robot.ContainerMaxCapacity)
                        {
                            Refactoring = false;
                            robot.PushContainer(new FuelEntity(Vector2.Zero, FuelEntity.FuelRadius, FuelToRefactor));
                            FuelToRefactor = 0;
                            return;
                        }
                        else
                        {
                            //Non empty container
                            robot.InvalidRefactorOperation++;
                            return;
                        }
                    }
                    else
                    {
                        //Refactoring
                        CyclesToEnd--;
                        return;
                    }
                }
                else
                {
                    if (robot.PeekContainer() == null || !(robot.PeekContainer() is RawMaterialEntity))
                    {
                        robot.InvalidRefactorOperation++;
                        return;
                    }
                    else
                    {
                        //Dismantle mineral
                        var m = (RawMaterialEntity) robot.PopContainer();
                        Refactoring = true;
                        CyclesToEnd = m.CycleToRefactor;
                        FuelToRefactor = m.MaterialToRefactor;
                        return; 
                    }
                }

            }
            //ELSE Idle 
        }
        /// <summary>
        /// Clone of effector 
        /// </summary>
        /// <returns></returns>
        public IEffector Clone()
        {
            return (IEffector) DeepClone();
        }
        /// <summary>
        /// Last settings of refactor
        /// </summary>
        public float[] lastSettigs = new float[1];

        /// <summary>
        /// Log Mineral refactor
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Log()
        {
            StringBuilder s = new StringBuilder("Mineral refactor: ");
            s.AppendLine("\t" + base.Log());
            s.AppendLine("\t Refactoring: " + this.Refactoring.ToString() + " Cycles to go: " + this.CyclesToEnd +
                         " Fuel to refactor: " + this.FuelToRefactor);
            s.AppendLine("\t Last settings: " + lastSettigs[0]);
            return s;
        }
    }
}