using SwarmSimFramework.Classes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SwarmSimFramework.Classes.Experiments;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.Robots;

namespace SwarmSimFramework
{
    class Program
    {
        static void Main(string[] args)
        {

            var v = WoodScene.InitPositionOfRobot();
            var r = new ScoutRobot(Vector2.Zero, 3,4);
            Console.WriteLine(Entity.EntityColor.ObstacleColor.ToString());
            var e = new TestingExperiment();
            e.Init();
            for (int i = 0; i < 100; i++)
            {
                e.MakeStep();
            }
        }
    }
}
