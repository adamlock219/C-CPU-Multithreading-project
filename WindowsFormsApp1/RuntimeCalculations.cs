using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class RuntimeCalculations
    {
        public List<double> TaskRuntimeAtRefFrequency { get; set; }
        public List<double> ProcessorActualFrequency { get; set; }
        public int ProcessorReferenceFrequency { get; set; }

        public List<double> RuntimePerTasks { get; set; }

        public double AllocationRuntime { get; set; }

        public double[,] RuntimeMatrix { get; set; }
        public List<double[,]> UnderDurationAllocations { get; set; } 

        public RuntimeCalculations(List<double> taskruntimes, List<double> processorfrequency, int referencefrequency) 
        {
            TaskRuntimeAtRefFrequency = taskruntimes;
            ProcessorActualFrequency = processorfrequency;
            ProcessorReferenceFrequency = referencefrequency;

            RuntimePerTasks = new List<double>();
            UnderDurationAllocations = new List<double[,]>();

            RuntimeMatrix = new double[taskruntimes.Count, processorfrequency.Count];
        }

        public void CalculateTaskRuntime()
        {
            int p = 0;
            

           foreach (double tasks in TaskRuntimeAtRefFrequency)
           {
                foreach (double procs in ProcessorActualFrequency)
                {
                    RuntimePerTasks.Add(tasks * ProcessorReferenceFrequency / procs);
                }
           }


            for (int x = 0; x < TaskRuntimeAtRefFrequency.Count; x++)
            {
                for (int y = 0; y < ProcessorActualFrequency.Count; y++)
                {
                    RuntimeMatrix[x, y] = RuntimePerTasks[p];
                    p++;
                }
            }


            /*for (int y = ProcessorActualFrequency.Count -1; y >= 0; y--)
            {
                double SumOfProcessor = 0;

                for (int x = TaskRuntimeAtRefFrequency.Count -1; x >= 0 ; x--)
                {
                    double holdingtaskruntime = RuntimeMatrix[x, y];

                    for (int columnbelow = ProcessorActualFrequency.Count - 1; columnbelow > y; columnbelow--)
                    {
                        if (RuntimeMatrix[x, columnbelow] != 0)
                        {
                            RuntimeMatrix[x, y] = 0;
                        }
                    }

                    

                    SumOfProcessor += RuntimeMatrix[x, y];

                    if (SumOfProcessor >= maxduration)
                    {
                        SumOfProcessor -= RuntimeMatrix[x, y];
                        RuntimeMatrix[x, y] = 0;
                    }

                    if (x == TaskRuntimeAtRefFrequency.Count -1 && SumOfProcessor + holdingtaskruntime <= maxduration)
                    {
                        RuntimeMatrix[x, y] = holdingtaskruntime;
                    }

                    if(y == ProcessorActualFrequency.Count - 1)
                    {
                        double checkiftasknotassigned = 0;
                        for (int w=0; w<ProcessorActualFrequency.Count; w++)
                        {
                            checkiftasknotassigned += RuntimeMatrix[x, w];
                        }
                        if(checkiftasknotassigned == 0 && SumOfProcessor + holdingtaskruntime <= maxduration)
                        {
                            RuntimeMatrix[x, y] = holdingtaskruntime;
                        }
                    }

                }
            }

            UnderDurationAllocations.Add(RuntimeMatrix);

            // from here use this in wcf service
            /* while (UnderDurationAllocations.Count != 10)
             {
                 int p = 0;
                 double[] sum = new double[ProcessorActualFrequency.Count];

                 for (int x = 0; x < TaskRuntimeAtRefFrequency.Count; x++)
                 {
                     for (int y = 0; y < ProcessorActualFrequency.Count; y++)
                     {
                         RuntimeMatrix[x, y] = RuntimePerTasks[p];
                         p++;
                     }
                 }

                 Random random = new Random();

                 for (int i = 0; i < TaskRuntimeAtRefFrequency.Count; i++)
                 {
                     int y = random.Next(ProcessorActualFrequency.Count);

                     for (int x = 0; x < y; x++)
                     {
                         RuntimeMatrix[i, x] = 0;
                     }
                     for (int x = ProcessorActualFrequency.Count - 1; x > y; x--)
                     {
                         RuntimeMatrix[i, x] = 0;
                     }
                 }

                 for (int i = 0; i < ProcessorActualFrequency.Count; i++)
                 {

                     for (int j = 0; j < TaskRuntimeAtRefFrequency.Count; j++)
                     {
                         sum[i] += RuntimeMatrix[j, i];
                     }
                 }

                 double max = 0;

                 for (int i = 0; i < ProcessorActualFrequency.Count; i++)
                 {
                     if (sum[i] > max)
                     {
                         max = sum[i];
                     }
                 }

                 if (max <= 6.3)
                 {
                     UnderDurationAllocations.Add(RuntimeMatrix);
                 }
             }*/


        }



        public void GetAllocationRuntime()
        {

        }

    }
}
