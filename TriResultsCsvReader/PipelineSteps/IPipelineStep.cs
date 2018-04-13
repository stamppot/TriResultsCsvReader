using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriResultsCsvReader
{
    public interface IPipelineStep
    {
        RaceStepData Process(RaceStepData step);
    }
}
