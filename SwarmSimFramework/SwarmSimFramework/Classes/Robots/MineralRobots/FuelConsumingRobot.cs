using SwarmSimFramework.Classes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SwarmSimFramework.Classes.Robots.MineralRobots
{
    abstract class FuelConsumingRobot : RobotEntity
    {
        protected FuelConsumingRobot(Vector2 middle, float radius, string name, IEffector[] effectors, ISensor[] sensors, float amountOfFuel,
            int sizeOfContainer = 0, int teamNumber = 1, float health = 100, float normalizeMax = 100, float normalizeMin = -100F, float orientation = 0) 
            : base(middle, radius, name, effectors, sensors, amountOfFuel, sizeOfContainer, teamNumber, health, normalizeMax, normalizeMin, orientation)
        {
        }
    }
}
