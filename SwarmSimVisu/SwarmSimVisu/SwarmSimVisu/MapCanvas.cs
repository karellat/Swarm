using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Threading;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;

namespace SwarmSimVisu
{
    public struct Circle
    {
        public Ellipse Ellipse;
        public string ColorKey; 
    }

    public struct Line
    {
        public RawVector2 A;
        public RawVector2 B;
        public string ColorKey;
        public float Stroke;

    }

    public class MapCanvas : D2dControl.D2dControl
    {
        //Actual drawing objects
        protected List<Circle> Circles;

        protected List<Line> Lines;

        //Cached objects of following frame 
        protected object AddingLock = new object();

        protected object DrawingLock = new object();
        protected List<Circle> CirclesCache;
        protected List<Line> LinesCache;
        protected bool ReadyToChangeFrame;

        protected RawColor4 BackgroundColor = new RawColor4(0, 0, 1.0f, 0.5f);


        public MapCanvas()
        {
            //Prepare lists
            Circles = new List<Circle>();
            Lines = new List<Line>();
            LinesCache = new List<Line>();
            CirclesCache = new List<Circle>();
            //Prepare color:
            resCache.Add("SIGNAL1", t => new SolidColorBrush(t, new RawColor4(1.0f, 0, 0, 0.5f)) {Opacity = 0.25f});
            resCache.Add("SIGNAL2", t => new SolidColorBrush(t, new RawColor4(0, 1.0f, 0, 0.5f)) {Opacity = 0.25f});
            resCache.Add("SIGNAL3", t => new SolidColorBrush(t, new RawColor4(0, 0, 0.5f, 0.5f)) {Opacity = 0.25f});
            resCache.Add("ObstacleColor", t => new SolidColorBrush(t, new RawColor4(0.827f, 0.827f, 0.827f, 1.0f)));
            resCache.Add("FuelColor", t => new SolidColorBrush(t, new RawColor4(0.000f, 0.000f, 0.000f, 1.0f)));
            resCache.Add("RawMaterialColor", t => new SolidColorBrush(t, new RawColor4(0.58f, 0.0f, 0.827f, 1.0f)));
            resCache.Add("WoodColor", t => new SolidColorBrush(t, new RawColor4(0.545f, 0.271f, 0.075f, 1.0f)));
            resCache.Add("ROBOT1", t => new SolidColorBrush(t, new RawColor4(0.000f, 1.00f, 0.000f, 0.0f)));
            resCache.Add("ROBOT2", t => new SolidColorBrush(t, new RawColor4(0.000f, 0.000f, 1.000f, 1.00f)));
            resCache.Add("HEAD", t => new SolidColorBrush(t, new RawColor4(1.000f, 0.000f, 1.000f, 1.0f)));
            resCache.Add("LINESENSOR", t => new SolidColorBrush(t, new RawColor4(0.0f, 0.0f, 0.0f, 1.0f)));
            resCache.Add("LINEEFECTOR", t => new SolidColorBrush(t, new RawColor4(0.184f, 0.310f, 0.310f, 1.0f)));
            resCache.Add("CIRCLEEFFECTOR", t => new SolidColorBrush(t, new RawColor4(1.000f, 1.000f, 0.00f, 1.0f)));
            resCache.Add("CIRCLESENSOR",t=> new SolidColorBrush(t,new RawColor4(0.961f, 0.871f, 0.702f,1.0f)));
        }
    

    /// <summary>
        /// Render actual scene
        /// </summary>
        /// <param name="target"></param>
        public override void Render(RenderTarget target)
        {
            lock (DrawingLock)
            {
                target.Clear(BackgroundColor);
               foreach (var c in Circles)
                    target.FillEllipse(c.Ellipse,resCache[c.ColorKey] as Brush);
                foreach (var l in Lines)
                    target.DrawLine(l.A,l.B,resCache[l.ColorKey] as Brush,l.Stroke);        
            }
        }

        /// <summary>
        /// Add circle to following frame
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="brush"></param>
        public void  AddCircle(Vector2 middle, float radius, string brush)
        {
            lock (AddingLock)
            {
                //Wait if frame is changing
                while (ReadyToChangeFrame)
                {
                    Monitor.Wait(AddingLock);
                }
                CirclesCache.Add(new Circle()
                {
                    ColorKey = brush,
                    Ellipse = new Ellipse(new RawVector2()
                    {
                        X = middle.X,
                        Y = middle.Y
                    }, radius, radius)
                });
            }
        }
        /// <summary>
        /// Add line to next frame
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="brush"></param>
        public void AddLine(Vector2 A, Vector2 B,string brush,float stroke)
        {
            lock (AddingLock)
            {
                //wait if  frame is changing
                while (ReadyToChangeFrame)
                {
                    Monitor.Wait(AddingLock);
                }
                LinesCache.Add(new Line()
                    {
                        A = new RawVector2() { X = A.X,Y = A.Y},
                        B = new RawVector2() { X = B.X,Y = B.Y},
                        ColorKey = brush,
                        Stroke = stroke
                    } 
                );
            }
        }
        /// <summary>
        /// Cache filled
        /// </summary>
        public void CompleteFrame()
        {
            //Set bool to switching cache with actual frame
            lock (AddingLock)
            {
                ReadyToChangeFrame = true;
            }
            //Switch lists
            lock (DrawingLock)
            {
                Circles = CirclesCache;
                Lines = LinesCache;
            }
            //Clean old casche & pulse sleeping threads
            lock (AddingLock)
            {
                CirclesCache = new List<Circle>();
                LinesCache = new List<Line>();
                ReadyToChangeFrame = false;
                Monitor.PulseAll(AddingLock);
            }
        }

    }
}