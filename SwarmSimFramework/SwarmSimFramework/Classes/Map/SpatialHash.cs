using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Map
{
    public class SpatialHash<T> where T: CircleEntity
    {
        private Dictionary<T, List<HashSet<T>>> dictionary;
        private HashSet<T>[,] boxes;
        private float BoxSize;
        private int heightCount;
        private int widthCount;

        /// <summary>
        /// Initialize new spatial hash map 
        /// </summary>
        public SpatialHash(float boxSize, float heigth, float width)
        {
            BoxSize = boxSize;
            this.heightCount = (int) Math.Ceiling(heigth / boxSize);
            this.widthCount = (int) Math.Ceiling(width / boxSize);

            boxes = new HashSet<T>[widthCount, heightCount];
            dictionary = new Dictionary<T, List<HashSet<T>>>();
        }

        //Structure changes 
        /// <summary>
        /// Add circle entity to the grid
        /// </summary>
        public void Add(T item)
        {
            List<HashSet<T>> containing_item = new List<HashSet<T>>();

            //Circle middle box
            int widthBox = (int) Math.Floor(item.Middle.X / BoxSize);
            int heightBox = (int) Math.Floor(item.Middle.Y / (int) BoxSize);

            //if not in more than one box
            if ((int) Math.Floor((item.Middle.X + item.Radius) / BoxSize) == widthBox &&
                (int) Math.Floor((item.Middle.X - item.Radius) / BoxSize) == widthBox &&
                (int) Math.Floor((item.Middle.Y + item.Radius) / BoxSize) == heightBox &&
                (int) Math.Floor((item.Middle.Y - item.Radius) / BoxSize) == heightBox)
            {
                if (boxes[widthBox, heightBox] == null)
                    boxes[widthBox, heightBox] = new HashSet<T>();

                boxes[widthBox, heightBox].Add(item);
                containing_item.Add(boxes[widthBox, heightBox]);
                dictionary.Add(item, containing_item);
                return;
            }

            //TODO: Insert to true containing boxes
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i + widthBox < 0 || j + heightBox < 0 ||
                        i + widthBox >= widthCount || j + heightBox >= heightCount) continue;
                  

                    if (boxes[widthBox + i, heightBox + j] == null)
                        boxes[widthBox + i, heightBox + j] = new HashSet<T>();

                    boxes[widthBox + i, heightBox + j].Add(item);
                    containing_item.Add(boxes[widthBox + i, heightBox + j]);
                }
            }

            dictionary.Add(item, containing_item);
        }

        public void Remove(T item)
        {
            if (!dictionary.ContainsKey(item))
                throw new ArgumentException("Item entity not in the list");

            foreach (var dic in dictionary[item])
                dic.Remove(item);

            dictionary.Remove(item);
        }

        public void Update(T item)
        {
            Remove(item);
            Add(item);
        }

        public int Count => dictionary.Count;

        //Intersections 
        public HashSet<T> PointIntersection(Vector2 point)
        {
            var output = new HashSet<T>();

            //Point inside the borders
            if (point.X < 0 || point.Y < 0 ||
                point.X > BoxSize * widthCount || point.Y > BoxSize * heightCount)
                return output; 
            
            //Intersection of 4 boxes
            if (point.X % BoxSize <= 0.00001 && point.Y % BoxSize <= 0.00001)
            {
                var X_1 = (int)Math.Floor((point.X + 0.1) / BoxSize); 
                var X_2 = (int)Math.Floor((point.X - 0.1) / BoxSize);
                var Y_1 = (int)Math.Floor((point.Y + 0.1) / BoxSize);
                var Y_2 = (int)Math.Floor((point.Y - 0.1) / BoxSize);

                if( X_1 < widthCount && Y_1 < heightCount &&  boxes[X_1, Y_1] != null)
                    output.UnionWith(boxes[X_1,Y_1]);
                if (X_1 < widthCount && Y_2 >= 0 && boxes[X_1, Y_2] != null)
                    output.UnionWith(boxes[X_1,Y_2]);
                if (X_2 >= 0 && Y_1 < heightCount && boxes[X_2, Y_1] != null)
                    output.UnionWith(boxes[X_2,Y_1]);
                if (X_2 >= 0 && Y_2 >= 0 && boxes[X_2, Y_2] != null)
                    output.UnionWith(boxes[X_2, Y_2]);
            }
            //Intersection of two parallel boxes
            else if (point.Y % BoxSize <= 0.00001)
            {
                var X_1 = (int) Math.Floor(point.X / BoxSize);
                var Y_1 = (int)Math.Floor((point.Y+0.1) / BoxSize);
                var Y_2 = (int)Math.Floor((point.Y-0.1)/ BoxSize);

                if (Y_1 < heightCount && boxes[X_1, Y_1] != null)
                    output.UnionWith(boxes[X_1, Y_1]);
                if (Y_2 >= 0 && boxes[X_1, Y_2] != null)
                    output.UnionWith(boxes[X_1, Y_2]);
            }
            else if (point.X % BoxSize <= 0.00001)
            {
                var X_1 = (int)Math.Floor((point.X + 0.1) / BoxSize); ;
                var X_2 = (int)Math.Floor((point.X - 0.1) / BoxSize); ;
                var Y_1 = (int)Math.Floor(point.Y / BoxSize);
                if (X_1 < widthCount && boxes[X_1, Y_1] != null)
                    output.UnionWith(boxes[X_1, Y_1]);
                if (X_2 >= 0 && boxes[X_2, Y_1] != null)
                    output.UnionWith(boxes[X_2, Y_1]);
            }
            else
            {
                var X_1 = (int)Math.Floor(point.X / BoxSize); 
                var Y_1 = (int)Math.Floor(point.Y / BoxSize);
                if (boxes[X_1, Y_1] != null)
                    output.UnionWith(boxes[X_1, Y_1]);
            }

            return output;
        }
        public HashSet<T> CircleIntersection(CircleEntity item)
        {
            var output = new HashSet<T>();
            int widthBox = (int)item.Middle.X / (int)BoxSize;
            int heightBox = (int)item.Middle.Y / (int)BoxSize;

            //TODO: Insert to true containing boxes
            //NAIVE solution 
            //for (int i = -1; i <= 1; i++)
            //{
            //    for (int j = -1; j <= 1; j++)
            //    {
            //        if (i + widthBox < 0 || j + heightBox < 0 || i + widthBox >= widthCount || j + heightBox >= heightCount) continue;

            //        if (boxes[widthBox + i, heightBox + j] != null)
            //        {
            //            foreach (var q in boxes[widthBox + i, heightBox + j])
            //                output.Add(q);
            //        }
            //    }
            //}

            //More complicated

            var top = item.Middle + Vector2.UnitY * item.Radius;
            var bottom = item.Middle - Vector2.UnitY * item.Radius;
            var left = item.Middle - Vector2.UnitX * item.Radius;
            var right = item.Middle + Vector2.UnitX * item.Radius;


            var upper_left = new Vector2(widthBox * BoxSize, heightBox * BoxSize);
            var upper_right = new Vector2((widthBox + 1) * BoxSize, heightBox * BoxSize);
            var lower_left = new Vector2(widthBox * BoxSize, (heightBox + 1) * BoxSize);
            var lower_right = new Vector2(widthBox * BoxSize, (heightBox + 1) * BoxSize);

            var list = new[] { upper_left, upper_right, lower_left, lower_right };

            output.UnionWith(PointIntersection(top));
            output.UnionWith(PointIntersection(bottom));
            output.UnionWith(PointIntersection(left));
            output.UnionWith(PointIntersection(right));

            foreach (var i in list)
            {
                if (Vector2.DistanceSquared(i, item.Middle) <= (item.Radius * item.Radius))
                    output.UnionWith(PointIntersection(i));
            }


            return output;
        }
        public HashSet<T> LineIntersection(LineEntity item)
        {
            var output = new HashSet<T>();
            //direction vector 
            var v = item.A-item.B;
            var n_X = v.Y;
            var n_Y = -1 * v.X;
            v.X = n_X;
            v.Y = n_Y; 
            var minus_c = (v.X * item.A.X + v.Y * item.A.Y);
            

            float max_Y = Math.Max(item.A.Y, item.B.Y);
            float min_Y = Math.Min(item.A.Y, item.B.Y);

            float max_X = Math.Max(item.A.X, item.B.X);
            float min_X = Math.Min(item.A.X, item.B.X);


            float actual_Y = (int) Math.Floor(min_Y/ BoxSize) * BoxSize;
          
            while (max_Y != min_Y && actual_Y <= max_Y)
            {
                //Count X coordinate
                var X =  (minus_c - actual_Y * v.Y) / v.X;
      
                output.UnionWith(PointIntersection(new Vector2(X,actual_Y)));

                actual_Y += BoxSize;
            }

            float actual_X = (int) Math.Floor(min_X / BoxSize) * BoxSize;

            while (min_X != max_X && actual_X <= max_X)
            {
                //Count Y coordinate
                var Y = (minus_c - actual_X * v.X) / v.Y; 
                output.UnionWith(PointIntersection(new Vector2(actual_X,Y)));

                actual_X += BoxSize;
            }

            output.UnionWith(PointIntersection(item.A));
            output.UnionWith(PointIntersection(item.B));
            return output;
        }

        /// <summary>
        /// Unit test method
        /// </summary>
        public T One()
        {
            //TODO: Not so nice for testing :] 
            if(Count != 1)
                throw new ArgumentException("One fuction is only for unit testing");

            foreach (var k in dictionary.Keys)
                return k;

            return null;
        }
    }
}