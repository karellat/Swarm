using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Map
{
    public class SpatialHash<T> : IEnumerable<T> where T: CircleEntity
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
            var containing_item = new List<HashSet<T>>();

            foreach (var box in GetBoxesContainingCircle(item,true))
            {
                box.Add(item);
                containing_item.Add(box);
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
            HashSet<T> output = new HashSet<T>();
            foreach (var box in GetBoxesContainingCircle(item,false))
               output.UnionWith(box);

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

        private List<HashSet<T>> GetBoxesContainingCircle(CircleEntity item, bool createEmpty = false)
        {
            List<HashSet<T>> output = new List<HashSet<T>>();
            int minWidthBox  =  (int)(item.Middle.X-item.Radius - BoxSize/10f) / (int)BoxSize;
            int minHeightBox =  (int)(item.Middle.Y-item.Radius  - BoxSize/10f) / (int)BoxSize;

            int maxWidthBox =  (int)(item.Middle.X   + item.Radius) / (int)BoxSize;
            int maxHeightBox = (int)(item.Middle.Y  + item.Radius) / (int)BoxSize;

            float r2 = item.Radius * item.Radius;
            for (int i = minWidthBox; i <= maxWidthBox; i++)
            {
                for (int j = minHeightBox ; j <= maxHeightBox; j++)
                {
                    if(i < 0 || j < 0 || i >= widthCount || j >= heightCount) continue;
                    //If point not in circle continue
                    //if (r2 < Vector2.DistanceSquared(new Vector2(i * BoxSize, j * BoxSize), item.Middle))
                    //    continue;
                    if (boxes[i, j] == null)
                    {
                        if (createEmpty)
                            boxes[i, j] = new HashSet<T>();
                        else
                            continue;
                    }

                    output.Add(boxes[i,j]);
                }
            }

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

        public IEnumerator<T> GetEnumerator()
        {
            return dictionary.Keys.GetEnumerator(); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<T> ToList()
        {
            return dictionary.Keys.ToList();
        }

       
    }
}
    
    
