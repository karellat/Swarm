using System.Numerics;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Robots
{
    /// <summary>
    /// WorkerRobot for the 1.scenario
    /// </summary>
    public class WorkerRobot:RobotEntity
    {
        public WorkerRobot(Vector2 middle, float radius,float amountOfFuel,float orientation = 0) 
            : base(middle, radius, "WorkerRobot", null, null, amountOfFuel, 5, 1, 100, 100, -100, orientation)
        {
        }
    }
}