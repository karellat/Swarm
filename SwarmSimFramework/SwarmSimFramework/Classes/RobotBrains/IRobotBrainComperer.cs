using System;
using System.Collections.Generic;
using SwarmSimFramework.Interfaces;

namespace SwarmSimFramework.Classes.RobotBrains
{
    public class IRobotBrainComperer:IComparer<IRobotBrain>
    {
        public int Compare(IRobotBrain x, IRobotBrain y)
        {
            if(x == null || y == null)
                throw  new ArgumentException("Some of the robot brain is null");
            if (x.Fitness >= y.Fitness)
                return 1;
            else
                return -1;
        }
    }

    public class IRobotDoubleComperer : IComparer<double>
    {
        public int Compare(double x, double y)
        {
            if (x >= y)
                return 1;
            else
                return -1;  
        }
    }
}