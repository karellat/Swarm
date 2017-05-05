﻿using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Map;
namespace SwarmSimFramework.Classes
{
    public interface ISensor
    {
        /// <summary>
        /// Count values of the sensor
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        float[] Count(RobotEntity robot, Map.Map map); 
        /// <summary>
        /// Get dimension of sensors 
        /// </summary>
        int Dimension { get; }
        /// <summary>
        /// maximum possible transmitted value
        /// </summary>
        float MaxOuputValue { get; }
        /// <summary>
        /// minimum possible transmitted value
        /// </summary>
        float MinOutputValue { get;  }
        /// <summary>
        /// Return shape of the sensor
        /// </summary>
        Entity.Shape GetShape { get; } 


    }
}