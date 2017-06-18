using SwarmSimFramework.Classes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SwarmSimFramework.Classes.Experiments;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.MultiThreadExperiment;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Robots;

namespace SwarmSimFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            MultiThreadExperiment<SingleLayerNeuronNetwork> exp;
           if(args[0] == "0")
             exp = new WoodWorkerPickUpMem();
           else
             exp = new WoodWorkerPickUp();
           exp.Run();
           Console.WriteLine("Simulation finnished");
           Console.ReadLine();
        }
    }
}
