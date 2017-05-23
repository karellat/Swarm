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
            List<CircleEntity> circles = new List<CircleEntity>();
            List<FuelEntity> fuels = new List<FuelEntity>();
            circles.Add(new ObstacleEntity(new Vector2(40,40),5));
            fuels.Add(new FuelEntity(new Vector2(95,50),3,50));
            circles.Add(new WoodEntity(new Vector2(60,60),5,59));
            circles.Add(new RawMaterialEntity(new Vector2(100,100),5,50,10));
            Map = new Map.Map(400,400,null, circles,fuels);
            Finnished = false;
        }

        public void MakeStep()
        {
            Map.MakeStep();
        }

        public bool Finnished { get; protected set; }
    }
}