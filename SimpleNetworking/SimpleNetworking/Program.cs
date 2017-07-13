using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.Experiments.TestingMaps;
using SwarmSimFramework.Classes.Map;
using SwarmSimFramework.Classes.MultiThreadExperiment;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Classes.Robots;
using SwarmSimFramework.Interfaces;
using SwarmSimFramework.SupportClasses;

namespace SimpleNetworking
{

    /// <summary>
    /// Struct for representing server adress 
    /// </summary>
    public struct ServerAddress
    {
        public string Ip;
        public int Port;
        public AddressFamily Family;
    }
    /// <summary>
    /// Struct for transfer map
    /// </summary>
  public  struct  MapPack
  {
      public float Height;
      public float Width;
     
      public List<RobotEntity> robots;
      public List<CircleEntity> passives;
      public List<FuelEntity> fuels;
      public List<RadioEntity> initRadio;

      public Map ConstructMap()
      {
          return new Map(Height,Width,robots,passives,fuels);
      }

      public override string ToString()
      {
          StringBuilder s = new StringBuilder("Map Pack Height ");
          s.Append(Height);
          s.AppendLine(" Width");
          s.AppendLine("Robots:");
          foreach (var r in robots)
          {
              s.AppendLine("\t " + r.Name);
          }
          s.AppendLine("Passive:");
          foreach (var r in passives)
          {
              s.AppendLine("\t " + r.Name);
          }
          return s.ToString();
      }
  }

    class Program
    {
        public static ServerAddress[] TestingAddress = new[] { new ServerAddress()
        {
            Ip= "127.0.0.1",
            Port = 6027,
            Family = AddressFamily.InterNetwork
           
        } };
        static void Main(string[] args)
        {
            if(args[0] == "SERVER")
            {
                ServerMain();
            }
            else if (args[0] == "CLIENT")
            {
                ClientMain(); 
            }
            //Console.WriteLine("Unknown situation");
        }

        private static void ClientMain()
        {
            //ClientMain(TestingAddress,ClientTcpCommunicator.exampleMap, ClientTcpCommunicator.exampleBrainModels);
            ClientMain(TestingAddress,ClientTcpCommunicator.exampleMap);

        }

        /// <summary>
        /// Main of client side 
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="mapPack"></param>
        /// <param name="brainModel"></param>
        private static void ClientMain(ServerAddress[] addresses,MapPack mapPack, BrainModel<SingleLayerNeuronNetwork>[] brainModel=null)
        {
            ClientTcpCommunicator client = new ClientTcpCommunicator();
            client.Connect(addresses);
            client.InitMap(mapPack);
            if (brainModel == null)

            {
                Console.WriteLine("Set server to generate new brain.");
                client.InitBrain();
            }
            else
            {
                Console.WriteLine("Send server brain to evaluate.");
                client.InitBrain(brainModel);
            }

            client.FinishEval();
            Console.WriteLine("Global fitness: " + client.BrainFitness);
            foreach (var abm in client.actualBrainModels)
            {
                Console.WriteLine("Brain: " + abm.Brain.ToString());
                Console.WriteLine("Robot: " + abm.Robot.ToString());
                  
            }
          
        }

