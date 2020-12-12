using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using Brushes = System.Windows.Media.Brushes;

using NozzleLib;

namespace SimuladorNozzle
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<int> posChart = new List<int>();                               // Select positions to show on the chart
        List<List<Rectangle>> recListChart= new List<List<Rectangle>>();
        List<Brush> brushesList;

        Nozzle nozzlesim = new Nozzle(3, 800, 0.5, 0.5, 31);                   //Nozzle where we would simulate
        public MainWindow()
        {
            InitializeComponent();
            CreateIndicator(400);
            PropertiesBoxSelection.Items.Add("Temperature");
            PropertiesBoxSelection.Items.Add("Velocity");
            PropertiesBoxSelection.Items.Add("Density");
            PropertiesBoxSelection.Items.Add("Pressure");
            // fill posChart of zeros
            fillSelectedList();

            createBrushesList();
            // computa todos los valores especificados
            nozzlesim.ComputeUntilPos(500);
            SetChartV();
        }

        //INITIAL SETTINGS
        private void DefaultValuesButton_Click(object sender, RoutedEventArgs e)
        {
            DivisionsTextBox.Text = "31";
            CourantTextBox.Text = "0.5";
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            PropertiesBoxSelection.SelectedIndex = 0;

            double C = Convert.ToDouble(CourantTextBox.Text);
            int divisions = Convert.ToInt32(DivisionsTextBox.Text);
            nozzlesim = new Nozzle(3, 800, 0.5, C, divisions);
            CreateNozzle(nozzlesim, 0);

        }

        //CONTROLS SIMULATOR
        private void PropertiesBoxSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateNozzle(nozzlesim, 0);             //**de moment 0 el temps, pero ja veurem quan tingui un timestep diferent
        }

        //CONTROLS CHARTS
        public void ClickButtonChart(Button button)
        {
            int selectedPos = 0;
            foreach (int position in posChart)
            {
                if (position == 1)
                    selectedPos++;
            }
            bool maximum = true;
            if (selectedPos < 10)
            {
                maximum = false;
            }
            int pos;
            if (button.Content.ToString().Split(' ')[2] == "1")
                pos = 10;
            else if (button.Content.ToString().Split(' ')[2] == "2")
                pos = 20;
            else if (button.Content.ToString().Split(' ')[2] == "3")
                pos = 30;
            else
                pos = Convert.ToInt32(Convert.ToDouble(button.Content.ToString().Split(' ')[2]));
            int showed = posChart[pos];
            if (showed == 0)   // pint of gray
            {
                if (maximum == true)
                {
                    MessageBox.Show("The maximum number of plots enabled to show are 10," + "\n" + "if some position are important delete another first");
                }
                else
                {
                    posChart[pos] = 1;
                    Color colorset = Color.FromRgb(153, 144, 144);
                    Brush colorBrush = new SolidColorBrush(colorset);
                    button.Background = colorBrush;
                }
            }
            else
            {
                posChart[pos] = 0;
                Color colorset = Color.FromRgb(232, 232, 232);
                Brush colorBrush = new SolidColorBrush(colorset);
                button.Background = colorBrush;
            }
            SetChartV();
            
            

        }
        private void buttChart0_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart0;
            ClickButtonChart(button);
        }
        
        private void buttChart1_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart1;
            ClickButtonChart(button);
        }
        private void buttChart2_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart2;
            ClickButtonChart(button);
        }
        private void buttChart3_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart3;
            ClickButtonChart(button);
        }
        private void buttChart4_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart4;
            ClickButtonChart(button);
        }
        private void buttChart5_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart5;
            ClickButtonChart(button);
        }
        private void buttChart6_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart6;
            ClickButtonChart(button);
        }
        private void buttChart7_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart7;
            ClickButtonChart(button);
        }
        private void buttChart8_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart8;
            ClickButtonChart(button);
        }
        private void buttChart9_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart9;
            ClickButtonChart(button);
        }
        private void buttChart10_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart10;
            ClickButtonChart(button);
        }
        private void buttChart11_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart11;
            ClickButtonChart(button);
        }
        private void buttChart12_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart12;
            ClickButtonChart(button);
        }
        private void buttChart13_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart13;
            ClickButtonChart(button);
        }
        private void buttChart14_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart14;
            ClickButtonChart(button);
        }
        private void buttChart15_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart15;
            ClickButtonChart(button);
        }
        private void buttChart16_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart16;
            ClickButtonChart(button);
        }
        private void buttChart17_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart17;
            ClickButtonChart(button);
        }
        private void buttChart18_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart18;
            ClickButtonChart(button);
        }
        private void buttChart19_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart19;
            ClickButtonChart(button);
        }
        private void buttChart20_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart20;
            ClickButtonChart(button);
        }
        private void buttChart21_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart21;
            ClickButtonChart(button);
        }
        private void buttChart22_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart22;
            ClickButtonChart(button);
        }
        private void buttChart23_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart23;
            ClickButtonChart(button);
        }
        private void buttChart24_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart24;
            ClickButtonChart(button);
        }
        private void buttChart25_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart25;
            ClickButtonChart(button);
        }
        private void buttChart26_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart26;
            ClickButtonChart(button);
        }
        private void buttChart27_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart27;
            ClickButtonChart(button);
        }
        private void buttChart28_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart28;
            ClickButtonChart(button);
        }
        private void buttChart29_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart29;
            ClickButtonChart(button);
        }
        private void buttChart30_Click(object sender, RoutedEventArgs e)
        {
            Button button = buttChart30;
            ClickButtonChart(button);
        }

        //FUNCTIONS SIMULATOR
        private void CreateIndicator(int filas)
        {
            int change = Convert.ToInt32(filas / 20);

            int count = 0;
            while (count < filas)
            {
                Indicator.RowDefinitions.Add(new RowDefinition());
                count++;
            }

            int i = 0;
            foreach (RowDefinition row in Indicator.RowDefinitions)
            {
                StackPanel panel = new StackPanel();
                byte A;
                byte R;
                byte G;
                byte B;
                if (i < change)
                {
                    A = 255;
                    double r = 0 + i * 128 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    G = 0;
                    double b = 0 + i * 32 / change;
                    B = Convert.ToByte(Math.Round(b, 0));

                }
                else if (i < 2 * change)
                {
                    int j = i - change;
                    A = 255;
                    R = 128;
                    double g = 0 + j * 22 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 32 + j * 24 / change;
                    B = Convert.ToByte(Math.Round(b, 0));

                }
                else if (i < 3 * change)
                {
                    int j = i - 2 * change;
                    A = 255;
                    double r = 128 + j * 36 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 22;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 56 + j * 16 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 4 * change)
                {
                    int j = i - 3 * change;
                    A = 255;
                    double r = 164 + j * 87 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 22 + j * 9 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 72 - j * 42 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 5 * change)
                {
                    int j = i - 4 * change;
                    A = 255;
                    double r = 251;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 31 + j * 67 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 30;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 6 * change)
                {
                    int j = i - 5 * change;
                    A = 255;
                    double r = 251 - j * 13 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 98 + j * 21 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 30 + j * 38 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 7 * change)
                {
                    int j = i - 6 * change;
                    A = 255;
                    double r = 238 + j * 17 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 119 + j * 130 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 68 - j * 22 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 8 * change)
                {
                    int j = i - 7 * change;
                    A = 255;
                    double r = 255 - j * 63 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 249 + j * 6 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 46 - j * 46 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 9 * change)
                {
                    int j = i - 8 * change;
                    A = 255;
                    double r = 192 - j * 192 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 255 - j * 71 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 0;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 10 * change)
                {
                    int j = i - 9 * change;
                    A = 255;
                    double r = 0 + j * 25 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 184 + j * 35 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 0 + j * 89 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 11 * change)
                {
                    int j = i - 10 * change;
                    A = 255;
                    double r = 25 + j * 26 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 219 + j * 36 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 89 + j * 89 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 12 * change)
                {
                    int j = i - 11 * change;
                    A = 255;
                    double r = 51 - j * 51 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 255 - j * 45 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 178 + j * 32 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 13 * change)
                {
                    int j = i - 12 * change;
                    A = 255;
                    double r = 0 + j * 15 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 210 - j * 83 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 210 - j * 83 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 14 * change)
                {
                    int j = i - 13 * change;
                    A = 255;
                    double r = 15 + j * 6 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 127 - j * 31 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 127 + j * 62 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 15 * change)
                {
                    int j = i - 14 * change;
                    A = 255;
                    double r = 21 - j * 10 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 96 - j * 38 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 189 - j * 31 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 16 * change)
                {
                    int j = i - 15 * change;
                    A = 255;
                    double r = 11 - j * 11 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 58 - j * 38 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 158 - j * 31 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 17 * change)
                {
                    int j = i - 16 * change;
                    A = 255;
                    double r = 0 + j * 65 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 20 + j * 8 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 127 + j * 60 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 18 * change)
                {
                    int j = i - 17 * change;
                    A = 255;
                    double r = 65 + j * 84 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 28 + j * 15 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 187 - j * 33 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else if (i < 19 * change)
                {
                    int j = i - 18 * change;
                    A = 255;
                    double r = 149 + j * 52 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 43 + j * 117 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 154 + j * 66 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                else
                {
                    int j = i - 19 * change;
                    A = 255;
                    double r = 201 + j * 18 / change;
                    R = Convert.ToByte(Math.Round(r, 0));
                    double g = 160 + j * 52 / change;
                    G = Convert.ToByte(Math.Round(g, 0));
                    double b = 220 + j * 35 / change;
                    B = Convert.ToByte(Math.Round(b, 0));
                }
                Color colorset = Color.FromArgb(A, R, G, B);
                panel.Background = new SolidColorBrush(colorset);
                Grid.SetRow(panel, i);
                Indicator.Children.Add(panel);
                i++;
            }
        }

        private void CreateNozzle(Nozzle nozzle, int t)
        {
            NozzleCanvas.Children.RemoveRange(0, NozzleCanvas.Children.Count);
            double width = 445 / (nozzle.GetDivisions());
            int count = 0;
            while (count < nozzle.GetDivisions())
            {
                int propind = PropertiesBoxSelection.SelectedIndex;
                double value = 0;
                if (propind == 0)
                {
                    value = nozzle.GetPosition(t, count).GetTemperature();
                }
                else if (propind == 1)
                {
                    value = nozzle.GetPosition(t, count).GetVelocity();
                }
                else if (propind == 2)
                {
                    value = nozzle.GetPosition(t, count).GetDensity();
                }
                else if (propind == 3)
                {
                    value = nozzle.GetPosition(t, count).GetPressure();
                }
                Color color = PrintColor(propind, value);
                Rectangle rect = new Rectangle();
                rect.Stroke = new SolidColorBrush(color);
                rect.Fill = new SolidColorBrush(color);
                rect.StrokeThickness = 2;
                rect.Width = width;
                rect.Height = 100 + (nozzle.GetPosition(0, count).GetArea() - 1) * 200 / 4.95;
                Canvas.SetLeft(rect, 5 + width * count);
                Canvas.SetTop(rect, 150 - (rect.Height - 100) * 100 / 200);
                NozzleCanvas.Children.Add(rect);
                count++;
            }

        }

        private Color PrintColor(int propind, double i)
        {
            double max = 1;
            double min = 0;
            if (propind == 0)
            {
                max = 1;
                min = 0.3;
            }
            else if (propind == 1)
            {
                max = 2;
                min = 0;
            }
            else if (propind == 2)
            {
                max = 1;
                min = 0.04;
            }
            else if (propind == 3)
            {
                max = 1;
                min = 0.005;
            }
            byte A = 255;
            byte R;
            byte G;
            byte B;

            i = max - i;
            double change = (max - min) / 20;

            if (i < change)
            {
                A = 255;
                double r = 0 + i * 128 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                G = 0;
                double b = 0 + i * 32 / change;
                B = Convert.ToByte(Math.Round(b, 0));

            }
            else if (i < 2 * change)
            {
                double j = i - change;
                A = 255;
                R = 128;
                double g = 0 + j * 22 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 32 + j * 24 / change;
                B = Convert.ToByte(Math.Round(b, 0));

            }
            else if (i < 3 * change)
            {
                double j = i - 2 * change;
                A = 255;
                double r = 128 + j * 36 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 22;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 56 + j * 16 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 4 * change)
            {
                double j = i - 3 * change;
                A = 255;
                double r = 164 + j * 87 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 22 + j * 9 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 72 - j * 42 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 5 * change)
            {
                double j = i - 4 * change;
                A = 255;
                double r = 251;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 31 + j * 67 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 30;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 6 * change)
            {
                double j = i - 5 * change;
                A = 255;
                double r = 251 - j * 13 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 98 + j * 21 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 30 + j * 38 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 7 * change)
            {
                double j = i - 6 * change;
                A = 255;
                double r = 238 + j * 17 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 119 + j * 130 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 68 - j * 22 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 8 * change)
            {
                double j = i - 7 * change;
                A = 255;
                double r = 255 - j * 63 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 249 + j * 6 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 46 - j * 46 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 9 * change)
            {
                double j = i - 8 * change;
                A = 255;
                double r = 192 - j * 192 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 255 - j * 71 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 0;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 10 * change)
            {
                double j = i - 9 * change;
                A = 255;
                double r = 0 + j * 25 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 184 + j * 35 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 0 + j * 89 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 11 * change)
            {
                double j = i - 10 * change;
                A = 255;
                double r = 25 + j * 26 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 219 + j * 36 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 89 + j * 89 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 12 * change)
            {
                double j = i - 11 * change;
                A = 255;
                double r = 51 - j * 51 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 255 - j * 45 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 178 + j * 32 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 13 * change)
            {
                double j = i - 12 * change;
                A = 255;
                double r = 0 + j * 15 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 210 - j * 83 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 210 - j * 83 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 14 * change)
            {
                double j = i - 13 * change;
                A = 255;
                double r = 15 + j * 6 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 127 - j * 31 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 127 + j * 62 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 15 * change)
            {
                double j = i - 14 * change;
                A = 255;
                double r = 21 - j * 10 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 96 - j * 38 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 189 - j * 31 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 16 * change)
            {
                double j = i - 15 * change;
                A = 255;
                double r = 11 - j * 11 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 58 - j * 38 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 158 - j * 31 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 17 * change)
            {
                double j = i - 16 * change;
                A = 255;
                double r = 0 + j * 65 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 20 + j * 8 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 127 + j * 60 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 18 * change)
            {
                double j = i - 17 * change;
                A = 255;
                double r = 65 + j * 84 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 28 + j * 15 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 187 - j * 33 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else if (i < 19 * change)
            {
                double j = i - 18 * change;
                A = 255;
                double r = 149 + j * 52 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 43 + j * 117 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 154 + j * 66 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            else
            {
                double j = i - 19 * change;
                A = 255;
                double r = 201 + j * 18 / change;
                R = Convert.ToByte(Math.Round(r, 0));
                double g = 160 + j * 52 / change;
                G = Convert.ToByte(Math.Round(g, 0));
                double b = 220 + j * 35 / change;
                B = Convert.ToByte(Math.Round(b, 0));
            }
            Color colortopaint = Color.FromArgb(A, R, G, B);
            return colortopaint;

        }

        //FUNCTIONS CHARTS

        //public void setCharts()
        //{
        //    Values1 = new ChartValues<double> { 3, 4, 6, 3, 2, 6 };
        //    Values2 = new ChartValues<double> { 5, 3, 5, 7, 3, 9 };
        //    DataContext = this;
        //}



        public void createBrushesList()
        {
            brushesList = new List<Brush>();
            brushesList.Add(Brushes.Black);
            brushesList.Add(Brushes.Yellow);
            brushesList.Add(Brushes.Red);
            brushesList.Add(Brushes.Green);
            brushesList.Add(Brushes.Purple);
            brushesList.Add(Brushes.Brown);
            brushesList.Add(Brushes.Orange);
            brushesList.Add(Brushes.Aqua);
            brushesList.Add(Brushes.DarkBlue);
            brushesList.Add(Brushes.Pink);



        }
        public void createRecColors(List<Brush> brushes)
        {
            int i = 0;
           
            
            gridRecChart.Children.Clear();
            while (i < gridRecChart.RowDefinitions.Count)
            {
                List<Rectangle> recFila = new List<Rectangle>();
                int j = 0;
                while (j < gridRecChart.ColumnDefinitions.Count)
                {
                    if (i == 3 && j == 1)
                        break;
                    
                    Rectangle rec = new Rectangle();
                    rec.Name = "rec" + (i * 10 + j).ToString();
                    rec.Height = 4;
                    //Color colorset = Color.FromArgb(0, 0, 255, 255);
                    //Brush colorBrush = new SolidColorBrush(colorset);
                    rec.Margin =new Thickness(1, 4, 1, 2);
                    rec.VerticalAlignment = VerticalAlignment.Bottom;
                    rec.Fill = brushes[i * 10 + j];
                    Grid.SetRow(rec, i);
                    Grid.SetColumn(rec, j);
                    gridRecChart.Children.Add(rec);
                   
                    j++;
                    
                }
                recListChart.Add(recFila);
                i++;
            }
        }
        public void SetChartV()
        {
            int filaCount = nozzlesim.GetRow(0).Count;
            List<List<double>> listV = new List<List<double>>();
            List<List<double>> listP = new List<List<double>>();
            List<List<double>> listT = new List<List<double>>();
            List<List<double>> listD = new List<List<double>>();
            int i = 0;
            int steps = 10; // suitable, between 1% to 2%, (for 500 samples between 5 and 10)
            List<Brush> ListBrush = new List<Brush>();
            int posBrushes = 0;
            foreach (int pos in posChart)
            {
                if (pos == 1)
                {
                    listV.Add(nozzlesim.GetColumnPar(i, "V", steps));
                    listP.Add(nozzlesim.GetColumnPar(i, "P", steps));
                    listT.Add(nozzlesim.GetColumnPar(i, "T", steps));
                    listD.Add(nozzlesim.GetColumnPar(i, "D", steps));
                    ListBrush.Add(brushesList[posBrushes]);
                    posBrushes++;
                }
                else
                {
                    listV.Add(new List<double>());
                    listP.Add(new List<double>());
                    listT.Add(new List<double>());
                    listD.Add(new List<double>());
                    ListBrush.Add(Brushes.Transparent);
                }
                i++;
            }

            createRecColors(ListBrush);

            // create the array of times
            List<double> timeList = nozzlesim.getTimeList(steps);
            var times = new string[timeList.Count];
            i = 0;
            foreach (double time in timeList)
            {
                times[i] = (Math.Round(time, 3)).ToString();
                i++;
            }


            createChart(chartV, listV, ListBrush, xAxisV, times);
            createChart(chartP, listP, ListBrush, xAxisP, times);
            createChart(chartT, listT, ListBrush, xAxisT, times);
            createChart(chartD, listD, ListBrush, xAxisD, times);

        }

        public void createChart(CartesianChart chart, List<List<double>> listV, List<Brush> ListBrush, Axis xAxis, string[] times)
        {
            chart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "x = 0",
                    //Values =  new ChartValues<double>(listV[0]),
                    Values =  new ChartValues<double>(listV[0]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[0],
                },
                new LineSeries
                {
                    Title = "x = 0.1",
                    Values =  new ChartValues<double>(listV[1]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[1],
                },
                new LineSeries
                {
                    Title = "x = 0.2",
                    Values =  new ChartValues<double>(listV[2]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[2],
                },
                new LineSeries
                {
                    Title = "x = 0.3",
                    Values =  new ChartValues<double>(listV[3]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[3],
                },
                new LineSeries
                {
                    Title = "x = 0.4",
                    Values =  new ChartValues<double>(listV[4]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[4],

                },
                new LineSeries
                {
                    Title = "x = 0.5",
                    Values =  new ChartValues<double>(listV[5]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[5],

                },
                new LineSeries
                {
                    Title = "x = 0.6",
                    Values =  new ChartValues<double>(listV[6]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[6],
                },
                new LineSeries
                {
                    Title = "x = 0.7",
                    Values =  new ChartValues<double>(listV[7]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[7],
                },
                new LineSeries
                {
                    Title = "x = 0.8",
                    Values =  new ChartValues<double>(listV[8]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[8],
                },
                new LineSeries
                {
                    Title = "x = 0.9",
                    Values =  new ChartValues<double>(listV[9]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[9],
                },
                new LineSeries
                {
                    Title = "x = 1",
                    Values =  new ChartValues<double>(listV[10]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[10],
                },
                new LineSeries
                {
                    Title = "x = 1.1",
                    Values =  new ChartValues<double>(listV[11]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[11],
                },
                new LineSeries
                {
                    Title = "x = 1.2",
                    Values =  new ChartValues<double>(listV[12]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[12],
                },
                new LineSeries
                {
                    Title = "x = 1.3",
                    Values =  new ChartValues<double>(listV[13]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[13],
                },
                new LineSeries
                {
                    Title = "x = 1.4",
                    Values =  new ChartValues<double>(listV[14]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[14],
                },
                new LineSeries
                {
                    Title = "x = 1.5",
                    Values =  new ChartValues<double>(listV[15]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[15],
                },
                new LineSeries
                {
                    Title = "x = 1.6",
                    Values =  new ChartValues<double>(listV[16]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[16],
                },
                new LineSeries
                {
                    Title = "x = 1.7",
                    Values =  new ChartValues<double>(listV[17]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[17],
                },
                new LineSeries
                {
                    Title = "x = 1.8",
                    Values =  new ChartValues<double>(listV[18]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[18],
                },
                new LineSeries
                {
                    Title = "x = 1.9",
                    Values =  new ChartValues<double>(listV[19]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[19],
                },
                new LineSeries
                {
                    Title = "x = 2.0",
                    Values =  new ChartValues<double>(listV[20]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[20],
                },
                new LineSeries
                {
                    Title = "x = 2.1",
                    Values =  new ChartValues<double>(listV[21]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[21],
                },
                new LineSeries
                {
                    Title = "x = 2.2",
                    Values =  new ChartValues<double>(listV[22]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[22],
                },
                new LineSeries
                {
                    Title = "x = 2.3",
                    Values =  new ChartValues<double>(listV[23]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[23],
                },
                new LineSeries
                {
                    Title = "x = 2.4",
                    Values =  new ChartValues<double>(listV[24]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[24],
                },
                new LineSeries
                {
                    Title = "x = 2.5",
                    Values =  new ChartValues<double>(listV[25]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[25],
                },
                new LineSeries
                {
                    Title = "x = 2.6",
                    Values =  new ChartValues<double>(listV[26]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[26],
                },
                new LineSeries
                {
                    Title = "x = 2.7",
                    Values =  new ChartValues<double>(listV[27]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[27],
                },
                new LineSeries
                {
                    Title = "x = 2.8",
                    Values =  new ChartValues<double>(listV[28]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[28],
                },
                new LineSeries
                {
                    Title = "x = 2.9",
                    Values =  new ChartValues<double>(listV[29]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[29],
                },
                new LineSeries
                {
                    Title = "x = 3",
                    Values =  new ChartValues<double>(listV[30]),
                    PointGeometry = null,
                    Fill=Brushes.Transparent,
                    Stroke = ListBrush[30],
                },
            };

            xAxis.Labels = times;
            xAxis.Separator.Step = times.Count() / 4;

            DataContext = this;
        }

        public void fillSelectedList()
        {
            int i = 0;
            while (i < nozzlesim.getN())
            {
                posChart.Add(0);
                i++;
            }
        }


    }
}





