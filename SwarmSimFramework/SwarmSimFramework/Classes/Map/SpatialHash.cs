using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Map
{
    public class SpatialHash
    {
        private Dictionary<CircleEntity,List<HashSet<CircleEntity>>> dictionary;
        private HashSet<CircleEntity>[,] boxes;
        private float BoxSize;
        private int heightCount;
        private int widthCount; 

        


        /// <summary>
        /// Initialize new spatial hash map 
        /// </summary>
        public SpatialHash(float boxSize,float heigth, float width)
        {
            BoxSize = boxSize;
            this.heightCount = (int) Math.Ceiling(heigth / boxSize);
            this.widthCount =  (int)  Math.Ceiling(width  / boxSize);
            
            boxes = new HashSet<CircleEntity>[widthCount,heightCount];
            dictionary = new Dictionary<CircleEntity, List<HashSet<CircleEntity>>>();
        }

        //Structure changes 
        /// <summary>
        /// Add circle entity to the grid
        /// </summary>
        public void Add(CircleEntity item)
        {
            List<HashSet<CircleEntity>>  containing_item = new List<HashSet<CircleEntity>>();
            
            //Circle middle box
            int widthBox = (int) item.Middle.X / (int) BoxSize;
            int heightBox = (int) item.Middle.Y / (int) BoxSize;

            //TODO: Insert to true containing boxes
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if(i + widthBox < 0 || j + heightBox < 0) continue;
                    Debug.Assert(i+widthBox < widthCount && i+heightBox < heightCount);

                    if (boxes[widthBox + i, heightBox + j] == null)  
                        boxes[widthBox + i, heightBox + j] = new HashSet<CircleEntity>();

                    boxes[widthBox + i, heightBox + j].Add(item);
                    containing_item.Add(boxes[widthBox + i, heightBox + j]);
                }
            }

           dictionary.Add(item,containing_item);
        } 

        public void Remove(CircleEntity item)
        {
            if (!dictionary.ContainsKey(item))
                throw new ArgumentException("Item entity not in the list");

            foreach (var dic in dictionary[item])
                dic.Remove(item); 
        }

        public void Update(CircleEntity item)
        {
            Remove(item);
            Add(item);
        }
        
        //Intersections 
        public HashSet<CircleEntity> PointIntersection(Vector2 point)
        {
            //Point inside the borders
            Debug.Assert(point.X >= 0);
            Debug.Assert(point.Y >= 0);
            Debug.Assert(point.X <= BoxSize*(widthCount+1));
            Debug.Assert(point.Y <= BoxSize * (heightCount+1));


            var output = new HashSet<CircleEntity>();
            
            //Intersection of 4 boxes
            if (point.X % BoxSize <= 0.00001 && point.Y % BoxSize <= 0.00001)
            {
                var X_1 = (int)Math.Floor((point.X + 0.1) / BoxSize); 
                var X_2 = (int)Math.Floor((point.X - 0.1) / BoxSize);
                var Y_1 = (int)Math.Floor((point.Y + 0.1) / BoxSize);
                var Y_2 = (int)Math.Floor((point.Y - 0.1) / BoxSize);

                output.UnionWith(boxes[X_1,Y_1]);
                output.UnionWith(boxes[X_1,Y_2]);
                output.UnionWith(boxes[X_2,Y_1]);
                output.UnionWith(boxes[X_2, Y_2]);
            }
            //Intersection of two parallel boxes
            else if (point.Y % BoxSize <= 0.00001)
            {
                var X_1 = (int) Math.Floor(point.X / BoxSize);
                var Y_1 = (int)Math.Floor((point.Y+0.1) / BoxSize);
                var Y_2 = (int)Math.Floor((point.Y-0.1)/ BoxSize);

                output.UnionWith(boxes[X_1, Y_1]);
                output.UnionWith(boxes[X_1, Y_2]);
            }
            else if (point.X % BoxSize <= 0.00001)
            {
                var X_1 = (int)Math.Floor((point.X + 0.1) / BoxSize); ;
                var X_2 = (int)Math.Floor((point.X - 0.1) / BoxSize); ;
                var Y_1 = (int)Math.Floor(point.X / BoxSize);
                output.UnionWith(boxes[X_1, Y_1]);
                output.UnionWith(boxes[X_2, Y_1]);
            }
            else
            {
                var X_1 = (int)Math.Floor(point.X / BoxSize); 
                var Y_1 = (int)Math.Floor(point.X / BoxSize); 

                output.UnionWith(boxes[X_1, Y_1]);
            }

            return output;
        }
        public HashSet<CircleEntity> CircleIntersection(CircleEntity item)
        {
            var output = new HashSet<CircleEntity>();
            int widthBox = (int)item.Middle.X / (int)BoxSize;
            int heightBox = (int)item.Middle.Y / (int)BoxSize;

            //TODO: Insert to true containing boxes
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i + widthBox < 0 || j + heightBox < 0) continue;
                    Debug.Assert(i + widthBox < widthCount && i + heightBox < heightCount);

                    if (boxes[widthBox + i, heightBox + j] != null)
                    {
                        foreach (var q in boxes[widthBox + i, heightBox + j])  
                           output.Add(q);
                    }
                }
            }

            return output;
        }

        public HashSet<CircleEntity> LineIntersection(LineEntity item)
        {
            var output = new HashSet<CircleEntity>();
            //direction vector 
            var v = item.A-item.B;
            var minus_c = (v.X * item.A.X + v.Y * item.A.Y);
            

            float max_Y = Math.Max(item.A.Y, item.B.Y);
            float min_Y = Math.Min(item.A.Y, item.B.Y);

            float max_X = Math.Max(item.A.X, item.B.X);
            float min_X = Math.Min(item.A.X, item.B.X);


            float actual_Y = (int) Math.Floor(min_Y/ BoxSize) * BoxSize;
          
            while (actual_Y >= max_Y)
            {
                //Count X coordinate
                var X = (minus_c - actual_Y * v.Y) / v.X;
                output.UnionWith(PointIntersection(new Vector2(X,actual_Y)));

                actual_Y += BoxSize;
            }

            float actual_X = (int) Math.Floor(min_X / BoxSize) * BoxSize;

            while (actual_X >= max_X)
            {
                //Count Y coordinate
                var Y = (minus_c - actual_X * v.X) / v.Y;
                output.UnionWith(PointIntersection(new Vector2(actual_X,Y)));

                actual_X += BoxSize;
            }

            return output;
        }

    }
}