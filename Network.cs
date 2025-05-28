using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

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

            double pathLoss = 20 * Math.Log10(distance) * 3;
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
                if (angleToTarget < 0)
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

        public bool IsWithinCoverage2(int targetX, int targetY, int senderX, int senderY, MapCell[,] map)
        {
            bool isInCoverage = false;
            if (map[senderX, senderY].bestStationCords[0] == X && map[senderX, senderY].bestStationCords[1] == Y)
            {
                if (map[targetX, targetY].bestStationCords[0] == X && map[targetX, targetY].bestStationCords[1] == Y)
                {
                    isInCoverage = true;
                }
            }
            return isInCoverage;
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
    public class PathFinder
    {
        public static Path findBestPath(MapCell[,] map, List<BaseStation> stations, MobileDevice sender, MobileDevice receiver)
        {
            BaseStation senderStation = stations[stations.FindIndex(item => item.X == map[sender.X, sender.Y].bestStationCords[0] && item.Y == map[sender.X, sender.Y].bestStationCords[1])];
            BaseStation receiverStation = stations[stations.FindIndex(item => item.X == map[receiver.X, receiver.Y].bestStationCords[0] && item.Y == map[receiver.X, receiver.Y].bestStationCords[1])];
            List<BaseStation> remaingStations = new List<BaseStation>();
            List<Path> PathList = new List<Path>();
            Path Path = new Path();
            var count = 0;
            var perms = Fact(stations.Count()-2);
            var smallestDist = 100000.0;
            while(count<perms)
            {
                var baseCnt = 0;
                var pathTemp = new Path();
                remaingStations.AddRange(stations);
                remaingStations.Remove(senderStation);
                //remaingStations.Remove(receiverStation);
                pathTemp.stations.Add(senderStation);
                baseCnt++;
                var result = GetNearestStation2(senderStation, remaingStations, PathList, baseCnt);
                var nearestStation = result.Item1;
                var distance = result.Item2;
                pathTemp.stations.Add(nearestStation);
                baseCnt++;
                pathTemp.distance += distance;
                remaingStations.Remove(nearestStation);
                while (nearestStation != receiverStation)
                {
                    result = GetNearestStation2(nearestStation, remaingStations, PathList, baseCnt);
                    var nearestStation2 = result.Item1;
                    distance = result.Item2;
                    if (nearestStation2 != nearestStation)
                    {
                        nearestStation = nearestStation2;
                        pathTemp.stations.Add(nearestStation);
                        baseCnt++;
                        pathTemp.distance += distance;
                        remaingStations.Remove(nearestStation);
                    }
                }
                if (smallestDist > distance)
                {
                    smallestDist = pathTemp.distance;
                }
                PathList.Add(pathTemp);
                count++;
            }
            Path = PathList[PathList.FindIndex(item => item.distance == smallestDist)];
            return Path;
        }

        public static Path findBestPath2(MapCell[,] map, List<BaseStation> stations, MobileDevice sender, MobileDevice receiver)
        {
            BaseStation senderStation = stations[stations.FindIndex(item => item.X == map[sender.X, sender.Y].bestStationCords[0] && item.Y == map[sender.X, sender.Y].bestStationCords[1])];
            BaseStation receiverStation = stations[stations.FindIndex(item => item.X == map[receiver.X, receiver.Y].bestStationCords[0] && item.Y == map[receiver.X, receiver.Y].bestStationCords[1])];
            List<BaseStation> remaingStations = new List<BaseStation>();
            List<Path> PathList = new List<Path>();
            Path Path = new Path();
            var perms = GetAllPathsWithOptionalStations(stations, senderStation, receiverStation);
            var smallestDist = 100000.0;
            foreach(var perm in perms)
            {
                double distance = 0.0;
                var pathTemp = new Path();
                for(int i = 0; i<perm.Count()-1; i++)
                {
                    float dx = perm[i].X - perm[i + 1].X;
                    float dy = perm[i].Y - perm[i + 1].Y;
                    var temp = Math.Sqrt(dx * dx + dy * dy);
                    distance += temp;
                }
                pathTemp.stations.AddRange(perm);
                pathTemp.distance = distance;
                if(smallestDist > distance)
                {
                    smallestDist = distance;
                }
                PathList.Add(pathTemp);
            }
            Path = PathList[PathList.FindIndex(item => item.distance == smallestDist)];
            return Path;
        }

        private static (BaseStation, double) GetNearestStation(BaseStation target, List<BaseStation> stations, List<Path> paths, int cnt)
        {
            BaseStation nearestStation = null;
            double distance = 100000.0;
            foreach(var station in stations)
            {
                float dx = target.X - station.X;
                float dy = target.Y - station.Y;
                var temp = Math.Sqrt(dx * dx + dy * dy);
                if (distance != 0 && distance > temp)
                {
                    distance = temp;
                    nearestStation = station;
                }
            }
            return (nearestStation, distance);
        }
        private static (BaseStation, double) GetNearestStation2(BaseStation target, List<BaseStation> stations, List<Path> paths, int index)
        {
            BaseStation nearestStation = null;
            double distance = 100000.0;
            var tempStations = stations;
            foreach (var station in tempStations)
            {
                bool flag = true;
                for(int i = 0; i< paths.Count(); i++)
                {
                    if (paths[i].stations[index].X == station.X && paths[i].stations[index].Y == station.Y)
                    {
                        flag = false;
                        break;
                    }
                }
                if(flag)
                {
                    float dx = target.X - station.X;
                    float dy = target.Y - station.Y;
                    var temp = Math.Sqrt(dx * dx + dy * dy);
                    if (distance != 0 && distance > temp)
                    {
                        distance = temp;
                        nearestStation = station;
                    }
                }
            }
            return (nearestStation, distance);
        }
        private bool IntersectsLine(Rectangle obstacle, int x1, int y1, int x2, int y2)
        {
            // Linie prostokąta (lewa, prawa, góra, dół)
            var lines = new[]
            {
                (obstacle.Left, obstacle.Top, obstacle.Right, obstacle.Top),      // Góra
                (obstacle.Right, obstacle.Top, obstacle.Right, obstacle.Bottom),  // Prawa
                (obstacle.Right, obstacle.Bottom, obstacle.Left, obstacle.Bottom),// Dół
                (obstacle.Left, obstacle.Bottom, obstacle.Left, obstacle.Top)     // Lewa
            };

            foreach (var (x3, y3, x4, y4) in lines)
            {
                if (LinesIntersect(x1, y1, x2, y2, x3, y3, x4, y4))
                    return true;
            }

            // Dodatkowo: sprawdź, czy cały odcinek mieści się w przeszkodzie
            if (obstacle.Contains(x1, y1) && obstacle.Contains(x2, y2))
                return true;

            return false;
        }

        private bool LinesIntersect(int x1, int y1, int x2, int y2,
                            int x3, int y3, int x4, int y4)
        {
            // Algorytm przecięcia dwóch odcinków
            float d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (d == 0) return false; // Równoległe

            float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / d;
            float u = ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / d;

            return t >= 0 && t <= 1 && u >= 0 && u <= 1;
        }
        private static long Fact(int n)
        {
            if (n == 0)
            {
                return 1;
            }
            return n * Fact(n - 1);
        }

        public static List<List<T>> GetPermutations<T>(List<T> originalList, T start, T end)
        {
            // Skopiuj i usuń start i end z listy (środkowe elementy)
            var middle = new List<T>(originalList);
            middle.Remove(start);
            middle.Remove(end);

            // Wygeneruj permutacje tylko środkowych
            var middlePermutations = GetPermutations(middle);

            // Dodaj start i end do każdej permutacji
            var result = new List<List<T>>();
            foreach (var perm in middlePermutations)
            {
                var fullPath = new List<T> { start };
                fullPath.AddRange(perm);
                fullPath.Add(end);
                result.Add(fullPath);
            }

            return result;
        }
        private static List<List<T>> GetPermutations<T>(List<T> list)
        {
            var result = new List<List<T>>();
            Permute(list, 0, list.Count - 1, result);
            return result;
        }
        private static void Permute<T>(List<T> list, int start, int end, List<List<T>> result)
        {
            if (start == end)
            {
                result.Add(new List<T>(list));
                return;
            }

            for (int i = start; i <= end; i++)
            {
                Swap(list, start, i);
                Permute(list, start + 1, end, result);
                Swap(list, start, i); // backtrack
            }
        }

        private static void Swap<T>(List<T> list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static List<List<T>> GetAllPathsWithOptionalStations<T>(List<T> allStations, T start, T end)
        {
            var middleStations = new List<T>(allStations);
            middleStations.Remove(start);
            middleStations.Remove(end);

            var allPaths = new List<List<T>>();

            var allSubsets = GetAllSubsets(middleStations);
            foreach (var subset in allSubsets)
            {
                var permutations = GetPermutations(subset);
                foreach (var perm in permutations)
                {
                    var path = new List<T> { start };
                    path.AddRange(perm);
                    path.Add(end);
                    allPaths.Add(path);
                }
            }

            return allPaths;
        }

        // Wszystkie możliwe podzbiory listy
        private static List<List<T>> GetAllSubsets<T>(List<T> list)
        {
            var subsets = new List<List<T>>();
            int subsetCount = 1 << list.Count; // 2^n podzbiorów

            for (int i = 0; i < subsetCount; i++)
            {
                var subset = new List<T>();
                for (int j = 0; j < list.Count; j++)
                {
                    if ((i & (1 << j)) != 0)
                    {
                        subset.Add(list[j]);
                    }
                }
                subsets.Add(subset);
            }

            return subsets;
        }
    }

    public class Path
    {
        public List<BaseStation> stations = new List<BaseStation>();
        public double distance;
    }
}
