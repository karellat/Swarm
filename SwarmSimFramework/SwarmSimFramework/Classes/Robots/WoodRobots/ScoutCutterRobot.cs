using System.Numerics;
using SwarmSimFramework.Classes.Effectors;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Robots.WoodRobots
{
    public class ScoutCutterRobot : RobotEntity
    {
        public ScoutCutterRobot() : this(Vector2.Zero)
        {
        }

        public ScoutCutterRobot(Vector2 middle, float orientation = 0)
            : base(middle, 2.5f, "ScoutRobot", null, null, 100, 0, 1, 100, 100, -100, orientation)
        {
            ISensor[] sensors = new ISensor[9];

            //Line Type Sensors
            sensors[0] = new LineTypeSensor(this, 50, DegreesToRadians(45));
            sensors[1] = new LineTypeSensor(this, 50, 0);
            sensors[2] = new LineTypeSensor(this, 50, DegreesToRadians(-45));
            //Locator
            sensors[3] = new LocatorSensor(this);
            //Type Line sensor
            sensors[4] = new TypeCircleSensor(this, 50);
            //Touch sensors
            sensors[5] = new TouchSensor(this, 0.1f, DegreesToRadians(90));
            sensors[6] = new TouchSensor(this, 0.1f, DegreesToRadians(180));
            sensors[7] = new TouchSensor(this, 0.1f, DegreesToRadians(270));
            //Radio sensor 
            sensors[8] = new RadioSensor(this, 100);

            this.Sensors = sensors;

            IEffector[] effectors = new IEffector[3];
            effectors[0] = new TwoWheelMotor(this, 3);
            effectors[1] = new WoodRefactor(this,10,0);
            effectors[2] = new RadioTransmitter(this, 200);

            this.Effectors = effectors;

            this.CountDimension();
    
        }
    }
}