using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Map
{
    public class EntityMap<T> : IEnumerable<T> where T : CircleEntity
    {
        public SpatialHash<T> spatialHash;
        private float boxSize;
        private float mapHeigth;
        private float mapWidth; 

        public EntityMap(float mapHeight, float mapWidth, float boxSize)
        {
            this.boxSize = boxSize;
            this.mapWidth = mapWidth;
            this.mapHeigth = mapHeight;
            spatialHash = new SpatialHash<T>(boxSize,mapHeight,mapWidth); 
        }

        public void Add(T c)
        {
            spatialHash.Add(c);
        }

        public void Clear()
        {
            spatialHash = new SpatialHash<T>(boxSize,mapHeigth,mapWidth);
        }

        public void Remove(T c)
        {
            spatialHash.Remove(c);
        }

        public int Count {
            get
            {
                return spatialHash.Count;
            }
        }
   
        public T One => spatialHash.One();

        
        public HashSet<T> CircleIntersection(CircleEntity item)
        {
            return spatialHash.CircleIntersection(item);
        }

        public HashSet<T> PointIntersection(Vector2 point)
        {
            return spatialHash.PointIntersection(point);
        }

        public HashSet<T> LineIntersection(LineEntity item)
        {
            return spatialHash.LineIntersection(item);
        }

        internal void RemoveAll(Func<T, bool> p)
        {
            List<T> willBeRemoved = new List<T>();
            foreach (var i in spatialHash)
            {
                if (p.Invoke(i))
                    willBeRemoved.Add(i);
            }

            foreach (var i in willBeRemoved)
            {
                Remove(i);
            }
        }

        public List<T> ToList()
        {
            return spatialHash.ToList();
        }


        public IEnumerator<T> GetEnumerator()
        {
           return spatialHash.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}