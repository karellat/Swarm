using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Map;
namespace SwarmSimFramework.Classes
{
    /// Represent bounds of interval 
    public struct Bounds
    {
        /// <summary>
        /// Maximum bound 
        /// </summary>
        public float Max;
        /// <summary>
        /// Minimum bound
        /// </summary>
        public float Min; 
    }
    /// Normalize functions   
    public struct NormalizeFunc
    {
        public float Rescale;
        public float Shift;

        public float Normalize(float x)
        {
            return (x * Rescale) + Shift;
        }

    }
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
        /// Bounds for every dimension of sensor output  
        /// </summary>
        Bounds[] LocalBounds { get; }
        /// <summary>
        /// Normalization func for dimension
        /// </summary>
        NormalizeFunc[] NormalizeFuncs { get; }
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