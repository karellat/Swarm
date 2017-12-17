using System.Numerics;
using SwarmSimFramework.Classes.Effectors;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Robots.WoodRobots
{
    public class ScoutCutterRobotMem:RobotEntity
    {
        public ScoutCutterRobotMem() : this(Vector2.Zero)
        {
        } 
        public ScoutCutterRobotMem(Vector2 middle, float orientation = 0)
            : base(middle, 2.5f, "WoodCutterM", null, null, 100, 0, 1, 100, 100, -100, orientation)
        {
            MemoryStick mem = new MemoryStick(10,this);
            ISensor[] sensors = new ISensor[15];
            //Line Type Sensors
            sensors[0] = new LineTypeSensor(this, 50, DegreesToRadians(45));
            sensors[1] = new LineTypeSensor(this, 50, 0);
            sensors[2] = new LineTypeSensor(this, 50, DegreesToRadians(-45));
            //Locator
            sensors[3] = new LocatorSensor(this);
            //Type Line sensor
            sensors[4] = new TypeCircleSensor(this, 50);
            //Radio sensor 
            sensors[5] = new RadioSensor(this, 100);
            sensors[6] = mem;
            //Touch sensors
            for (int i = 0; i < 8; i++)
            {
                sensors[i + 7] = new TouchSensor(this, 1.0f, DegreesToRadians(i * 45));
            }


            this.Sensors = sensors;

            IEffector[] effectors = new IEffector[4];
            effectors[0] = new TwoWheelMotor(this, 3);
            effectors[1] = new RadioTransmitter(this, new[] { -1,0 }, 200);
            effectors[2] = new WoodRefactor(this, 10, 0);
            effectors[3] = mem;
            this.Effectors = effectors;

            this.CountDimension();
        }
    }
}