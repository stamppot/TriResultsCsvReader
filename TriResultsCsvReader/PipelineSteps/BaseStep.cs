using System;
using System.Collections.Generic;

namespace TriResultsCsvReader
{
    public abstract class BaseStep : IPipelineStep
    {
        private readonly Action<string> _outputWriter;
        protected readonly List<string> InfoLogs;

        public BaseStep()
        {
            _outputWriter = (str => Console.WriteLine(str));
        }

        public BaseStep(List<string> infoLogs)
        {
            InfoLogs = infoLogs;
        }

        public abstract RaceEnvelope Process(RaceEnvelope step);

        protected void WriteInfo(string message)
        {
            if (null != _outputWriter)
            {
                _outputWriter.Invoke(message + Environment.NewLine);
            }

            if (null != InfoLogs)
            {
                InfoLogs.Add(message + Environment.NewLine);
            }
        }
    }
}
