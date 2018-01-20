using System;

namespace TriResultsCsvReader
{
    public abstract class BaseStep : IPipelineStep
    {
        private readonly Action<string> _outputWriter;

        public BaseStep()
        {
            _outputWriter = (str => Console.WriteLine(str));
        }

        public abstract StepData Process(StepData step);

        protected void WriteOutput(string message)
        {
            if (null != _outputWriter)
            {
                _outputWriter.Invoke(message);
            }
        }
    }
}
