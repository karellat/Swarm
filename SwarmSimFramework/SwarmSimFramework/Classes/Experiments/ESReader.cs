using System;
using System.Collections.Generic;
using System.IO;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Interfaces;

namespace SwarmSimFramework.Classes.Experiments
{
    public class ESReader
    {
        private struct ESSettings
        {
            public IFitnessCounter f;
            public MapModel map;
            public BrainModel<SingleLayerNeuronNetwork>[][] brainModels; 
        }

        private enum ReaderState
        {
            Es,F,Map,Idle
        }

        public static EvolutionaryStrategies ReadFrom(string path)
        {
            StreamReader streamReader = new StreamReader(path);
            string name = streamReader.ReadLine();


            Dictionary<string, string> ESfields = new Dictionary<string, string>();
            Dictionary<string, string> Ffields = new Dictionary<string, string>();
            Dictionary<string, string> Mapfields = new Dictionary<string, string>();
            ReaderState state = ReaderState.Idle;
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                if (line == "")
                    continue;
                else if (line.StartsWith("#"))
                {
                    if (line.StartsWith("#ES SETTINGS"))
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
                        ESfields[line.Split(new[] {':'})[0]] = line.Split(new[] {':'})[1];
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


            throw new NotImplementedException();
         }
    }