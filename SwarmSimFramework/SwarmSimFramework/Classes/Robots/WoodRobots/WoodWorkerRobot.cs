using System.Numerics;
using SwarmSimFramework.Classes.Effectors;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Robots.WoodRobots
{
    public class WoodWorkerRobot : RobotEntity
    {
        public WoodWorkerRobot() : this(Vector2.Zero) { } 
        public WoodWorkerRobot(Vector2 middle, float orientation = 0)
            : base(middle, 5f, "WoodWorker", null, null, 100, 5, 1, 100, 100, -100, orientation)
        {
            ISensor[] sensors = new ISensor[13];
            //Line Type Sensors
            sensors[0] = new LineTypeSensor(this, 30, DegreesToRadians(45));
            sensors[1] = new LineTypeSensor(this, 30, 0);
            sensors[2] = new LineTypeSensor(this, 30, DegreesToRadians(-45));
            //Locator
            sensors[3] = new LocatorSensor(this);
            //Radio sensor 
            sensors[4] = new RadioSensor(this, 100);
            //Touch sensors
            for (int i = 0; i < 8; i++)
            {
                sensors[i + 5] = new TouchSensor(this, 2.0f, DegreesToRadians(i * 45));
            }
            this.Sensors = sensors;

            IEffector[] effectors = new IEffector[3];
            effectors[0] = new TwoWheelMotor(this, 2);
            effectors[1] = new RadioTransmitter(this, new[] { -1 ,1}, 200);
            effectors[2] = new Picker(this, 10, 0);
            this.Effectors = effectors;

            this.CountDimension();
        }
    }
}