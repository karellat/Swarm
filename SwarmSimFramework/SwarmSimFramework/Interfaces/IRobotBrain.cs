using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SwarmSimFramework.Classes.Entities;
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
        /// <summary>
        /// Log current brain 
        /// </summary>
        /// <returns></returns>
        StringBuilder Log();
        /// <summary>
        /// Deserialize brain 
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        IRobotBrain DeserializeBrain(string jsonString);
        /// <summary>
        /// Serialize this brain
        /// </summary>
        /// <returns></returns>
        string SerializeBrain();
    }

    public static class BrainSerializer
    {
        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All
        };
        public static IRobotBrain[] DeserializeArray(string jsonString)
        {
            return JsonConvert.DeserializeObject<IRobotBrain[]>(jsonString, JsonSettings);
        }

        public static string SerializeArray(IRobotBrain[] brains)
        {
            return JsonConvert.SerializeObject(brains, JsonSettings);

        }

        public static T[] DeserializeArray<T>(string jsonString) where T : IRobotBrain
        {
            return JsonConvert.DeserializeObject<T[]>(jsonString, JsonSettings);
        }

        public static string SerializeArray<T>(T[] brains) where T : IRobotBrain
        {
            return JsonConvert.SerializeObject(brains, JsonSettings);
        }

        public static IRobotBrain DeserializeBrain(string jsonString)
        {
            return JsonConvert.DeserializeObject<IRobotBrain>(jsonString, JsonSettings);
        }
    }
    /// <summary>
    /// Model of brain with model  of the robot
    /// </summary>
    public struct BrainModel<T> where T : IRobotBrain
    {
        public RobotEntity Robot;
        public T Brain;

        public bool SuitableBrain(IRobotBrain brain)
        {
            return Brain.GetType() == brain.GetType() && brain.IoDimension.Input == Brain.IoDimension.Input
                   && Brain.IoDimension.Output == brain.IoDimension.Output;
        }

        public bool SuitableRobot(RobotEntity entity)
        {
            return Robot.GetType() == entity.GetType();
        }
    }

    public static class BrainModelsSerializer
    {
        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All
        };
        public static BrainModel<T>[] DeserializeArray<T>(string jsonString) where T : IRobotBrain
        {
            return JsonConvert.DeserializeObject<BrainModel<T>[]>(jsonString, JsonSettings);
        }

        public static string SerializeArray<T>(BrainModel<T>[] brains) where T : IRobotBrain
        {
            return JsonConvert.SerializeObject(brains, JsonSettings);
        }
    }

}
