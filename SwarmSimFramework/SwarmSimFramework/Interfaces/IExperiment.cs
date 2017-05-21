using SwarmSimFramework.Classes.Map;

namespace SwarmSimFramework.Interfaces
{
    public interface IExperiment
    {
        Map Map { get; }

        void Init();

        void MakeStep();

        bool Finnished { get; }

    }
}