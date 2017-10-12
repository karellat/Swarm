using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwarmSimFramework.Interfaces;
using System.ComponentModel;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Map;
using System.IO;
using System.Reflection;
using SwarmSimFramework.Classes.Entities; 

namespace SwarmSimFramework.Classes.MultiThreadExperiment
{
    class MTExperimentReader
    {
        public static string ReturnFields(Object obj)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var item in obj.GetType().GetFields())
            {
                if (!item.IsPublic)
                    continue;
                stringBuilder.Append(item.Name);
                stringBuilder.Append(":");
                stringBuilder.AppendLine(item.FieldType.Name);
            }

            return stringBuilder.ToString(); 
        } 

        public static MultiThreadExperiment<SingleLayerNeuronNetwork> ExperimentFromConfig(string path)
        {
            StreamReader streamReader = new StreamReader(path);
            string name = streamReader.ReadLine();
            var type = Type.GetType("SwarmSimFramework.Classes.MultiThreadExperiment." + name);
            MultiThreadExperiment<SingleLayerNeuronNetwork> exp =  (MultiThreadExperiment<SingleLayerNeuronNetwork>) Activator.CreateInstance(type); 

            Dictionary<string, string> fields = new Dictionary<string, string>();
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                if (line == "")
                    continue;
                fields[line.Split(new[] { ':' })[0]] = line.Split(new[] { ':' })[1]; 
            }

            foreach (var item in fields)
            {
                FieldInfo fieldInfo = exp.GetType().GetField(item.Key);
                var value = ParseExperimentValue(item.Value,fieldInfo.FieldType);
                fieldInfo.SetValue(exp, value); 
            }

            return exp;
        }

        public static Object ParseExperimentValue(string value,Type type)
        { 
            if(type.Equals(typeof(int))) 
            {
                return int.Parse(value); 
            }
            else if (type.Equals(typeof(double)))
            {
                return double.Parse(value);
            }
            else if (type.Equals(typeof(string)))
            {
                return value; 
            }
            else if (type.Equals(typeof(long)))
            {
                return long.Parse(value);
            }
            else if(type.Equals(typeof(RobotModel[])))
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
                        var modelType = Type.GetType("SwarmSimFramework.Classes.Robots."+ constructor[0]);
                        RobotEntity robot = (RobotEntity)Activator.CreateInstance(modelType);
                        int count = int.Parse(args[1]);
                        models.Add(new RobotModel() { model = robot, amount = count });

                    }
                }
                return models.ToArray();
            }
            else
            {
                throw new  NotImplementedException(); 
            }

        }

     
    }
}
