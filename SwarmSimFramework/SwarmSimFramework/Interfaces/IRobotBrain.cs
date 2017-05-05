using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmSimFramework.Interfaces
{
    public interface IRobotBrain
    {
        /// <summary>
        /// Decide based on sensor read values
        /// </summary>
        /// <param name="readValues"></param>
        /// <returns></returns>
       float[] Decide(float[] readValues);
        /// <summary>
        /// Minimum possible value of forwarding values
        /// </summary>
        float MinOutputValue { get; }
        /// <summary>
        /// Maximum possible value of forwarding values
        /// </summary>
        float MaxOutputValue { get;  }
        /// <summary>
        /// Minimum possible value of receiving values 
        /// </summary>
        float MinInputValue { get;  }
        /// <summary>
        /// Maximum possible value of receiving values 
        /// </summary>
        float MaxInputValue { get;  }
        /// <summary>
        /// Dimension of recieving values
        /// </summary>
        int InputDimension { get;  }
        /// <summary>
        /// Dimension of forwarding values
        /// </summary>
        int OutputDimension { get;  }
        /// <summary>
        /// Clean copy of robot brain 
        /// </summary>
        /// <returns></returns>
        IRobotBrain GetCleanCopy();
    }
}
