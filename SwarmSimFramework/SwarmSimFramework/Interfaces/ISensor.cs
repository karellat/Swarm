using SwarmSimFramework.Classes.Entities;
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
        /// Make initial connection methods, normalize inputs etc.
        /// </summary>
        /// <param name="robot"></param>
        void ConnectToRobot(RobotEntity robot);
        /// <summary>
        /// Get dimension of sensor
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
        /// <summary>
        /// Create clone of the sehsor
        /// </summary>
        /// <returns></returns>
        ISensor Clone();

    }
}