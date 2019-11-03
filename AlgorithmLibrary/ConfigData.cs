using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmLibrary
{
    public class ConfigData
    {
        public List<double> TaskRuntimeByFrequency { get; set; }
        public List<double> ProcessorFrequency { get; set; }
        public List<double> Coefficients { get; set; }

        public List<double> TaskRuntimes { get; set; }

        public int MaxTasks { get; set; }
        public int MaxProcessors { get; set; }
        public double MaxDuration { get; set; }

        public int RefFrequency { get; set; }

    }
}
