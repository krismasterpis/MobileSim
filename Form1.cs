using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Windows.Forms;

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

        private MapCell[,] map = new MapCell[GridCount, GridCount];
        private PlacementMode currentMode = PlacementMode.BaseStation;
        private List<BaseStation> baseStations = new List<BaseStation>();
        MobileDevice senderDev;
        MobileDevice receiverDev;
        private List<Rectangle> obstacles = new List<Rectangle>();
        private enum PlacementMode { BaseStation, Obstacle, Sender, Receiver }


        public Form1()
        {
            InitializeComponent();
            pictureBoxMap.Paint += pictureBoxMap_Paint;
            pictureBoxMap.MouseClick += pictureBoxMap_MouseClick;
            comboBoxMode.SelectedIndexChanged += comboBoxMode_SelectedIndexChanged;
            mapPanel.MouseWheel += mapPanel_MouseWheel;
            comboBoxMode.SelectedIndex = 0;
            InitMap();
        }

        private void InitMap()
        {
            pictureBoxMap.Size = new Size(1000, 1000);
            pictureBoxMap.Location = new Point(0, 3);
            for (int x = 0; x < GridCount; x++)
                for (int y = 0; y < GridCount; y++)
                    map[x, y] = new MapCell { Type = CellType.Empty };
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

        private void pictureBoxMap_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / (int)(CellSize * zoomFactor);
            int y = e.Y / (int)(CellSize * zoomFactor);
            if (x < 0 || y < 0 || x >= GridCount || y >= GridCount) return;

            switch (currentMode)
            {
                case PlacementMode.BaseStation:
                    map[x, y].Type = CellType.BaseStation;
                    BaseStation station = new BaseStation(x,y,10,10);
                    baseStations.Add(station);
                    break;
                case PlacementMode.Obstacle:
                    map[x, y].Type = CellType.Obstacle;
                    var obstacle = new Rectangle(x,y,CellSize,CellSize);
                    obstacles.Add(obstacle);
                    break;
                case PlacementMode.Sender:
                    map[x, y].Type = CellType.Sender;
                    senderDev = new MobileDevice(x,y);
                    break;
                case PlacementMode.Receiver:
                    map[x, y].Type = CellType.Receiver;
                    receiverDev = new MobileDevice(x, y);
                    break;
            }
            pictureBoxMap.Invalidate();
            Debug.WriteLine($"Selected X: {x}, Y:{y}");
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

        public BaseStation FindBestStation(List<BaseStation> stations, MobileDevice sender, MobileDevice receiver, List<Rectangle> obstacles)
        {
            BaseStation bestStation = null;
            double maxSignal = double.MinValue;

            foreach (var station in stations)
            {
                double obstacleLoss = 0;
                foreach (var obs in obstacles)
                {
                    if (IntersectsLine(obs, station.X, station.Y, receiver.X, receiver.Y))
                    {
                        obstacleLoss += 10; // ka¿dy budynek np. -10 dB
                    }
                    if (IntersectsLine(obs, station.X, station.Y, sender.X, sender.Y))
                    {
                        obstacleLoss += 10; // ka¿dy budynek np. -10 dB
                    }
                }

                double signal = station.CalculateSignalStrength(receiver.X, receiver.Y, obstacleLoss);

                if (signal > maxSignal)
                {
                    maxSignal = signal;
                    bestStation = station;
                }
            }

            return bestStation;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var beststation = FindBestStation(baseStations,senderDev, receiverDev, obstacles);
            Debug.WriteLine($"Best station is located on X:{beststation.X} and Y:{beststation.Y}");
        }
    }
    public enum CellType { Empty, BaseStation, Obstacle, Sender, Receiver }

    public class MapCell
    {
        public CellType Type { get; set; }
    }
}
