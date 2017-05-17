using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// Bounds for communication with brain, input and output bounds
        /// </summary>
        Bounds InOutBounds { get; }
        /// <summary>
        /// Local bounds of intern values
        /// </summary>
        Bounds[] LocalBounds { get;  }
        /// <summary>
        /// Dimension of input values 
        /// </summary>
        int InputDimension { get;  }
        /// <summary>
        /// Dimension of forwarding values
        /// </summary>
        int OutputDimension { get;  }
        /// <summary>
        /// Normalization fncs 
        /// </summary>
        NormalizeFunc[] NormalizeFuncs { get; }
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
