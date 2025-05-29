using log4net.Appender;
using log4net.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace MobileSim
{
    public partial class Form1 : Form
    {
        const int MapSize = 1000;
        const int CellSize = 10;
        const int GridCount = MapSize / CellSize;

        private float zoomFactor = 1.0f;
        private const float ZoomStep = 0.1f;
        private const float MinZoom = 0.2f;
        private const float MaxZoom = 5.0f;

        /*        double Gain = 17.0;             // Zysk anteny kierunkowej w dBi (typowe: 14–18 dBi)
                double TransmitPower = 40.0;    // Moc nadawcza w dBm (40 dBm = 10 W)
                double Height = 30.0;           // Wysokoœæ zawieszenia anteny (w metrach)
                bool IsDirectional = true;      // Antena kierunkowa
                double BeamWidth = 90.0;        // Szerokoœæ wi¹zki w stopniach (typowe 60–90°)
                double Direction = 0.0;       // Kierunek g³ównej wi¹zki anteny (np. 120°)*/

        double Gain = 5.0;              // Zysk anteny dookólnej (typowo 2–6 dBi)
        double TransmitPower = 30.0;    // Moc nadawcza w dBm (30 dBm = 1 W)
        double Height = 10.0;           // Wysokoœæ na budynku lub maszcie
        bool IsDirectional = false;     // Antena dookólna (omni)
        double BeamWidth = 360.0;       // Pe³na dookólna charakterystyka
        double Direction = 0.0;         // Kierunek nieistotny przy dookólnej

        private int basesCount = 0;
        private int obstaclesCount = 0;

        private bool signalLineEnab = false;
        private bool obstcPlcd = false;

        private MapCell[,] map = new MapCell[GridCount, GridCount];
        private PlacementMode currentMode = PlacementMode.BaseStation;
        private List<BaseStation> baseStations = new List<BaseStation>();
        private BaseStation bestStation;
        private Path bestPath;
        MobileDevice senderDev;
        MobileDevice receiverDev;
        private List<Rectangle> obstacles = new List<Rectangle>();
        private enum PlacementMode { BaseStation, Obstacle, Sender, Receiver }


        public Form1()
        {
            InitializeComponent();
            pictureBoxMap.Paint += pictureBoxMap_Paint;
            pictureBoxMap.MouseClick += pictureBoxMap_MouseClick;
            pictureBoxMap.MouseMove += pictureBoxMap_MouseMove;
            comboBoxMode.SelectedIndexChanged += comboBoxMode_SelectedIndexChanged;
            mapPanel.MouseWheel += mapPanel_MouseWheel;
            comboBoxMode.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            trackBar1.Minimum = 0;
            trackBar1.Maximum = 20;
            trackBar1.Value = (int)Gain;
            labelGain.Text = $"Gain (dBi) = {Gain}";
            trackBar2.Minimum = 0;
            trackBar2.Maximum = 60;
            trackBar2.Value = (int)TransmitPower;
            labelTransmit.Text = $"Transmit power (dBm) = {TransmitPower}";
            trackBar3.Minimum = 0;
            trackBar3.Maximum = 50;
            trackBar2.Value = (int)Height;
            labelHeight.Text = $"Height (m) = {Height}";
            numericUpDownDirection.Minimum = 0;
            numericUpDownDirection.Maximum = 360;
            numericUpDownDirection.Value = 360;
            numericUpDownBeamWidth.Minimum = 0;
            numericUpDownBeamWidth.Maximum = 360;
            numericUpDownBeamWidth.Value = 90;
            InitMap();
        }

        private void InitMap()
        {
            pictureBoxMap.Size = new Size(1000, 1000);
            pictureBoxMap.Location = new Point(0, 3);
            for (int x = 0; x < GridCount; x++)
                for (int y = 0; y < GridCount; y++)
                    map[x, y] = new MapCell { Type = CellType.Empty, signalStrength = -100 };
        }

        private void mapPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            float oldZoom = zoomFactor;

            if (e.Delta > 0)
            {
                if (zoomFactor < 5)
                {
                    zoomFactor += ZoomStep;
                }
                else
                {
                    zoomFactor = 5;
                }
            }
            else
            {
                if (zoomFactor > 1)
                {
                    zoomFactor -= ZoomStep;
                }
                else
                {
                    zoomFactor = 1;
                }
            }
            label1.Text = $"Zoom: {zoomFactor * 100}%";
            zoomFactor = Math.Max(MinZoom, Math.Min(MaxZoom, zoomFactor));

            // Zoom w miejsce kursora
            Point mousePos = mapPanel.PointToClient(Control.MousePosition);
            ZoomAt(mousePos, zoomFactor);
        }

        private void ZoomAt(Point focusPoint, float newZoom)
        {
            //float ratio = newZoom / oldZoom;
            float ratio = newZoom;
            int newWidth = (int)(1000 * ratio);
            int newHeight = (int)(1000 * ratio);

            // Przesuniêcie scrolla tak, by utrzymaæ fokus
            mapPanel.AutoScrollPosition = new Point(0, 0); // resetuj
            Point scrollPos = mapPanel.AutoScrollPosition;

            int offsetX = (int)((focusPoint.X + mapPanel.HorizontalScroll.Value) * ratio - focusPoint.X);
            int offsetY = (int)((focusPoint.Y + mapPanel.VerticalScroll.Value) * ratio - focusPoint.Y);

            pictureBoxMap.Size = new Size(newWidth, newHeight);
            mapPanel.AutoScrollPosition = new Point(offsetX, offsetY);

            pictureBoxMap.Invalidate();
        }
        private void pictureBoxMap_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.ScaleTransform(zoomFactor, zoomFactor);
            for (int x = 0; x < GridCount; x++)
            {
                for (int y = 0; y < GridCount; y++)
                {
                    Rectangle rect = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);
                    g.DrawRectangle(Pens.LightGray, rect);
                }
            }
            if (signalLineEnab && bestPath != null && senderDev != null && receiverDev != null)
            {
                DrawSignalLine(bestPath, e, g);
            }
            DrawSignalCircle(g, baseStations, obstcPlcd);
            obstcPlcd = false;
            using (Font emojiFont = new Font("Segoe UI Emoji", 10))
            {
                string emoji;
                for (int x = 0; x < GridCount; x++)
                {
                    for (int y = 0; y < GridCount; y++)
                    {
                        Rectangle rect = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);
                        switch (map[x, y].Type)
                        {
                            case CellType.BaseStation:
                                //g.FillRectangle(Brushes.Blue, rect);
                                emoji = Char.ConvertFromUtf32(Convert.ToInt32("1F4E1", 16));
                                g.DrawString(emoji, emojiFont, Brushes.Black, new PointF(x * CellSize - 0.5f * CellSize, y * CellSize - 0.5f * CellSize));
                                break;
                            case CellType.Obstacle:
                                emoji = Char.ConvertFromUtf32(Convert.ToInt32("1F6A7", 16));
                                g.DrawString(emoji, emojiFont, Brushes.Black, new PointF(x * CellSize - 0.5f * CellSize, y * CellSize - 0.5f * CellSize));
                                //g.FillRectangle(Brushes.Gray, rect);
                                break;
                            case CellType.Sender:
                                emoji = Char.ConvertFromUtf32(Convert.ToInt32("1F6DC", 16));
                                g.DrawString(emoji, emojiFont, Brushes.Black, new PointF(x * CellSize - 0.5f * CellSize, y * CellSize - 0.5f * CellSize));
                                //g.FillRectangle(Brushes.Green, rect);
                                break;
                            case CellType.Receiver:
                                emoji = Char.ConvertFromUtf32(Convert.ToInt32("1F4F1", 16));
                                g.DrawString(emoji, emojiFont, Brushes.Black, new PointF(x * CellSize - 0.5f * CellSize, y * CellSize - 0.5f * CellSize));
                                //g.FillRectangle(Brushes.Red, rect);
                                break;
                            default:
                                break;
                                /*                        default:
                                                            g.DrawRectangle(Pens.LightGray, rect);
                                                            break;*/
                        }
                    }
                }
            }
        }
        private void pictureBoxMap_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            int X = (int)Math.Floor((double)(x / 10));
            int Y = (int)Math.Floor((double)(y / 10));
            // Upewnij siê, ¿e kursor znajduje siê w granicach obrazu
            if (X >= 0 && X < MapSize / CellSize && Y >= 0 && Y < MapSize / CellSize)
            {
                label6.Text = $"X: {X}";
                label7.Text = $"Y: {Y}";
                label8.Text = $"Cell type:{map[X, Y].Type.ToString()}";
                label9.Text = $"Signal strength:{Math.Round(map[X, Y].signalStrength, 3).ToString()}";
                label12.Text = $"X: {map[X, Y].bestStationCords[0]}";
                label11.Text = $"Y: {map[X, Y].bestStationCords[1]}";
            }
            else
            {
                label6.Text = $"X:";
                label7.Text = $"Y:";
            }
        }
        private void pictureBoxMap_MouseClick(object sender, MouseEventArgs e)
        {
            signalLineEnab = false;
            int x = e.X / (int)(CellSize * zoomFactor);
            int y = e.Y / (int)(CellSize * zoomFactor);
            if (x < 0 || y < 0 || x >= GridCount || y >= GridCount) return;
            if (map[x, y].Type != CellType.Empty)
            {
                if (map[x, y].Type == CellType.BaseStation)
                {
                    if (basesCount > 0)
                    {
                        basesCount--;
                        baseStations.RemoveAt(baseStations.FindIndex(item => item.X == x && item.Y == y));
                        label2.Text = $"Base stations ({basesCount}/10)";
                        for (int gx = 0; gx < 100; gx++)
                        {
                            for (int gy = 0; gy < 100; gy++)
                            {
                                if (map[gx, gy].bestStationCords[0] == x && map[gx, gy].bestStationCords[1] == y)
                                {
                                    map[gx, gy].Clear(force: false);
                                }
                            }
                        }
                    }
                }
                if (map[x, y].Type == CellType.Obstacle)
                {
                    if (obstaclesCount > 0)
                    {
                        obstaclesCount--;
                        obstacles.RemoveAt(obstacles.FindIndex(item => item.X == x && item.Y == y));
                        label3.Text = $"Obstacles ({obstaclesCount}/50)";
                    }
                }
                if (map[x, y].Type == CellType.Receiver)
                {
                    receiverDev = null;
                    label5.Text = $"Receivers (0/1)";
                }
                if (map[x, y].Type == CellType.Sender)
                {
                    senderDev = null;
                    label4.Text = $"Senders (0/1)";
                }
                map[x, y].Type = CellType.Empty;
            }
            else
            {
                switch (currentMode)
                {
                    case PlacementMode.BaseStation:
                        if (baseStations.Count() < 10 && map[x, y].Type != CellType.BaseStation)
                        {
                            map[x, y].Type = CellType.BaseStation;
                            //BaseStation station = new BaseStation(x, y, 6, 30,true,90,90);
                            BaseStation station = new BaseStation(x, y, Gain, TransmitPower, IsDirectional, Direction, BeamWidth);
                            baseStations.Add(station);
                            basesCount++;
                            label2.Text = $"Base stations ({basesCount}/10)";
                        }
                        break;
                    case PlacementMode.Obstacle:
                        if (obstacles.Count() < 50 && map[x, y].Type != CellType.Obstacle)
                        {
                            map[x, y].Type = CellType.Obstacle;
                            var obstacle = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);
                            obstacles.Add(obstacle);
                            obstaclesCount++;
                            label3.Text = $"Obstacles ({obstaclesCount}/50)";
                            obstcPlcd = true;
                        }
                        break;
                    case PlacementMode.Sender:
                        if (senderDev == null && map[x, y].Type != CellType.Sender)
                        {
                            map[x, y].Type = CellType.Sender;
                            senderDev = new MobileDevice(x, y);
                            label4.Text = $"Senders (1/1)";
                        }
                        break;
                    case PlacementMode.Receiver:
                        if (receiverDev == null && map[x, y].Type != CellType.Receiver)
                        {
                            map[x, y].Type = CellType.Receiver;
                            receiverDev = new MobileDevice(x, y);
                            label5.Text = $"Receivers (1/1)";
                        }
                        break;
                }
            }
            pictureBoxMap.Invalidate();
            Debug.WriteLine($"Selected X: {x}, Y:{y}");
        }

        private void DrawSignalCircle(Graphics g, List<BaseStation> stations, bool obstacleplaced = false)
        {
            if (obstacleplaced)
            {
                for (int gx = 0; gx < 100; gx++)
                {
                    for (int gy = 0; gy < 100; gy++)
                    {
                        map[gx, gy].Clear(force: false);
                    }
                }
            }
            for (int gx = 0; gx < 100; gx++)
            {
                for (int gy = 0; gy < 100; gy++)
                {
                    int centerX = gx * CellSize + CellSize / 2;
                    int centerY = gy * CellSize + CellSize / 2;
                    foreach (var station in stations)
                    {
                        double obstacleLoss = 0;
                        foreach (var obs in obstacles)
                        {
                            if (IntersectsLine(obs, station.X * CellSize + CellSize / 2, station.Y * CellSize + CellSize / 2, gx * CellSize + CellSize / 2, gy * CellSize + CellSize / 2))
                            {
                                obstacleLoss += 5; // ka¿dy budynek np. -5 dB
                            }
                        }

                        double signal = station.CalculateSignalStrength(gx, gy, obstacleLoss);
                        if (signal > -80)
                        {
                            if (signal > map[gx, gy].signalStrength)
                            {
                                map[gx, gy].signalStrength = signal;
                                map[gx, gy].bestStationCords = [station.X, station.Y];
                            }
                        }
                    }
                }
            }
            for (int gx = 0; gx < 100; gx++)
            {
                for (int gy = 0; gy < 100; gy++)
                {
                    if (map[gx, gy].signalStrength > -100)
                    {
                        Color color = SignalStrengthToColor(map[gx, gy].signalStrength);
                        using (var brush = new SolidBrush(color))
                        {
                            g.FillRectangle(brush, gx * CellSize, gy * CellSize, CellSize, CellSize);
                        }
                    }
                    else
                    {
                        Rectangle rect = new Rectangle(gx * CellSize, gy * CellSize, CellSize, CellSize);
                        g.DrawRectangle(Pens.LightGray, rect);
                    }
                }
            }
        }

        private Color SignalStrengthToColor(double dBm)
        {
            dBm = Math.Clamp(dBm, -100, -40);
            double t = (dBm + 100) / 60.0;

            int r = (int)(255 * t);
            int g = 0;
            int b = (int)(255 * (1 - t));

            return Color.FromArgb(100, r, g, b); // pó³przezroczysty
        }
        private void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentMode = (PlacementMode)comboBoxMode.SelectedIndex;
        }

        private bool IntersectsLine(Rectangle obstacle, int x1, int y1, int x2, int y2)
        {
            // Linie prostok¹ta (lewa, prawa, góra, dó³)
            var lines = new[]
            {
                (obstacle.Left, obstacle.Top, obstacle.Right, obstacle.Top),      // Góra
                (obstacle.Right, obstacle.Top, obstacle.Right, obstacle.Bottom),  // Prawa
                (obstacle.Right, obstacle.Bottom, obstacle.Left, obstacle.Bottom),// Dó³
                (obstacle.Left, obstacle.Bottom, obstacle.Left, obstacle.Top)     // Lewa
            };

            foreach (var (x3, y3, x4, y4) in lines)
            {
                if (LinesIntersect(x1, y1, x2, y2, x3, y3, x4, y4))
                    return true;
            }

            // Dodatkowo: sprawdŸ, czy ca³y odcinek mieœci siê w przeszkodzie
            if (obstacle.Contains(x1, y1) && obstacle.Contains(x2, y2))
                return true;

            return false;
        }

        private bool LinesIntersect(int x1, int y1, int x2, int y2,
                            int x3, int y3, int x4, int y4)
        {
            // Algorytm przeciêcia dwóch odcinków
            float d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            if (d == 0) return false; // Równoleg³e

            float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / d;
            float u = ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / d;

            return t >= 0 && t <= 1 && u >= 0 && u <= 1;
        }

        private void DrawSignalLine(Path path, PaintEventArgs e, Graphics g)
        {
            for (int i = 0; i < path.stations.Count() - 1; i++)
            {
                using (Pen Pen = new Pen(Color.Red, 3))
                {
                    // Punkty pocz¹tkowy i koñcowy linii
                    Point start = new Point(path.stations[i].X * CellSize + CellSize / 2, path.stations[i].Y * CellSize + CellSize / 2);
                    Point end = new Point(path.stations[i + 1].X * CellSize + CellSize / 2, path.stations[i + 1].Y * CellSize + CellSize / 2);
                    // Narysuj liniê
                    e.Graphics.DrawLine(Pen, start, end);
                }
            }
            using (Pen Pen = new Pen(Color.Green, 3))
            {
                // Punkty pocz¹tkowy i koñcowy linii
                Point sender = new Point(senderDev.X * CellSize + CellSize / 2, senderDev.Y * CellSize + CellSize / 2);
                Point receiver = new Point(receiverDev.X * CellSize + CellSize / 2, receiverDev.Y * CellSize + CellSize / 2);
                Point start = new Point(path.stations[0].X * CellSize + CellSize / 2, path.stations[0].Y * CellSize + CellSize / 2);
                Point end = new Point(path.stations[path.stations.Count() - 1].X * CellSize + CellSize / 2, path.stations[path.stations.Count() - 1].Y * CellSize + CellSize / 2);

                // Narysuj liniê
                e.Graphics.DrawLine(Pen, sender, start);
                e.Graphics.DrawLine(Pen, end, receiver);
            }
        }

        public void Logg(string text)
        {
            if (loggerTextBox.InvokeRequired)
            {
                loggerTextBox.Invoke(new Action<string>(Logg), text);
                return;
            }
            loggerTextBox.AppendText(DateTime.Now.ToString("[HH:mm:ss] ") + text + Environment.NewLine);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (baseStations.Count() > 0)
            {
                bool senderInCoverage = senderDev.IsInCoverage(map);
                bool receiverInCoverage = receiverDev.IsInCoverage(map);
                if (senderInCoverage && receiverInCoverage)
                {
                    Logg($"Sender ({senderDev.X};{senderDev.Y}) connected with receiver ({receiverDev.X};{receiverDev.Y})!");
                    if (map[senderDev.X, senderDev.Y].bestStationCords[0] == map[receiverDev.X, receiverDev.Y].bestStationCords[0] && map[senderDev.X, senderDev.Y].bestStationCords[1] == map[receiverDev.X, receiverDev.Y].bestStationCords[1])
                    {
                        try
                        {
                            bestPath = new Path();
                            bestPath.stations.Add(baseStations[baseStations.FindIndex(item => item.X == map[senderDev.X, senderDev.Y].bestStationCords[0] && item.Y == map[senderDev.X, senderDev.Y].bestStationCords[1])]);
                            signalLineEnab = true;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        bestPath = PathFinder.findBestPath2(map, baseStations, senderDev, receiverDev);
                        if (bestPath != null)
                        {
                            signalLineEnab = true;
                        }
                        else
                        {
                            Logg("There are no available routes!");
                        }
                    }
                }
                else
                {
                    if (senderInCoverage)
                    {
                        Logg("Receiver is out of all base station's ranges!");
                    }
                    else if (receiverInCoverage)
                    {
                        Logg("Sender is out of all base station's ranges!");
                    }
                    else
                    {
                        Logg("Sender and Receiver are out of all base station's ranges!");
                    }
                }
            }
            pictureBoxMap.Invalidate();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Gain = trackBar1.Value;
            labelGain.Text = $"Gain (dBi) = {Gain}";
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            TransmitPower = trackBar2.Value;
            labelTransmit.Text = $"Transmit power (dBm) = {TransmitPower}";
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            Height = trackBar3.Value;
            labelHeight.Text = $"Height (m) = {Height}";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                IsDirectional = false;
            }
            else
            {
                IsDirectional = true;
            }
        }

        private void numericUpDownDirection_ValueChanged(object sender, EventArgs e)
        {
            Direction = (double)numericUpDownDirection.Value;
            labelDirection.Text = $"Direction ({Direction})";
        }

        private void numericUpDownBeamWidth_ValueChanged(object sender, EventArgs e)
        {
            BeamWidth = (double)numericUpDownBeamWidth.Value;
            labelBW.Text = $"Beam Width ({BeamWidth})";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            obstacles.Clear();
            baseStations.Clear();
            for (int gx = 0; gx < 100; gx++)
            {
                for (int gy = 0; gy < 100; gy++)
                {
                    map[gx, gy].Clear(true);
                }
            }
            pictureBoxMap.Invalidate();
        }
    }
    public enum CellType { Empty, BaseStation, Obstacle, Sender, Receiver }

    public class MapCell
    {
        public CellType Type { get; set; }
        public double signalStrength { get; set; }
        public int[] bestStationCords = new int[2] { -1, -1 };

        public void Clear(bool force)
        {
            if(force)
            {
                Type = CellType.Empty;
            }
            signalStrength = -1000;
            bestStationCords = new int[2] {-1, -1 };
        }
    }
}
