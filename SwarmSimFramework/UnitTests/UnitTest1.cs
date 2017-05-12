using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SwarmSimFramework.Classes.Entities;
using System.Numerics;
using System.Security.Cryptography;
using SwarmSimFramework.Classes;
using SwarmSimFramework.Classes.Map;

namespace UnitTests
{
    internal class Circle : CircleEntity
    {
        public Circle(Vector2 middle, float radius, float orientation = 0) : base(middle, radius, "CIRCLE", orientation)
        {

        }

        public Circle(Vector2 middle, float radius, Vector2 rotationMiddle, float orientation = 0) : base(middle,
            radius, "CIRCLE", rotationMiddle, orientation)
        {
            
        }
        public override Entity DeepClone()
        {
            throw new NotImplementedException();
        }
    }

    internal class Line : LineEntity
    {
        public Line(Vector2 a, Vector2 b, Vector2 rotationMiddle, float orientation = 0) : base(a, b, rotationMiddle,
            "LINE", orientation)
        {
        }

        public override Entity DeepClone()
        {
            throw new NotImplementedException();
        }
    }

    internal class EmptyRobot : RobotEntity
    {
        public EmptyRobot(Vector2 middle, float radius) : base(middle,radius,"EmptyRobot",new  IEffector[0],new ISensor[0], 0)
        {
        }
    }
    [TestClass]
    public class LineTests
    {
        [TestMethod]
        public void InitTest()
        {
            Line l = new Line(new Vector2(1, 1), new Vector2(2, 2), new Vector2(0, 0));

            Assert.AreEqual(l.A, new Vector2(1, 1));
            Assert.AreEqual(l.B, new Vector2(2, 2));
            Assert.AreEqual(new Vector2(0, 0), l.RotationMiddle);
            Assert.AreEqual(Entity.Shape.LineSegment, l.GetShape);
        }

        [TestMethod]
        public void Rotation1Test()
        {
            Line l = new Line(new Vector2(1, 0), new Vector2(2, 0), new Vector2(0, 0));
            l.RotateDegrees(90);
            Assert.AreEqual(new Vector2(0, 1), l.A);
            Assert.AreEqual(new Vector2(0, 2), l.B);
            Assert.AreEqual(new Vector2(0, 0), l.RotationMiddle);
            Assert.AreEqual(((float) Math.PI / 2.0f), l.Orientation);

        }

        [TestMethod]
        public void Rotation2Test()
        {
            Line l = new Line(new Vector2(4, 3), new Vector2(5, 3), new Vector2(3, 3));
            l.RotateDegrees(90);
            Assert.AreEqual(new Vector2(3, 3), l.RotationMiddle);
            Assert.AreEqual(new Vector2(3, 4), l.A);
            Assert.AreEqual(new Vector2(3, 5), l.B);
            Assert.AreEqual(((float) Math.PI / 2.0f), l.Orientation);
        }

        [TestMethod]
        public void Rotation3Test()
        {
            Line l = new Line(new Vector2(4, 3), new Vector2(5, 3), new Vector2(3, 3));
            l.RotateDegrees(-90);
            Assert.AreEqual(new Vector2(3, 3), l.RotationMiddle);
            Assert.AreEqual(new Vector2(3, 2), l.A);
            Assert.AreEqual(new Vector2(3, 1), l.B);
            Assert.AreEqual(((float) Math.PI / 2.0f) * 3.0f, l.Orientation);
        }

        [TestMethod]
        public void Moving1Test()
        {
            Line l = new Line(new Vector2(3, 3), new Vector2(4, 4), new Vector2(3, 3));
            l.MoveTo(new Vector2(4, 2));
            Assert.AreEqual(new Vector2(4, 2), l.A);
            Assert.AreEqual(new Vector2(4, 2), l.RotationMiddle);
            Assert.AreEqual(new Vector2(5, 3), l.B);
            Assert.AreEqual(0, l.Orientation);
        }

        [TestMethod]
        public void Moving2Test()
        {
            Line l = new Line(new Vector2(1, 1), new Vector2(2, 2), new Vector2(0, 0));
            l.MoveTo(new Vector2(-1, -1));
            Assert.AreEqual(new Vector2(-1, -1), l.RotationMiddle);
            Assert.AreEqual(new Vector2(0, 0), l.A);
            Assert.AreEqual(new Vector2(1, 1), l.B);
            Assert.AreEqual(0, l.Orientation);
        }

