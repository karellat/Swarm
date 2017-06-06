using System.Text;
using SwarmSimFramework.Classes.Entities;
using SwarmSimFramework.Classes.RobotBrains;
using SwarmSimFramework.Interfaces;

namespace SwarmSimFramework.Classes.Experiments.WoodCuttingExperiment
{
    public class WoodCuttingExperimentWalking : Experiment<SingleLayerNeuronNetwork>
    {
        /// <summary>
        /// Directory for ass
        /// </summary>
        protected string WorkingDir = "walkwoodE";
        /// <summary>
        /// Init of wood cutting experiment walking
        /// </summary>
        public override void Init()
        {
            //Prepare stuff for serialization 
            System.IO.Directory.CreateDirectory(WorkingDir);

            //Prepare fitness count 

        }
        /// <summary>
        /// Fitness of individual
        /// </summary>
        /// <param name="robotEntity"></param>
        protected override void CountIndividualFitness(RobotEntity robotEntity)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Single map iterations 
        /// </summary>
        protected override void SingleMapSimulation()
        {
            throw new System.NotImplementedException();
        }

        protected override void SingleGeneration()
        {
            throw new System.NotImplementedException();
        }
    }
}