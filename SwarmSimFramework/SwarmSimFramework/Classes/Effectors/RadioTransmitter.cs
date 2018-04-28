using System.Numerics;
using System.Security.Policy;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.SupportClasses;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

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
        [JsonProperty]
        protected RadioEntity radioSignal;
        /// <summary>
        /// Dimension of effector 
        /// </summary>
        [JsonProperty]
        public int Dimension { get; protected set; }

        [JsonProperty]
        private int[] availableSignals;

        /// <summary>
        /// Add transmitting to map 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            

        Debug.Assert(settings.Length == availableSignals.Length);
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
            if (availableSignals[index] < 0)
                return;
            
            radioSignal.ValueOfSignal = availableSignals[index];
            map.RadioEntities.Add(radioSignal); 
        }
        /// <summary>
        /// Local bounds of internvalues, transmitting Value
        /// </summary>
        [JsonProperty]
        public Bounds[] LocalBounds { get; protected set; }
        /// <summary>
        /// Normalization funcs to robot bounds
        /// </summary>
        [JsonProperty]
        public NormalizeFunc[] NormalizeFuncs { get; protected set;  }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="robot"> Robot that will be connect to the transmitter </param>
        /// <param name="availableSignals"> if -1 do not transmit </param>
        /// <param name="radiusOfTransmitting"> radius of the signal</param>
        public RadioTransmitter(RobotEntity robot,int[] availableSignals, float radiusOfTransmitting) : base(robot.Middle, 0,
            "RadioTransmitter")
        {
            //Create representation of radio signal 
            radioSignal = new RadioEntity(robot.Middle,radiusOfTransmitting,0);
            Debug.Assert(availableSignals.All((x) => x >= -1 && x <= 2 ));
            this.availableSignals = availableSignals; 
            //Create localBounds and normalization fncs
            Dimension = availableSignals.Length;
            LocalBounds = new Bounds[Dimension];
            for (int i = 0; i < Dimension; i++)
            {
                LocalBounds[i] = new Bounds() {Min = -100, Max = 100};
            }
            NormalizeFuncs = MakeNormalizeFuncs(robot.NormalizedBound, LocalBounds);

        }
        /// <summary>
        /// Json constructor
        /// </summary>
        [JsonConstructor]
        protected RadioTransmitter() : base("RadioTransmiter")
        {
            
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