﻿using System.Numerics;
using SwarmSimFramework.Classes.Effectors;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Robots.MineralRobots
{
    /// <summary>
    /// WorkerRobot for the 1.scenario
    /// </summary>
    public class WorkerRobotMem:RobotEntity
    {
        public WorkerRobotMem(int amountOfFuel) : this(Vector2.Zero, amountOfFuel) { }
        public WorkerRobotMem(Vector2 middle,float amountOfFuel,float orientation = 0) 
            : base(middle, 5, "WorkerRobot", null, null, amountOfFuel, 5, 1, 100, 100, -100, orientation)
        {
            MemoryStick mem = new MemoryStick(10, this);
            ISensor[] sensors = new ISensor[12];
            //FUEL Sensors
            sensors[0] = new FuelLineSensor(this, 15, DegreesToRadians(45));
            sensors[1] = new FuelLineSensor(this, 15, 0);
            sensors[2] = new FuelLineSensor(this, 15, DegreesToRadians(-45));
            //Line Type Sensors
            sensors[3] = new LineTypeSensor(this, 15, DegreesToRadians(45));
            sensors[4] = new LineTypeSensor(this, 15, 0);
            sensors[5] = new LineTypeSensor(this, 15, DegreesToRadians(-45));
            //Locator
            sensors[6] = new LocatorSensor(this);
            //Touch sensors
            sensors[7] = new TouchSensor(this, 0.1f, DegreesToRadians(90));
            sensors[8] = new TouchSensor(this, 0.1f, DegreesToRadians(180));
            sensors[9] = new TouchSensor(this, 0.1f, DegreesToRadians(270));
            //Radio sensor 
            sensors[10] = new RadioSensor(this, 100);
            sensors[11] = mem;
            this.Sensors = sensors;

            IEffector[] effectors = new IEffector[4];
            effectors[0] = new TwoWheelMotor(this, 1.5f);
            effectors[1] = new RadioTransmitter(this, 200);
            effectors[2] = new Picker(this,20,0);
            effectors[3] = mem; 
            this.Effectors = effectors;

            this.CountDimension();


        }
    }
}