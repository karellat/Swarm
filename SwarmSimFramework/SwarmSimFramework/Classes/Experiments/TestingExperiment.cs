using System.Collections.Generic;
using System.Net;
using System.Numerics;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Interfaces;

namespace SwarmSimFramework.Classes.Experiments
{
    public class TestingExperiment : IExperiment
    {
        public static RobotEntity r = new ScoutRobot(new Vector2(100,100),100,0);
        public static Map.Map map;
        public Map.Map Map { get; protected set; }
        public void Init()
        {
            //List<RobotEntity> robotList = new List<RobotEntity> {(RobotEntity) r.DeepClone()};
            Map = new Map.Map(400,400);
            Finnished = false;
        }

        public void MakeStep()
        {
            Map.MakeStep();
        }

        public bool Finnished { get; protected set; }
    }
}