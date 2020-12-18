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
using System.Windows.Threading;
using CredentialManagement;

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
        List<Button> listaboton = new List<Button>();
        int steps;

        DispatcherTimer clock= new DispatcherTimer();
        bool auto = false;
        TimeSpan clockTime;

        bool initiated = false;

        double maxT;
        double maxV;
        double maxD;
        double maxP;
        double minT;
        double minV;
        double minD;
        double minP;



        Nozzle nozzlesim /*= new Nozzle(3, 800, 0.5, 0.5, 31)*/;                   //Nozzle where we would simulate
        public MainWindow()
        {
            InitializeComponent();

            CreateIndicator(400);
            PropertiesBoxSelection.Items.Add("Temperature");
            PropertiesBoxSelection.Items.Add("Velocity");
            PropertiesBoxSelection.Items.Add("Density");
            PropertiesBoxSelection.Items.Add("Pressure");
            // fill posChart of zeros
            //fillSelectedList();

            //createBrushesList();
            //// computa todos los valores especificados
            //nozzlesim.ComputeUntilPos(1401);
            //calculateMinMax();
            ////inizialitzem el step
            //steps = 0;
            //setDimensionlessCharts();
            //CreateListaButtons();

            // hacemos no visibles los labels de step y time
            labelStep.Visibility = Visibility.Hidden;
            labelTime.Visibility = Visibility.Hidden;
            // Hacemos Visibles los rectangulos transpoarentes que no nos dejan clicar a ningun sitio
            rectangleCharts.Visibility = Visibility.Visible;
            rectanglePanel.Visibility = Visibility.Visible;

            //cramos charts
            //SetChart();

            //Set the timer
            clock.Tick += new EventHandler(clock_time_Tick);
            clock.Interval = new TimeSpan(1000000); //Pongo por defecto que haga un tick cada 1 segundo
            clockTime = new TimeSpan(0);

            //Pongo los botones del chart todos en una lista

        }

		

		//INITIAL SETTINGS
		private void DefaultValuesButton_Click(object sender, RoutedEventArgs e)
        {
            DivisionsTextBox.Text = "31";
            CourantTextBox.Text = "0.5";
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (initiated == false)
            {
                if (DivisionsTextBox.Text != "" && CourantTextBox.Text != "")
                {
                    

                    CreateButton.Content = "SIMULATING...";

                    decimal c = decimal.Parse(CourantTextBox.Text.Replace('.',','));
                    double C = Convert.ToDouble(c);
                    int divisions = Convert.ToInt32(DivisionsTextBox.Text);
                    textStep.Content = "0"; labelStep.Visibility = Visibility.Visible;
                    textTime.Content = "0"; labelTime.Visibility = Visibility.Visible;
                    rectangleCharts.Visibility = Visibility.Hidden;
                    rectanglePanel.Visibility = Visibility.Hidden;
                    initiated = true;


                    nozzlesim = new Nozzle(3, 800, 0.5, C, divisions);
                    // computa todos los valores especificados
                    nozzlesim.ComputeUntilPos(1401);
                    calculateMinMax();
                    //inizialitzem el step
                    steps = 0;
                    setDimensionlessCharts();
                    CreateListaButtons();

                    // fill posChart of zeros
                    fillSelectedList();
                    // create the brushes List
                    createBrushesList();



                    PropertiesBoxSelection.SelectedIndex = 0;
                    CreateNozzle(nozzlesim, 0);
                    SetChart();
                }
                else
                    MessageBox.Show("Set some parameters first," + "\n" + "check if some of the boxes above are empty");
            }
            else
            {
                MessageBox.Show("The simulation already began");
            }
        }

        //CONTROLS SIMULATOR
        private void PropertiesBoxSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PropertiesBoxSelection.SelectedIndex != -1)
                CreateNozzle(nozzlesim, steps);             //**de moment 0 el temps, pero ja veurem quan tingui un timestep diferent
        }

        //CONTROLS CHARTS


        public double[] ampliate(double max, double min)
        {
            double[] MaxMin = new double[2] { max + (max - min) / 20, min - (max - min) / 20 };
            return MaxMin;
        }
        
        private void DimensionlessButton_Click(object sender, RoutedEventArgs e)
        {
            if (initiated == true)
            {
                double[] MaxMinD = ampliate(maxD, minD);
                double[] MaxMinV = ampliate(maxV, minV);
                double[] MaxMinT = ampliate(maxT, minT);
                double[] MaxMinP = ampliate(maxP, minP);
                if (DimensionlessButton.IsChecked == true)
                {

                    yAxisD.MaxValue = MaxMinD[0];
                    yAxisD.MinValue = MaxMinD[1];
                    yAxisD.Title = "Density [ ]";
                    chartD.HorizontalAlignment = HorizontalAlignment.Stretch;
                    yAxisV.MaxValue = MaxMinV[0];
                    yAxisV.MinValue = MaxMinV[1];
                    yAxisV.Title = "Velocity [ ]";
                    chartV.HorizontalAlignment = HorizontalAlignment.Stretch;
                    yAxisT.MaxValue = MaxMinT[0];
                    yAxisT.MinValue = MaxMinT[1];
                    yAxisT.Title = "Temperature [ ]";
                    chartV.HorizontalAlignment = HorizontalAlignment.Stretch;
                    yAxisP.MaxValue = MaxMinP[0];
                    yAxisP.MinValue = MaxMinP[1];
                    yAxisP.Title = "Pressure [ ]";
                    chartV.HorizontalAlignment = HorizontalAlignment.Stretch;
                }
                else
                {
                    double[] dimArray = nozzlesim.getDimensionalValues();
                    List<double> dimensinlesValues = new List<double> { dimArray[2], dimArray[3], dimArray[1], dimArray[4] };
                    yAxisD.MaxValue = MaxMinD[0] * dimensinlesValues[3];
                    yAxisD.MinValue = MaxMinD[1] * dimensinlesValues[3];
                    chartD.HorizontalAlignment = HorizontalAlignment.Stretch;
                    yAxisD.Title = "Density [ kg / m^3 ]";
                    yAxisV.MaxValue = MaxMinV[0] * dimensinlesValues[0];
                    yAxisV.MinValue = MaxMinV[1] * dimensinlesValues[0];
                    chartV.HorizontalAlignment = HorizontalAlignment.Stretch;
                    yAxisV.Title = "Velocity [ m / s ]";
                    yAxisT.MaxValue = MaxMinT[0] * dimensinlesValues[2];
                    yAxisT.MinValue = MaxMinT[1] * dimensinlesValues[2];
                    chartT.HorizontalAlignment = HorizontalAlignment.Stretch;
                    yAxisT.Title = "Temperature [ ºC ]";
                    yAxisP.MaxValue = MaxMinP[0] * dimensinlesValues[1] / 100;
                    yAxisP.MinValue = MaxMinP[1] * dimensinlesValues[1] / 100;
                    chartP.HorizontalAlignment = HorizontalAlignment.Stretch;
                    yAxisP.Title = "Pressure [ hPa ]";
                }
                SetChart();
            }
            else
            {

            }
        }
        public void setDimensionlessCharts()
        {
            double[] MaxMinD = ampliate(maxD, minD);
            double[] MaxMinV = ampliate(maxV, minV);
            double[] MaxMinT = ampliate(maxT, minT);
            double[] MaxMinP = ampliate(maxP, minP);

            yAxisD.MaxValue = MaxMinD[0];
            yAxisD.MinValue = MaxMinD[1];
            yAxisD.Title = "Density [ ]";
            chartD.HorizontalAlignment = HorizontalAlignment.Stretch;
            yAxisV.MaxValue = MaxMinV[0];
            yAxisV.MinValue = MaxMinV[1];
            yAxisV.Title = "Velocity [ ]";
            chartV.HorizontalAlignment = HorizontalAlignment.Stretch;
            yAxisT.MaxValue = MaxMinT[0];
            yAxisT.MinValue = MaxMinT[1];
            yAxisT.Title = "Temperature [ ]";
            chartV.HorizontalAlignment = HorizontalAlignment.Stretch;
            yAxisP.MaxValue = MaxMinP[0];
            yAxisP.MinValue = MaxMinP[1];
            yAxisP.Title = "Pressure [ ]";
            chartV.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

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
            SetChart();
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
        private void MaxDensity_Click(object sender, RoutedEventArgs e)
        {
            row0.Height = new GridLength(320);
            chartD.Height = 300;
            col0.Width = new GridLength(490.2);
            row1.Height = new GridLength(0);
            col1.Width = new GridLength(0);

            MaxDensity.Visibility = Visibility.Hidden;
            MinDensity.Visibility = Visibility.Visible;
            rectangleCharts.Visibility = Visibility.Hidden;
        }
        private void MaxVelocity_Click(object sender, RoutedEventArgs e)
        {
            row0.Height = new GridLength(320);
            chartV.Height = 300;
            col1.Width = new GridLength(490.2);
            row1.Height = new GridLength(0);
            col0.Width = new GridLength(0);
            MaxVelocity.Visibility = Visibility.Hidden;
            MinVelocity.Visibility = Visibility.Visible;
        }
        private void MaxTemperature_Click(object sender, RoutedEventArgs e)
        {
            row1.Height = new GridLength(320);
            chartT.Height = 300;
            col0.Width = new GridLength(490.2);
            row0.Height = new GridLength(0);
            col1.Width = new GridLength(0);
            MaxTemperature.Visibility = Visibility.Hidden;
            MinTemperature.Visibility = Visibility.Visible;
        }
        private void MaxPressure_Click(object sender, RoutedEventArgs e)
        {
            row1.Height = new GridLength(320);
            chartP.Height = 300;
            col1.Width = new GridLength(490.2);
            row0.Height = new GridLength(0);
            col0.Width = new GridLength(0);
            MaxPressure.Visibility = Visibility.Hidden;
            MinPressure.Visibility = Visibility.Visible;
        }
        private void MinDensity_Click(object sender, RoutedEventArgs e)
        {
            row0.Height = new GridLength(175);
            chartD.Height = 150;
            col0.Width = new GridLength(245.2);
            row1.Height = new GridLength(175);
            col1.Width = new GridLength(245.2);
            MaxDensity.Visibility = Visibility.Visible;
            MinDensity.Visibility = Visibility.Hidden;
        }
        private void MinVelocity_Click(object sender, RoutedEventArgs e)
        {
            row0.Height = new GridLength(175);
            chartV.Height = 150;
            col0.Width = new GridLength(245.2);
            row1.Height = new GridLength(175);
            col1.Width = new GridLength(245.2);
            MaxVelocity.Visibility = Visibility.Visible;
            MinVelocity.Visibility = Visibility.Hidden;
        }
        private void MinTemperature_Click(object sender, RoutedEventArgs e)
        {
            row0.Height = new GridLength(175);
            chartT.Height = 150;
            col0.Width = new GridLength(245.2);
            row1.Height = new GridLength(175);
            col1.Width = new GridLength(245.2);
            MaxTemperature.Visibility = Visibility.Visible;
            MinTemperature.Visibility = Visibility.Hidden;
        }
        private void MinPressure_Click(object sender, RoutedEventArgs e)
        {
            row0.Height = new GridLength(175);
            chartP.Height = 150;
            col0.Width = new GridLength(245.2);
            row1.Height = new GridLength(175);
            col1.Width = new GridLength(245.2);
            MaxPressure.Visibility = Visibility.Visible;
            MinPressure.Visibility = Visibility.Hidden;
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

                Button rectbutton = new Button();
                rectbutton.Background = new SolidColorBrush(color);
                rectbutton.Name = "button"+Convert.ToString(count);
                rectbutton.Height = 100 + (nozzle.GetPosition(0, count).GetArea() - 1) * 200 / 4.95;
                rectbutton.Width = width;
                rectbutton.BorderBrush= new SolidColorBrush(color);
                Canvas.SetLeft(rectbutton, 5 + width * count);
                Canvas.SetTop(rectbutton, 150 - (rectbutton.Height - 100) * 100 / 200);
                rectbutton.Click += Rectbutton_Click;
                NozzleCanvas.Children.Add(rectbutton);
                count++;
            }


        }

        private void Rectbutton_Click(object sender, RoutedEventArgs e)
        {

            Button button = (Button)sender;
            string nombre = button.Name.Split('n')[1];
            foreach(Button boton in listaboton)
            {
                if(boton.Name.Split('t')[3]==nombre)
                {
                    ClickButtonChart(boton);
                }
            }
            
        }

        private Color PrintColor(int propind, double i)
        {
            
            double max = 1;
            double min = 0;
            if (propind == 0)
            {
                max = maxT;
                min = minT;
            }
            else if (propind == 1)
            {
                max = maxV;
                min = minV;
            }
            else if (propind == 2)
            {
                max = maxD;
                min = minD;
            }
            else if (propind == 3)
            {
                max = maxP;
                min = minP;
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
                B = Convert.ToByte(Math.Round(b, 1));

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

        public void calculateMinMax()
        {
            int i = 0;
            int N = nozzlesim.GetDivisions();
            int J = nozzlesim.getTimeList().Count();
            if (nozzlesim.Getmalla() != null)
            {
                while (i < N)
                {
                    List<Position> column = nozzlesim.GetColumn(i);
                    int j = 0;
                    while (j < J)
                    {
                        if (i == 0 && j == 0)
                        {
                            minD = column[j].GetDensity(); maxD = minD;
                            minP = column[j].GetPressure(); maxP = minP;
                            minV = column[j].GetVelocity(); maxV = minV;
                            minT = column[j].GetTemperature(); maxT = minT;
                        }
                        else
                        {
                            double density = column[j].GetDensity();
                            if (maxD < density)
                                maxD = density;
                            else if (minD > density)
                                minD = density;
                            double pressure = column[j].GetPressure();
                            if (maxP < pressure)
                                maxP = pressure;
                            else if (minP > pressure)
                                minP = pressure;
                            double velocity = column[j].GetVelocity();
                            if (maxV < velocity)
                                maxV = velocity;
                            else if (minV > velocity)
                                minV = velocity;
                            double temperature = column[j].GetTemperature();
                            if (maxT < temperature)
                                maxT = temperature;
                            else if (minT > temperature)
                                minT = temperature;

                        }
                        j++;
                    }
                    i++;
                }
            }
            else
            {
                maxD = new double();
                minD = new double();
                maxV = new double();
                minV = new double();
                maxP = new double();
                minP = new double();
                maxT = new double();
                minT = new double();
            }
        }

        //FUNCTIONS CHARTS

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
        
        public void SetChart()
        {

            double secondsperstep = Math.Round(Convert.ToDouble(clockTime.Minutes * 60 + clockTime.Seconds + clockTime.Milliseconds / 1000));
            double division = secondsperstep / 5;
            char[] disv_char = (division.ToString()).ToCharArray();

            if (nozzlesim.Getmalla() != null)
            {
                if ((auto == false) || (clock.Interval > new TimeSpan(30000000)) || (disv_char.Count() != 16 && clock.Interval < new TimeSpan(30000000)))
                {

                    List<List<double>> listV = new List<List<double>>();
                    List<List<double>> listP = new List<List<double>>();
                    List<List<double>> listT = new List<List<double>>();
                    List<List<double>> listD = new List<List<double>>();
                    int stepsChart; // suitable, between 1% to 2%, (for 500 samples between 5 and 10)

                    if (this.steps > 1000)
                        stepsChart = 40;
                    else if (this.steps > 500)
                        stepsChart = 20;
                    else if (this.steps > 250)
                        stepsChart = 10;
                    else if (this.steps > 100)
                        stepsChart = 5;
                    else if (this.steps > 50)
                        stepsChart = 2;
                    else if (this.steps > 25)
                        stepsChart = 1;
                    else
                        stepsChart = 0;
                    int i = 0;
                    int finStep = steps;
                    List<Brush> ListBrush = new List<Brush>();
                    int posBrushes = 0;
                    List<double> dimensinlesValues;
                    if (DimensionlessButton.IsChecked == false)
                    {
                        // T V P D
                        double[] dimArray = nozzlesim.getDimensionalValues();
                        dimensinlesValues = new List<double> { dimArray[2], dimArray[3] / 100, dimArray[1], dimArray[4] };
                    }
                    else
                    {
                        dimensinlesValues = new List<double> { 1, 1, 1, 1 };
                    }
                    foreach (int pos in posChart)
                    {
                        if (pos == 1)
                        {
                            listV.Add(nozzlesim.GetColumnPar(i, "V", stepsChart, dimensinlesValues, finStep));
                            listP.Add(nozzlesim.GetColumnPar(i, "P", stepsChart, dimensinlesValues, finStep));
                            listT.Add(nozzlesim.GetColumnPar(i, "T", stepsChart, dimensinlesValues, finStep));
                            listD.Add(nozzlesim.GetColumnPar(i, "D", stepsChart, dimensinlesValues, finStep));
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
                    List<double> timeList = nozzlesim.getTimeList(stepsChart, finStep);
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
            }
            else
            {
                chartV.Series = new SeriesCollection();
                chartP.Series = new SeriesCollection();
                chartT.Series = new SeriesCollection();
                chartD.Series = new SeriesCollection();
            }
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
            double sep;
            //if (steps = )
            //    sep = 1;
            //else
            //    sep = Math.Round(steps / 2.0 ,0);
            if (steps == 0)
                sep = 1;
            else
                sep = steps;


            xAxis.Labels = times;
            xAxis.Separator.Step = sep;

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


        public void DataGrid1_Loaded()
        {
            List<Position> una_lista = nozzlesim.position_initial_conditions;
            DataGrid1.ItemsSource = una_lista;
        }



        // Controls of the STEP AUTO PAUSE CLOCK...


        private void NextStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (initiated == true)
            {
                steps = steps + 1;
                textStep.Content = steps.ToString();
                textTime.Content = nozzlesim.getTimeList()[steps].ToString()+" sec";
                CreateNozzle(nozzlesim, steps);
                SetChart();
            }
            else
                MessageBox.Show("The simulate has to be initiated first," + "\n" + "create the nozzle to start!!");
        }


        private void clock_time_Tick(object sender, EventArgs e)
        {
            steps = steps + 1;
            clockTime = clockTime+clock.Interval;
            textStep.Content = steps.ToString();
            textTime.Content = nozzlesim.getTimeList()[steps].ToString() + " sec";
            CreateNozzle(nozzlesim, steps);
            SetChart();
        }

        private void AutoButton_Click(object sender, RoutedEventArgs e)
		{
            Auto();
        }

        public void Auto()
        {
            if (initiated == true)
            {
                if (auto == false)
                {
                    clock.Start();
                    NextStepButton.IsEnabled = false;
                    auto = true;
                    Color colorset = Color.FromRgb(153, 144, 144);
                    Brush colorBrush = new SolidColorBrush(colorset);
                    AutoButton.Background = colorBrush;
                    AutoButton.Content = "PAUSE";
                }
                else
                {
                    clock.Stop();
                    NextStepButton.IsEnabled = true;
                    auto = false;
                    Color colorset = Color.FromRgb(232, 232, 232);
                    Brush colorBrush = new SolidColorBrush(colorset);
                    AutoButton.Background = colorBrush;
                    AutoButton.Content = "AUTO";
                }
            }
            else
                MessageBox.Show("The simulate has to be initiated first," + "\n" + "create the nozzle to start!!");
        }
        private void PauseButton_Click(object sender, RoutedEventArgs e)
		{
            clock.Stop();
            NextStepButton.IsEnabled = true;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult respuesta = MessageBox.Show(
                "Se va a reiniciar el simulador, se perderán los cambios","Warning",MessageBoxButton.OKCancel);

            switch (respuesta)
            {
                case MessageBoxResult.OK:
                    Restart();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
            
        }
        public void Restart()
        {
            Color colorset = Color.FromRgb(232, 232, 232);
            Brush colorBrush = new SolidColorBrush(colorset);
            clock.Interval = new TimeSpan(1000000);
            if (auto == false)
            {

            }
            else
            {
                clock.Stop();
                NextStepButton.IsEnabled = true;
                auto = false;
                AutoButton.Background = colorBrush;
                AutoButton.Content = "AUTO";
            }
            clockTime = new TimeSpan(0);
            nozzlesim = new Nozzle();
            calculateMinMax();
            fillSelectedList();
            steps = 0;

            labelStep.Visibility = Visibility.Hidden; textStep.Content = "";
            labelTime.Visibility = Visibility.Hidden; textTime.Content = "";
            // Hacemos Visibles los rectangulos transpoarentes que no nos dejan clicar a ningun sitio
            rectangleCharts.Visibility = Visibility.Visible;
            rectanglePanel.Visibility = Visibility.Visible;
            List<Brush> ListBrush = new List<Brush>();
            foreach (int pos in posChart)
            {
                ListBrush.Add(Brushes.Transparent);
            }

            createRecColors(ListBrush);

            buttChart0.Background = colorBrush;
            buttChart1.Background = colorBrush;
            buttChart2.Background = colorBrush;
            buttChart3.Background = colorBrush;
            buttChart4.Background = colorBrush;
            buttChart5.Background = colorBrush;
            buttChart6.Background = colorBrush;
            buttChart7.Background = colorBrush;
            buttChart8.Background = colorBrush;
            buttChart9.Background = colorBrush;

            buttChart10.Background = colorBrush;
            buttChart11.Background = colorBrush;
            buttChart12.Background = colorBrush;
            buttChart13.Background = colorBrush;
            buttChart14.Background = colorBrush;
            buttChart15.Background = colorBrush;
            buttChart16.Background = colorBrush;
            buttChart17.Background = colorBrush;
            buttChart18.Background = colorBrush;
            buttChart19.Background = colorBrush;

            buttChart20.Background = colorBrush;
            buttChart21.Background = colorBrush;
            buttChart22.Background = colorBrush;
            buttChart23.Background = colorBrush;
            buttChart24.Background = colorBrush;
            buttChart25.Background = colorBrush;
            buttChart26.Background = colorBrush;
            buttChart27.Background = colorBrush;
            buttChart28.Background = colorBrush;
            buttChart29.Background = colorBrush;
            buttChart30.Background = colorBrush;
            initiated = false;
            CreateButton.Content = "CREATE";
            DivisionsTextBox.Text = "";
            CourantTextBox.Text = "";
            SetChart();
            if (DimensionlessButton.IsChecked == true)
            { }
            else
            {
                DimensionlessButton.IsChecked = true;
                setDimensionlessCharts();
            }

            row0.Height = new GridLength(175);
            chartD.Height = 150;
            chartV.Height = 150;
            chartT.Height = 150;
            chartP.Height = 150;
            col0.Width = new GridLength(245.2);
            row1.Height = new GridLength(175);
            col1.Width = new GridLength(245.2);
            MaxDensity.Visibility = Visibility.Visible;
            MinDensity.Visibility = Visibility.Hidden;

            MaxVelocity.Visibility = Visibility.Visible;
            MinVelocity.Visibility = Visibility.Hidden;

            MaxTemperature.Visibility = Visibility.Visible;
            MinTemperature.Visibility = Visibility.Hidden;

            MaxPressure.Visibility = Visibility.Visible;
            MinPressure.Visibility = Visibility.Hidden;

            NozzleCanvas.Children.Clear();
            PropertiesBoxSelection.SelectedIndex = -1;
            nozzlesim = new Nozzle(3, 800, 0.5, 0.5, 31);
            nozzlesim.ComputeUntilPos(1401);
            calculateMinMax();

        }
        private void CreateListaButtons()
        {
            listaboton.Add(buttChart0);
            listaboton.Add(buttChart1);
            listaboton.Add(buttChart2);
            listaboton.Add(buttChart3);
            listaboton.Add(buttChart4);
            listaboton.Add(buttChart5);
            listaboton.Add(buttChart6);
            listaboton.Add(buttChart7);
            listaboton.Add(buttChart8);
            listaboton.Add(buttChart9);
            listaboton.Add(buttChart10);
            listaboton.Add(buttChart11);
            listaboton.Add(buttChart12);
            listaboton.Add(buttChart13);
            listaboton.Add(buttChart14);
            listaboton.Add(buttChart15);
            listaboton.Add(buttChart16);
            listaboton.Add(buttChart17);
            listaboton.Add(buttChart18);
            listaboton.Add(buttChart19);
            listaboton.Add(buttChart20);
            listaboton.Add(buttChart21);
            listaboton.Add(buttChart22);
            listaboton.Add(buttChart23);
            listaboton.Add(buttChart24);
            listaboton.Add(buttChart25);
            listaboton.Add(buttChart26);
            listaboton.Add(buttChart27);
            listaboton.Add(buttChart28);
            listaboton.Add(buttChart29);
            listaboton.Add(buttChart30);

        }

    }
}





