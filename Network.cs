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
        public bool IsDirectional { get; set; } // np. "omni" lub "directional"
        public double BeamWidth { get; set; }
        public double Direction { get; set; } // w stopniach 0–360
        public BaseStation(int x, int y, double gain, double transmitPower, bool isdirectional = false, double direction = 0, double beamwidth = 0)//, double height, string radiationPattern, double direction)
        {
            X = x;
            Y = y;
            Gain = gain;
            TransmitPower = transmitPower;
            IsDirectional = isdirectional;
            Direction = direction;
            BeamWidth = beamwidth;
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
        public double IsWithinCoverage(int targetX, int targetY, int senderX, int senderY, double obstacleLoss = 0)
        {
            // Oblicz odległość
            float dx = targetX - X;
            float dy = targetY - Y;
            float sdx = senderX - X;
            float sdy = senderY - Y;
            double distance = Math.Sqrt(dx * dx + dy * dy) + Math.Sqrt(sdx * sdx + sdy * sdy);

            // Sprawdź kąt (dla anten kierunkowych)
            if (IsDirectional)
            {
                double angleToTarget = Math.Atan2(dx, -dy) * (180.0 / Math.PI);
                if(angleToTarget < 0)
                {
                    angleToTarget += 360;
                }
                double diff = Math.Abs(angleToTarget - Direction);
                if (diff > BeamWidth / 2)
                    return -1000; // poza zakresem kierunkowym
            }

            // Sygnał odebrany (prosty model propagacji)
            double Pr_dBm = TransmitPower + Gain - 20 * Math.Log10(distance + 1) - obstacleLoss;

            //return Pr_dBm >= -80; // np. minimalna moc odbiorcza
            return Pr_dBm;
        }

        private double NormalizeAngle(double angle)
        {
            angle %= 360;
            if (angle < 0) angle += 360;
            return angle;
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
