using System.Numerics;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Effectors
{
    /// <summary>
    /// Refactor entity on top of container, if mineral entity
    /// </summary>
    public class MineralRefactor : CircleEntity,IEffector
    {
        protected int CyclesToEnd;
        protected float FuelToRefactor;
        protected bool Refactoring;
        public Bounds[] LocalBounds { get; }
        public NormalizeFunc[] NormalizeFuncs { get; protected set;  }
        public int Dimension { get; }
        /// <summary>
        /// Create new Mineral Refactor 
        /// </summary>
        /// <param name="robot"></param>
        public MineralRefactor(RobotEntity robot) : base(robot.Middle,0,"Mineral Refactor Effector")
        {
            //Create normalization fncs
            LocalBounds = new Bounds[1];
            LocalBounds[0] = new Bounds() {Max = 2,Min=0};
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound,LocalBounds);
        }

        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }


        public void ConnectToRobot(RobotEntity robot)
        {
            this.MoveTo(robot.Middle);
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound, LocalBounds);
        }

        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            //Update position 
            this.MoveTo(robot.Middle);

            var s = settings.Normalize(NormalizeFuncs)[0];
            //Refactor
            if (s >= 0 && s <= 1)
            {
                //refactor currently running 
                if (Refactoring)
                {
                    if (CyclesToEnd == 0)
                    {
                        if (robot.ActualContainerSize < robot.ContainerMaxCapacity)
                        {
                            Refactoring = false;
                            robot.PushContainer(new FuelEntity(Vector2.Zero, FuelEntity.FuelRadius, FuelToRefactor));
                            FuelToRefactor = 0;
                            return;
                        }
                        else
                        {
                            //Non empty container
                            robot.InvalidOperationWithRefactor++;
                            return;
                        }
                    }
                    else
                    {
                        CyclesToEnd--;
                        return;
                    }
                }
                else
                {
                    if (robot.PeekContainer() == null || !(robot.PeekContainer() is MineralEntity))
                    {
                        robot.InvalidOperationWithRefactor++;
                        return;
                    }
                    else
                    {
                        //Dismantle mineral
                        var m = (MineralEntity) robot.PopContainer();
                        Refactoring = true;
                        CyclesToEnd = m.CycleToRefactor;
                        FuelToRefactor = m.FuelToRefactor;
                        return; 
                    }
                }

            }
            //ELSE Idle 
        }


        public IEffector Clone()
        {
            return (IEffector) DeepClone();
        }
    }
}