        [TestMethod]
        public void Moving3Test()
        {
            Line l = new Line(new Vector2(5, 0), new Vector2(5.5f, 0), new Vector2(4, 0));
            l.MoveTo(new Vector2(4.5f, 0));
            Assert.AreEqual(new Vector2(4.5f, 0), l.RotationMiddle);
            Assert.AreEqual(new Vector2(5.5f, 0), l.A);
            Assert.AreEqual(new Vector2(6, 0), l.B);
            Assert.AreEqual(0, l.Orientation);
        }

        [TestMethod]
        public void LenghtTest()
        {
            Line l = new Line(new Vector2(0, 1), new Vector2(0, 2), new Vector2(0, 0));
            l.RotateDegrees(370);
            Assert.AreEqual(l.Length, 1);
            l.MoveTo(new Vector2(8.9f, 9.1f));
            Assert.AreEqual(1, l.Length);
        }
    }

    [TestClass]
    public class CircleTests
    {


        [TestMethod]
        public void InitTest()
        {
            Circle c = new Circle(new Vector2(4, 3), 1);
            Assert.AreEqual(new Vector2(4, 3), c.Middle);
            Assert.AreEqual(new Vector2(4, 4), c.FPoint);
            Assert.AreEqual(1, c.Radius);
            Assert.AreEqual(new Vector2(4, 3), c.RotationMiddle);
            Assert.AreEqual(Entity.Shape.Circle, c.GetShape);
            //JIT methods 
            c.RotateDegrees(3);
            c.MoveTo(new Vector2(0, 0));
        }

        [TestMethod]
        public void Rotation1Test()
        {
            Circle c = new Circle(new Vector2(4, 3), 1, Entity.DegreesToRadians(90));
            Assert.AreEqual(new Vector2(3, 3), c.FPoint);
            Assert.AreEqual(new Vector2(4, 3), c.Middle);
            Assert.AreEqual(Entity.DegreesToRadians(90), c.Orientation);
        }

        [TestMethod]
        public void Rotation2Test()
        {
            Circle c = new Circle(new Vector2(2, 0), 2);
            Assert.AreEqual(new Vector2(2, 2), c.FPoint);
            c.RotateDegrees(-90);
            Assert.AreEqual(new Vector2(4, 0), c.FPoint);
        }

        [TestMethod]
        public void Rotation3Test()
        {
            Circle c = new Circle(new Vector2(5, 1), 1);
            c.RotateDegrees(90);
            Assert.AreEqual(new Vector2(4, 1), c.FPoint);
            c.MoveTo(1.0f);
            Assert.AreEqual(new Vector2(4, 1), c.Middle);
            Assert.AreEqual(new Vector2(3, 1), c.FPoint);
            c.RotateDegrees(180);
            c.MoveTo(1.0f);
            Assert.AreEqual(new Vector2(5, 1), c.Middle);
            Assert.AreEqual(new Vector2(6, 1), c.FPoint);
            c.RotateDegrees(90);
            Assert.AreEqual(new Vector2(5, 2), c.FPoint);
        }

        [TestMethod]
        public void Moving1Test()
        {
            CircleEntity c = new Circle(Vector2.Zero, 1);
            c.MoveTo(1);
            Assert.AreEqual(new Vector2(0, 1), c.Middle);
            Assert.AreEqual(new Vector2(0, 2), c.FPoint);
        }

        [TestMethod]
        public void Moving2Test()
        {
            CircleEntity c = new Circle(new Vector2(5, 1), 2, Entity.DegreesToRadians(90));
            c.MoveTo(2);
            Assert.AreEqual(new Vector2(3, 1), c.Middle);
            Assert.AreEqual(new Vector2(1, 1), c.FPoint);
        }

        [TestMethod]
        public void Moving3Test()
        {
            CircleEntity c = new Circle(new Vector2(2, 3), 1, Entity.DegreesToRadians(-135));
            c.MoveTo((float) Math.Sqrt(2.0));
            Assert.AreEqual(new Vector2(3, 2), c.Middle);
        }

