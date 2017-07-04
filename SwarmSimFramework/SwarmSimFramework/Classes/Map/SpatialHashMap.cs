using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Numerics;

namespace SwarmSimFramework.Classes.Map
{
    public sealed class SpatialHashMap<T>
    {
        //IN STRUCT 
        private struct GridBox<T>
        {
           //
        }
        //GLOBAL VARIABLES 
        /// <summary>
        /// Maximum height of map 
        /// </summary>
        public float Height { get; protected set; }
        /// <summary>
        /// Maximum width of map
        /// </summary> 
        public float Width { get; protected set; }
        /// <summary>
        /// Boxes in single row
        /// </summary>
        public int BoxesInRow { get; protected set; }
        /// <summary>
        /// Boxes in single column
        /// </summary>
        public int BoxesInColumn { get; protected set; }
        /// <summary>
        /// Maximum radius of added items 
        /// </summary>
        public float MaximumRadius { get; protected set; }

        //LOCAL VARIABLES
        /// <summary>
        /// Height of single box 
        /// </summary>
        public float BoxHeight { get; protected set; }
        /// <summary>
        /// Width of single box
        /// </summary>
        public float BoxWidth { get; protected set; }
        
        //GRID of the map
        private List<T>[][] grid;



        //Methods:
        //INSERT
        /// <summary>
        /// Insert new item to the grid 
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="item"></param>
        public void Insert(Vector2 middle, float radius, T item)
        {
            throw new NotImplementedException();
        }
        //REMOVE
        /// <summary>
        /// Remove item from all position of grid 
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Remove item from selected boxes
        /// </summary>
        /// <param name="item"></param>
        public void Remove(Vector2 middle, float radius, T item)
        {
            throw new NotImplementedException();
        }
        //UPDATE
        /// <summary>
        /// Update position of given item
        /// </summary>
        /// <param name="item"></param>
        public void Update(T item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Update position of selected boxes
        /// </summary>
        /// <param name="oldMiddle"></param>
        /// <param name="oldRadius"></param>
        /// <param name="newMiddle"></param>
        /// <param name="newRadius"></param>
        /// <param name="item"></param>
        public void Update(Vector2 oldMiddle, float oldRadius, Vector2 newMiddle, float newRadius, T item)
        {
            
        }

        //CONSTRUCTORS
        public SpatialHashMap(float MaxHeight, float MaxWidth,float MaxRadius)
        {
            throw new NotImplementedException();
            
        }


    }
}