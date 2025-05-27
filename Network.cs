using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileSim
{
    public class BaseStation
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Gain { get; set; }
        public double TransmitPower { get; set; }
        public double Height { get; set; }
        public string RadiationPattern { get; set; } // np. "omni" lub "directional"
        public double Direction { get; set; } // w stopniach 0–360
        public BaseStation(int x, int y, double gain, double transmitPower)//, double height, string radiationPattern, double direction)
        {
            X = x;
            Y = y;
            Gain = gain;
            TransmitPower = transmitPower;
        }
        public double CalculateSignalStrength(int targetX, int targetY, double obstacleLoss = 0)
        {
            double dx = targetX - X;
            double dy = targetY - Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            if (distance == 0) distance = 1;

            double pathLoss = 20 * Math.Log10(distance);
            return TransmitPower + Gain - pathLoss - obstacleLoss;
        }
    }

    public class MobileDevice
    {
        public int X { get; set; }
        public int Y { get; set; }

        public MobileDevice(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
