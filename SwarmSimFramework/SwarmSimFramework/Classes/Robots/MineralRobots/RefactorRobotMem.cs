using System.Numerics;
using SwarmSimFramework.Classes.Effectors;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Robots.MineralRobots
{
    public class RefactorRobotMem:RobotEntity
    {
        public RefactorRobotMem(int amountOfFuel) : this(Vector2.Zero, amountOfFuel) { }
        public RefactorRobotMem(Vector2 middle, float amountOfFuel, float orientation = 0)
            : base(middle, 10, "RefactorRobot", null, null, amountOfFuel, 5, 1, 100, 100, -100, orientation)
        {
            //set fuel consumation
            this.BurnFuelPerMove = 1;

            //Memory stick 
            MemoryStick mem = new MemoryStick(10, this);
            ISensor[] sensors = new ISensor[7];
            //Fuel & Line sensor 
            sensors[0] = new FuelLineSensor(this,10,0);
            sensors[1] = new LineTypeSensor(this,10,0);
            //Touch sensors
            sensors[2] = new TouchSensor(this, 0.1f, DegreesToRadians(90));
            sensors[3] = new TouchSensor(this, 0.1f, DegreesToRadians(180));
            sensors[4] = new TouchSensor(this, 0.1f, DegreesToRadians(270));

            sensors[5] = new RadioSensor(this, 100);
            sensors[6] = mem;
            this.Sensors = sensors;

            IEffector[] effectors = new IEffector[7];
            effectors[0] = new RadioTransmitter(this,200);
            effectors[1] = new TwoWheelMotor(this,0.5f);
            effectors[2] = new Picker(this,20,0);
            effectors[3] = new Picker(this,20,DegreesToRadians(90));
            effectors[4] = new Picker(this, 20, DegreesToRadians(180));
            effectors[5] = new Picker(this, 20, DegreesToRadians(270));
            effectors[6] = mem;
            this.Effectors = effectors; 

            this.CountDimension();
            
        }
    }
}