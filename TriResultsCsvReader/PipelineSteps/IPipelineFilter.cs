namespace TriResultsCsvReader
{
    public interface IPipelineStep
    {
        RaceEnvelope Process(RaceEnvelope raceStepData);
    }

    public interface IReduceStep : IPipelineStep
    {
        //RaceEnvelope Process(RaceEnvelope raceStepData);
    }
}
