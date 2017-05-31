using System.Text;
using SwarmSimFramework.Classes.Map;

namespace SwarmSimFramework.Interfaces
{
    public interface IExperiment
    {
        Map Map { get; }

        void Init();

        void MakeStep();

        bool Finnished { get; }
        /// <summary>
        /// Thread safe operation for reading metainfos
        /// </summary>
        StringBuilder ExperimentInfo { get; }

    }
}