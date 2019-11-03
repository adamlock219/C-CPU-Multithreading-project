using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class EnergyCalculations
    {
        public double Coefficient0 { get; set; }
        public double Coefficient1 { get; set; }
        public double Coefficient2 { get; set; }

        public EnergyCalculations(double c2, double c1, double c0)
        {
            Coefficient2 = c2;
            Coefficient1 = c1;
            Coefficient0 = c0;
        }

        public double EnergyPerSecond(double frequency)
        {
            return (Coefficient2 * frequency * frequency + Coefficient1 * frequency + Coefficient0);
        }

        public double TotalEnergy(double frequency, double duration)
        {
            return (this.EnergyPerSecond(frequency) * duration);
        }
    }
}
