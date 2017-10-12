using System.Numerics;
using SwarmSimFramework.Classes.Effectors;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Robots.MineralRobots
{
    public class RefactorRobot:RobotEntity
    {
        public RefactorRobot(int amountOfFuel) : this(Vector2.Zero, amountOfFuel) { }
        public RefactorRobot(Vector2 middle, float amountOfFuel, float orientation = 0)
            : base(middle, 10, "RefactorRobot", null, null, amountOfFuel, 5, 1, 100, 100, -100, orientation)
        {
            //

            ISensor[] sensors = new ISensor[6];
            sensors[0] = new FuelLineSensor(this,10,0);
            sensors[1] = new LineTypeSensor(this,10,0);
            //Touch sensors
            sensors[2] = new TouchSensor(this, 0.1f, DegreesToRadians(90));
            sensors[3] = new TouchSensor(this, 0.1f, DegreesToRadians(180));
            sensors[4] = new TouchSensor(this, 0.1f, DegreesToRadians(270));

            sensors[5] = new RadioSensor(this, 100);

            this.Sensors = sensors;

            IEffector[] effectors = new IEffector[6];
            effectors[0] = new RadioTransmitter(this,200);
            effectors[1] = new TwoWheelMotor(this,0.5f);
            effectors[2] = new Picker(this,20,0);
            effectors[3] = new Picker(this,20,DegreesToRadians(90));
            effectors[4] = new Picker(this, 20, DegreesToRadians(180));
            effectors[5] = new Picker(this, 20, DegreesToRadians(270));

            this.Effectors = effectors; 

            this.CountDimension();
            
        }
    }
}