        [TestMethod]
        public void MovingToPoint1Test()
        {
            CircleEntity c = new Circle(new Vector2(2, 3), 1);
            c.MoveTo(new Vector2(0, 0));
            Assert.AreEqual(new Vector2(0, 0), c.Middle);
            Assert.AreEqual(new Vector2(0, 1), c.FPoint);
        }

        [TestMethod]
        public void MovingToPoint2Test()
        {

            CircleEntity c = new Circle(Vector2.Zero, 1, Entity.DegreesToRadians(90));
            c.MoveTo(new Vector2(4, 1));
            Assert.AreEqual(new Vector2(3, 1), c.FPoint);
            Assert.AreEqual(new Vector2(4, 1), c.Middle);
            Assert.AreEqual(new Vector2(4, 1), c.RotationMiddle);
            Assert.AreEqual(Entity.DegreesToRadians(90), c.Orientation);
        }

        [TestMethod]
        public void DifferentRotationMiddle()
        {
            CircleEntity c = new Circle(new Vector2(0, 3), 1, Vector2.Zero); 
            c.RotateDegrees(90);
            Assert.AreEqual(new Vector2(-3,0),c.Middle);
            Assert.AreEqual(new Vector2(-4,0),c.FPoint);
            c.RotateDegrees(90);
            Assert.AreEqual(new Vector2(0,-3),c.Middle);
            Assert.AreEqual(new Vector2(0,-4),c.FPoint);
            c.RotateDegrees(90);
            Assert.AreEqual(new Vector2(3,0),c.Middle);
            Assert.AreEqual(new Vector2(4,0),c.FPoint);
            c.RotateDegrees(90);
            Assert.AreEqual(new Vector2(0,3),c.Middle);
            Assert.AreEqual(new Vector2(0,4),c.FPoint);
        }
    }

    [TestClass]
    public class CircleCollisionMapTests
    {
        [TestMethod]
        public void InitTest()
        {
            Map map = new Map(400, 600, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            //Borders assignment check
            Assert.AreEqual(new Vector2(0, 0), map.A);
            Assert.AreEqual(new Vector2(600, 0), map.B);
            Assert.AreEqual(new Vector2(0, 400), map.C);
            Assert.AreEqual(new Vector2(600, 400), map.D);
            Assert.AreEqual(0, map.Cycle);
            Assert.AreEqual(400, map.MaxHeight);
            Assert.AreEqual(600, map.MaxWidth);
            Assert.AreEqual(map.FuelEntities.Count, 0);
        }

        [TestMethod]
        public void CircleBordersCollisionTest()
        {
            Map map = new Map(400, 600, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            Assert.IsTrue(map.Collision(new Circle(new Vector2(1, 1), 1)));
        }

        [TestMethod]
        public void CircleBorderCollision2Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            Assert.IsFalse(map.Collision(new Circle(new Vector2(2, 2), 1)));
        }

        [TestMethod]
        public void CircleBorderCollision3Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            Assert.IsTrue(map.Collision(new Circle(new Vector2(99, 5), 2)));
        }

        [TestMethod]
        public void CircleBorderCollison4Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            Assert.IsTrue(map.Collision(new Circle(new Vector2(5, 99), 1)));
        }

