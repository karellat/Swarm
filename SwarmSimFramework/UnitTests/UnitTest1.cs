using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SwarmSimFramework.Classes.Entities;
using System.Numerics;
namespace UnitTests
{

 
    [TestClass]
    public class LineTests
    {
        protected class Line : LineEntity
        {
            public Line(Vector2 a, Vector2 b, Vector2 rotationMiddle, float orientation = 0) : base(a, b, rotationMiddle, "LINE", orientation)
            {
            }

            public override Entity DeepClone()
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void InitTest()
        {
            Line l = new Line(new Vector2(1,1),new Vector2(2,2),new Vector2(0,0));

            Assert.AreEqual(l.A,new Vector2(1,1));
            Assert.AreEqual(l.B, new Vector2(2, 2)); 
            Assert.AreEqual(new Vector2(0,0),l.GetRotationMiddle());
            Assert.AreEqual(Entity.Shape.LineSegment, l.GetShape);
        }

        [TestMethod]
        public void RotationTest()
        {
            Line l = new Line(new Vector2(1, 0), new Vector2(2, 0), new Vector2(0, 0));
            l.RotateDegrees(90);
            Assert.AreEqual(new Vector2(0,1),l.A);
            Assert.AreEqual(new Vector2(0, 2), l.B);
            Assert.AreEqual(new Vector2(0, 0), l.GetRotationMiddle());
            Assert.AreEqual(((float)Math.PI / 2.0f), l.Orientation);

        }

        [TestMethod]
        public void Rotation2Test()
        {
            Line l = new Line(new Vector2(4,3),new Vector2(5,3),new Vector2(3,3));
            l.RotateDegrees(90);
            Assert.AreEqual(new Vector2(3,3),l.GetRotationMiddle());
            Assert.AreEqual(new Vector2(3, 4), l.A);
            Assert.AreEqual(new Vector2(3, 5), l.B);
            Assert.AreEqual(((float)Math.PI / 2.0f), l.Orientation);
        }

        [TestMethod]
        public void Rotation3Test()
        {
            Line l = new Line(new Vector2(4, 3), new Vector2(5, 3), new Vector2(3, 3));
            l.RotateDegrees(-90);
            Assert.AreEqual(new Vector2(2,3),l.GetRotationMiddle());
            Assert.AreEqual(new Vector2(3, 2), l.A);
            Assert.AreEqual(new Vector2(3, 1), l.B);
            Assert.AreEqual(((float) Math.PI / 2.0f) * 3.0f, l.Orientation);
        }

        [TestMethod]
        public void Moving1Test()
        {
            Line l = new Line(new Vector2(3,3),new Vector2(4,4),new Vector2(3,3));
            l.MoveTo(new Vector2(4,2));
            Assert.AreEqual(new Vector2(4, 2), l.A);
            Assert.AreEqual(new Vector2(4, 2), l.GetRotationMiddle());
            Assert.AreEqual(new Vector2(5,3),l.B);
            Assert.AreEqual(0, l.Orientation);
        }

        [TestMethod]
        public void Moving2Test()
        {
            Line l = new Line(new Vector2(1,1),new Vector2(2,2),new Vector2(0,0));
            l.MoveTo(new Vector2(-1,-1));
            Assert.AreEqual(new Vector2(-1,-1), l.GetRotationMiddle());
            Assert.AreEqual(new Vector2(0, 0), l.A);
            Assert.AreEqual(new Vector2(1, 1), l.B);
            Assert.AreEqual(0,l.Orientation);
        }

        [TestMethod]
        public void Moving3Test()
        {
            Line l = new Line(new Vector2(5,0),new Vector2(5.5f,0),new Vector2(4,0));
            l.MoveTo(new Vector2(4.5f,0));
            Assert.AreEqual(new Vector2(4.5f,0),l.GetRotationMiddle());
            Assert.AreEqual(new Vector2(5.5f,0),l.A);
            Assert.AreEqual(new Vector2(6, 0), l.B);
            Assert.AreEqual(0, l.Orientation);
        }

        [TestMethod]
        public void LenghtTest()
        {
            Line l = new Line(new Vector2(0,1),new Vector2(0,2), new Vector2(0,0));
            l.RotateDegrees(370);
            Assert.AreEqual(l.Length,1);
            l.MoveTo(new Vector2(8.9f,9.1f));
        }
    }
}
