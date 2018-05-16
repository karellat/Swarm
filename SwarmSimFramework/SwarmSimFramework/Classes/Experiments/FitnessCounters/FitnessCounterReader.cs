using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Experiments.FitnessCounters
{
    public static class FitnessCounterReader
    {
        public enum Scene
        {
            Wood, 
            Mineral, 
            Competive
        }

        public static Object ParseExperimentValue(string value, Type type)
        {
            if (type == typeof(int))
            {
                return int.Parse(value);
            }
            else if (type == typeof(double))
            {
                return double.Parse(value);
            }
            else if (type == typeof(bool))
            {
                return bool.Parse(value);
            }
            else if (type == typeof(float))
            {
                return float.Parse(value);
            }
            else if (type == typeof(string))
            {
                return value;
            }
            else if (type == typeof(long))
            {
                return long.Parse(value);
            }
            else if (type == typeof(RobotModel[]))
            {
                var robots = value.Split(new[] { ';' });
                var models = new List<RobotModel>();
                foreach (var r in robots)
                {
                    var args = r.Split(new[] { '-' });
                    var constructor = args[0].Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                    if (constructor.Length == 2)
                    {
                        var modelType = Type.GetType("SwarmSimFramework.Classes.Robots." + constructor[0]);
                        Int32 amountOfFuel = int.Parse(constructor[1]);
                        RobotEntity robot = (RobotEntity)Activator.CreateInstance(modelType, new object[] { amountOfFuel });
                        int count = int.Parse(args[1]);
                        models.Add(new RobotModel() { model = robot, amount = count });
                    }
                    else if (constructor.Length != 1)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        var modelType = Type.GetType("SwarmSimFramework.Classes.Robots." + constructor[0]);
                        RobotEntity robot = (RobotEntity)Activator.CreateInstance(modelType);
                        int count = int.Parse(args[1]);
                        models.Add(new RobotModel() { model = robot, amount = count });

                    }
                }
                return models.ToArray();
            }
            else if (type == typeof(MapModel))
            {
                var mapType = Type.GetType("SwarmSimFramework.Classes.Map." + value);
                MapModel m = (MapModel)Activator.CreateInstance(mapType);
                return m;
            }
            else if (type == typeof(IFitnessCounter))
            {
                var counterType = Type.GetType("SwarmSimFramework.Classes.Experiments.FitnessCounters." + value);
                IFitnessCounter f = (IFitnessCounter)Activator.CreateInstance(counterType);
                return f;
            }
            else if (type == typeof(BrainModel<SingleLayerNeuronNetwork>[][]))
            {
                var robots = value.Split(new[] { ';', '=' });
                int threadsCount = int.Parse(robots[0]);
                BrainModel<SingleLayerNeuronNetwork>[][] models = new BrainModel<SingleLayerNeuronNetwork>[threadsCount][];
                for (int i = 0; i < threadsCount; i++)
                {
                    models[i] = new BrainModel<SingleLayerNeuronNetwork>[robots.Length - 1];
                    for (var index = 1; index < robots.Length; index++)
                    {
                        var r = robots[index];
                        var parameters = r.Split(new[] { '-' });
                        var robotConstructor = parameters[0].Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                        var robotType = Type.GetType("SwarmSimFramework.Classes.Robots." + robotConstructor[0]);
                        models[i][index - 1].Robot = (RobotEntity)Activator.CreateInstance(robotType);




                        if (parameters[1] == "G")
                        {
#if DEBUG
                            Console.WriteLine("ESReader: Generating new brain - {0}", parameters[1]);
#endif
                            models[i][index - 1].Brain = SingleLayerNeuronNetwork.GenerateNewRandomNetwork(new IODimension()
                            {
                                Input = models[i][index - 1].Robot.SensorsDimension,
                                Output = models[i][index - 1].Robot.EffectorsDimension
                            });
                        }
                        else if (parameters[1].Contains("[i]"))
                        {

                            string path = parameters[1].Replace("[i]", i.ToString());
#if DEBUG
                            Console.WriteLine("ESReader: Reading brain from file - {0}", path);
#endif
                            StreamReader reader = new StreamReader(path);
                            string brainJson = reader.ReadToEnd();
                            reader.Close();
                            var brain = JsonConvert.DeserializeObject<SingleLayerNeuronNetwork>(brainJson, BrainModelsSerializer.JsonSettings);
                            models[i][index - 1].Brain = brain;
                        }
                        else
                        {
                            throw new ArgumentException("Brains format");
                        }
                    }
                }
                return models;
            }
            else
            {
                throw new NotImplementedException();

            }

        }


        public static IFitnessCounter GetCounter(Scene scene, Dictionary<string, string> settings)
        {
            switch (scene)
            {
                case Scene.Wood:
                    var woodFitnessCounter = new WoodSceneFitnessCounter();
                    foreach (var s in settings)
                    {
                        FieldInfo fieldInfo = woodFitnessCounter.GetType().GetField(s.Key);
                        var value = ParseExperimentValue(s.Value, fieldInfo.FieldType);
                        fieldInfo.SetValue(woodFitnessCounter, value);
                    }
                    return woodFitnessCounter;
                case Scene.Mineral:
                    throw new NotImplementedException();

                case Scene.Competive:
                    throw new NotImplementedException();
            }
            throw new NotImplementedException();
        }
    }
}