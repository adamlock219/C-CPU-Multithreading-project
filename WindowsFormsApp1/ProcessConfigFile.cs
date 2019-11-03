using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

//(runtime * reference freq)  and then divide that by processor freq  Use that calc the task runtime is done at the reference freq   And then you divide by the new processor one

namespace WindowsFormsApp1
{
    class ProcessConfigFile
    {
        private string FileName { get; set; }
        public List<string> ErrorList { get; set; }

        private int[] LimitsTask { get; set; }
        private int[] LimitsProcessor { get; set; }
        private double[] LimitsProcessorFrequencies { get; set; }

        private double ProgramMaxDurations { get; set; }
        private int ProgramTasks { get; set; }
        private int ProgramProcessors { get; set; }

        private int ReferenceFrequency { get; set; }

        public Dictionary<double, double> RuntimeByTaskID { get; set; }
        public Dictionary<double, double> FrequencyByProcessorID { get; set; }
        public Dictionary<double, double> ValueByCoefficientID { get; set; }

        public List<double[,]> UnderMaxDurationAllocations { get; set; }

        public string FinalAnswer { get; set; }
        public string VMIPS { get; set; }

        public ProcessConfigFile(string filename)
        {
            FileName = filename;
            ErrorList = new List<string>();
            LimitsTask = new int[2];
            LimitsProcessor = new int[2];
            LimitsProcessorFrequencies = new double[2];
            RuntimeByTaskID = new Dictionary<double, double>();
            FrequencyByProcessorID = new Dictionary<double, double>();
            ValueByCoefficientID = new Dictionary<double, double>();
        }


        public void ParseTan()
        {
            WebClient webClient = new WebClient();
            Stream stream = webClient.OpenRead(FileName);
            StreamReader r = new StreamReader(stream);
            

                while (!r.EndOfStream)
                {
                    string line = r.ReadLine();


                    Console.WriteLine(line);

                }

                parselimits();
                parseProgramData();
                ParseLibrary();
                CalculateRuntime();
                DataSentToVM();
                CalculateEnergy();

                r.Close();

            
        }

        public void parselimits()
        {

            char[] delimiterChars = { ',' };

            WebClient webClient = new WebClient();
            Stream stream = webClient.OpenRead(FileName);
            StreamReader r = new StreamReader(stream);


            while (!r.EndOfStream)
            {
                string line = r.ReadLine();

                if (line != null)
                    line.Trim();
                
                //Storing all these as arrays
                if (line.StartsWith("LIMITS-TASK"))
                {

                    string[] items = line.Split(delimiterChars);
                    int j = 0;
                    if (!items.Contains("1") || !items.Contains("500"))
                    {
                        ErrorList.Add("tasks can only be 1 and have a maximum of 500");
                    }
                    else
                    {
                        for (int i = 0; i < items.Length; i++)
                        {
                            if (items[i] == "1" || items[i] == "500")
                            {
                                LimitsTask[j] = Convert.ToInt32(items[i]);
                                j++;
                            }
                        }
                    }
                }

                if (line.StartsWith("LIMITS-PROCESSORS"))
                {

                    string[] items = line.Split(delimiterChars);
                    int j = 0;

                    if (!items.Contains("1") || !items.Contains("1000"))
                    {
                        ErrorList.Add("processors can only be 1 and have a maximum of 1000");
                    }
                    else
                    {
                        for (int i = 0; i < items.Length; i++)
                        {
                            if (items[i] == "1" || items[i] == "1000")
                            {
                                LimitsProcessor[j] = Convert.ToInt32(items[i]);
                                j++;
                            }
                        }
                    }
                }

                if (line.StartsWith("LIMITS-PROCESSOR-FREQUENCIES"))
                {

                    string[] items = line.Split(delimiterChars);
                    int j = 0;

                   
                        for (int i = 0; i < items.Length; i++)
                        {
                            if (items[i].Any(char.IsDigit))
                            {
                                LimitsProcessorFrequencies[j] = Convert.ToDouble(items[i]);
                                j++;
                            }
                        }
                    
                }


            }
            r.Close();
            //Console.WriteLine("the number of tasks is correct with:" + TaskNumber);
        }


        public void parseProgramData()
        {

            char[] delimiterChars = { ',' };

            WebClient webClient = new WebClient();
            Stream stream = webClient.OpenRead(FileName);
            StreamReader r = new StreamReader(stream);

            while (!r.EndOfStream)
            {
                string line = r.ReadLine();

                if (line != null)
                    line.Trim();

                if (line.StartsWith("PROGRAM-MAXIMUM-DURATION"))
                {

                    string[] items = line.Split(delimiterChars);

                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i].Any(char.IsDigit))
                        {
                            ProgramMaxDurations = Convert.ToDouble(items[i]);
                        }
                    }
                }

