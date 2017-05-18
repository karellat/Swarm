﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Entities
{
    public class RadioEntity : CircleEntity
    {
        /// <summary>
        /// Value of transmitting signal 
        /// </summary>
        public int ValueOfSignal;
        /// <summary>
        /// Create clone of radioEnity
        /// </summary>
        /// <returns></returns>
        public override Entity DeepClone()
        {
            return (Entity) this.MemberwiseClone();
        }
        /// <summary>
        /// RadioSignal Entity
        /// </summary>
        /// <param name="middle"></param>
        /// <param name="radius"></param>
        /// <param name="valueOfSignal"></param>
        public RadioEntity(Vector2 middle, float radius, int valueOfSignal) : base(middle, radius, "RadioSignalEntity")
        {
            if(valueOfSignal > SignalValueBounds.Min && valueOfSignal < SignalValueBounds.Max)
                ValueOfSignal = valueOfSignal;
            else
                throw new ArgumentException("Unsupported value of signal.");        
        }
        /// <summary>
        /// Interval of used values 
        /// </summary>
        public static Bounds SignalValueBounds = new Bounds() {Max = 3, Min = 0};
    }
}
