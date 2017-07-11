using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using GridBoxes;
using MathNet.Numerics.Optimization;
using SwarmSimFramework.Classes.Entities;

namespace SwarmSimFramework.Classes.Map
{
    public class MapHashMapBridge<T> : IEnumerable<T> where T : CircleEntity
    {
        public static float MaxHeight;
        public static float MaxWidth;
        public static float BoxSize;
        private List<T> internStorage = new List<T>();
        private SpatialHashMap<T, ListGridbox<T>> hashMap;


        public MapHashMapBridge(List<T> items)
        {
            internStorage = items;
            hashMap = new SpatialHashMap<T, ListGridbox<T>>(MaxHeight,MaxWidth,BoxSize);
        }

        public MapHashMapBridge()
        {
            hashMap = new SpatialHashMap<T, ListGridbox<T>>(MaxHeight,MaxWidth,BoxSize);

        }

        public void Clear()
        {
            internStorage.Clear();
        }

        public void Add(T item)
        {
            internStorage.Add(item);
        }

        public int Count => internStorage.Count;


        public T this[int i] => internStorage[i];

        public void RemoveAll(Func<T,bool> lambda)
        {
            internStorage.RemoveAll(lambda.Invoke);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return internStorage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Remove(T item)
        {
            internStorage.Remove(item);
        }

        public List<T> ToList()
        {
            return internStorage;
        }
    }
}