        [TestMethod]
        public void CircleCircleCollision1Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(50, 50), 1));
            Circle c = new Circle(new Vector2(45, 45), 2);
            Assert.IsFalse(map.Collision(c));
            Assert.IsTrue(map.Collision(c, new Vector2(48, 48)));
        }

        [TestMethod]
        public void CircleCircleCollision2Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(50, 50), 1));
            Circle c = new Circle(new Vector2(45, 45), 2);
            Assert.IsTrue(map.Collision(c, new Vector2(49, 49)));
        }

        [TestMethod]
        public void CircleCircleCollision3Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(50, 50), 1));
            Circle c = new Circle(new Vector2(48, 48), 1);
            Assert.IsFalse(map.Collision(c));
            c.MoveTo(new Vector2(49, 49));
            Assert.IsTrue(map.Collision(c));
        }
    }

    [TestClass]
    public class LineCollisionMapTests
    {
        [TestMethod]
        public void VectorPosInfTest()
        {
            Vector2 inf = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity), inf);
        }

        [TestMethod]
        public void InitTest()
        {
            Map map = new Map(150, 150, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            Line l = new Line(new Vector2(60, 60), new Vector2(50, 60), new Vector2(40, 60));
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity), map.Collision(l).IntersectionPoint);
            Assert.AreEqual(map.Collision(l).CollidingEntity, null);
        }

        [TestMethod]
        public void NoInterSection1Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            Line l = new Line(new Vector2(50, 50), new Vector2(50, 40), new Vector2(50, 60));
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity), map.Collision(l).IntersectionPoint);
            l.RotateDegrees(90);
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity), map.Collision(l).IntersectionPoint);
            l.RotateDegrees(180);
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity), map.Collision(l).IntersectionPoint);
            l.MoveTo(new Vector2(40, 40));
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity), map.Collision(l).IntersectionPoint);
        }

        [TestMethod]
        public void NoInterSection2Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(3, 2), 1));
            Line l = new Line(new Vector2(3, 0.999f), new Vector2(3, 0.5f), new Vector2(3, 0));
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity), map.Collision(l).IntersectionPoint);
        }

        [TestMethod]
        public void NoInterSection3Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(3, 2), 1));
            map.PasiveEntities.Add(new Circle(new Vector2(6, 2), 1));
            Line l = new Line(new Vector2(4.5f, 1), new Vector2(4.5f, 2), new Vector2(4.5f, 0));
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity), map.Collision(l).IntersectionPoint);
        }

        [TestMethod]
        public void InterSection1Test()
        {
            Map map = new Map(150, 150, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(2, 2), 1));
            Line l = new Line(new Vector2(2, 4), new Vector2(2, 3), new Vector2(2, 4));
            Assert.AreEqual(new Vector2(2, 3), map.Collision(l).IntersectionPoint);
            l.MoveTo(new Vector2(2, 3.5f));
            Assert.AreEqual(new Vector2(2, 3), map.Collision(l).IntersectionPoint);
        }

        [TestMethod]
        public void InterSection2Test()
        {
            Map map = new Map(150, 150, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(2.5f, 1.5f), 0.5f));
            map.PasiveEntities.Add(new Circle(new Vector2(3.5f, 0.5f), 0.5f));
            Line l = new Line(new Vector2(1, 3), new Vector2(4, 0), new Vector2(1, 3));
            Vector2 i = map.Collision(l).IntersectionPoint;
            Assert.IsTrue(i.X > 2 && i.X < 2.5f && i.Y > 1.5f && i.Y < 2.0);
        }

        [TestMethod]
        public void InterSection3Test()
        {
            Map map = new Map(150, 150, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(2.5f, 1.5f), 0.5f));
            Line l = new Line(new Vector2(1, 1.5f), new Vector2(2, 1.5f), new Vector2(1, 1.5f));
            Assert.AreEqual(new Vector2(2, 1.5f), map.Collision(l).IntersectionPoint);
        }
    }

    [TestClass]
    public class IntervalMapping
    {
        [TestMethod]
        public void BoundsTest()
        {
            var b = Entity.MakeNormalizationFunc(new Bounds() {Max =2, Min = 0},
                new Bounds() {Min = -100, Max = 100});
            Assert.AreEqual(100, b.Rescale);
            Assert.AreEqual(-100,b.Shift);
        }
    }

    [TestClass]
    public class TouchSensorTest
    {
        [TestMethod]
        public void InitTest1()
        {
            EmptyRobot r = new EmptyRobot(new Vector2(2,0),1);
            TouchSensor s1 = new TouchSensor(r,0.5f,0);
            TouchSensor s2 = new TouchSensor(r,0.5f,Entity.DegreesToRadians(180));
            Assert.AreEqual(s1.Middle,new Vector2(2,1));
            Assert.AreEqual(s1.Radius,0.5f);
            Assert.AreEqual(s1.FPoint,new Vector2(2,1.5f));
            Assert.AreEqual(s2.Middle,new Vector2(2,-1));
            Assert.AreEqual(s2.FPoint,new Vector2(2,-0.5f));
            Assert.AreEqual(s2.Radius,0.5f);
            TouchSensor s3 = new TouchSensor(r,0.5f,Entity.DegreesToRadians(90));
            Assert.AreEqual(s3.Middle, new Vector2(1,0));
            Assert.AreEqual(s3.Radius, 0.5f);
            Assert.AreEqual(s3.FPoint, new Vector2(1, 0.5f));
            TouchSensor s4 = new TouchSensor(r, 0.5f,Entity.DegreesToRadians(270));
            Assert.AreEqual(s4.Middle, new Vector2(3, 0));
            Assert.AreEqual(s4.Radius, 0.5f);
            Assert.AreEqual(s4.FPoint, new Vector2(3, 0.5f));
        }

        [TestMethod]
        public void Touch1Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>()); 
            map.PasiveEntities.Add(new Circle(new Vector2(0.5f,2),0.2f ));
            EmptyRobot r = new EmptyRobot(new Vector2(2, 2), 1);
            TouchSensor s1 = new TouchSensor(r, 0.5f, 0);
            TouchSensor s2 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(180));
            TouchSensor s3 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(90));
            TouchSensor s4 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(270));
            var o = s1.Count(r, map); 
            Assert.AreEqual(-100f,o[0]);
            o = s2.Count(r, map);
            Assert.AreEqual(-100.0f,o[0]);
            o = s3.Count(r, map);
            Assert.AreEqual( 100.0f,o[0]);
            o = s4.Count(r, map);
            Assert.AreEqual(-100.0f,o[0]);
        }

        [TestMethod]
        public void Touch2Test()
        {

            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(0.5f, 2), 0.2f));
            EmptyRobot r = new EmptyRobot(new Vector2(1, 1), 1);
            TouchSensor s1 = new TouchSensor(r, 0.5f, 0);
            TouchSensor s2 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(180));
            TouchSensor s3 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(90));
            TouchSensor s4 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(270));
            r.MoveTo(new Vector2(2,2));
            var o = s1.Count(r, map);
            Assert.AreEqual(-100f, o[0]);
            o = s2.Count(r, map);
            Assert.AreEqual(-100.0f, o[0]);
            o = s3.Count(r, map);
            Assert.AreEqual(100.0f, o[0]);
            o = s4.Count(r, map);
            Assert.AreEqual(-100.0f, o[0]);
        }

        [TestMethod]
        public void Touch3Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(0.5f, 2), 0.2f));
            EmptyRobot r = new EmptyRobot(new Vector2(1, 1), 1);
            TouchSensor s1 = new TouchSensor(r, 0.5f, 0);
            TouchSensor s2 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(180));
            TouchSensor s3 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(90));
            TouchSensor s4 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(270));
            r.MoveTo(new Vector2(2, 2));

            var o = s1.Count(r, map);
            Assert.AreEqual(-100f, o[0]);
            o = s2.Count(r, map);
            Assert.AreEqual(-100.0f, o[0]);
            o = s3.Count(r, map);
            Assert.AreEqual(100.0f, o[0]);
            o = s4.Count(r, map);
            Assert.AreEqual(-100.0f, o[0]);

            r.RotateDegrees(90);
            o = s1.Count(r, map);
            Assert.AreEqual(100f, o[0]);
            o = s2.Count(r, map);
            Assert.AreEqual(-100.0f, o[0]);
            o = s3.Count(r, map);
            Assert.AreEqual(-100.0f, o[0]);
            o = s4.Count(r, map);
            Assert.AreEqual(-100.0f, o[0]);

            r.RotateDegrees(90);
            o = s1.Count(r, map);
            Assert.AreEqual(-100f, o[0]);
            o = s2.Count(r, map);
            Assert.AreEqual(-100.0f, o[0]);
            o = s3.Count(r, map);
            Assert.AreEqual(-100.0f, o[0]);
            o = s4.Count(r, map);
            Assert.AreEqual(100.0f, o[0]);

            r.RotateDegrees(90);
            o = s1.Count(r, map);
            Assert.AreEqual(-100f, o[0]);
            o = s2.Count(r, map);
            Assert.AreEqual(100.0f, o[0]);
            o = s3.Count(r, map);
            Assert.AreEqual(-100.0f, o[0]);
            o = s4.Count(r, map);
            Assert.AreEqual(-100.0f, o[0]);
        }
    }

    [TestClass]
    public class LineTypeSensorTests
    {
        
    }
}
