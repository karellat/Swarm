using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SwarmSimFramework.Classes.Entities;
using System.Numerics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using SwarmSimFramework.Classes;
using SwarmSimFramework.Classes.Effectors;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.SupportClasses;

namespace UnitTests
{
    internal class Circle : CircleEntity
    {
        public Circle(Vector2 middle, float radius, float orientation = 0) : base(middle, radius, "CIRCLE", orientation)
        {
            Color = EntityColor.ObstacleColor;
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
        public EmptyRobot(Vector2 middle, float radius, int containerSize = 0) : base(middle, radius, "EmptyRobot",
            new IEffector[0], new ISensor[0], 100, containerSize)
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
            Assert.AreEqual(new Vector2(-3, 0), c.Middle);
            Assert.AreEqual(new Vector2(-4, 0), c.FPoint);
            c.RotateDegrees(90);
            Assert.AreEqual(new Vector2(0, -3), c.Middle);
            Assert.AreEqual(new Vector2(0, -4), c.FPoint);
            c.RotateDegrees(90);
            Assert.AreEqual(new Vector2(3, 0), c.Middle);
            Assert.AreEqual(new Vector2(4, 0), c.FPoint);
            c.RotateDegrees(90);
            Assert.AreEqual(new Vector2(0, 3), c.Middle);
            Assert.AreEqual(new Vector2(0, 4), c.FPoint);
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
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity),
                map.Collision(l).IntersectionPoint);
            Assert.AreEqual(map.Collision(l).CollidingEntity, null);
        }

        [TestMethod]
        public void NoInterSection1Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            Line l = new Line(new Vector2(50, 50), new Vector2(50, 40), new Vector2(50, 60));
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity),
                map.Collision(l).IntersectionPoint);
            l.RotateDegrees(90);
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity),
                map.Collision(l).IntersectionPoint);
            l.RotateDegrees(180);
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity),
                map.Collision(l).IntersectionPoint);
            l.MoveTo(new Vector2(40, 40));
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity),
                map.Collision(l).IntersectionPoint);
        }

        [TestMethod]
        public void NoInterSection2Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(3, 2), 1));
            Line l = new Line(new Vector2(3, 0.999f), new Vector2(3, 0.5f), new Vector2(3, 0));
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity),
                map.Collision(l).IntersectionPoint);
        }

        [TestMethod]
        public void NoInterSection3Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(3, 2), 1));
            map.PasiveEntities.Add(new Circle(new Vector2(6, 2), 1));
            Line l = new Line(new Vector2(4.5f, 1), new Vector2(4.5f, 2), new Vector2(4.5f, 0));
            Assert.AreEqual(new Vector2(float.PositiveInfinity, float.PositiveInfinity),
                map.Collision(l).IntersectionPoint);
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
            var b = Entity.MakeNormalizationFunc(new Bounds() {Max = 2, Min = 0},
                new Bounds() {Min = -100, Max = 100});
            Assert.AreEqual(100, b.Rescale);
            Assert.AreEqual(-100, b.Shift);
        }
    }

    [TestClass]
    public class TouchSensorTest
    {
        [TestMethod]
        public void InitTest1()
        {
            EmptyRobot r = new EmptyRobot(new Vector2(2, 0), 1);
            TouchSensor s1 = new TouchSensor(r, 0.5f, 0);
            TouchSensor s2 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(180));
            Assert.AreEqual(s1.Middle, new Vector2(2, 1));
            Assert.AreEqual(s1.Radius, 0.5f);
            Assert.AreEqual(s1.FPoint, new Vector2(2, 1.5f));
            Assert.AreEqual(s2.Middle, new Vector2(2, -1));
            Assert.AreEqual(s2.FPoint, new Vector2(2, -0.5f));
            Assert.AreEqual(s2.Radius, 0.5f);
            TouchSensor s3 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(90));
            Assert.AreEqual(s3.Middle, new Vector2(1, 0));
            Assert.AreEqual(s3.Radius, 0.5f);
            Assert.AreEqual(s3.FPoint, new Vector2(1, 0.5f));
            TouchSensor s4 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(270));
            Assert.AreEqual(s4.Middle, new Vector2(3, 0));
            Assert.AreEqual(s4.Radius, 0.5f);
            Assert.AreEqual(s4.FPoint, new Vector2(3, 0.5f));
        }

        [TestMethod]
        public void Touch1Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            map.PasiveEntities.Add(new Circle(new Vector2(0.5f, 2), 0.2f));
            EmptyRobot r = new EmptyRobot(new Vector2(2, 2), 1);
            TouchSensor s1 = new TouchSensor(r, 0.5f, 0);
            TouchSensor s2 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(180));
            TouchSensor s3 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(90));
            TouchSensor s4 = new TouchSensor(r, 0.5f, Entity.DegreesToRadians(270));
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
        public void Touch2Test()
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
        public static float ObstacleColorCount =
            (float) Math.Round((float) Entity.EntityColor.ObstacleColor / (float) Entity.EntityColorCount * 200 - 100,
                4);

        [TestMethod]
        public void InitTest()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            LineTypeSensor l1 = new LineTypeSensor(r, 1, 0);
            LineTypeSensor l2 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(90));
            LineTypeSensor l3 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(180));
            LineTypeSensor l4 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(270));

            Assert.AreEqual(new Vector2(3, 4), l1.A);
            Assert.AreEqual(new Vector2(3, 5), l1.B);
            Assert.AreEqual(new Vector2(2, 3), l2.A);
            Assert.AreEqual(new Vector2(1, 3), l2.B);
            Assert.AreEqual(new Vector2(3, 2), l3.A);
            Assert.AreEqual(new Vector2(3, 1), l3.B);
            Assert.AreEqual(new Vector2(4, 3), l4.A);
            Assert.AreEqual(new Vector2(5, 3), l4.B);
        }

        [TestMethod]
        public void IntersectionTest()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            LineTypeSensor l1 = new LineTypeSensor(r, 1, 0);
            LineTypeSensor l2 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(90));
            LineTypeSensor l3 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(180));
            LineTypeSensor l4 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(270));

            map.PasiveEntities.Add(new Circle(new Vector2(3, 6), 1));
            float[] o = l1.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            Assert.AreEqual(o[1], 100.0f);
            for (int i = 2; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i],-100.0f);
            }
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
        }

        [TestMethod]
        public void Intersection2Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            LineTypeSensor l1 = new LineTypeSensor(r, 1, 0);
            LineTypeSensor l2 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(90));
            LineTypeSensor l3 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(180));
            LineTypeSensor l4 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(270));

            map.PasiveEntities.Add(new Circle(new Vector2(3, 5.5f), 1));
            float[] o = l1.Count(r, map);
            Assert.AreEqual(o[0], 0.0f);
            Assert.AreEqual(o[1], 100.0f);
            for (int i = 2; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
        }

        [TestMethod]
        public void RotationTest()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            LineTypeSensor l1 = new LineTypeSensor(r, 1, 0);
            LineTypeSensor l2 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(90));
            LineTypeSensor l3 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(180));
            LineTypeSensor l4 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(270));
            map.PasiveEntities.Add(new Circle(new Vector2(3, 5.5f), 1));

            float[] o = l1.Count(r, map);
            Assert.AreEqual(o[0], 0.0f);
            Assert.AreEqual(o[1], 100.0f);
            for (int i = 2; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            r.RotateDegrees(-90);
            o = l1.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 0.0f);
            Assert.AreEqual(o[1], 100.0f);
            for (int i = 2; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            r.RotateDegrees(-90);
            o = l1.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 0.0f);
            Assert.AreEqual(o[1], 100.0f);
            for (int i = 2; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }

            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            r.RotateDegrees(-90);
            o = l1.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 0.0f);
            Assert.AreEqual(o[1], 100.0f);
            for (int i = 2; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
        }

        [TestMethod]
        public void MovingTest()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            LineTypeSensor l1 = new LineTypeSensor(r, 1, 0);
            LineTypeSensor l2 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(90));
            LineTypeSensor l3 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(180));
            LineTypeSensor l4 = new LineTypeSensor(r, 1, Entity.DegreesToRadians(270));
            map.PasiveEntities.Add(new Circle(new Vector2(7, 3), 1.5f));

            float[] o = l1.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }

            r.MoveTo(new Vector2(4, 3));



            o = l1.Count(r, map);
            Assert.AreEqual(new Vector2(4, 5), l1.B);
            Assert.AreEqual(new Vector2(4, 4), l1.A);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l2.Count(r, map);
            Assert.AreEqual(new Vector2(2, 3), l2.B);
            Assert.AreEqual(new Vector2(3, 3), l2.A);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l3.Count(r, map);
            Assert.AreEqual(new Vector2(4, 2), l3.A);
            Assert.AreEqual(new Vector2(4, 1), l3.B);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l4.Count(r, map);

            Assert.AreEqual(new Vector2(5, 3), l4.A);
            Assert.AreEqual(new Vector2(6, 3), l4.B);
            Assert.AreEqual(o[0], 0.0f);
            Assert.AreEqual(o[1], 100.0f);
            for (int i = 2; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }

            r.MoveTo(new Vector2(4.5f, 3));

            o = l1.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            for (int i = 1; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], -100.0f);
            Assert.AreEqual(o[1], 100.0f);
            for (int i = 2; i <= Entity.EntityColorCount; i++)
            {
                Assert.AreEqual(o[i], -100.0f);
            }
        }
    }

    [TestClass]
    public class RadioSensorTests
    {
        public static float[] NonCollidingOutput = new[] {-100.0f,0.0f,0.0f, -100.0f, 0.0f, 0.0f , -100.0f, 0.0f, 0.0f };

        [TestMethod]
        public void InitTest()
        {
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            RadioSensor rs = new RadioSensor(r, 2);
            Assert.AreEqual(new Vector2(3, 3), rs.Middle);
            Assert.AreEqual(new Vector2(3, 3), rs.FPoint);
            var o = rs.Count(r, map);
            TestExtensions.AssertArrayEquality(o, NonCollidingOutput);

        }

        [TestMethod]
        public void SingleIntersectionTest()
        {
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            RadioSensor rs = new RadioSensor(r, 2);
            map.RadioEntities.Add(new RadioEntity(new Vector2(3, 5), 0.1f, ((int) RadioEntity.SignalValueBounds.Max)-1));
            float[] predOutput = new[] {-100.0f, 0, 0, -100.0f, 0, 0, 100.0f,0,100};
            TestExtensions.AssertArrayEquality(predOutput, rs.Count(r, map));
        }

        [TestMethod]
        public void DoubleIntersectionTest()
        {
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            RadioSensor rs = new RadioSensor(r, 2);
            map.RadioEntities.Add(new RadioEntity(new Vector2(3, 5), 0.1f, 0));
            map.RadioEntities.Add(new RadioEntity(new Vector2(3, 1), 0.1f, 1));
            float[] predOutput = new[] {100.0f, 0, 100.0f, 100.0f, 0, -100.0f,-100.0f,0,0};
            TestExtensions.AssertArrayEquality(predOutput, rs.Count(r, map));
        }

        [TestMethod]
        public void DoubleSameValueTest()
        {
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            RadioSensor rs = new RadioSensor(r, 2);
            map.RadioEntities.Add(new RadioEntity(new Vector2(3, 5), 0.1f, 0));
            map.RadioEntities.Add(new RadioEntity(new Vector2(3, 1), 0.1f, 1));
            map.RadioEntities.Add(new RadioEntity(new Vector2(4, 5), 0.5f, 0));

            float[] predOutput = new[] {100.0f,0.5f * (float) Math.Sqrt(4.0f/17.0f) * 100.0f, 2.0f * (float)Math.Sqrt(4.0f / 17.0f) * 100.0f, 100.0f, 0, -100.0f,-100.0f,0.0f,0.0f};
            TestExtensions.AssertArrayEquality(predOutput, rs.Count(r, map));
        }

        [TestMethod]
        public void ManySignalsTest()
        {
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            RadioSensor rs = new RadioSensor(r, 2);
            map.RadioEntities.Add(new RadioEntity(new Vector2(3, 5), 0.1f, 1));
            map.RadioEntities.Add(new RadioEntity(new Vector2(4, 5), 0.1f, 1));
            map.RadioEntities.Add(new RadioEntity(new Vector2(4, 5), 0.5f, 0));
            map.RadioEntities.Add(new RadioEntity(new Vector2(4, 5), 0.5f, 0));
            map.RadioEntities.Add(new RadioEntity(new Vector2(4, 5), 0.5f, 0));
            map.RadioEntities.Add(new RadioEntity(new Vector2(4, 5), 0.5f, 2));
            float sqt5 = (float) Math.Sqrt(5);
            float[] predOutput = new[] {100.0f, 1.0f/sqt5 * 100, 2.0f/sqt5 * 100, 100, 0, 100.0f,100.0f, 1.0f / sqt5 * 100, 2.0f / sqt5 * 100};
            TestExtensions.AssertArrayEquality(predOutput, rs.Count(r, map));
        }

    }

    [TestClass]
    public class FuelLineSensorTests
    {
        [TestMethod]
        public void InitTest()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            FuelLineSensor l1 = new FuelLineSensor(r, 1, 0);
            FuelLineSensor l2 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(90));
            FuelLineSensor l3 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(180));
            FuelLineSensor l4 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(270));

            Assert.AreEqual(new Vector2(3, 4), l1.A);
            Assert.AreEqual(new Vector2(3, 5), l1.B);
            Assert.AreEqual(new Vector2(2, 3), l2.A);
            Assert.AreEqual(new Vector2(1, 3), l2.B);
            Assert.AreEqual(new Vector2(3, 2), l3.A);
            Assert.AreEqual(new Vector2(3, 1), l3.B);
            Assert.AreEqual(new Vector2(4, 3), l4.A);
            Assert.AreEqual(new Vector2(5, 3), l4.B);
        }

        [TestMethod]
        public void IntersectionTest()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            FuelLineSensor l1 = new FuelLineSensor(r, 1, 0);
            FuelLineSensor l2 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(90));
            FuelLineSensor l3 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(180));
            FuelLineSensor l4 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(270));

            map.FuelEntities.Add(new FuelEntity(new Vector2(3, 6), 1, 0));
            float[] o = l1.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
        }

        [TestMethod]
        public void Intersection2Test()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            FuelLineSensor l1 = new FuelLineSensor(r, 1, 0);
            FuelLineSensor l2 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(90));
            FuelLineSensor l3 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(180));
            FuelLineSensor l4 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(270));

            map.FuelEntities.Add(new FuelEntity(new Vector2(3, 5.5f), 1, 0));
            float[] o = l1.Count(r, map);
            Assert.AreEqual(o[0], 0.0f);
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
        }

        [TestMethod]
        public void RotationTest()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            FuelLineSensor l1 = new FuelLineSensor(r, 1, 0);
            FuelLineSensor l2 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(90));
            FuelLineSensor l3 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(180));
            FuelLineSensor l4 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(270));
            map.FuelEntities.Add(new FuelEntity(new Vector2(3, 5.5f), 1, 0));

            float[] o = l1.Count(r, map);
            Assert.AreEqual(o[0], 0.0f);
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            r.RotateDegrees(-90);
            o = l1.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 0.0f);
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            r.RotateDegrees(-90);
            o = l1.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 0.0f);
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            r.RotateDegrees(-90);
            o = l1.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 0.0f);
        }

        [TestMethod]
        public void MovingTest()
        {
            Map map = new Map(100, 100, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(3, 3), 1);
            FuelLineSensor l1 = new FuelLineSensor(r, 1, 0);
            FuelLineSensor l2 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(90));
            FuelLineSensor l3 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(180));
            FuelLineSensor l4 = new FuelLineSensor(r, 1, Entity.DegreesToRadians(270));
            map.FuelEntities.Add(new FuelEntity(new Vector2(7, 3), 1.5f, 0));

            float[] o = l1.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);

            r.MoveTo(new Vector2(4, 3));



            o = l1.Count(r, map);
            Assert.AreEqual(new Vector2(4, 5), l1.B);
            Assert.AreEqual(new Vector2(4, 4), l1.A);
            Assert.AreEqual(o[0], 100.0f);
            o = l2.Count(r, map);
            Assert.AreEqual(new Vector2(2, 3), l2.B);
            Assert.AreEqual(new Vector2(3, 3), l2.A);
            Assert.AreEqual(o[0], 100.0f);
            o = l3.Count(r, map);
            Assert.AreEqual(new Vector2(4, 2), l3.A);
            Assert.AreEqual(new Vector2(4, 1), l3.B);
            Assert.AreEqual(o[0], 100.0f);
            o = l4.Count(r, map);

            Assert.AreEqual(new Vector2(5, 3), l4.A);
            Assert.AreEqual(new Vector2(6, 3), l4.B);
            Assert.AreEqual(o[0], 0.0f);

            r.MoveTo(new Vector2(4.5f, 3));

            o = l1.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l2.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l3.Count(r, map);
            Assert.AreEqual(o[0], 100.0f);
            o = l4.Count(r, map);
            Assert.AreEqual(o[0], -100.0f);
        }
    }

    [TestClass]
    public class LocatorTests
    {
        [TestMethod]
        public void InitTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());

            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1);
            LocatorSensor ls = new LocatorSensor(r);
            TestExtensions.AssertArrayEquality(new[] {0.0f, 0.0f, 0.0f, 100.0f}, ls.Count(r, map));

        }

        [TestMethod]
        public void MoveTest()
        {
            Map map = new Map(400, 400, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(200, 200), 1);
            LocatorSensor ls = new LocatorSensor(r);
            TestExtensions.AssertArrayEquality(new[] {0.0f, 0.0f, 0.0f, 100.0f}, ls.Count(r, map));
            r.MoveTo(new Vector2(200, 201));
            TestExtensions.AssertArrayEquality(new[] {0.0f, 0.5f, 0.0f, 100.0f}, ls.Count(r, map));
            r.MoveTo(new Vector2(201, 201));
            TestExtensions.AssertArrayEquality(new[] {0.5f, 0.5f, 0.0f, 100.0f}, ls.Count(r, map));
            r.MoveTo(new Vector2(198, 198));
            TestExtensions.AssertArrayEquality(new[] {-1, -1, 0, 100.0f}, ls.Count(r, map));
            r.MoveTo(new Vector2(134, 246));
            TestExtensions.AssertArrayEquality(new[] {-33, 23, 0, 100.0f}, ls.Count(r, map));
        }

        [TestMethod]
        public void RotationTest()
        {
            Map map = new Map(400, 400, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(200, 200), 1);
            LocatorSensor ls = new LocatorSensor(r);
            TestExtensions.AssertArrayEquality(new[] {0.0f, 0.0f, 0.0f, 100.0f}, ls.Count(r, map));
            r.RotateDegrees(90);
            TestExtensions.AssertArrayEquality(new[] {0, 0, -100.0f, 0}, ls.Count(r, map));
            r.RotateDegrees(90);
            TestExtensions.AssertArrayEquality(new[] {0, 0, 0, -100.0f}, ls.Count(r, map));
            r.RotateDegrees(90);
            TestExtensions.AssertArrayEquality(new[] {0, 0, 100.0f, 0}, ls.Count(r, map));
            r.RotateDegrees(90);
            TestExtensions.AssertArrayEquality(new[] {0.0f, 0.0f, 0.0f, 100.0f}, ls.Count(r, map));
            r.RotateDegrees(45);
            TestExtensions.AssertArrayEquality(new[] {0.0f, 0.0f, -70.71068f, 70.71068f}, ls.Count(r, map));
        }
    }

    [TestClass]
    public class TypeCircleSensorTests
    {
        [TestMethod]
        public void InitTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(30, 30), 5);
            TypeCircleSensor ts = new TypeCircleSensor(r, 50);
            TestExtensions.AssertArrayEquality(new[] {-100.0f, -100, -100}, ts.Count(r, map));
        }

        [TestMethod]
        public void IntersectionTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(20, 20), 5);
            TypeCircleSensor ts = new TypeCircleSensor(r, 50);
            map.PasiveEntities.Add(new Circle(new Vector2(10, 10), 1));
            map.PasiveEntities.Add(new Circle(new Vector2(13, 10), 1));
            map.PasiveEntities.Add(new Circle(new Vector2(10, 13), 1));
            map.PasiveEntities.Add(new Circle(new Vector2(13, 13), 1));
            for (int i = 0; i < 8; i++)
                map.PasiveEntities.Add(new Circle(new Vector2(13, 13), 1));
            map.Robots.Add(r);
            TestExtensions.AssertArrayEquality(new[] {-100.0f, 100, -100}, ts.Count(r, map));
        }

        [TestMethod]
        public void Intersection2Test()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(30, 30), 5);
            TypeCircleSensor ts = new TypeCircleSensor(r, 50);
            map.PasiveEntities.Add(new Circle(new Vector2(10, 10), 1));
            map.PasiveEntities.Add(new Circle(new Vector2(13, 10), 1));
            map.PasiveEntities.Add(new Circle(new Vector2(10, 13), 1));
            map.PasiveEntities.Add(new Circle(new Vector2(13, 13), 1));
            map.PasiveEntities.Add(new Circle(new Vector2(140, 140), 1));
            map.Robots.Add(r);
            TestExtensions.AssertArrayEquality(new[] {-100.0f, -20, -100}, ts.Count(r, map));
            r.MoveTo(new Vector2(160, 160));
            TestExtensions.AssertArrayEquality(new[] {-100.0f, -80, -100}, ts.Count(r, map));
        }
    }

    [TestClass]
    public class TwoWheelMotorTests
    {
        [TestMethod]
        public void InitTest()
        {
            TwoWheelMotor.MaxVelocityChange = 2.0f;
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1);
            TwoWheelMotor tm = new TwoWheelMotor(r, 1);
            for (int i = 0; i < 98; i++)
            {
                Vector2 m = r.Middle;
                m.Y++;
                tm.Effect(new[] {100.0f, 100}, r, map);
                Assert.AreEqual(m, tm.Middle);
                Assert.AreEqual(m, r.Middle);
            }
            Vector2 m2 = r.Middle;
            tm.Effect(new[] {100.0f, 100}, r, map);
            Assert.AreEqual(m2, tm.Middle);
            Assert.AreEqual(m2, r.Middle);
            Assert.AreEqual(1, r.CollisionDetected);
            for (int i = 0; i < 196; i++)
            {
                Vector2 m = r.Middle;
                m.Y--;
                tm.Effect(new[] {-100.0f, -100.0f}, r, map);
                Assert.AreEqual(m, tm.Middle);
                Assert.AreEqual(m, r.Middle);
            }
            m2 = r.Middle;
            tm.Effect(new[] {-100.0f, -100.0f}, r, map);
            Assert.AreEqual(m2, tm.Middle);
            Assert.AreEqual(m2, r.Middle);
            Assert.AreEqual(2, r.CollisionDetected);


        }

        [TestMethod]
        public void RotationTest()
        {
            TwoWheelMotor.MaxVelocityChange = 300;
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1);
            TwoWheelMotor twm = new TwoWheelMotor(r, (float) Math.PI / 2.0f);
            Vector2 m = r.Middle;

            twm.Effect(new[] {-100.0f, 100.0f}, r, map);
            Assert.AreEqual(m, r.Middle);
            Assert.AreEqual(m, twm.Middle);
            Assert.AreEqual(new Vector2(101, 100), r.FPoint);
            Assert.AreEqual(new Vector2(101, 100), twm.FPoint);
            twm.Effect(new[] {-100.0f, 100.0f}, r, map);
            Assert.AreEqual(m, r.Middle);
            Assert.AreEqual(m, twm.Middle);
            Assert.AreEqual(new Vector2(100, 99), r.FPoint);
            Assert.AreEqual(new Vector2(100, 99), twm.FPoint);
            twm.Effect(new[] {-100.0f, 100.0f}, r, map);
            Assert.AreEqual(m, r.Middle);
            Assert.AreEqual(m, twm.Middle);
            Assert.AreEqual(new Vector2(99, 100), r.FPoint);
            Assert.AreEqual(new Vector2(99, 100), twm.FPoint);
            twm.Effect(new[] {-100.0f, 100.0f}, r, map);
            Assert.AreEqual(m, r.Middle);
            Assert.AreEqual(m, twm.Middle);
            Assert.AreEqual(new Vector2(100, 101), r.FPoint);
            Assert.AreEqual(new Vector2(100, 101), twm.FPoint);
            twm.Effect(new[] {-100.0f, 100.0f}, r, map);
            Assert.AreEqual(m, r.Middle);
            Assert.AreEqual(m, twm.Middle);
            Assert.AreEqual(new Vector2(101, 100), r.FPoint);
            Assert.AreEqual(new Vector2(101, 100), twm.FPoint);
        }

        [TestMethod]
        public void MovementAndRotationTest()
        {
            TwoWheelMotor.MaxVelocityChange = 300;
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1);
            TwoWheelMotor twm = new TwoWheelMotor(r, 100.0f);
            Vector2 m = r.Middle;

            twm.Effect(new[] {(float) -Math.PI / 2.0f, (float) Math.PI / 2.0f}, r, map);
            Assert.AreEqual(m, r.Middle);
            Assert.AreEqual(m, twm.Middle);
            Assert.AreEqual(new Vector2(101, 100), r.FPoint);
            Assert.AreEqual(new Vector2(101, 100), twm.FPoint);
            twm.Effect(new[] {1.0f, 1.0f}, r, map);
            m.X++;
            Assert.AreEqual(m, r.Middle);
            Assert.AreEqual(m, twm.Middle);
            Assert.AreEqual(new Vector2(102, 100), r.FPoint);
            Assert.AreEqual(new Vector2(102, 100), twm.FPoint);
            twm.Effect(new[] {1.0f, 1.0f}, r, map);
            m.X++;
            Assert.AreEqual(m, r.Middle);
            Assert.AreEqual(m, twm.Middle);
            Assert.AreEqual(new Vector2(103, 100), r.FPoint);
            Assert.AreEqual(new Vector2(103, 100), twm.FPoint);
            twm.Effect(new[] {(float) -Math.PI, (float) Math.PI}, r, map);
            Assert.AreEqual(m, r.Middle);
            Assert.AreEqual(m, twm.Middle);
            Assert.AreEqual(new Vector2(101, 100), r.FPoint);
            Assert.AreEqual(new Vector2(101, 100), twm.FPoint);
            twm.Effect(new[] {1.0f, 1.0f}, r, map);
            m.X--;
            Assert.AreEqual(m, r.Middle);
            Assert.AreEqual(m, twm.Middle);
            Assert.AreEqual(new Vector2(100, 100), r.FPoint);
            Assert.AreEqual(new Vector2(100, 100), twm.FPoint);
        }
    }

    [TestClass]
    public class RadioTransmittingTest
    {
        [TestMethod]
        public void InitTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1);
            RadioTransmitter rt = new RadioTransmitter(r, 10);
            rt.Effect(new[] {0,0,1.0f}, r, map);
            Assert.AreEqual(new Vector2(100, 100), map.RadioEntities[0].Middle);
            Assert.AreEqual(10, map.RadioEntities[0].Radius);
            Assert.AreEqual(2, map.RadioEntities[0].ValueOfSignal);
            map.MakeStep();
            Assert.AreEqual(0, map.RadioEntities.Count);
        }

        [TestMethod]
        public void MoveTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1);
            RadioTransmitter rt = new RadioTransmitter(r, 10);
            rt.Effect(new[] {100.0f,0,0}, r, map);
            Assert.AreEqual(new Vector2(100, 100), map.RadioEntities[0].Middle);
            Assert.AreEqual(10, map.RadioEntities[0].Radius);
            Assert.AreEqual(0, map.RadioEntities[0].ValueOfSignal);
            map.MakeStep();
            r.MoveTo(new Vector2(10, 10));
            rt.Effect(new[] {0,1.0f,0}, r, map);
            Assert.AreEqual(new Vector2(10, 10), map.RadioEntities[0].Middle);
            Assert.AreEqual(10, map.RadioEntities[0].Radius);
            Assert.AreEqual(1, map.RadioEntities[0].ValueOfSignal);

        }
    }

    [TestClass]
    public class PickerTests
    {
        [TestMethod]
        public void InitTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1, 1);
            Picker p = new Picker(r, 10, 0);
            Assert.AreEqual(p.RotationMiddle, r.Middle);
            Assert.AreEqual(p.A, r.FPoint);
            Assert.AreEqual(p.B, new Vector2(100, 111));
            r.MoveTo(new Vector2(100, 101));
            p.Effect(new[] {0.0f,0.0f,100.0f}, r, map);
            Assert.AreEqual(0, r.InvalidContainerOperation);
            Assert.AreEqual(p.RotationMiddle, r.Middle);
            Assert.AreEqual(p.A, r.FPoint);
            Assert.AreEqual(p.B, new Vector2(100, 112));
            r.MoveTo(new Vector2(100, 100));
            p.Effect(new[] {0.0f,0.0f,100f}, r, map);
            Assert.AreEqual(p.RotationMiddle, r.Middle);
            Assert.AreEqual(p.A, r.FPoint);
            Assert.AreEqual(p.B, new Vector2(100, 111));
            r.RotateDegrees(-90);
            p.Effect(new[] {0.0f,0.0f,100.0f}, r, map);
            Assert.AreEqual(p.RotationMiddle, r.Middle);
            Assert.AreEqual(p.A, r.FPoint);
            Assert.AreEqual(p.B, new Vector2(111, 100));
        }

        [TestMethod]
        public void PickUpTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1, 1);
            Picker p = new Picker(r, 10, 0);
            Circle c = new Circle(new Vector2(100, 105), 3);
            map.PasiveEntities.Add(c);
            Assert.AreEqual(1, map.PasiveEntities.Count);
            Assert.AreEqual(0, r.ActualContainerCount);
            p.Effect(new[] {0,100.0f,0}, r, map);
            Assert.AreEqual(0, map.PasiveEntities.Count);
            Assert.AreEqual(1, r.ActualContainerCount);
            Assert.AreEqual(c, r.PeekContainer());
        }

        [TestMethod]
        public void PutTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1, 1);
            Picker p = new Picker(r, 10, 0);
            Circle c = new Circle(new Vector2(100, 105), 3);
            r.PushContainer(c);
            Assert.AreEqual(0, map.PasiveEntities.Count);
            Assert.AreEqual(1, r.ActualContainerCount);
            Assert.AreEqual(c, r.PeekContainer());
            p.Effect(new[] {100.0f,0,0}, r, map);
            Assert.AreEqual(1, map.PasiveEntities.Count);
            Assert.AreEqual(0, r.ActualContainerCount);
            Assert.AreEqual(p.B, c.Middle);
            var fc = p.B;
            fc.Y += c.Radius;
            Assert.AreEqual(c.FPoint, fc);

        }

        [TestMethod]
        public void PickUpNothingTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1, 1);
            Picker p = new Picker(r, 10, 0);
            Circle c = new Circle(new Vector2(150, 150), 3);
            map.PasiveEntities.Add(c);
            Assert.AreEqual(1, map.PasiveEntities.Count);
            Assert.AreEqual(0, r.ActualContainerCount);
            p.Effect(new[] {0,100.0f,0}, r, map);
            Assert.AreEqual(1, map.PasiveEntities.Count);
            Assert.AreEqual(0, r.ActualContainerCount);
            Assert.IsTrue(null == r.PopContainer());
            Assert.AreEqual(1, r.InvalidContainerOperation);
        }

        [TestMethod]
        public void PutNothingTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1, 1);
            Picker p = new Picker(r, 10, 0);
            Circle c = new Circle(new Vector2(150, 150), 3);
            map.PasiveEntities.Add(c);
            Assert.AreEqual(1, map.PasiveEntities.Count);
            Assert.AreEqual(0, r.ActualContainerCount);
            Assert.AreEqual(0, r.InvalidContainerOperation);
            p.Effect(new[] {100.0f,0,0}, r, map);
            Assert.AreEqual(1, map.PasiveEntities.Count);
            Assert.AreEqual(0, r.ActualContainerCount);
            Assert.AreEqual(1, r.InvalidContainerOperation);
        }

        [TestMethod]
        public void PickUpBiggerEntity()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1, 1);
            Picker p = new Picker(r, 10, 0);
            Circle c = new Circle(new Vector2(100, 120), 11);
            map.PasiveEntities.Add(c);
            Assert.AreEqual(1, map.PasiveEntities.Count);
            Assert.AreEqual(0, r.ActualContainerCount);
            p.Effect(new[] {0,100.0f,0}, r, map);
            Assert.AreEqual(1, map.PasiveEntities.Count);
            Assert.AreEqual(0, r.ActualContainerCount);
            Assert.AreEqual(null, r.PeekContainer());

        }

        [TestMethod]
        public void PutDownCollidingEntity()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1, 1);
            Picker p = new Picker(r, 10, 0);
            Circle c = new Circle(new Vector2(100, 120), 15);
            map.PasiveEntities.Add(c);
            Circle c1 = new Circle(new Vector2(0, 0), 1);
            r.PushContainer(c1);
            p.Effect(new[] {100.0f,0,0}, r, map);
            Assert.AreEqual(1, map.PasiveEntities.Count);
            Assert.AreEqual(1, r.ActualContainerCount);
            Assert.AreEqual(c1, r.PeekContainer());

        }
    }

    [TestClass]
    public class RefactorTests
    {
        [TestMethod]
        public void InitTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1, 1);
            MineralRefactor mr = new MineralRefactor(r);
            Assert.AreEqual(r.Middle, mr.Middle);
            Assert.AreEqual(r.Middle, mr.FPoint);
            Assert.AreEqual(0, r.InvalidRefactorOperation);
            //Idle 
            mr.Effect(new[] {100.0f}, r, map);
            Assert.AreEqual(r.Middle, mr.Middle);
            Assert.AreEqual(r.Middle, mr.FPoint);
            Assert.AreEqual(0, r.InvalidRefactorOperation);
            r.MoveTo(new Vector2(150, 150));
            mr.Effect(new[] {100.0f}, r, map);
            Assert.AreEqual(r.Middle, mr.Middle);
            Assert.AreEqual(r.Middle, mr.FPoint);
            Assert.AreEqual(0, r.InvalidRefactorOperation);

        }

        [TestMethod]
        public void EmptyContainer()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1, 1);
            MineralRefactor mr = new MineralRefactor(r);
            Assert.AreEqual(0, r.InvalidRefactorOperation);
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(1, r.InvalidRefactorOperation);
            Assert.IsFalse(mr.Refactoring);
            Assert.AreEqual(0, mr.CyclesToEnd);
            Assert.AreEqual(0, mr.FuelToRefactor);
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(2, r.InvalidRefactorOperation);
            Assert.IsFalse(mr.Refactoring);
            Assert.AreEqual(0, mr.CyclesToEnd);
            Assert.AreEqual(0, mr.FuelToRefactor);
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(3, r.InvalidRefactorOperation);
            Assert.IsFalse(mr.Refactoring);
            Assert.AreEqual(0, mr.CyclesToEnd);
            Assert.AreEqual(0, mr.FuelToRefactor);
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(4, r.InvalidRefactorOperation);
            Assert.IsFalse(mr.Refactoring);
            Assert.AreEqual(0, mr.CyclesToEnd);
            Assert.AreEqual(0, mr.FuelToRefactor);
        }

        [TestMethod]
        public void RefactoringMineralTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1, 1);
            MineralRefactor mr = new MineralRefactor(r);
            r.PushContainer(new RawMaterialEntity(Vector2.Zero, 1, 100, 2));
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(0, r.InvalidRefactorOperation);
            Assert.IsTrue(mr.Refactoring);
            Assert.AreEqual(2, mr.CyclesToEnd);
            Assert.AreEqual(100, mr.FuelToRefactor);
            Assert.AreEqual(0, r.ActualContainerCount);
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(0, r.ActualContainerCount);
            Assert.AreEqual(0, r.InvalidRefactorOperation);
            Assert.IsTrue(mr.Refactoring);
            Assert.AreEqual(1, mr.CyclesToEnd);
            Assert.AreEqual(100, mr.FuelToRefactor);
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(0, r.ActualContainerCount);
            Assert.AreEqual(0, r.InvalidRefactorOperation);
            Assert.IsTrue(mr.Refactoring);
            Assert.AreEqual(0, mr.CyclesToEnd);
            Assert.AreEqual(100, mr.FuelToRefactor);
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(1, r.ActualContainerCount);
            Assert.AreEqual(0, r.InvalidRefactorOperation);
            Assert.IsFalse(mr.Refactoring);
            Assert.AreEqual(0, mr.CyclesToEnd);
            Assert.AreEqual(0, mr.FuelToRefactor);
            Assert.IsTrue(r.PeekContainer() is FuelEntity);
        }

        [TestMethod]
        public void RefactoringMineralFullStackTest()
        {
            Map map = new Map(200, 200, new List<RobotEntity>(), new List<CircleEntity>(), new List<FuelEntity>());
            EmptyRobot r = new EmptyRobot(new Vector2(100, 100), 1, 1);
            MineralRefactor mr = new MineralRefactor(r);
            r.PushContainer(new RawMaterialEntity(Vector2.Zero, 1, 100, 2));
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(0, r.InvalidRefactorOperation);
            Assert.IsTrue(mr.Refactoring);
            Assert.AreEqual(2, mr.CyclesToEnd);
            Assert.AreEqual(100, mr.FuelToRefactor);
            Assert.AreEqual(0, r.ActualContainerCount);
            r.PushContainer(new Circle(Vector2.Zero, 0));
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(0, r.InvalidRefactorOperation);
            Assert.IsTrue(mr.Refactoring);
            Assert.AreEqual(1, mr.CyclesToEnd);
            Assert.AreEqual(100, mr.FuelToRefactor);
            Assert.AreEqual(1, r.ActualContainerCount);
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(0, r.InvalidRefactorOperation);
            Assert.IsTrue(mr.Refactoring);
            Assert.AreEqual(0, mr.CyclesToEnd);
            Assert.AreEqual(100, mr.FuelToRefactor);
            Assert.AreEqual(1, r.ActualContainerCount);
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(1, r.InvalidRefactorOperation);
            Assert.IsTrue(mr.Refactoring);
            Assert.AreEqual(0, mr.CyclesToEnd);
            Assert.AreEqual(100, mr.FuelToRefactor);
            Assert.AreEqual(1, r.ActualContainerCount);
            mr.Effect(new[] {-100.0f}, r, map);
            Assert.AreEqual(2, r.InvalidRefactorOperation);
            Assert.IsTrue(mr.Refactoring);
            Assert.AreEqual(0, mr.CyclesToEnd);
            Assert.AreEqual(100, mr.FuelToRefactor);
            Assert.AreEqual(1, r.ActualContainerCount);
        }
    }

    [TestClass]
    public class WeaponTests
    {
        [TestMethod]
        public void InitTest()
        {
            Map map = new Map(100, 100);
            EmptyRobot er = new EmptyRobot(new Vector2(50, 50), 1);
            Weapon w = new Weapon(er, 10, 20, 0);
            Assert.AreEqual(er.FPoint, w.A);
            var v = er.FPoint;
            v.Y += 10;
            Assert.AreEqual(v, w.B);
            w.Effect(new[] {0.0f,1.0f,0.0f}, er, map);
            Assert.AreEqual(0, er.InvalidWeaponOperation);
            er.MoveTo(new Vector2(30, 30));
            w.Effect(new[] {0.0f,1,0}, er, map);
            Assert.AreEqual(er.FPoint, w.A);
            v = er.FPoint;
            v.Y += 10;
            Assert.AreEqual(v, w.B);
        }

        [TestMethod]
        public void NoRobotAttack()
        {
            Map map = new Map(100, 100);
            EmptyRobot er = new EmptyRobot(new Vector2(50, 50), 1);
            Weapon w = new Weapon(er, 10, 20, 0);
            w.Effect(new[] {1.0f,0,0}, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            w.Effect(new[] {0,0,1.0f}, er, map);
            Assert.AreEqual(2, er.InvalidWeaponOperation);
        }

        [TestMethod]
        public void AttackFriend()
        {
            Map map = new Map(100, 100);
            EmptyRobot er = new EmptyRobot(new Vector2(50, 50), 1);
            Weapon w = new Weapon(er, 10, 20, 0);
            EmptyRobot fr = new EmptyRobot(new Vector2(50, 55), 1);
            map.Robots.Add(fr);
            w.Effect(new[] {0,0,100.0f}, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            Assert.AreEqual(100, fr.Health);
            w.Effect(new[] {100.0f,0,0}, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            Assert.AreEqual(80, fr.Health);
            w.Effect(new[] { 100.0f, 0, 0 }, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            Assert.AreEqual(60, fr.Health);
            w.Effect(new[] { 100.0f, 0, 0 }, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            Assert.AreEqual(40, fr.Health);
            w.Effect(new[] { 100.0f, 0, 0 }, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            Assert.AreEqual(20, fr.Health);
            w.Effect(new[] { 100.0f, 0, 0 }, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            Assert.AreEqual(0, fr.Health);
            Assert.IsFalse(fr.Alive);
            Assert.AreEqual(1, map.Robots.Count);
            Assert.IsFalse(map.Robots[0].Alive);
            Assert.AreEqual(1, map.FuelEntities.Count);
            Assert.AreEqual(100, map.FuelEntities[0].Capacity);
            int j = 1;
            for (int i = 0; i < 100; i++)
            {
                j++;
                w.Effect(new[] { 100.0f, 0, 0 }, er, map);
                Assert.AreEqual(j, er.InvalidWeaponOperation);
                j++;
                w.Effect(new[] { 0, 0, 100.0f }, er, map);
                Assert.AreEqual(j, er.InvalidWeaponOperation);
            }
        }

        [TestMethod]
        public void AttackEnemy()
        {
            Map map = new Map(100, 100);
            EmptyRobot er = new EmptyRobot(new Vector2(50, 50), 1);
            Weapon w = new Weapon(er, 10, 20, 0);
            EnemyRobot e = new EnemyRobot(new Vector2(50, 55), 1);
            map.Robots.Add(e);
            w.Effect(new[] { 100.0f, 0, 0 }, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            Assert.AreEqual(100, e.Health);
            w.Effect(new[] { 0, 0, 100.0f }, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            Assert.AreEqual(80, e.Health);
            w.Effect(new[] { 0, 0, 100.0f }, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            Assert.AreEqual(60, e.Health);
            w.Effect(new[] { 0, 0, 100.0f }, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            Assert.AreEqual(40, e.Health);
            w.Effect(new[] { 0, 0, 100.0f }, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            Assert.AreEqual(20, e.Health);
            w.Effect(new[] { 0, 0, 100.0f }, er, map);
            Assert.AreEqual(1, er.InvalidWeaponOperation);
            Assert.AreEqual(0, e.Health);
            Assert.IsFalse(e.Alive);
            Assert.AreEqual(1, map.Robots.Count);
            Assert.IsFalse(map.Robots[0].Alive);
            Assert.AreEqual(1, map.FuelEntities.Count);
            Assert.AreEqual(100, map.FuelEntities[0].Capacity);
            int j = 1;
            for (int i = 0; i < 100; i++)
            {
                j++;
                w.Effect(new[] { 100.0f, 0, 0 }, er, map);
                Assert.AreEqual(j, er.InvalidWeaponOperation);
                j++;
                w.Effect(new[] { 0, 0, 100.0f }, er, map);
                Assert.AreEqual(j, er.InvalidWeaponOperation);
            }
        }

        class EnemyRobot : RobotEntity
        {
            public EnemyRobot(Vector2 middle, float radius, float amountOfFuel = 100, int sizeOfContainer = 0,
                int teamNumber = 2, float health = 100, float normalizeMax = 100, float normalizeMin = -100,
                float orientation = 0) : base(middle, radius, "Enemy Robot", new IEffector[0], new ISensor[0],
                amountOfFuel, sizeOfContainer, teamNumber, health, normalizeMax, normalizeMin, orientation)
            {
            }
        }
    }

    [TestClass]
    public class WoodRefactorTests
    {
        [TestMethod]
        public void InitTest()
        {
            Map map = new Map(150,150);
            EmptyRobot er = new EmptyRobot(new Vector2(30,30),5,0);
            WoodRefactor wr = new WoodRefactor(er,10,0);
            Assert.AreEqual(new Vector2(30,35),wr.A);
            Assert.AreEqual(new Vector2(30,45),wr.B);
            Assert.AreEqual(0, er.Orientation);
            wr.Effect(new []{-100.0f},er,map);
            Assert.AreEqual(new Vector2(30, 35), wr.A);
            Assert.AreEqual(new Vector2(30, 45), wr.B);
            Assert.AreEqual(0,er.InvalidRefactorOperation);
            wr.Effect(new[] { 100.0f }, er, map);
            Assert.AreEqual(new Vector2(30, 35), wr.A);
            Assert.AreEqual(new Vector2(30, 45), wr.B);
            Assert.AreEqual(1, er.InvalidRefactorOperation);
        }

        [TestMethod]
        public void CutTree()
        {
            Map map = new Map(150, 150);
            EmptyRobot er = new EmptyRobot(new Vector2(30, 30), 5, 0);
            WoodRefactor wr = new WoodRefactor(er, 10, 0);
            Assert.AreEqual(new Vector2(30, 35), wr.A);
            Assert.AreEqual(new Vector2(30, 45), wr.B);
            Assert.AreEqual(0, er.Orientation);
            wr.Effect(new[] { 100.0f }, er, map);
            Assert.AreEqual(1, er.InvalidRefactorOperation);
            RawMaterialEntity rme = new RawMaterialEntity(new Vector2(30,40),2,10,0);
            map.PasiveEntities.Add(rme);
            Assert.AreEqual(1,map.PasiveEntities.Count);
            Assert.AreEqual(Entity.EntityColor.RawMaterialColor,map.PasiveEntities[0].Color);
            wr.Effect(new []{100.0f},er,map);
            Assert.AreEqual(Entity.EntityColor.WoodColor,map.PasiveEntities[0].Color);
        }

        [TestMethod]
        public void MovingCutTest()
        {
            Map map = new Map(150, 150);
            EmptyRobot er = new EmptyRobot(new Vector2(30, 30), 5, 0);
            WoodRefactor wr = new WoodRefactor(er, 10, 0);
            map.PasiveEntities.Add(new RawMaterialEntity(new Vector2(30, 146), 1, 20, 0));
            for (int i = 0; i < 100; i++)
            {
                wr.Effect(new [] {100.0f},er,map);
                Assert.AreEqual(i+1,er.InvalidRefactorOperation);
                Assert.AreEqual(new Vector2(30, 35 +i), wr.A);
                Assert.AreEqual(new Vector2(30, 45 +i), wr.B);
                Assert.AreEqual(i+1, er.InvalidRefactorOperation);
                Assert.AreEqual(Entity.EntityColor.RawMaterialColor, map.PasiveEntities[0].Color);
                er.MoveTo(er.Middle + new Vector2(0,1));
            }
            wr.Effect(new[] { 100.0f }, er, map);
            Assert.AreEqual(Entity.EntityColor.WoodColor, map.PasiveEntities[0].Color);
        }
    }

    [TestClass]
    public class MemeTest
    {
        [TestMethod]
        public void InitTest()
        {
            Map map = new Map(150, 150);
            EmptyRobot er = new EmptyRobot(new Vector2(30, 30), 5, 0);
            MemoryStick ms = new MemoryStick(4,er);
            var f = new[] {0.0f, -20.0f, 40, -99};
            ms.Effect(f,er,map);

            TestExtensions.AssertArrayEquality(f,ms.Count(er,map));

        }
    }

    [TestClass]
    public class DiscoveryTesting
    {
        [TestMethod]
        public void NoDiscovery()
        {
            List<CircleEntity> l = new List<CircleEntity>();
            var rm = new RawMaterialEntity(new Vector2(30, 44), 1, 90, 1);
            l.Add(rm);
            Map map = new Map(150,150);
            EmptyRobot er = new EmptyRobot(new Vector2(30,30),5);
            LineTypeSensor  ls = new LineTypeSensor(er,10,0);
            ls.Count(er, map);
            Assert.IsFalse(rm.Discovered);
        }

        [TestMethod]
        public void Discovery()
        {
            List<CircleEntity> l = new List<CircleEntity>();
            var rm = new RawMaterialEntity(new Vector2(30, 44), 1, 90, 1);
            l.Add(rm);
            Map map = new Map(150, 150,null,l);
            EmptyRobot er = new EmptyRobot(new Vector2(30, 30), 5);
            LineTypeSensor ls = new LineTypeSensor(er, 10, 0);
           
            ls.Count(er, map);
            Assert.IsTrue(rm.Discovered);
        }
    }

    public static class TestExtensions
    {
        public static void AssertArrayEquality(float[] a, float[] b)
        {
            Assert.AreEqual(a.Length, b.Length);
            for (int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(a[i], b[i]);
            }
        }
    }
}
