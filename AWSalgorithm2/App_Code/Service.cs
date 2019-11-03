using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using AlgorithmLibrary;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class Service : IService
{
    public List<double[,]> UnderDurationAllocation { get; set; }
    public List<double> RuntimeOfAllocation { get; set; }
    public List<double> TotalEnergyOfAllocation { get; set; }


    public string AlgorithmTwo(ConfigData cd)
    {
        string s = "";

        UnderDurationAllocation = new List<double[,]>();
        RuntimeOfAllocation = new List<double>();
        TotalEnergyOfAllocation = new List<double>();

        int t = 0;

        double c0 = cd.Coefficients[t];
        double c1 = cd.Coefficients[t + 1];
        double c2 = cd.Coefficients[t + 2];


            int p = 0;

            double[] sum = new double[cd.ProcessorFrequency.Count];
            double[,] RuntimeMatrix = new double[cd.TaskRuntimeByFrequency.Count, cd.ProcessorFrequency.Count];

            for (int x = 0; x < cd.TaskRuntimeByFrequency.Count; x++)
            {
                for (int y = 0; y < cd.ProcessorFrequency.Count; y++)
                {
                    RuntimeMatrix[x, y] = cd.TaskRuntimes[p];
                    p++;
                }
            }


        for (int y = cd.ProcessorFrequency.Count - 1; y >= 0; y--)
        {
            double SumOfProcessor = 0;

            for (int x = cd.TaskRuntimeByFrequency.Count - 1; x >= 0; x--)
            {
                //double holdingtaskruntime = RuntimeMatrix[x, y];

                for (int columnbelow = cd.ProcessorFrequency.Count - 1; columnbelow > y; columnbelow--)
                {
                    if (RuntimeMatrix[x, columnbelow] != 0)
                    {
                        RuntimeMatrix[x, y] = 0;
                    }
                }

                SumOfProcessor += RuntimeMatrix[x, y];

                if(SumOfProcessor >= cd.MaxDuration)
                {
                    SumOfProcessor -= RuntimeMatrix[x, y];
                    RuntimeMatrix[x, y] = 0;
                }

                /*if (y == cd.ProcessorFrequency.Count - 1)
                {
                    double checkiftasknotassigned = 0;
                    for (int w = 0; w < cd.ProcessorFrequency.Count; w++)
                    {
                        checkiftasknotassigned += RuntimeMatrix[x, w];
                    }
                    if (checkiftasknotassigned == 0 && SumOfProcessor + holdingtaskruntime <= cd.MaxDuration)
                    {
                        RuntimeMatrix[x, y] = holdingtaskruntime;
                    }
                }*/

            }
        }

        for (int i = 0; i < cd.ProcessorFrequency.Count; i++)
        {

            for (int j = 0; j < cd.TaskRuntimeByFrequency.Count; j++)
            {
                sum[i] += RuntimeMatrix[j, i];
            }
        }

        double max = 0;

        for (int i = 0; i < cd.ProcessorFrequency.Count; i++)
        {
            if (sum[i] > max)
            {
                max = sum[i];
            }
        }

        RuntimeOfAllocation.Add(max);
        UnderDurationAllocation.Add(RuntimeMatrix);
        
        

        foreach (double[,] allocation in UnderDurationAllocation)
        {
            double u = 0;
            for (int i = 0; i < cd.ProcessorFrequency.Count; i++)
            {

                for (int j = 0; j < cd.TaskRuntimeByFrequency.Count; j++)
                {
                    u += TotalEnergy(cd.ProcessorFrequency[i], c0, c1, c2, allocation[j, i]);
                }
            }

            TotalEnergyOfAllocation.Add(u);
        }

        foreach (double[,] allocation in UnderDurationAllocation)
        {
            for (int i = 0; i < cd.ProcessorFrequency.Count; i++)
            {

                for (int j = 0; j < cd.TaskRuntimeByFrequency.Count; j++)
                {
                    if (allocation[j, i] != 0)
                    {
                        allocation[j, i] = 1;
                    }
                    s += allocation[j, i].ToString() + "\t";
                }
                s += "\r\n";
            }
            break;
        }

        for (int i = 0; i < 1; i++)
        {
            s += "Runtime:  " + RuntimeOfAllocation[i].ToString() + "\r\n" + "Total Energy:  " + TotalEnergyOfAllocation[i].ToString();
        }


        return s;

    }

    public double TotalEnergy(double frequency, double Coefficient0, double Coefficient1, double Coefficient2, double duration)
    {
        return (Coefficient2 * frequency * frequency + Coefficient1 * frequency + Coefficient0) * duration;
    }
}
