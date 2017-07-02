using System.Numerics;
using SwarmSimFramework.Classes.Effectors;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Robots
{
    public class WoodWorkerRobotMem : RobotEntity
    {
        public WoodWorkerRobotMem(Vector2 middle, float orientation = 0)
            : base(middle, 5f, "WorkerRobotMEM", null, null, 100, 5, 1, 100, 100, -100, orientation)
        {
            ISensor[] sensors = new ISensor[9];
            //Line Type Sensors
            sensors[0] = new LineTypeSensor(this, 25, DegreesToRadians(45));
            sensors[1] = new LineTypeSensor(this, 25, 0);
            sensors[2] = new LineTypeSensor(this, 25, DegreesToRadians(-45));
            //Locator
            sensors[3] = new LocatorSensor(this);
            //Touch sensors
            sensors[4] = new TouchSensor(this, 0.1f, DegreesToRadians(90));
            sensors[5] = new TouchSensor(this, 0.1f, DegreesToRadians(180));
            sensors[6] = new TouchSensor(this, 0.1f, DegreesToRadians(270));
            //Radio sensor 
            sensors[7] = new RadioSensor(this, 100);
            //Memory
            var m = new MemoryStick(10,this);
            sensors[8] = m;
            this.Sensors = sensors;

            IEffector[] effectors = new IEffector[4];
            effectors[0] = new TwoWheelMotor(this, 2);
            effectors[1] = new RadioTransmitter(this, 200);
            effectors[2] = new Picker(this, 10, 0);
            effectors[3] = m;
            this.Effectors = effectors;

            this.CountDimension();
        }
    }

}