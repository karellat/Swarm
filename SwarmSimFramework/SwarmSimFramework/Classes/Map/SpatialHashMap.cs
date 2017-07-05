using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.Numerics;

namespace SwarmSimFramework.Classes.Map
{

    //STRUCT OF SINGLE GRID BOX
    /// <summary>
    /// Representation of single box of map grid
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGridBox<T>
    {
        /// <summary>
        /// Return amount of items in the box
        /// </summary>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// Return true if item is in the box
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Contains(T item);
        /// <summary>
        /// Remove given item from box
        /// </summary>
        /// <param name="item"></param>
        void Remove(T item);
        /// <summary>
        /// Add given item to box
        /// </summary>
        /// <param name="item"></param>
        void Add(T item);

        /// <summary>
        /// All items of the box 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        T[] AllItem();
    }
    /// <summary>
    /// Grid box list implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ListGridbox<T> : IGridBox<T>
    {
        private List<T> _list;

        public void Add(T item)
        {
            //New list initialization
            if (_list == null)
                _list = new List<T>();

            if (_list.Contains(item)) return;

            _list.Add(item);
        }

        public T[] AllItem()
        {
            return _list.ToArray();
        }

        public bool Contains(T item)
        {
            //Empty list
            if (_list == null)
                return false;
            return _list.Contains(item);
        }

        public int Count()
        {
            if (_list == null)
                return 0;
            return _list.Count;
        }

        public void Remove(T item)
        {
            if (_list == null)
                throw new ArgumentException("Empty box, invalid removing operation.");
#if DEBUG

            if (!_list.Remove(item))
                throw new ArgumentException("Invalid removing operation, unexisting item! : " + item.ToString());
            if (!_list.Contains(item))
                throw new ArgumentException("Invalid removing operation, more instance of item in a box! Item : " + item.ToString());

#else
                list.Remove(item);
#endif

        }
    }

    public sealed class SpatialHashMap<ItemType,GridBoxType> where GridBoxType : IGridBox<ItemType>
    {
        
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

        ///GRID of the map
        private IGridBox<ItemType>[][] gridBoxes;



        //Methods:
        //INSERT
        /// <summary>
        /// Insert new item to the grid 
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="item"></param>
        public void Insert(Vector2 middle, float radius, ItemType item)
        {
            throw new NotImplementedException();
        }
        //REMOVE
        /// <summary>
        /// Remove item from all position of grid 
        /// </summary>
        /// <param name="item"></param>
        public void Remove(ItemType item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Remove item from selected boxes
        /// </summary>
        /// <param name="item"></param>
        public void Remove(Vector2 middle, float radius, ItemType item)
        {
            throw new NotImplementedException();
        }
        //UPDATE
        /// <summary>
        /// Update position of given item
        /// </summary>
        /// <param name="item"></param>
        public void Update(ItemType item)
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
        public void Update(Vector2 oldMiddle, float oldRadius, Vector2 newMiddle, float newRadius, ItemType item)
        {
            
        }

        private IGridBox<ItemType>[] NearBoxes(Vector2 middle, float radius)
        {
            //Boxes 
            if (radius > MaximumRadius)
                throw new ArgumentException("Bigger item than given max. Radius: " + radius);
            float MaxRow = middle.Y + radius;
            float MinRow = middle.Y - radius;

            float MaxColumn = middle.X + radius;
            float MinColumn = middle.Y + radius;

            int MaxRowIndex = (int) Math.Floor(MaxRow);
            int MinRowIndex = (int) Math.Floor(MinRow);

            int MaxColumnIndex = (int) Math.Floor(MaxColumn);
            int MinColumnIndex = (int) Math.Floor(MinColumn);



        }
        //CONSTRUCTORS
        /// <summary>
        /// Create new grid with square boxes 
        /// </summary>
        /// <param name="MaxHeight"></param>
        /// <param name="MaxWidth"></param>
        /// <param name="MaxRadius"></param>
        public SpatialHashMap(float MaxHeight, float MaxWidth,float MaxRadius)
        {
            //Map parameters
            Height = MaxHeight;
            Width = MaxWidth;

            this.MaximumRadius = MaxRadius;
            BoxHeight = MaxRadius;
            BoxWidth = MaxRadius;

            BoxesInRow =(int) Math.Ceiling(Width / BoxWidth);
            BoxesInColumn = (int) Math.Ceiling(Height / BoxHeight);

            //initiate of grid 
            gridBoxes = new IGridBox<ItemType>[BoxesInColumn][];
            for (int i = 0; i < BoxesInColumn; i++)
                gridBoxes[i] = new IGridBox<ItemType>[BoxesInRow];
        }


    }
}