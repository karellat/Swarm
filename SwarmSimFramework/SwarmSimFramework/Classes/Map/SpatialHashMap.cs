using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace GridBoxes
{

    //STRUCT OF SINGLE GRID BOX
    /// <summary>
    /// Representation of single box of map grid
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGridBox<T>
    {
        int Row { get; }
        int Column { get; }
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
        public ListGridbox(int row, int column)
        {
            this.Row = row;
            this.Column = column;
            _list = null;
        }
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

        public int Row { get; }
        public int Column { get; }

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

        public override string ToString()
        {
            if (_list != null)
                return _list.ToString();
            else
                return "Empty gridBox";
        }

        public override int GetHashCode()
        {
            return Row * 10000 + Column;
        }
    }

    public sealed class SpatialHashMap<ItemType, GridBoxType> where GridBoxType : IGridBox<ItemType>
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
        /// Column of single box
        /// </summary>
        public float BoxWidth { get; protected set; }

        ///GRID of the map
        public IGridBox<ItemType>[][] gridBoxes;


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
            foreach (var i in NearBoxes(middle, radius))
            {
                i.Add(item);
            }
        }

        //REMOVE
        /// <summary>
        /// Remove item from all position of grid 
        /// </summary>
        /// <param name="item"></param>
        public void Remove(ItemType item)
        {
            //TODO: Slow
            foreach (var row in gridBoxes)
            {
                foreach (var cell in row)
                {
                    if (cell.Contains(item))
                        cell.Remove(item);
                }
            }
        }

        /// <summary>
        /// Remove item from selected boxes
        /// </summary>
        /// <param name="item"></param>
        public void Remove(Vector2 middle, float radius, ItemType item)
        {
            foreach (var box in NearBoxes(middle, radius))
            {
                box.Remove(item);
            }
        }

        //UPDATE
        /// <summary>
        /// Update position of given item
        /// </summary>
        /// <param name="item"></param>
        public void Update(Vector2 newMiddle, float radius, ItemType item)
        {
            //TODO: slow 
            Remove(item);
            Insert(newMiddle, radius, item);
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
            Remove(oldMiddle, oldRadius, item);
            Insert(newMiddle, newRadius, item);
        }
        /// <summary>
        /// Update position of selected boxes
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void Update(Vector2 oldMiddle, Vector2 newMiddle, float radius, ItemType item)
        {
            Update(oldMiddle, radius, newMiddle, radius, item);
        }
        //INTERSECTIONS 
        public ItemType[] CircleIntersections(Vector2 middle, float radius)
        {
            HashSet<ItemType> output = new HashSet<ItemType>();
            foreach (var box in NearBoxes(middle, radius))
            {
                foreach (var i in box.AllItem())
                    output.Add(i);
            }

            return output.ToArray();
        }
        /// <summary>
        /// Intersection line & grid map 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public ItemType[] LineIntersections(Vector2 A, Vector2 B)
        {
            HashSet<ItemType> output = new HashSet<ItemType>();
            foreach (var box in NearBoxes(A, B))
            {
                foreach (var i in box.AllItem())
                    output.Add(i);
            }
            return output.ToArray();
        }

        /// <summary>
        /// Boxes intersect give circle
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private IGridBox<ItemType>[] NearBoxes(Vector2 middle, float radius)
        {
            IGridBox<ItemType>[] output;
            //Boxes 
            if (radius > MaximumRadius)
                throw new ArgumentException("Bigger item than given max. Radius: " + radius);
            float MaxRow = middle.Y + radius;
            float MinRow = middle.Y - radius;

            float MaxColumn = middle.X + radius;
            float MinColumn = middle.X - radius;

            //Check collision with the border

            int MaxRowIndex = (int)Math.Floor(MaxRow) / (int)BoxHeight;
            int MinRowIndex;
            if (MinRow % BoxHeight != 0)
                MinRowIndex = (int)Math.Floor(MinRow) / (int)BoxHeight;
            else
                //Check collision with the border
                MinRowIndex = (int)Math.Floor(MinRow) / (int)BoxHeight - 1;


            int MaxColumnIndex = (int)Math.Floor(MaxColumn) / (int)BoxWidth;
            int MinColumnIndex;
            if (MinColumn % BoxWidth != 0)
                MinColumnIndex = (int)Math.Floor(MinColumn) / (int)BoxWidth;
            else
                //Check collision with the border
                MinColumnIndex = (int)Math.Floor(MinColumn) / (int)BoxWidth - 1;

            //Axis dividing 4 cell
            float axisY = MaxRowIndex * BoxHeight;
            float axisX = MaxColumnIndex * BoxWidth;

            //All four boxes
            Vector2 m = new Vector2(MaxColumnIndex * BoxWidth, MaxRowIndex * BoxHeight);

            if (Vector2.DistanceSquared(m, middle) < radius * radius)
            {
                output = new IGridBox<ItemType>[4];
                output[0] = gridBoxes[MinRowIndex][MinColumnIndex];
                output[1] = gridBoxes[MinRowIndex][MaxColumnIndex];
                output[2] = gridBoxes[MaxRowIndex][MinColumnIndex];
                output[3] = gridBoxes[MaxRowIndex][MaxColumnIndex];
                return output;
            }

            //box with the middle
            int middleColumn;
            int middleRow;
            //Number of interfering cells
            List<IGridBox<ItemType>> cells = new List<IGridBox<ItemType>>(4);
            if (middle.Y > MaxRowIndex * BoxWidth)
            {
                middleRow = MaxRowIndex;
                //Max row max column 
                if (middle.X > MaxColumnIndex * BoxHeight)
                {
                    middleColumn = MaxColumnIndex;
                    //intersect the lower box 
                    if (middle.Y - radius <= axisY)
                    {
                        if (gridBoxes[MinRowIndex][middleColumn] != null)
                            cells.Add(gridBoxes[MinRowIndex][middleColumn]);
                    }
                    //intersect the left box
                    if (middle.X - radius <= axisX)
                    {
                        if (gridBoxes[middleRow][MinColumnIndex] != null)
                            cells.Add(gridBoxes[middleRow][MinColumnIndex]);
                    }
                }
                //Max row min column
                else
                {
                    middleColumn = MinColumnIndex;
                    //intersect the lower box 
                    if (middle.Y - radius <= axisY)
                    {
                        if (gridBoxes[MinRowIndex][middleColumn] != null)
                            cells.Add(gridBoxes[MinRowIndex][middleColumn]);
                    }
                    //intersect the right box 
                    if (middle.X + radius >= axisX)
                    {
                        if (gridBoxes[middleRow][MaxColumnIndex] != null)
                            cells.Add(gridBoxes[middleRow][MaxColumnIndex]);
                    }
                }
            }
            else
            {
                middleRow = MinRowIndex;
                //Min row max column
                if (middle.X > MaxColumnIndex * BoxHeight)
                {
                    middleColumn = MaxColumnIndex;
                    //intersect the upper box
                    if (middle.Y + radius >= axisY)
                        cells.Add(gridBoxes[MaxRowIndex][middleColumn]);
                    //intersect the left box
                    if (middle.X - radius <= axisX)
                        cells.Add(gridBoxes[middleRow][MinColumnIndex]);

                }
                //min row min column
                else
                {
                    middleColumn = MinColumnIndex;
                    //intersect the upper box 
                    if (middle.Y + radius >= axisY)
                        cells.Add(gridBoxes[MaxRowIndex][middleColumn]);
                    //intersect the right box 
                    if (middle.X + radius >= axisX)
                        cells.Add(gridBoxes[middleRow][MaxColumnIndex]);
                }
            }

            //Add middle
            cells.Add(gridBoxes[middleRow][middleColumn]);

            return cells.ToArray();
        }

        /// <summary>
        /// Boxes intersect line segment
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public IGridBox<ItemType>[] NearBoxes(Vector2 A, Vector2 B)
        {

            //Direction vector
            Vector2 actualP = A;

            Vector2 dir = B - A;
            if (dir == Vector2.Zero)
                return NearBoxes(A);
            dir = Vector2.Normalize(dir);
            HashSet<IGridBox<ItemType>> output = new HashSet<IGridBox<ItemType>>();
            //First point
            foreach (var b in NearBoxes(A))
                output.Add(b);
            //Actual Row & column index 
            int ActualRowIndex = (int)Math.Floor(A.Y) / (int)BoxHeight;
            int RowShift = dir.Y == 0 ? 0 : (dir.Y > 0 ? 1 : -1);
            int ActualColumnIndex = (int)Math.Floor(A.X) / (int)BoxWidth;
            int ColumnShift = dir.X == 0 ? 0 : (dir.X > 0 ? 1 : -1);
            int FollowingRowIndex = ActualRowIndex;
            int FollowingColumnIndex = ActualColumnIndex;

            while (true)
            {
                //Shift to the next column
                //               X coordinate of the nearest X box 
                float shiftX = dir.X == 0 ? float.PositiveInfinity : Math.Abs(FollowingColumnIndex * BoxWidth - actualP.X) / Math.Abs(dir.X);
                //Shift to the next row 
                float shiftY = dir.Y == 0 ? float.PositiveInfinity : Math.Abs(FollowingRowIndex * BoxHeight - actualP.Y) / Math.Abs(dir.Y);
                //Shift to the B

                float shiftB = dir.Y != 0 ? Math.Abs(B.Y - actualP.Y) / Math.Abs(dir.Y) : Math.Abs(B.X - actualP.X) / Math.Abs(dir.X);


                if (shiftB <= shiftX && shiftB <= shiftY)
                {
                    var boxes = NearBoxes(B);
                    if (boxes != null)
                    {
                        foreach (var b in boxes)
                            output.Add(b);
                    }
                    break;
                }
                else if (shiftX < shiftY)
                {

                    actualP += dir * shiftX;
                    var boxes = NearBoxes(actualP);
                    if (boxes != null)
                    {
                        foreach (var b in boxes)
                            output.Add(b);
                    }
                    ActualColumnIndex = FollowingColumnIndex;
                    FollowingColumnIndex += ColumnShift;
                }
                else if (shiftX == shiftY)
                {
                    actualP += dir * shiftX;
                    var boxes = NearBoxes(actualP);
                    if (boxes != null)
                    {
                        foreach (var b in boxes)
                            output.Add(b);
                    }
                    ActualColumnIndex = FollowingColumnIndex;
                    FollowingColumnIndex += ColumnShift;
                    ActualRowIndex = FollowingRowIndex;
                    FollowingRowIndex += RowShift;
                }
                else
                {
                    actualP += dir * shiftY;
                    var boxes = NearBoxes(actualP);
                    if (boxes != null)
                    {
                        foreach (var b in boxes)
                            output.Add(b);
                    }
                    ActualRowIndex = FollowingRowIndex;
                    FollowingRowIndex += RowShift;
                }

            }
            return output.ToArray();
        }

        /// <summary>
        /// Boxes intersect a point 
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        private IGridBox<ItemType>[] NearBoxes(Vector2 A)
        {
            if (A.X % BoxWidth != 0)
            {
                int ColumnIndex = (int)Math.Floor(A.X) / (int)BoxWidth;
                if (ColumnIndex < 0 || ColumnIndex >= BoxesInRow) return null;
                if (A.Y % BoxHeight != 0)
                {
                    //Single box
                    int RowIndex = (int)Math.Floor(A.Y) / (int)BoxHeight;
                    if (RowIndex < 0 || RowIndex >= BoxesInColumn) return null;
                    return new IGridBox<ItemType>[] { gridBoxes[RowIndex][ColumnIndex] };
                }
                else
                {
                    //Upper and lower box
                    int MaxRowIndex = (int)Math.Floor(A.Y + 0.1) / (int)BoxHeight;
                    int MinRowIndex = (int)Math.Floor(A.Y - 0.1) / (int)BoxHeight;
                    if (MinRowIndex >= 0 && MinRowIndex < BoxesInColumn)
                    {
                        if (MaxRowIndex >= 0 && MaxRowIndex < BoxesInColumn)
                            return new IGridBox<ItemType>[]
                                {gridBoxes[MaxRowIndex][ColumnIndex], gridBoxes[MinRowIndex][ColumnIndex]};
                        else
                            return new IGridBox<ItemType>[] { gridBoxes[MinRowIndex][ColumnIndex] };
                    }
                    else
                    {
                        if (MaxRowIndex >= 0 && MaxRowIndex < BoxesInColumn)
                            return new IGridBox<ItemType>[]
                                {gridBoxes[MaxRowIndex][ColumnIndex]};
                        else
                            return null;
                    }
                }
            }
            else
            {

                List<int> possibleColumnIndex = new List<int>(2);
                //
                int MaxColumnIndex = (int)Math.Floor(A.X + 0.1f) / (int)BoxWidth;
                if (MaxColumnIndex >= 0 && MaxColumnIndex < BoxesInRow)
                    possibleColumnIndex.Add(MaxColumnIndex);
                //
                int MinColumnIndex = (int)Math.Floor(A.X - 0.1f) / (int)BoxWidth;
                if (MinColumnIndex >= 0 && MinColumnIndex < BoxesInRow)
                    possibleColumnIndex.Add(MinColumnIndex);

                if (A.Y % BoxHeight != 0)
                {
                    //Left & right  box
                    int RowIndex = (int)Math.Floor(A.Y) / (int)BoxHeight;
                    if (RowIndex >= 0 && RowIndex < BoxesInColumn)
                    {
                        IGridBox<ItemType>[] o = new IGridBox<ItemType>[possibleColumnIndex.Count];
                        for (int i = 0; i < o.Length; i++)
                            o[i] = gridBoxes[RowIndex][possibleColumnIndex[i]];
                        return o;
                    }
                    else
                        return null;
                }
                else
                {
                    //All of 4 boxes
                    List<int> possibleRowIndex = new List<int>(2);
                    //
                    int MaxRowIndex = (int)Math.Floor(A.Y + 0.1) / (int)BoxHeight;
                    if (MaxRowIndex >= 0 && MaxRowIndex < BoxesInColumn)
                        possibleRowIndex.Add(MaxRowIndex);
                    //
                    int MinRowIndex = (int)Math.Floor(A.Y - 0.1) / (int)BoxHeight;
                    if (MinRowIndex >= 0 && MinRowIndex < BoxesInColumn)
                        possibleRowIndex.Add(MinRowIndex);

                    //output
                    IGridBox<ItemType>[] o = new IGridBox<ItemType>[possibleColumnIndex.Count * possibleRowIndex.Count];
                    for (int i = 0; i < possibleColumnIndex.Count; i++)
                    {
                        for (int j = 0; j < possibleRowIndex.Count; j++)
                            o[2 * i + j] = gridBoxes[possibleRowIndex[j]][possibleColumnIndex[i]];
                    }
                    return o;

                }
            }
        }

        //CONSTRUCTORS
        /// <summary>
        /// Create new grid with square boxes 
        /// </summary>
        /// <param name="MaxHeight"></param>
        /// <param name="MaxWidth"></param>
        /// <param name="SizeOfSquare"></param>
        public SpatialHashMap(float MaxHeight, float MaxWidth, float SizeOfSquare)
        {
            //Map parameters
            Height = MaxHeight;
            Width = MaxWidth;

            this.MaximumRadius = SizeOfSquare / 2;
            BoxHeight = SizeOfSquare;
            BoxWidth = SizeOfSquare;

            BoxesInRow = (int)Math.Ceiling(Width / BoxWidth);
            BoxesInColumn = (int)Math.Ceiling(Height / BoxHeight);

            //initiate of grid 
            gridBoxes = new IGridBox<ItemType>[BoxesInRow][];
            for (int i = 0; i < BoxesInRow; i++)
            {
                gridBoxes[i] = new IGridBox<ItemType>[BoxesInColumn];
                for (int j = 0; j < BoxesInColumn; j++)
                {
                    gridBoxes[i][j] = new ListGridbox<ItemType>(i, j);
                }
            }
        }


    }
}