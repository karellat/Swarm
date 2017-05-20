using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Interfaces
{
    /// <summary>
    /// Decition maker of robot
    /// </summary>
    public interface IRobotBrain
    {
        /// <summary>
        /// get or set fitness of brain 
        /// </summary>
        double Fitness { get; set; }
        /// <summary>
        /// Activation fncs transforming brain output
        /// </summary>
        Func<float, float> ActivationFnc { get; }
        /// <summary>
        /// Dimension of input & ouput
        /// </summary>
        IODimension IoDimension { get; }
        /// <summary>
        /// Bounds of input and Output values
        /// </summary>
        Bounds InOutBounds { get; }
        /// <summary>
        /// Decide based on sensor read values
        /// </summary>
        /// <param name="readValues"></param>
        /// <returns></returns>
        float[] Decide(float[] readValues);
        /// <summary>
        /// Clean copy of robot brain 
        /// </summary>
        /// <returns></returns>
        IRobotBrain GetCleanCopy();
 
    }
}
