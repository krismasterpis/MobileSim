using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.AxHost;

namespace MobileSim
{
    public class BaseStation
    {
        private int scale = 10;
        private int pathLossScale = 3;
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
            Gain = gain/scale;
            TransmitPower = transmitPower / scale;
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

            double pathLoss = 20 * Math.Log10(distance) * pathLossScale;

            if (IsDirectional)
            {
                double angleToTarget = Math.Atan2(dx, -dy) * (180.0 / Math.PI);
                if (angleToTarget < 0)
                {
                    angleToTarget += 360;
                }
                //double diff = Math.Abs(angleToTarget - Direction);
                double diff = (angleToTarget + 360) % 360;
                double start = (Direction - BeamWidth / 2 + 360) % 360;
                double end = (Direction + BeamWidth / 2 + 360) % 360;
                if (targetX != X | targetY != Y)
                    if (diff < start || diff > end)
                        return -1000; // poza zakresem kierunkowym
            }

            return TransmitPower + Gain - pathLoss - obstacleLoss;
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

        public bool IsInCoverage(MapCell[,] map)
        {
            try
            {
                if (map[map[X, Y].bestStationCords[0], map[X, Y].bestStationCords[1]].Type == CellType.BaseStation)
                {
                    return true;
                }
            }
            catch { return false; }
            return false;
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
            var perms = GetAllPathsWithOptionalStations(stations, senderStation, receiverStation);
            var bestScore = 100000.0;
            double dist;
            BaseStation station = null;
            double alpha = 1;
            double beta = 0.5;
            List<List<BaseStation>> newPerms = new List<List<BaseStation>>();
            if (perms.Count > 0)
            {
                var i = 1;
                while(station != receiverStation && newPerms.Count()!=1)
                {
                    if(newPerms.Count() > 0)
                    {
                        perms.Clear();
                        perms.AddRange(newPerms);
                        newPerms.Clear();
                    }
                    for(int j = 0; j < perms.Count(); j++)
                    {
                        if (i < perms[j].Count())
                        {
                            double distance = 0.0;
                            var pathTemp = new Path();
                            float dx = perms[j][i - 1].X - perms[j][i].X;
                            float dy = perms[j][i - 1].Y - perms[j][i].Y;
                            float dxend = perms[j][i].X - receiverStation.X;
                            float dyend = perms[j][i].Y - receiverStation.Y;
                            var temp = Math.Sqrt(dx * dx + dy * dy);
                            var tempEnd = Math.Sqrt(dxend * dxend + dyend * dyend);
                            var score = alpha * temp + beta * tempEnd;
                            var intersectedcells = GetCellsOnLine(map, perms[j][i - 1].X, perms[j][i - 1].Y, perms[j][i].X, perms[j][i].Y);
                            if (intersectedcells.Any(obj => obj.signalStrength < -80))
                            {
                                continue;
                            }
                            if (intersectedcells.Any(obj => obj.Type == CellType.Obstacle))
                            {
                                score += 50;
                            }
                            if (bestScore > score)
                            {
                                station = perms[j][i];
                                bestScore = score;
                            }
                        }
                    }
                    for (int j = 0; j < perms.Count(); j++)
                    {
                        if (i < perms[j].Count())
                        {
                            if (perms[j][i] == station)
                            {
                                newPerms.Add(perms[j]);
                            }
                        }
                    }
                    i++;
                    bestScore = 10000;
                }
                Path.stations.AddRange(newPerms[0]);
            }
            else //przypadek dla mniej niż 3 stacji
            {
                if (senderStation == receiverStation)
                {
                    Path.stations.Add(senderStation);
                    Path.distance = 0;
                }
                else
                {
                    Path.stations.Add(senderStation);
                    Path.stations.Add(receiverStation);
                    Path.distance = 0;
                }
            }
            return Path;
        }
        private static List<MapCell> GetCellsOnLine(MapCell[,] map, int x0, int y0, int x1, int y1)
        {
            var cells = new List<MapCell>();

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            int width = map.GetLength(0);
            int height = map.GetLength(1);

            while (true)
            {
                // Upewnij się, że punkt jest w zakresie mapy
                if (x0 >= 0 && x0 < width && y0 >= 0 && y0 < height)
                {
                    cells.Add(map[x0, y0]);
                }

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return cells;
        }

        public static Path findBestPath2(MapCell[,] map, List<BaseStation> stations, MobileDevice sender, MobileDevice receiver)
        {
            BaseStation senderStation = stations[stations.FindIndex(item => item.X == map[sender.X, sender.Y].bestStationCords[0] && item.Y == map[sender.X, sender.Y].bestStationCords[1])];
            BaseStation receiverStation = stations[stations.FindIndex(item => item.X == map[receiver.X, receiver.Y].bestStationCords[0] && item.Y == map[receiver.X, receiver.Y].bestStationCords[1])];
            List<BaseStation> remaingStations = new List<BaseStation>();
            List<Path> PathList = new List<Path>();
            Path Path = new Path();
            var perms = GetAllPathsWithOptionalStations(stations, senderStation, receiverStation);
            var bestScore = 100000.0;
            double dist;
            BaseStation station = null;
            double alpha = 1;
            double beta = 0.1;
            double gamma = 1;
            List<List<BaseStation>> newPerms = new List<List<BaseStation>>();
            List<double> scores = new List<double>();
            if (perms.Count > 0)
            {
                foreach (var perm in perms)
                {
                    double distance = 0.0;
                    var pathTemp = new Path();
                    var score = 0.0;
                    for (int i = 1; i < perm.Count(); i++)
                    {
                        float dx = perm[i - 1].X - perm[i].X;
                        float dy = perm[i - 1].Y - perm[i].Y;
                        float dxend = perm[i].X - receiverStation.X;
                        float dyend = perm[i].Y - receiverStation.Y;
                        var temp = Math.Sqrt(dx * dx + dy * dy);
                        var tempEnd = Math.Sqrt(dxend * dxend + dyend * dyend);
                        score += alpha * temp + beta * tempEnd;
                        var intersectedcells = GetCellsOnLine(map, perm[i - 1].X, perm[i - 1].Y, perm[i].X, perm[i].Y);
                        if (intersectedcells.Any(obj => obj.signalStrength < -80))
                        {
                            score = -1;
                            break;
                        }
                        if (intersectedcells.Any(obj => obj.Type == CellType.Obstacle))
                        {
                            score += 50;
                        }
                    }
                    if(score >= 0)
                    {
                        float dxstart = perm[0].X - sender.X;
                        float dystart = perm[0].Y - sender.Y;
                        var tempStart = Math.Sqrt(dxstart * dxstart + dystart * dystart);
                        score += tempStart * gamma;
                        float dxendd = perm.Last().X - receiver.X;
                        float dyendd = perm.Last().Y - receiver.Y;
                        var tempEndd = Math.Sqrt(dxendd * dxendd + dyendd * dyendd);
                        score += tempEndd * gamma;
                        pathTemp.stations.AddRange(perm);
                        pathTemp.score = score;
                        scores.Add(score);
                        if (bestScore > score)
                        {
                            bestScore = score;
                        }
                        PathList.Add(pathTemp);
                    }
                }
                if(PathList.Count() > 0)
                {
                    Path = PathList[PathList.FindIndex(item => item.score <= bestScore && item.score >= 0)];
                    Debug.WriteLine(bestScore);
                }
                else
                {
                    Path = null;
                }
            }
            else //przypadek dla mniej niż 3 stacji
            {
                if(senderStation == receiverStation)
                {
                    Path.stations.Add(senderStation);
                    Path.distance = 0;
                }
                else
                {
                    Path.stations.Add(senderStation);
                    Path.stations.Add(receiverStation);
                    Path.distance = 0;
                }
            }
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
                    //var path = new List<T>();
                    path.AddRange(perm);
                    path.Add(end);
                    allPaths.Add(path);
                }
            }

            return allPaths;
        }

        public static List<List<T>> GetAllPathsWithOptionalStations2<T>(List<T> allStations, T start, T end)
        {
            var middleStations = new List<T>(allStations);
            //middleStations.Remove(start);
            //middleStations.Remove(end);

            var allPaths = new List<List<T>>();

            var allSubsets = GetAllSubsets(middleStations);
            foreach (var subset in allSubsets)
            {
                var permutations = GetPermutations(subset);
                foreach (var perm in permutations)
                {
                    //var path = new List<T> { start };
                    var path = new List<T>();
                    path.AddRange(perm);
                    //path.Add(end);
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
        public List<double> distances = new List<double>();
        public double distance;
        public double score;
    }
}
