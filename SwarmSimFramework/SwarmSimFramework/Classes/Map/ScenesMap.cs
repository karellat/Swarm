using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Map
{
    public struct RobotModel
    {
        public RobotEntity model;
        public int amount; 
    }
    public static class WoodScene
    {
        //WOOD EXPERIMENT - VARIABLES 
        /// <summary>
        /// Amount woods
        /// </summary>
        public static int AmountOfWoods;
        /// <summary>
        /// Amount trees
        /// </summary>
        public static int AmountOfTrees;

        /// <summary>
        /// Max amount of robots in map 
        /// </summary>
        public static int MaxOfAmountRobots = 9;
        /// <summary>
        /// Height of map
        /// </summary>
        public static float MapHeight = 800;
        /// <summary>
        /// Width of map 
        /// </summary>
        public static float MapWidth = 1200;
        /// <summary>
        /// Max size of robot
        /// </summary>
        public static float RobotMaxRadius = 5;
        /// <summary>
        /// Init positions of vector 
        /// </summary>
        /// <returns></returns>
        public static Vector2[] InitPositionOfRobot()
        {
            List<Vector2> vectors = new List<Vector2>();
            float sW = MapWidth / 2 + 22;
            float sH = MapHeight / 2 + 22;
            for (int i = 0; i < 5; i++)
            {
                var nW = sW - 11 * i;
                var nH = sH - 11 * i;
                if (nW == MapWidth/2 && nH == MapHeight/2)
                {
                    vectors.Add(new Vector2(nW,nH));
                }
                else
                {
                    vectors.Add(new Vector2(nW,MapHeight/2));
                    vectors.Add(new Vector2(MapWidth/2,nH));
                }
            }
            return vectors.ToArray();
        }
        //CREATION OF MAP 
        /// <summary>
        /// Create empty map for wood scene
        /// </summary>
        /// <returns></returns>
        public static Map MakeEmptyMap()
        {
            //Prepare objects of map 
            RawMaterialEntity tree = new RawMaterialEntity(new Vector2(0, 0), 5, 10, 10);
            WoodEntity wood = new WoodEntity(new Vector2(0, 0), 5, 10);
            ObstacleEntity initPosition = new ObstacleEntity(new Vector2(MapWidth / 2, MapHeight / 2), 30);
            //Generate randomly deployed tree
            Classes.Map.Map preparedMap = new Classes.Map.Map(MapHeight, MapWidth, null, new List<CircleEntity>() { initPosition });
            List<CircleEntity> trees =
                Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, tree, AmountOfTrees);
            var tp = trees.ToList();
            tp.Add(initPosition);
            preparedMap = new Classes.Map.Map(MapHeight, MapWidth, null, tp);
            List<CircleEntity> woods = Classes.Map.Map.GenerateRandomPos<CircleEntity>(preparedMap, wood, AmountOfWoods);
            List<CircleEntity> passive = woods;

            List<RadioEntity> constSignals = new List<RadioEntity>(new []{new RadioEntity(new Vector2(MapWidth/2,MapHeight/2),50,0)});
            foreach (var t in trees)
                passive.Add(t);
            return new Map(MapHeight,MapWidth,null,passive,null,constSignals);

        }
        /// <summary>
        /// Create map with given models 
        /// </summary>
        /// <param name="robotModels"></param>
        /// <param name="amountOfRobots"></param>
        /// <returns></returns>
        public static Map MakeMap(RobotModel[] models)
        {
            //Check size & amount 
            int amountOfRobot = 0;
            foreach (var m in models)
            {
                amountOfRobot += m.amount; 
                if(m.model.Radius > RobotMaxRadius)
                    throw new ArgumentException("Robot with bigger radius");
                if(amountOfRobot > MaxOfAmountRobots)
                    throw new ArgumentException("More robots than maximum size");
            }
            List<RobotEntity> robots = new List<RobotEntity>();
            foreach (var m in models)
            {
                for (int i = 0;  i < m.amount; ++i)
                {
                    robots.Add((RobotEntity) m.model.DeepClone());
                }
            }
            Vector2[] initPos = InitPositionOfRobot();
            for (var index = 0; index < robots.Count; index++)
            {
                var r = robots[index];
                r.MoveTo(initPos[index]);
            }

            Map emptyMap = MakeEmptyMap(); 
            return new Map(MapHeight,MapWidth,robots,emptyMap.PasiveEntities,emptyMap.FuelEntities,emptyMap.constantRadioSignal);
        }
    }
}