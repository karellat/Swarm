﻿using System.Numerics;
using SwarmSimFramework.Classes.Effectors;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Robots.MineralRobots
{
    /// <summary>
    /// ScoutRobot for the 1. Scenario
    /// </summary>
    public class ScoutRobot : RobotEntity
    {
        public ScoutRobot(int amountOfFuel) : this(Vector2.Zero, amountOfFuel) { }
        public ScoutRobot(Vector2 middle, float amountOfFuel,float orientation = 0 )
            : base(middle, 2.5f, "ScoutRobot",null ,null , amountOfFuel, 0, 1, 100, 100, -100, orientation)
        {
            //set fuel consumation
            this.BurnFuelPerMove = 1;
            ISensor[] sensors = new ISensor[12];
            //FUEL Sensors
            sensors[0] = new FuelLineSensor(this,50,DegreesToRadians(45));
            sensors[1] = new FuelLineSensor(this,50,0);
            sensors[2] = new FuelLineSensor(this,50,DegreesToRadians(-45));
            //Line Type Sensors
            sensors[3] = new LineTypeSensor(this,50,DegreesToRadians(45));
            sensors[4] = new LineTypeSensor(this,50,0);
            sensors[5] = new LineTypeSensor(this,50,DegreesToRadians(-45));
            //Locator
            sensors[6] = new LocatorSensor(this);
            //Type Line sensor
            sensors[7] =  new TypeCircleSensor(this,50);
            //Touch sensors
            sensors[8] = new TouchSensor(this,0.1f,DegreesToRadians(90));
            sensors[9] = new TouchSensor(this, 0.1f, DegreesToRadians(180));
            sensors[10] = new TouchSensor(this,0.1f,DegreesToRadians(270));
            //Radio sensor 
            sensors[11] = new RadioSensor(this,100);

            this.Sensors = sensors;

            IEffector[] effectors = new IEffector[2];
            effectors[0] = new TwoWheelMotor(this,3);
            effectors[1] = new RadioTransmitter(this,200);

            this.Effectors = effectors;

            this.CountDimension();
        }

    }
}