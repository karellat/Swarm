using SwarmSimFramework.Classes.Entities;

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
        /// Effect the entity on the map with given settings
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="robot"></param>
        /// <param name="map"></param>
        void Effect(float[] settings, RobotEntity robot, Map.Map map);
        /// <summary>
        /// Get maximum possible value of receiving settings
        /// </summary>
        float MaxInputValue { get; }
        /// <summary>
        /// Get minimum possible value of receiving settings
        /// </summary>
        float MinInputValue { get; }
        /// <summary>
        /// Get shape of representing entity
        /// </summary>
        Entity.Shape GetShape { get; }
        /// <summary>
        /// Create clone of the effector
        /// </summary>
        /// <returns></returns>
        IEffector Clone();
    }
}