using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SwarmSimFramework.Classes.Experiments
{
    public static class SettingsReader
    {
        public static Dictionary<string, string> GetSettingsFromFile(string filePath)
        {
            StreamReader file = new StreamReader(filePath);
            Dictionary<string,string> dic = new Dictionary<string, string>();

            while (!file.EndOfStream)
            {
                var line = file.ReadLine(); 
                dic[line.Split(new[] {':'})[0]] = line.Split(new[] {':'})[1];
            }
            file.Close();
            return dic; 
        }

  
    }
}