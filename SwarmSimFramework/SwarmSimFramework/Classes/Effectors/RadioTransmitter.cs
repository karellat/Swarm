using System.Numerics;
using System.Security.Policy;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.SupportClasses;
using System;
using System.Text;

namespace SwarmSimFramework.Classes.Effectors
{
    /// <summary>
    /// State control sensor to create signals
    /// </summary>
    public class RadioTransmitter : CircleEntity,IEffector
    {
        /// <summary>
        /// Intern radio signal
        /// </summary>
        protected RadioEntity radioSignal;
        /// <summary>
        /// Dimension of effector 
        /// </summary>
        public int Dimension { get; }
        /// <summary>
        /// Add transmitting to map 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            //Update position
            this.MoveTo(robot.Middle);
            radioSignal.MoveTo(robot.Middle);

            lastSettings = settings;
            float max = settings[0];
            int index = 0;
            for (int i = 1; i < settings.Length; i++)
            {
                if (settings[i] > max)
                {
                    index = i;
                    max = settings[i];
                }
            }
            //Change signal value & added it to the map 
            radioSignal.ValueOfSignal = index;
            map.RadioEntities.Add(radioSignal); 
        }
        /// <summary>
        /// Local bounds of internvalues, transmitting Value
        /// </summary>
        public Bounds[] LocalBounds { get; }
        /// <summary>
        /// Normalization funcs to robot bounds
        /// </summary>
        public NormalizeFunc[] NormalizeFuncs { get; protected set;  }
        /// <summary>
        /// Create new radio transmitting effector 
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="radiusOfTransmitting"></param>
        public RadioTransmitter(RobotEntity robot, float radiusOfTransmitting) : base(robot.Middle, 0,
            "RadioTransmitter")
        {
            //Create representation of radio signal 
            radioSignal = new RadioEntity(robot.Middle,radiusOfTransmitting,0);
            //Create localBounds and normalization fncs
            Dimension = 3;
            LocalBounds = new Bounds[Dimension];
            for (int i = 0; i < Dimension; i++)
            {
                LocalBounds[i] = new Bounds() {Min = -100, Max = 100};
            }
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound, LocalBounds);

        }
        /// <summary>
        /// Create deep clone 
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            var r = (RadioTransmitter) this.MemberwiseClone();
            r.radioSignal = (RadioEntity) r.radioSignal.DeepClone();
            return r;
        }
        /// <summary>
        /// Connect to robot
        /// </summary>
        /// <param name="robot"></param>
        public void ConnectToRobot(RobotEntity robot)
        {
            //Change position 
            Middle = robot.Middle;
            FPoint = robot.Middle;
            //Make new normalization fncs 
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound, LocalBounds);

        }
        /// <summary>
        /// Create clone of sensor 
        /// </summary>
        /// <returns></returns>
        public IEffector Clone()
        {
            return (IEffector)DeepClone();
        }
        /// <summary>
        /// 
        /// </summary>
        public float[] lastSettings = new float[4];
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Log()
        {
            StringBuilder s = new StringBuilder("RadioTransmitter: ");
            s.AppendLine("\t" + base.Log());
            s.Append("\t "); 
            for (int i = 0; i < lastSettings.Length; i++)
            {
                s.Append(i + ": " + lastSettings[i] + "; ");
            }
            s.AppendLine();
            return s;
        }
    }
}