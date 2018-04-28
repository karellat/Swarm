using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Experiments
{
    public class ESReader
    {
        private class ESSettings
        {
            public IFitnessCounter f = null; 
            public MapModel MapModel;
            public BrainModel<SingleLayerNeuronNetwork>[][] BrainModels = null;
            public RobotModel[] RobotModels = null;
        }

        private enum ReaderState
        { 
            EsSetting,
            Es,
            F,
            Map,
            Idle
        }

        public static EvolutionaryStrategies ReadFrom(string path)
        {
            StreamReader streamReader = new StreamReader(path);
            string mapName = streamReader.ReadLine();

            Dictionary<string, string> ESfields = new Dictionary<string, string>();
            Dictionary<string, string> ESSettingsfields = new Dictionary<string, string>();
            Dictionary<string, string> Ffields = new Dictionary<string, string>();
            Dictionary<string, string> Mapfields = new Dictionary<string, string>();

            ReaderState state = ReaderState.Idle;
            //Reading from file
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                if (line == "")
                    continue;
                else if (line.StartsWith("#"))
                {
                    if (line.StartsWith("#ES SETTINGS"))
                        state = ReaderState.EsSetting;
                    else if (line.StartsWith("#ES"))
                        state = ReaderState.Es;
                    else if (line.StartsWith("#F SETTINGS"))
                        state = ReaderState.F;
                    else if (line.StartsWith("#MAP SETTINGS"))
                        state = ReaderState.Map;
                    else
                        throw new NotImplementedException("Reading unknown state");

                    continue;
                }

                switch (state)
                {
                    case ReaderState.Es:
                    {
                        ESfields[line.Split(new[] { ':' })[0]] = line.Split(new[] { ':' })[1];
                        break;
                    }
                        
                    case ReaderState.EsSetting:
                    {
                        ESSettingsfields[line.Split(new[] {':'})[0]] = line.Split(new[] {':'})[1];
                        break;
                    }
                    case ReaderState.F:
                    {
                        Ffields[line.Split(new[] {':'})[0]] = line.Split(new[] {':'})[1];
                        break;
                    }
                    case ReaderState.Map:
                    {
                        Mapfields[line.Split(new[] {':'})[0]] = line.Split(new[] {':'})[1];
                        break;
                    }
                    default:
                    {
                        throw new NotImplementedException("Invalid state");

                    }

                }
            }
            
            //Seting part 

            ESSettings esSettings = new ESSettings();
            switch (mapName)
            {
                case ("WoodScene"):
                {
                    //MAP 
                    foreach (var item in Mapfields)
                    {
                        FieldInfo fieldInfo = typeof(WoodScene).GetField(item.Key);
                        var value = ParseExperimentValue(item.Value, fieldInfo.FieldType);
                        fieldInfo.SetValue(null,value);
                    }
                    break;
                }
                case ("MineralScene"):
                {
                    //MAP
                    foreach (var item in Mapfields)
                    {
                        FieldInfo fieldInfo = typeof(MineralScene).GetField(item.Key);
                        var value = ParseExperimentValue(item.Value, fieldInfo.FieldType); 
                        fieldInfo.SetValue(null,value);
                    }
                    break;
                }
                case ("CompetiveScene"):
                {
                    //MAP
                    foreach (var item in Mapfields)
                    {
                        FieldInfo fieldInfo = typeof(CompetitiveScene<SingleLayerNeuronNetwork>).GetField(item.Key);
                        var value = ParseExperimentValue(item.Value, fieldInfo.FieldType);
                        fieldInfo.SetValue(null, value);
                    }
                    break;
                }
                default:
                {
                    throw new ArgumentException("Wrong format of input file");
                }
        }



            foreach (var item in ESSettingsfields)
            {
                FieldInfo fieldInfo = esSettings.GetType().GetField(item.Key);
                var value = ParseExperimentValue(item.Value, fieldInfo.FieldType);
                fieldInfo.SetValue(esSettings,value);
            }
            //Create map 
            switch (mapName)
            {
                case ("WoodScene"):
                {
                    esSettings.MapModel = WoodScene.MakeMapModel(esSettings.RobotModels);
                    break;
                }
                case ("MineralScene"):
                {
                    esSettings.MapModel = MineralScene.MakeMapModel(esSettings.RobotModels);
                    break;
                }
                case ("CompetiveScene"):
                {
                    esSettings.MapModel =
                        CompetitiveScene<SingleLayerNeuronNetwork>.MakeMapModel(esSettings.RobotModels);
                    break;
                }
            }


            foreach (var item in Ffields)
            {
                FieldInfo fieldInfo = esSettings.f.GetType().GetField(item.Key);
                var value = ParseExperimentValue(item.Value, fieldInfo.FieldType);
                fieldInfo.SetValue(esSettings.f,value);
            }

            //Create ES
            EvolutionaryStrategies output = new EvolutionaryStrategies(esSettings.f,esSettings.MapModel,esSettings.BrainModels);

            foreach (var item in ESfields)
            {
                FieldInfo fieldInfo = output.GetType().GetField(item.Key);
                var value = ParseExperimentValue(item.Value, fieldInfo.FieldType);
                fieldInfo.SetValue(output, value);
            }

            streamReader.Close();
            return output; 
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
               var mapType =  Type.GetType("SwarmSimFramework.Classes.Map." + value);
                MapModel m = (MapModel) Activator.CreateInstance(mapType);
                return m; 
            }
            else if (type == typeof(IFitnessCounter))
            {
                var counterType = Type.GetType("SwarmSimFramework.Classes.Experiments.FitnessCounters." + value);
                IFitnessCounter f = (IFitnessCounter) Activator.CreateInstance(counterType);
                return f; 
            }
            else if (type == typeof(BrainModel<SingleLayerNeuronNetwork>[][]))
            {
                var robots = value.Split(new[] {';','='});
                int threadsCount = int.Parse(robots[0]);
                BrainModel<SingleLayerNeuronNetwork>[][] models = new BrainModel<SingleLayerNeuronNetwork>[threadsCount][];
                for (int i = 0; i < threadsCount; i++)
                {
                    models[i] = new BrainModel<SingleLayerNeuronNetwork>[robots.Length-1];
                    for (var index = 1; index < robots.Length; index++)
                    {
                        var r = robots[index];
                        RobotEntity Robot;
                        var parameters = r.Split(new[] {'-'});
                        var robotConstructor =
                            parameters[0].Split(new[] {'(', ')'}, StringSplitOptions.RemoveEmptyEntries);
                        var robotType = Type.GetType("SwarmSimFramework.Classes.Robots." + robotConstructor[0]);
                        if (robotConstructor.Length > 1)
                        {
                            int argumentsCount = robotConstructor.Length - 1;
                            Object[] arguments = new object[argumentsCount];
                            for (int j = 0; j < argumentsCount; j++)
                            {
                                arguments[j] = int.Parse(robotConstructor[j+1]);
                            }
                            models[i][index-1].Robot = (RobotEntity) Activator.CreateInstance(robotType,arguments);
                        }
                        else
                            models[i][index-1].Robot = (RobotEntity) Activator.CreateInstance(robotType);




                        if (parameters[1] == "G")
                        {
#if DEBUG
                            Console.WriteLine("ESReader: Generating new brain - {0}", parameters[1]);
#endif
                            models[i][index-1].Brain = SingleLayerNeuronNetwork.GenerateNewRandomNetwork(new IODimension()
                            {
                                Input = models[i][index-1].Robot.SensorsDimension,
                                Output = models[i][index-1].Robot.EffectorsDimension
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
                            var brain = JsonConvert.DeserializeObject<SingleLayerNeuronNetwork>(brainJson,BrainModelsSerializer.JsonSettings);
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
    }
}