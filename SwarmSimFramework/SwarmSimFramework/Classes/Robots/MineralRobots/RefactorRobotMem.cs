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
            ISensor[] sensors = new ISensor[11];
            //Fuel & Line sensor 
            sensors[0] = new FuelLineSensor(this,70,0);
            sensors[1] = new LineTypeSensor(this,70,0);

            //Touch sensors
            sensors[2] = new TouchSensor(this, 0.3f, DegreesToRadians(90));
            sensors[3] = new TouchSensor(this, 0.3f, DegreesToRadians(180));
            sensors[4] = new TouchSensor(this, 0.3f, DegreesToRadians(270));

            sensors[5] = new RadioSensor(this, 100);
            sensors[6] = mem;
            sensors[7] = new FuelLineSensor(this, 70, DegreesToRadians(35));
            sensors[8] = new LineTypeSensor(this, 70, DegreesToRadians(35));
            sensors[9] = new FuelLineSensor(this, 70, DegreesToRadians(-35));
            sensors[10] = new LineTypeSensor(this, 70, DegreesToRadians(-35));

            this.Sensors = sensors;

            IEffector[] effectors = new IEffector[8];
            effectors[0] = new RadioTransmitter(this, new[] { -1,2 }, 200);
            effectors[1] = new TwoWheelMotor(this,1);
            effectors[2] = new Picker(this,20,0);
            effectors[3] = new Picker(this,20,DegreesToRadians(90));
            effectors[4] = new Picker(this, 20, DegreesToRadians(180));
            effectors[5] = new Picker(this, 20, DegreesToRadians(270));
            effectors[6] = mem;
            effectors[7] = new MineralRefactor(this);
            this.Effectors = effectors; 

            this.CountDimension();
            
        }
    }
}