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

            var s = settings.Normalize(NormalizeFuncs);
        }


        public IEffector Clone()
        {
            return (IEffector) DeepClone();
        }
    }
}