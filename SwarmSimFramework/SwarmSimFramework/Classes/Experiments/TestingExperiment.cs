using System.Collections.Generic;
using System.Net;
using System.Numerics;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Experiments
{
    public class TestingExperiment : IExperiment
    {
        public static RobotEntity r = new ScoutRobot(new Vector2(200,200),100);
        public static Map.Map map;
        public Map.Map Map { get; protected set; }
        public void Init()
        {
            List<RobotEntity> robotList = new List<RobotEntity> {(RobotEntity) r.DeepClone()};
            var robot = robotList[0];
            var brain =  new FixedBrain(new IODimension() { Input = r.SensorsDimension, Output = r.EffectorsDimension }, r.NormalizedBound);

            robot.Brain = brain;
            brain.Output[0] = 20;
            brain.Output[1] = -20;
            brain.Output[2] = -20;
            brain.Output[3] = 20;
            brain.Output[4] = -20;

            List<CircleEntity> circles = new List<CircleEntity>();
            List<FuelEntity> fuels = new List<FuelEntity>();

            float nx = 100;
            float ny = 100;
            for (int i = 0; i < 11; i++)
            {
               
                for (int j = 0; j < 11; j++)
                {
                    if(i == j && i == 5) continue;
                    
                    circles.Add(new ObstacleEntity(new Vector2(nx  + i* 20, ny +j * 20), 5));
                }
            }
            Map = new Map.Map(400,400,robotList, circles,fuels);
            Finnished = false;
        }

        public void MakeStep()
        {
            Map.MakeStep();
        }

        public bool Finnished { get; protected set; }
    }
}