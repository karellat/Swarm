using System.Numerics;
using SwarmSimFramework.Classes.Effectors;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Robots
{
    public class ScoutRobot : RobotEntity
    {
        public ScoutRobot(Vector2 middle, float radius, float amountOfFuel,float orientation = 0 )
            : base(middle, radius, "ScoutRobot",null ,null , amountOfFuel, 0, 1, 100, 100, -100, orientation)
        {
            ISensor[] newSensors = new ISensor[12];
            //FUEL Sensors
            newSensors[0] = new FuelLineSensor(this,10,DegreesToRadians(45));
            newSensors[1] = new FuelLineSensor(this,10,0);
            newSensors[2] = new FuelLineSensor(this,10,DegreesToRadians(-45));
            //Line Type Sensors
            newSensors[3] = new LineTypeSensor(this,10,DegreesToRadians(45));
            newSensors[4] = new LineTypeSensor(this,10,0);
            newSensors[5] = new LineTypeSensor(this,10,DegreesToRadians(-45));
            //Locator
            newSensors[6] = new LocatorSensor(this);
            //Type Line sensor
            newSensors[7] =  new TypeCircleSensor(this,50);
            //Touch sensors
            newSensors[8] = new TouchSensor(this,0.1f,DegreesToRadians(90));
            newSensors[9] = new TouchSensor(this, 0.1f, DegreesToRadians(180));
            newSensors[10] = new TouchSensor(this,0.1f,DegreesToRadians(270));
            //Radio sensor 
            newSensors[11] = new RadioSensor(this,100);

            this.Sensors = newSensors;

            IEffector[] newEffectors = new IEffector[2];
            newEffectors[0] = new TwoWheelMotor(this,3);
            newEffectors[1] = new RadioTransmitter(this,200);

            this.Effectors = newEffectors;

            this.CountDimension();
        }

    }
}