                if (line.StartsWith("PROGRAM-TASKS"))
                {

                    string[] items = line.Split(delimiterChars);

                        for (int i = 0; i < items.Length; i++)
                        {
                            if (items[i].Any(char.IsDigit))
                            {
                                ProgramTasks = Convert.ToInt32(items[i]);
                            }
                        }
                    
                }

                if (line.StartsWith("PROGRAM-PROCESSORS"))
                {

                    string[] items = line.Split(delimiterChars);

                        for (int i = 0; i < items.Length; i++)
                        {
                            if (items[i].Any(char.IsDigit))
                            {
                                ProgramProcessors = Convert.ToInt32(items[i]);
                            }
                        }
                    
                }

                if (line.StartsWith("RUNTIME-REFERENCE-FREQUENCY"))
                {

                    string[] items = line.Split(delimiterChars);

                    for (int i = 0; i < items.Length; i++)
                    {
                        if (items[i].Any(char.IsDigit))
                        {
                            ReferenceFrequency = Convert.ToInt32(items[i]);
                        }
                    }
                }

            }
            r.Close();
        }

        public void ParseLibrary()
        {
            char[] delimiterChars = { ',' };

            var textFromFile = (new WebClient()).DownloadString(FileName);

            double[] taskRuntime = new double[ProgramTasks];
            double[] processorFrequency = new double[ProgramProcessors];
            int[] coefficientValue = new int[2];

            string[] lines = textFromFile.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                //pulls the 5 lines after 'PROCESSOR-ID'
                if (lines[i].StartsWith("TASK-ID"))
                {
                    int k = 0;
                    string[] array = new string[ProgramTasks];
                    for (int j = 1; j <= ProgramTasks; j++)
                    {
                        array[k] =  lines[i + j];
                        k++;

                    }
                    string join = "";
                    join = string.Join(",", array);
                    
                    //split at ',' and convert to int array
                    string[] items = join.Split(delimiterChars);
                    taskRuntime = Array.ConvertAll<string, double>(items, new Converter<string, double>(Convert.ToDouble));

                    //add to dictionary
                    for (int j=0; j<ProgramTasks + ProgramTasks; j++)
                    {
                        RuntimeByTaskID.Add(taskRuntime[j], taskRuntime[j+1]);
                        j++;
                    }
                }

                //pulls the 3 lines after 'PROCESSOR-ID'
                if (lines[i].StartsWith("PROCESSOR-ID"))
                {
                    int k = 0;
                    string[] array = new string[ProgramProcessors];
                    for (int j = 1; j <= ProgramProcessors; j++)
                    {
                        array[k] = lines[i + j];
                        k++;

                    }
                    string join = "";
                    join = string.Join(",", array);

                    //split at ',' and convert to int array
                    string[] items = join.Split(delimiterChars);
                    processorFrequency = Array.ConvertAll<string, double>(items, new Converter<string, double>(Convert.ToDouble));

                    //add to dictionary
                    for (int j = 0; j < ProgramProcessors + ProgramProcessors; j++)
                    {
                        FrequencyByProcessorID.Add(processorFrequency[j], processorFrequency[j + 1]);
                        j++;
                    }
                }

                //pulls the 3 lines after 'COEFFICIENT-ID' 
                if (lines[i].StartsWith("COEFFICIENT-ID"))
                {
                    string[] array = { lines[i + 1], lines[i + 2], lines[i + 3] };
                    string join = "";
                    join = string.Join(",", array);

                    //split at ',' and convert to int array
                    string[] items = join.Split(delimiterChars);
                    coefficientValue = Array.ConvertAll<string, int>(items, new Converter<string, int>(Convert.ToInt32));

                    //add to dictionary
                    for (int j = 0; j < 6; j++)
                    {
                        ValueByCoefficientID.Add(coefficientValue[j], coefficientValue[j + 1]);
                        j++;
                    }
                }
            }
        }

        public void CalculateRuntime()
        {
            List<double> taskruntimebyreffreq = new List<double>();
            List<double> processorfrequency = new List<double>();

            foreach (KeyValuePair<double, double> entry in RuntimeByTaskID)
            {
                taskruntimebyreffreq.Add(entry.Value);
            }

            foreach (KeyValuePair<double, double> entry in FrequencyByProcessorID)
            {
                processorfrequency.Add(entry.Value);
            }

            RuntimeCalculations runtimeCalculations = new RuntimeCalculations(taskruntimebyreffreq, processorfrequency, ReferenceFrequency);

            runtimeCalculations.CalculateTaskRuntime();

        }

        public void CalculateEnergy()
        {
            double c0 = 0;
            double c1 = 0;
            double c2 = 0;

            //need to pull the coefficients from dikctionary to make instantiate a new energy calculations class 
            foreach (KeyValuePair<double, double> entry in ValueByCoefficientID)
            {
                if(entry.Key == 0)
                {
                    c0 = entry.Value;
                }

                if (entry.Key == 1)
                {
                    c1 = entry.Value;
                }

                if (entry.Key == 2)
                {
                    c2 = entry.Value;
                }
            }
            EnergyCalculations e = new EnergyCalculations(c2, c1, c0);

            //will give the energy per second by each processor, using the frequencies in the dictionary
            foreach(KeyValuePair<double,double> entry in FrequencyByProcessorID)
            {
                if (entry.Key == 1)
                {
                    Console.WriteLine(e.EnergyPerSecond(entry.Value));
                }

                if (entry.Key == 2)
                {
                    Console.WriteLine(e.EnergyPerSecond(entry.Value));
                }

                if (entry.Key == 3)
                {
                    Console.WriteLine(e.EnergyPerSecond(entry.Value));
                }

            }

            //need runtime to complete calculations........ too complicated to workout, left it to late ????????????????
        }

        public void DataSentToVM()
        {
            AlgorithmLibrary.ConfigData cd = new AlgorithmLibrary.ConfigData();

            
            List<double> taskruntimebyreffreq = new List<double>();
            List<double> processorfrequency = new List<double>();
            List<double>  coefficients = new List<double>();

            foreach (KeyValuePair<double, double> entry in RuntimeByTaskID)
            {
                taskruntimebyreffreq.Add(entry.Value);
            }

            foreach (KeyValuePair<double, double> entry in FrequencyByProcessorID)
            {
                processorfrequency.Add(entry.Value);
            }

            foreach (KeyValuePair<double, double> entry in ValueByCoefficientID)
            {
                coefficients.Add(entry.Value);
            }

            RuntimeCalculations runtimeCalculations = new RuntimeCalculations(taskruntimebyreffreq, processorfrequency, ReferenceFrequency);

            runtimeCalculations.CalculateTaskRuntime();

            cd.ProcessorFrequency = processorfrequency;
            cd.TaskRuntimeByFrequency = taskruntimebyreffreq;
            cd.TaskRuntimes = runtimeCalculations.RuntimePerTasks;
            cd.MaxDuration = ProgramMaxDurations;
            cd.Coefficients = coefficients;
            cd.MaxProcessors = ProgramProcessors;
            cd.MaxTasks = ProgramTasks;
            cd.RefFrequency = ReferenceFrequency;

            ServiceReference1.ServiceClient localws = new ServiceReference1.ServiceClient();
            Alg2.ServiceClient localwsalg2 = new Alg2.ServiceClient();

            //was using a cloud architecture on AWS to solve aswell

            /*AlgVM1.ServiceClient ALGvmone = new AlgVM1.ServiceClient();
            ALGAVM2.ServiceClient ALGvmtwo = new ALGAVM2.ServiceClient();
            AlgAVM3.ServiceClient ALGavmthree = new AlgAVM3.ServiceClient();

            ALGBVM1.ServiceClient ALGbvmone = new ALGBVM1.ServiceClient();
            ALGBVM2.ServiceClient ALGbvmtwo = new ALGBVM2.ServiceClient();
            ALGBVM3.ServiceClient ALGbvmthree = new ALGBVM3.ServiceClient();*/

            List<double[]> catchList = new List<double[]>();

            if (FileName == "https://sit323sa.blob.core.windows.net/at2/TestA.txt")
            {
                FinalAnswer += "LOCAL WEBSERVICE" + "\r\n" + localwsalg2.AlgorithmTwo(cd);

                FinalAnswer += "\r\n" + localws.AlgorithmOne(cd) + "\r\n";

                //FinalAnswer += "AWS WEBSERVICE" + "\r\n" + ALGvmone.AlgorithmOne(cd);
                //FinalAnswer += "AWS WEBSERVICE" + "\r\n" + ALGvmtwo.AlgorithmOne(cd);
                //FinalAnswer += "AWS WEBSERVICE" + "\r\n" + ALGavmthree.AlgorithmOne(cd);

                //FinalAnswer += "AWS WEBSERVICE" + "\r\n" + ALGbvmone.AlgorithmTwo(cd);
                //FinalAnswer += "AWS WEBSERVICE" + "\r\n" + ALGbvmtwo.AlgorithmTwo(cd);
                //FinalAnswer += "AWS WEBSERVICE" + "\r\n" + ALGbvmthree.AlgorithmTwo(cd);
            }
            else
            {
                FinalAnswer += localwsalg2.AlgorithmTwo(cd) + "\r\n";

                //FinalAnswer += "AWS WEBSERVICE" + "\r\n" + ALGbvmone.AlgorithmTwo(cd);
                //FinalAnswer += "AWS WEBSERVICE" + "\r\n" + ALGbvmtwo.AlgorithmTwo(cd);
                //FinalAnswer += "AWS WEBSERVICE" + "\r\n" + ALGbvmthree.AlgorithmTwo(cd);

            }

            

        }
    }
}