        static byte[] _data;  // 1
        static Socket _listeningSocket; // 1
        static Socket _comunicationSocket;
        /// <summary>
        /// Main of server side
        /// </summary>
        private static void ServerMain()
        {
            ServerTcpCommunicator s = new ServerTcpCommunicator();
            s.Listen();
            Map m = s.InitMap();
            BrainModel<SingleLayerNeuronNetwork>[] brainModels = s.InitBrains();
            List<BrainModel<SingleLayerNeuronNetwork>> bModels = new List<BrainModel<SingleLayerNeuronNetwork>>();
            foreach (var r in m.Robots)
            {
                if (brainModels.Length == 0)
                {
                    //Generate new Brains
                    
                    bool suitableModel = false;
                    foreach (var mo in bModels)
                    {
                        if (mo.SuitableRobot(r))
                        {
                            r.Brain = mo.Brain;
                            suitableModel = true;
                            break;
                        }

                    }
                    if (!suitableModel)
                    {
                        var nB = SingleLayerNeuronNetwork.GenerateNewRandomNetwork(new IODimension()
                        {
                            Input = r.SensorsDimension,
                            Output = r.EffectorsDimension
                        });
                        bModels.Add(new BrainModel<SingleLayerNeuronNetwork>()
                            { Brain = nB,Robot=r});
                        r.Brain = nB;
                    }
                }
                else
                {
                    foreach (var bm in brainModels)
                    {
                        if (bm.SuitableRobot(r))
                            r.Brain = bm.Brain;
                    }
                }
            }

            if (brainModels.Length == 0)
                brainModels = bModels.ToArray();

            for (int i = 0; i < 1000; i++)
            {
                m.MakeStep();
                if(i % 100 == 0)
                Console.WriteLine("Map made {0} step",i);
            }

            double fitness = WoodExperimentMt.CountFitnessOfMap(m);

            s.FinishEval(brainModels,fitness);
        
        }

     
    }
    /// <summary>
    /// Static class for TCP socket easy Control 
    /// </summary>
    public static class TcpControl
    {
        // Status tag
        public const string MapSendTag = "MAP SEND";
        public const string MapAccept ="MAP ACCEPT";
        public const string SendBrainTag = "SEND BRAIN";
        public const string GenerateBrainTag = "GENERATE BRAIN";
        public const string MapResendTag = "MAP RESEND";
        public const string MapCheckTag = "MAP CHECK";
        public const string BrainCheckTag = "BRAIN CHECK";
        public const string FitSendTag = "FIT SEND";
        public const string SimulationFinishedTag = "SIM FINNISHED";
 

        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All
        };

        public const char ETX = (char) 3;
        

