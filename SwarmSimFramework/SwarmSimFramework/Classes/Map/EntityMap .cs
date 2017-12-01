using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Map
{
    public class EntityMap<T> : IEnumerable<T> where T : CircleEntity
    {
        public List<T> list;
        public SpatialHash spatialHash;
        private float boxSize;
        private float mapHeigth;
        private float mapWidth; 

        public EntityMap(float mapHeight, float mapWidth, float boxSize)
        {
            this.boxSize = boxSize;
            this.mapWidth = mapWidth;
            this.mapHeigth = mapHeight;
            spatialHash = new SpatialHash(boxSize,mapHeight,mapWidth); 
            list = new List<T>();
        }

        public void Add(T c)
        {
            spatialHash.Add(c);
            list.Add(c);
        }

        public void Clear()
        {
            spatialHash = new SpatialHash(boxSize,mapHeigth,mapWidth);
            list.Clear();
        }

        public void Remove(T c)
        {
            spatialHash.Remove(c);
            list.Remove(c);
        }

        public int Count {
            get
            {
                Debug.Assert(list.Count == spatialHash.Count);
                return list.Count;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int key] => list[key];

        public List<T> ToList => list;

        public HashSet<CircleEntity> CircleIntersection(CircleEntity item)
        {
            return spatialHash.CircleIntersection(item);
        }

        public HashSet<CircleEntity> PointIntersection(Vector2 point)
        {
            return spatialHash.PointIntersection(point);
        }

        public HashSet<CircleEntity> LineIntersection(LineEntity item)
        {
            return spatialHash.LineIntersection(item);
        }

    }
}