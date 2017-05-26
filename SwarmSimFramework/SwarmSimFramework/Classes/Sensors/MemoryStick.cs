using System;
using System.Security.Policy;
using System.Text;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{
    /// <summary>
    /// Additional memory 
    /// </summary>
    public class MemoryStick : CircleEntity,ISensor,IEffector
    {
        /// <summary>
        /// Created memory stick with given size 
        /// </summary>
        /// <param name="sizeOFMemory"></param>
        /// <param name="robot"></param>
        public MemoryStick(int sizeOFMemory,RobotEntity robot) : base(robot.Middle,0,"Memory stick")
        {
            //No need for orientation 
            FPoint = Middle;
            //Set bounds 
            Dimension = sizeOFMemory;
            Memory = new float[sizeOFMemory];
            LocalBounds = new Bounds[Dimension];
            for (int i = 0; i < Dimension; i++)
            {
                LocalBounds[i] = robot.NormalizedBound;
            }
            //no need for normalization

        }
        /// <summary>
        /// Make Deep clone of entity
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }
        /// <summary>
        /// return saved values 
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public float[] Count(RobotEntity robot, Map.Map map)
        {
           //Update middle 
           if(robot.Middle != Middle)
                this.MoveTo(robot.Middle);

           return Memory;
        }
        /// <summary>
        /// Size of memory
        /// </summary>
        public int Dimension { get; protected set; }
        /// <summary>
        /// Set bounds  and middle to local 
        /// </summary>
        /// <param name="robot"></param>
        public void ConnectToRobot(RobotEntity robot)
        {
            //Clean memory
            for (int i = 0; i < Dimension; i++)
            {
                Memory[i] = 0;
            }
            //Update middle 
            if (robot.Middle != Middle)
                this.MoveTo(robot.Middle);

            //set bouds
            for (int i = 0; i < Dimension; i++)
            {
                LocalBounds[i] = robot.NormalizedBound;
            }
        }
        /// <summary>
        /// Write to the memory
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        public void Effect(float[] settings, RobotEntity robot, Map.Map map)
        {
            //Update middle 
            if (robot.Middle != Middle)
                this.MoveTo(robot.Middle);

            if (settings.Length != Dimension)
                throw new ArgumentException("Not supported length of saving values");
            for (int i = 0; i < settings.Length; i++)
            {
                if (settings[i] < LocalBounds[i].Min || settings[i] > LocalBounds[i].Max)
                    throw new ArgumentException("Saving not supported value");
                Memory[i] = settings[i];
            }

        }
        /// <summary>
        /// Local bounds 
        /// </summary>
        public Bounds[] LocalBounds { get; }
        /// <summary>
        /// Unnecessary normalization fncs 
        /// </summary>
        public NormalizeFunc[] NormalizeFuncs { get; }
        /// <summary>
        /// Intern implementation of memory
        /// </summary>
        public float[] Memory;
        /// <summary>
        /// Log actual memory stick
        /// </summary>
        /// <returns></returns>
        public override StringBuilder Log()
        {
            StringBuilder s = new StringBuilder("Memory: ");
            s.AppendLine("\t" + base.Log());
            for (int i = 0; i < Dimension; i++)
            {
                s.AppendLine("\t [" + i + "] = " + Memory[i]);
            }
            return s;
        }
        /// <summary>
        /// Clone memory
        /// </summary>
        /// <returns></returns>
        IEffector IEffector.Clone()
        {
            return (IEffector) DeepClone();
        }
        /// <summary>
        /// Clone memory
        /// </summary>
        /// <returns></returns>
        ISensor ISensor.Clone()
        {
            return (ISensor) DeepClone();
        }
    }
}