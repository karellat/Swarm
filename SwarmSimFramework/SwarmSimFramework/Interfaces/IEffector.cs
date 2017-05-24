using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes
{
    public interface IEffector
    {

        /// <summary>
        /// Dimension of effector
        /// </summary>
        /// <returns></returns>
        int Dimension { get; }
        /// <summary>
        /// Make initial connection methods, normalize inputs etc.
        /// </summary>
        /// <param name="robot"></param>
        void ConnectToRobot(RobotEntity robot);
        /// <summary>
        /// Effect the entity on the map with given settings
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        void Effect(float[] settings, RobotEntity robot, Map.Map map);
        /// <summary>
        /// Bounds for every dimension of effector intern settings 
        /// </summary>
        Bounds[] LocalBounds { get; }
        /// <summary>
        /// Normalization func from robot bounds to intern settings
        /// </summary>
        NormalizeFunc[] NormalizeFuncs { get; }
        /// <summary>
        /// Get shape of representing entity
        /// </summary>
        Entity.Shape GetShape { get; }
        /// <summary>
        /// Create clone of the effector
        /// </summary>
        /// <returns></returns>
        IEffector Clone();
        /// <summary>
        /// Make log of current effector
        /// </summary>
        /// <returns></returns>
        StringBuilder Log();
    }
}