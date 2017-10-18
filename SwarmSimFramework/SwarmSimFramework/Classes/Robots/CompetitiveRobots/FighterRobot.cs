using System.Numerics;
using SwarmSimFramework.Classes.Effectors;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Robots.MineralRobots
{
    /// <summary>
    /// FighterRobot for the 3.scenario
    /// </summary>
    public class FighterRobot:RobotEntity
    {
        public FighterRobot() : this(Vector2.Zero) { }
        public FighterRobot(Vector2 middle, float amountOfFuel=0,float orientation = 0) 
            : base(middle, 5, "WorkerRobot", null, null, amountOfFuel, 5, 1, 100, 100, -100, orientation)
        {
            Health = 1500;
            ISensor[] sensors = new ISensor[8];
            //Line Type Sensors
            sensors[0] = new LineTypeSensor(this,30, DegreesToRadians(45));
            sensors[1] = new LineTypeSensor(this, 30, 0);
            sensors[2] = new LineTypeSensor(this, 30, DegreesToRadians(-45));
            //Locator
            sensors[3] = new LocatorSensor(this);
            //Touch sensors
            sensors[4] = new TouchSensor(this, 0.1f, DegreesToRadians(90));
            sensors[5] = new TouchSensor(this, 0.1f, DegreesToRadians(180));
            sensors[6] = new TouchSensor(this, 0.1f, DegreesToRadians(270));
            //Radio sensor 
            sensors[7] = new RadioSensor(this, 100);

            this.Sensors = sensors;

            IEffector[] effectors = new IEffector[7];
            effectors[0] = new TwoWheelMotor(this, 1.5f);
            effectors[1] = new RadioTransmitter(this, 200);
            effectors[2] = new Picker(this,20,0);
            effectors[3] = new Weapon(this,15,500,DegreesToRadians(45));
            effectors[4] = new Weapon(this, 15, 500, DegreesToRadians(0));
            effectors[5] = new Weapon(this, 15, 500, DegreesToRadians(-45));
            effectors[6] = new Weapon(this,15,500,DegreesToRadians(180));

            this.Effectors = effectors;

            this.CountDimension();


        }
    }
}