        /// <summary>
        /// Read text from socket, waiting for ETX
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReadTextFromSocket(Socket s)
        {
            byte[] recieveBuffer = new byte[s.ReceiveBufferSize];
            int realSize = s.Receive(recieveBuffer);
            byte[] recieveBytes = new byte[realSize];
            Array.Copy(recieveBuffer,recieveBytes,realSize);
            List<byte> recieveList = new List<byte>(recieveBytes);
            while (recieveList.Last() != (byte) ETX)
            {
                realSize = s.Receive(recieveBuffer);
                for (int i = 0; i < realSize; i++)
                {
                    recieveList.Add(recieveBuffer[i]);
                }
            }

            recieveList.RemoveAt(recieveList.Count-1);
            return Encoding.Default.GetString(recieveList.ToArray());
        }
        /// <summary>
        /// Send text through socket with ETX on the end of text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="s"></param>
        public static void SendTextThroughSocket(string text,Socket s)
        {
           if( text.Contains((char) 3)) 
                throw new NotImplementedException("Text contains unprintable character ETX");
            int indexOfFirstByte = 0;
            byte[] sendingBytes = Encoding.Default.GetBytes(text +ETX);
            byte[] sendingBuffer;
            if (sendingBytes.Length <= s.SendBufferSize)
            {
                sendingBuffer = new byte[sendingBytes.Length];
            }
            else
            {
                sendingBuffer = new byte[s.SendBufferSize];
            }
             
            while (indexOfFirstByte < sendingBytes.Length)
            {
                Console.WriteLine("Sending from index {0} length {1} of total length {2}",indexOfFirstByte.ToString(),sendingBuffer.Length.ToString(),sendingBuffer.Length.ToString());
                Array.Copy(sendingBytes,indexOfFirstByte,sendingBuffer,0,sendingBuffer.Length);
                s.Send(sendingBuffer);
                indexOfFirstByte += sendingBuffer.Length;
                if (sendingBytes.Length - indexOfFirstByte <= s.SendBufferSize)
                {
                    sendingBuffer = new byte[sendingBytes.Length - indexOfFirstByte];
                }
                else
                {
                    sendingBuffer = new byte[s.SendBufferSize];
                }
            }
        }

    }

    /// <summary>
    /// Communication implemetntation through TCP IP 
    /// </summary>
    public class ClientTcpCommunicator
    {
        //example of map 
        public static MapPack exampleMap = new MapPack()
        {
            Height=600,
            Width = 800,
            fuels =  new List<FuelEntity>(),
            initRadio =  new List<RadioEntity>(),
            passives = new List<CircleEntity>() { new ObstacleEntity(new System.Numerics.Vector2(20,20),10), new ObstacleEntity(new System.Numerics.Vector2(40, 50), 10) },
            robots = new List<RobotEntity>() { new ScoutCutterRobot(new Vector2(98,44)), new ScoutCutterRobot(new Vector2(34, 44)), new ScoutCutterRobot(new Vector2(98, 100)) }
        };
        //model of robot
        public static ScoutCutterRobot model = new ScoutCutterRobot(Vector2.Zero);
        //example of models
        public static BrainModel<SingleLayerNeuronNetwork>[] exampleBrainModels = new[]
        {
            new BrainModel<SingleLayerNeuronNetwork>()
            {
                Robot = new ScoutCutterRobot(Vector2.Zero),
                Brain = SingleLayerNeuronNetwork.GenerateNewRandomNetwork(new IODimension()
                {
                    Input = model.SensorsDimension,
                    Output = model.EffectorsDimension
                })
            }
        };
        /// <summary>
        /// Curently communicating sockets
        /// </summary>
        public List<Socket> OpenSockets = new List<Socket>();

        //Connect communication with server 
        public BrainModel<SingleLayerNeuronNetwork>[] actualBrainModels;
        //fitness of the brain 
        public double BrainFitness; 
        /// <summary>
        /// connect to address
        /// </summary>
        /// <param name="serverAddresses"></param>
        public void Connect(ServerAddress[] serverAddresses)
        {
            //Try to connect to servers possible & add sucessful to OpenSockets
            foreach (var a in serverAddresses)
            {
                var s = new Socket(a.Family, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    s.Connect(IPAddress.Parse(a.Ip), a.Port);
                    OpenSockets.Add(s);
                }
                catch
                {
                    //Wrong connection
                }
            }
        }
        /// <summary>
        /// Prepare map on server side
        /// </summary>
        /// <param name="map"></param>
        public void InitMap(MapPack map)
        {
            string mapJson = JsonConvert.SerializeObject(map, TcpControl.JsonSettings);
            byte[] jsonBytes = Encoding.Default.GetBytes(mapJson);

            foreach (var os in OpenSockets)
            {
                
                if (SendMap(os, mapJson, jsonBytes))
                    Console.WriteLine("Map was sucessfully send");
                else
                {
                    Console.WriteLine("Socket lost - {0} ",os.ToString());
                    throw new NotImplementedException("MY OWN EXCEPTION - mapAccept  tag not recieved");
                }

                string MapStatus = TcpControl.ReadTextFromSocket(os);
                int attempts = 0;
                while (MapStatus == TcpControl.MapResendTag && attempts < 100)
                {
                    attempts++;
                    if (SendMap(os, mapJson, jsonBytes))
                        Console.WriteLine("Map was sucessfully send");
                    else
                    {
                        Console.WriteLine("Socket lost - {0} ", os.ToString());
                        throw new NotImplementedException("MY OWN EXCEPTION - mapAccept  tag not recieved");
                    }
                    MapStatus = TcpControl.ReadTextFromSocket(os);
                }
                if(attempts == 100)
                    throw new NotImplementedException("MY OWN EXCEPTION - too much attempts");
                if (MapStatus == TcpControl.MapCheckTag)
                    Console.WriteLine("Map was sucessfully initiate; socket:{0} ", os.LocalEndPoint);
                else
                    throw new NotImplementedException("MY OWN EXCEPTION - map check tag not accepted");
                
            }
           
            
        }
        /// <summary>
        /// Prepare brains on the server side
        /// </summary>
        /// <param name="brainModels"></param>
        public void InitBrain(BrainModel<SingleLayerNeuronNetwork>[] brainModels)
        {
            actualBrainModels = brainModels;
            foreach (var os in OpenSockets)
            {
                TcpControl.SendTextThroughSocket(TcpControl.SendBrainTag, os);
                string serializedBrain = BrainModelsSerializer.SerializeArray<SingleLayerNeuronNetwork>(brainModels);
                byte[] jsonBytes = Encoding.Default.GetBytes(serializedBrain);
                
                TcpControl.SendTextThroughSocket(serializedBrain,os);
                string brainCheck = TcpControl.ReadTextFromSocket(os);
                if (brainCheck == TcpControl.BrainCheckTag)
                    Console.WriteLine("Brain was sent to Socket: {0}", os.LocalEndPoint);
                else
                    throw new NotImplementedException("Sending brain not check ");
            }
        }
        /// <summary>
        /// Prepare generating brain on the server side
        /// </summary>
        public void InitBrain()
        {
            foreach (var os in OpenSockets)
            {
                TcpControl.SendTextThroughSocket(TcpControl.GenerateBrainTag,os);
                string brainCheck = TcpControl.ReadTextFromSocket(os);
                if (brainCheck == TcpControl.BrainCheckTag)
                {
                    Console.WriteLine("Brain was generated on Socket: {0}", os.LocalEndPoint);
                }
                else
                {
                    throw new NotImplementedException("Generating not check ");
                }
            }
            
        }
        /// <summary>
        /// Send map pack 
        /// </summary>
        /// <param name="os"></param>
        /// <param name="mapJson"></param>
        /// <param name="jsonBytes"></param>
        /// <returns></returns>
        private bool SendMap(Socket os, string mapJson, byte[] jsonBytes)
        {
            //Send map tag
            TcpControl.SendTextThroughSocket(TcpControl.MapSendTag, os);
            //Check os listeng 
            byte[] receiveBuffer = new byte[os.ReceiveBufferSize];
            string checkTag = TcpControl.ReadTextFromSocket(os);
            if (checkTag == TcpControl.MapAccept)
            {
                //send serialized  map 
                Console.WriteLine("Sending {0} bytes", jsonBytes.Length);
                TcpControl.SendTextThroughSocket(mapJson, os);
                return true;
            }
            else
            {
                //KIll socket
                return false;
            }
        }

        /// <summary>
        /// Finishing simulation on server and reading result
        /// </summary>
        /// <param name="brainRequest"></param>
        public void FinishEval(bool brainRequest = true)
        {
            foreach (var os in OpenSockets)
            {
                string checkTag = TcpControl.ReadTextFromSocket(os);
                if (checkTag == TcpControl.SimulationFinishedTag)
                {
                    if (brainRequest)
                    {
                        TcpControl.SendTextThroughSocket(TcpControl.SendBrainTag, os);
                        Console.WriteLine("Brain requested");
                        string brainJson = TcpControl.ReadTextFromSocket(os);
                        BrainModel<SingleLayerNeuronNetwork>[] newBrains = BrainModelsSerializer
                            .DeserializeArray<SingleLayerNeuronNetwork>(brainJson);
                        actualBrainModels = newBrains;
                        Console.WriteLine("Requesting fitness");
                    }
                    else
                        Console.WriteLine("Only fitness requested");

                        TcpControl.SendTextThroughSocket(TcpControl.FitSendTag,os);
                        string fitnessS = TcpControl.ReadTextFromSocket(os);
                        double fitness = double.Parse(fitnessS);
                        BrainFitness = fitness;

                    
                }
                else
                    throw new NotImplementedException("Unexpected tag");
            }
           
        }

    }
    /// <summary>
    /// Server side communication
    /// </summary>
    public class ServerTcpCommunicator
    {
        /// <summary>
        /// Socket, where server listens
        /// </summary>
        public Socket _listeningSocket;
        /// <summary>
        /// Socket, where communication with client is going on
        /// </summary>
        public Socket _comunicationSocket;

        public ServerTcpCommunicator()
        {
            _listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // 2
            _listeningSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6027)); // 3
        }
        /// <summary>
        /// waiting for client connection
        /// </summary>
        public void Listen()
        {
            Console.WriteLine("Listening");
            _listeningSocket.Listen(1); // 4
            _comunicationSocket = _listeningSocket.Accept(); // 5
            Console.WriteLine("Listen at " + _comunicationSocket.RemoteEndPoint);
        }
        /// <summary>
        /// Prepare map from client
        /// </summary>
        /// <returns></returns>
        public Map InitMap()
        {
            Map outputMap = null;
            int attempts = 0;
            while (outputMap == null && attempts < 100)
            {
                if(attempts != 0)
                    TcpControl.SendTextThroughSocket(TcpControl.MapResendTag,_comunicationSocket);
                attempts++;
                string mapSendTag = TcpControl.ReadTextFromSocket(_comunicationSocket);
                Console.WriteLine("Recieve: " + mapSendTag);
                if (mapSendTag == TcpControl.MapSendTag)
                {
                    TcpControl.SendTextThroughSocket(TcpControl.MapAccept, _comunicationSocket);
                    string mapText = TcpControl.ReadTextFromSocket(_comunicationSocket);
                    Console.WriteLine("Recieving {0} bytes ", mapText.Length);

                    MapPack mapPack = JsonConvert.DeserializeObject<MapPack>(mapText, TcpControl.JsonSettings);
                    Console.WriteLine(mapPack.ToString());
                    outputMap = new Map(mapPack.Height, mapPack.Width, mapPack.robots, mapPack.passives, mapPack.fuels);
                    
                }
                else
                    throw new NotImplementedException("Connection tag check failed, end communication ");
            }
            if (attempts == 100)
                throw new NotImplementedException("Too much attempts");
            //otherwise successfully
            TcpControl.SendTextThroughSocket(TcpControl.MapCheckTag,_comunicationSocket);
            return outputMap;

        }
        /// <summary>
        /// Finish simulation & send result
        /// </summary>
        /// <param name="brainModels"></param>
        /// <param name="fitness"></param>
        public void FinishEval(BrainModel<SingleLayerNeuronNetwork>[] brainModels, double fitness)
        {
            Console.WriteLine("Simulation finished.");
            TcpControl.SendTextThroughSocket(TcpControl.SimulationFinishedTag,_comunicationSocket);
            string status = TcpControl.ReadTextFromSocket(_comunicationSocket);
            switch (status)
            {
                case TcpControl.SendBrainTag:
                {
                    Console.WriteLine("Brain was requested.");
                    //send brain
                    string modelsJson = BrainModelsSerializer.SerializeArray(brainModels);
                    TcpControl.SendTextThroughSocket(modelsJson, _comunicationSocket);
                    string fitnessTag = TcpControl.ReadTextFromSocket(_comunicationSocket);
                    if (fitnessTag == TcpControl.FitSendTag)
                    {
                        string fitnessS = fitness.ToString();
                        TcpControl.SendTextThroughSocket(fitnessS,_comunicationSocket);
                    }
                    else
                    {
                        throw new NotImplementedException("Unknown tag ");
                    }
                    break;
            }
                case TcpControl.FitSendTag:
                {
                    //send only brain fitness
                    Console.WriteLine("Only fitness requested");
                    string fitnessS = fitness.ToString();
                    TcpControl.SendTextThroughSocket(fitnessS, _comunicationSocket);
                    break;
                }
                default:
                {
                    throw new NotImplementedException("Unknown tag ");
                }
            }
        }
        /// <summary>
        /// Prepare brains from client
        /// </summary>
        /// <returns></returns>
        public BrainModel<SingleLayerNeuronNetwork>[] InitBrains()
        {
            string braintag = TcpControl.ReadTextFromSocket(_comunicationSocket);
            switch (braintag)
            {
                case TcpControl.GenerateBrainTag:
                {
                    Console.WriteLine("Brain set to be generated");
                    TcpControl.SendTextThroughSocket(TcpControl.BrainCheckTag, _comunicationSocket);
                    return new BrainModel<SingleLayerNeuronNetwork>[0];
                    break;
                }
                case TcpControl.SendBrainTag:
                {
                    string serializeBrainModels = TcpControl.ReadTextFromSocket(_comunicationSocket);
                    BrainModel<SingleLayerNeuronNetwork>[] brainModels =
                        BrainModelsSerializer.DeserializeArray<SingleLayerNeuronNetwork>(serializeBrainModels);
                    TcpControl.SendTextThroughSocket(TcpControl.BrainCheckTag, _comunicationSocket);
                    Console.WriteLine("Brain was accepted and deserialized" );
                        return brainModels;
                }
                default:
                {
                    throw new NotImplementedException("Unknown situation, tag");
                }
            }

        }
    }
}
 