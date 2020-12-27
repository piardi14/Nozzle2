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
        List<List<Rectangle>> recListChart = new List<List<Rectangle>>();
        List<Brush> brushesList; // Select positions to show on the chart
        List<int> brushesPos;
        List<Button> listaboton;
        int steps;

        DispatcherTimer clock = new DispatcherTimer();
        bool auto = false;
        TimeSpan clockTime;

        TimeSpan lastChartUpdate = new TimeSpan();
        bool plotChanged = false;
        TimeSpan lastLabeTick;

        bool initiated = false;
        bool unitsShowTable = false;
        bool fixTable = false;
        Button fixedButton;

        bool advanced = false;

        double maxT;
        double maxV;
        double maxD;
        double maxP;
        double minT;
        double minV;
        double minD;
        double minP;



        Nozzle nozzlesim;                   //Nozzle where we would simulate
        public MainWindow()
        {
            InitializeComponent();

            // mwake non enablen the button crate 
            CreateButton.IsEnabled = false;

            CreateIndicator(400);
            Indicator.Visibility = Visibility.Hidden;
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
            rayahorizontal.Visibility = Visibility.Hidden;
            panelShow.Visibility = Visibility.Hidden;

            RectanglesInitial(Visibility.Visible);
            panelAdvanced2.Visibility = Visibility.Hidden;
            buttonAdvanced.Visibility = Visibility.Visible;
            buttonNewAdvanced.Visibility = Visibility.Hidden;

            xAxisD.MaxValue = 0.1;
            xAxisT.MaxValue = 0.1;
            xAxisV.MaxValue = 0.1;
            xAxisP.MaxValue = 0.1;


            //Set the timer
            clock.Tick += new EventHandler(clock_time_Tick);
            clock.Interval = new TimeSpan(2000000); //Pongo por defecto que haga un tick cada 1 segundo
            clockTime = new TimeSpan(0);


        }



        //INITIAL SETTINGS
        private void DefaultValuesButton_Click(object sender, RoutedEventArgs e)
        {
            DefaultValues();
        }
        public void DefaultValues()
        {
            DivisionsTextBox.Text = "31";
            CourantTextBox.Text = "0.5";
            if (advanced == false)
                CreateButton.IsEnabled = true;
        }
        public void DefaultValuesClear()
        {
            DivisionsTextBox.Text = "";
            CourantTextBox.Text = "";
            CreateButton.IsEnabled = false;
        }
        private void DivisionsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int div = Convert.ToInt32(DivisionsTextBox.Text.ToString());
                decimal precourant = decimal.Parse(CourantTextBox.Text.Replace('.',','));
                double courant = Convert.ToDouble(precourant);

               

                if ((div == 11 || div == 21 || div == 31 || div == 41 || div == 51 || div == 61)&&courant<1&&courant>0)
                {
                    alertDivisionsLabel.Visibility = Visibility.Hidden;
                    try
                    {
                        decimal cou = Convert.ToDecimal(CourantTextBox.Text);
                        if (CourantTextBox.Text != "" && advanced == false)
                            CreateButton.IsEnabled = true;
                    }
                    catch (FormatException)
                    {
                        CreateButton.IsEnabled = false;
                    }
                    catch (NullReferenceException)
                    {
                        CreateButton.IsEnabled = false;
                    }

                }
                else
                {
                    CreateButton.IsEnabled = false;
                    alertDivisionsLabel.Visibility = Visibility.Visible;
                }

            }
            catch (FormatException)
            {
                if (DivisionsTextBox.Text != "")
                {
                    CreateButton.IsEnabled = false;
                    alertDivisionsLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    CreateButton.IsEnabled = false;
                    alertDivisionsLabel.Visibility = Visibility.Hidden;
                }
            }
            catch (NullReferenceException)
            {
                if (DivisionsTextBox.Text != "")
                {
                    CreateButton.IsEnabled = false;
                    alertDivisionsLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    CreateButton.IsEnabled = false;
                    alertDivisionsLabel.Visibility = Visibility.Hidden;
                }
            }
        }
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            
            Create(false, 0);
        }

        public void Create(bool Advanced, double newRateArea)  // Create ow can be used on the advanced study also, Advanced == true needs a newRateArea to create, if rate= 5 : 1, newRateArea=5
        {
            if (DivisionsTextBox.Text != "" && CourantTextBox.Text != "")
            {
                CreateButton.Content = "SIMULATING...";
                CreateButton.IsEnabled = false;
                DefaultValuesButton.IsEnabled = false;

                DivisionsTextBox.IsEnabled = false;
                CourantTextBox.IsEnabled = false;

                decimal c = decimal.Parse(CourantTextBox.Text.Replace('.', ','));
                double C = Convert.ToDouble(c);
                int divisions = Convert.ToInt32(DivisionsTextBox.Text);
                textStep.Content = "0"; labelStep.Visibility = Visibility.Visible;
                textTime.Content = "0"; labelTime.Visibility = Visibility.Visible;
                rectangleAutoStep.Visibility = Visibility.Visible;

                RectanglesInitial(Visibility.Hidden);
                
                rayahorizontal.Visibility = Visibility.Visible;
                Indicator.Visibility = Visibility.Visible;
                panelShow.Visibility = Visibility.Hidden;
                initiated = true;


                
                if(Advanced==true)
                {
                    nozzlesim = new Nozzle(3, 2800, 1.95, 2, C, divisions, newRateArea);    //si estamos en el estudio avanzado, cambiará el area acorde a lo que se ha especificado
                }
                else
                {
                    nozzlesim = new Nozzle(3, 2800, 1.95, 2, C, divisions);
                }

                // computa todos los valores especificados
                nozzlesim.ComputeUntilPos(1401);
                calculateMinMax();
                //escribe los labels max y min en el indicador
                WriteIndicatorMaxMin(0);
                //inizialitzem el step
                steps = 0;
                setDimensionlessCharts();

                //create the buttons of the charts
                createButtCharts();
                // fill posChart of zeros
                fillSelectedList();
                // create the brushes List
                createBrushesList();

                lastLabeTick = new TimeSpan(0);



                PropertiesBoxSelection.SelectedIndex = 0;
                CreateNozzle(nozzlesim, 0);
                plotChanged = true;
                SetChart();
                clock.Start();
            }
            else
                MessageBox.Show("Set some parameters first," + "\n" + "check if some of the boxes above are empty");
        }
        //CONTROLS SIMULATOR
        private void PropertiesBoxSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PropertiesBoxSelection.SelectedIndex != -1)
            {
                WriteIndicatorMaxMin(PropertiesBoxSelection.SelectedIndex);
                CreateNozzle(nozzlesim, steps);             //**de moment 0 el temps, pero ja veurem quan tingui un timestep diferent
            }

        }

        //CONTROLS CHARTS


        public double[] ampliate(double max, double min)
        {
            double[] MaxMin = new double[2] { max + (max - min) / 20, min - (max - min) / 20 };
            return MaxMin;
        }

        private void DimensionlessButton_Click(object sender, RoutedEventArgs e)
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
                Position dimenssional = nozzlesim.getDimensionalPosition();
                yAxisD.MaxValue = MaxMinD[0] * dimenssional.GetDensity();
                yAxisD.MinValue = MaxMinD[1] * dimenssional.GetDensity();
                chartD.HorizontalAlignment = HorizontalAlignment.Stretch;
                yAxisD.Title = "Density [ kg / m^3 ]";
                yAxisV.MaxValue = MaxMinV[0] * dimenssional.GetVelocity();
                yAxisV.MinValue = MaxMinV[1] * dimenssional.GetVelocity();
                chartV.HorizontalAlignment = HorizontalAlignment.Stretch;
                yAxisV.Title = "Velocity [ m / s ]";
                yAxisT.MaxValue = MaxMinT[0] * dimenssional.GetTemperature();
                yAxisT.MinValue = MaxMinT[1] * dimenssional.GetTemperature();
                chartT.HorizontalAlignment = HorizontalAlignment.Stretch;
                yAxisT.Title = "Temperature [ ºC ]";
                yAxisP.MaxValue = MaxMinP[0] * dimenssional.GetPressure() * dimenssional.R / 100;
                yAxisP.MinValue = MaxMinP[1] * dimenssional.GetPressure() * dimenssional.R / 100;
                chartP.HorizontalAlignment = HorizontalAlignment.Stretch;
                yAxisP.Title = "Pressure [ hPa ]";

            }
            plotChanged = true;
            SetChart();

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
            foreach (int position in brushesPos)
            {
                if (position != -1)
                    selectedPos++;
            }
            int firstEmpty = brushesList.Count()-1;
            
            int i = 0;
            while (i < brushesPos.Count())
            {
                int j = 0;
                bool foundFirst = false;
                while (j< brushesPos.Count())
                {
                    if (brushesPos[j] == i)
                    {
                        foundFirst = true;
                        break;
                    }
                    j++;
                }
                if (foundFirst == false)
                {
                    firstEmpty = i;
                    break;
                }
                i++;
            }
            bool maximum = true;
            if (selectedPos < 10)
            {
                maximum = false;
            }
            if (selectedPos == 0)
            {
                NoSeriesD.Visibility = Visibility.Hidden;
                NoSeriesV.Visibility = Visibility.Hidden;
                NoSeriesT.Visibility = Visibility.Hidden;
                NoSeriesP.Visibility = Visibility.Hidden;
            }

            int pos = Convert.ToInt32(Convert.ToDecimal(button.Content.ToString().Split(' ')[2]) * 10);

            int showed = brushesPos[pos];
            if (showed == -1)   // pint of gray
            {
                if (maximum == true)
                {
                    MessageBox.Show("The maximum number of plots enabled to show are 10," + "\n" + "if some position are important delete another first");

                }
                else
                {
                    brushesPos[pos] = firstEmpty;
                    Color colorset = Color.FromRgb(153, 144, 144);
                    Brush colorBrush = new SolidColorBrush(colorset);
                    button.Background = colorBrush;
                }
            }
            else
            {
                brushesPos[pos] = - 1;
                Color colorset = Color.FromRgb(232, 232, 232);
                Brush colorBrush = new SolidColorBrush(colorset);
                button.Background = colorBrush;
            }
            plotChanged = true;
            SetChart();
        }
        private void buttChart_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
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
            double width = (double)435 / (nozzle.GetDivisions());
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
                rectbutton.Name = "button" + Convert.ToString(count);
                rectbutton.Height = 100 + (nozzle.GetPosition(0, count).GetArea() - 1) * 200 / 4.95;
                rectbutton.Width = width;
                rectbutton.BorderBrush = new SolidColorBrush(color);
                Canvas.SetLeft(rectbutton, 5 + width * count);
                Canvas.SetTop(rectbutton, 150 - (rectbutton.Height - 100) * 100 / 200);
                rectbutton.Click += Rectbutton_Click;
                rectbutton.MouseEnter += Rectbutton_MouseEnter;
                rectbutton.MouseLeave += Rectbutton_MouseLeave;
                rectbutton.MouseRightButtonDown += Rectbutton_RightClick;
                NozzleCanvas.Children.Add(rectbutton);
                count++;
            }

            // crea el valor maximo del chart del nozzle
            xAxiscolores.MaxValue = Convert.ToDouble(nozzlesim.GetDivisions()) * 3 / (Convert.ToDouble(nozzlesim.GetDivisions()) - 1);
        }

        private void Rectbutton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string nombre = button.Name.Split('n')[1];
            foreach (Button boton in listaboton)
            {
                if (boton.Name.Split('t')[3] == nombre)
                {
                    ClickButtonChart(boton);
                }
            }
        }
        private void Rectbutton_MouseEnter(object sender, RoutedEventArgs e)
        {
            WriteTable(sender);
        }
        public void WriteTable(object sender)
        {
            Position position; Position Dimenssion; bool procede = true;
            if (fixTable == false && sender.GetType().Name == "Button")
            {
                Button button = (Button)sender;
                Dimenssion = nozzlesim.getDimensionalPosition();
                int row = Convert.ToInt32(button.Name.ToString().Split('n')[1]);
                position = nozzlesim.GetRow(steps)[row];

            }
            else if (fixTable == true )
            {
                Button button = fixedButton;
                Dimenssion = nozzlesim.getDimensionalPosition();
                int row = Convert.ToInt32(button.Name.ToString().Split('n')[1]);
                position = nozzlesim.GetRow(steps)[row];
            }
            else 
            {
                procede = false;
                position = new Position(); 
                Dimenssion= new Position();
            }
            if (procede == true)
            {
                if (unitsShowTable == true)
                {
                    labelValueX.Content = Math.Round(position.GetX(), 1);
                    labelValueT.Content = Math.Round(position.GetTemperature() * Dimenssion.GetTemperature(), 2);
                    labelValueD.Content = Math.Round(position.GetDensity() * Dimenssion.GetDensity(), 2);
                    labelValueV.Content = Math.Round(position.GetVelocity() * Dimenssion.GetVelocity(), 2);
                    labelValueP.Content = Math.Round(position.GetPressure() * Dimenssion.GetPressure() * Dimenssion.R, 2);
                    labelValueA.Content = Math.Round(position.GetArea(), 2);
                    dimensionIndicator.Content = "dimensional";
                }
                else
                {
                    labelValueX.Content = Math.Round(position.GetX(), 1);
                    labelValueT.Content = Math.Round(position.GetTemperature(), 2);
                    labelValueD.Content = Math.Round(position.GetDensity(), 2);
                    labelValueV.Content = Math.Round(position.GetVelocity(), 2);
                    labelValueP.Content = Math.Round(position.GetPressure(), 2);
                    labelValueA.Content = Math.Round(position.GetArea(), 2);
                    dimensionIndicator.Content = "non-dimensional";
                }
                panelShow.Visibility = Visibility.Visible;
                leftClickTip.Visibility = Visibility.Visible;
            }
        }
        private void Rectbutton_RightClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (fixTable == false)
            {
                DimensionlessButtonShow.IsChecked = false;
                fixTable = true;
                panelShow.Visibility = Visibility.Visible;
                fixedIndicator.Visibility = Visibility.Visible;
                fixedButton = button;

                dimensionTipR.Text = "Right click on the mouse to cancel attachment";
            }
            else
            {
                fixTable = false;
                panelShow.Visibility = Visibility.Hidden;
                fixedIndicator.Visibility = Visibility.Hidden;
                dimensionTipR.Text = " Right click on the mouse to attach the table";
            }
            DimensionlessButtonShow.IsChecked = true;
        }
        private void CheckedBox_Show(object sender, RoutedEventArgs e)
        {

            Button button = fixedButton;
            Position Dimenssion = nozzlesim.getDimensionalPosition();
            int row = Convert.ToInt32(button.Name.ToString().Split('n')[1]);
            Position position = nozzlesim.GetRow(steps)[row];

            if (DimensionlessButtonShow.IsChecked == false)
            {
                labelValueX.Content = Math.Round(position.GetX(), 1).ToString() + " m";
                labelValueT.Content = Math.Round(position.GetTemperature() * Dimenssion.GetTemperature(), 2).ToString() + " ºC";
                labelValueD.Content = Math.Round(position.GetDensity() * Dimenssion.GetDensity(), 2).ToString() + " Kg/m^3";
                labelValueV.Content = Math.Round(position.GetVelocity() * Dimenssion.GetVelocity(), 2).ToString() + " m/s";
                labelValueP.Content = Math.Round(position.GetPressure() * Dimenssion.GetPressure() * Dimenssion.R / 100, 2).ToString() + " hPa";
                labelValueA.Content = Math.Round(position.GetArea(), 2).ToString() + " m^2";
                dimensionIndicator.Content = "dimensional";
            }
            else
            {
                labelValueX.Content = Math.Round(position.GetX(), 1);
                labelValueT.Content = Math.Round(position.GetTemperature(), 2);
                labelValueD.Content = Math.Round(position.GetDensity(), 2);
                labelValueV.Content = Math.Round(position.GetVelocity(), 2);
                labelValueP.Content = Math.Round(position.GetPressure(), 2);
                labelValueA.Content = Math.Round(position.GetArea(), 2);
                dimensionIndicator.Content = "non-dimensional";
            }
            panelShow.Visibility = Visibility.Visible;

        }
        private void Rectbutton_MouseLeave(object sender, RoutedEventArgs e)
        {
            if (fixTable == false)
                panelShow.Visibility = Visibility.Hidden;

            leftClickTip.Visibility = Visibility.Hidden;
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

        public void createButtCharts()
        {
            //< !--< Button x: Name = "buttChart0" Grid.Row = "0" Grid.Column = "0" FontSize = "10" Content = "x = 0" Background = "#FFE8E8E8" Margin = "1,1,1,1" Click = "buttChart0_Click" Cursor = "Hand" ></ Button >

            int filas = (nozzlesim.GetDivisions() - 1) / 10 + 1; // if div == 31;  4 filas
            int i = 0;
            listaboton = new List<Button>();
            while (i < filas)
            {

                gridButtChart.RowDefinitions.Add(new RowDefinition());
                gridRecChart.RowDefinitions.Add(new RowDefinition());
                int j = 0;
                while (j < 10)
                {
                    Color color = Color.FromRgb(232, 232, 232);
                    Button button = new Button();
                    button.Background = new SolidColorBrush(color);
                    button.Name = "buttChart" + Convert.ToString(i * 10 + j);
                    double contenidox = Math.Round((i * (double)10 + j) / (double)10 / (filas - (double)1) * (double)3, 2);
                    button.Content = "x = " + Convert.ToString(contenidox);
                    if (filas == 5)
                        button.FontSize = 9;
                    else if (filas == 6)
                        button.FontSize = 6;
                    else if (filas == 7)
                        button.FontSize = 5;
                    else
                        button.FontSize = 10;

                    button.VerticalContentAlignment = VerticalAlignment.Center;
                    button.Margin = new Thickness(1, 1, 1, 1);
                    button.Cursor = Cursors.Hand;
                    button.Click += buttChart_Click;
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    gridButtChart.Children.Add(button);
                    listaboton.Add(button);
                    j++;
                    if (i == filas - 1)
                        break;
                }
                i++;
            }

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
                    if (i == gridRecChart.RowDefinitions.Count - 1 && j == 1)
                        break;

                    Rectangle rec = new Rectangle();
                    rec.Name = "rec" + (i * 10 + j).ToString();
                    rec.Height = 4;
                    //Color colorset = Color.FromArgb(0, 0, 255, 255);
                    //Brush colorBrush = new SolidColorBrush(colorset);
                    rec.Margin = new Thickness(1, 4, 1, 2);
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
            if (nozzlesim.Getmalla() != null)
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
                Position dimens;
                if (DimensionlessButton.IsChecked == false)
                {
                    // T V P D
                    dimens = nozzlesim.getDimensionalPosition();
                }
                else
                {
                    dimens = new Position(0, 1, 1, 1, 0);
                }
                foreach (int pos in brushesPos)
                {
                    if (pos != -1)
                    {
                        listV.Add(nozzlesim.GetColumnPar(i, "V", stepsChart, dimens, finStep));
                        listP.Add(nozzlesim.GetColumnPar(i, "P", stepsChart, dimens, finStep));
                        listT.Add(nozzlesim.GetColumnPar(i, "T", stepsChart, dimens, finStep));
                        listD.Add(nozzlesim.GetColumnPar(i, "D", stepsChart, dimens, finStep));
                        ListBrush.Add(brushesList[brushesPos[i]]);
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
                int sel = 0;
                foreach (int pos in brushesPos)
                    if (pos != -1)
                        sel++;
                int maxUpdate = 1;
                if (sel > 4)
                    maxUpdate = 2;
                if (lastChartUpdate > new TimeSpan(maxUpdate * 10000000) && plotChanged == true)
                {
                    plotChanged = false;
                    lastChartUpdate = new TimeSpan(0);
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


            chart.Series = new SeriesCollection();
            int i = 0;
            while (i < listV.Count)
            {
                LineSeries linSerie = new LineSeries
                {
                    Title = "x = " + (i / 10.0).ToString(),
                    Name = "lineChart_" + i.ToString(),
                    Values = new ChartValues<double>(listV[i]),
                    PointGeometry = null,
                    Fill = Brushes.Transparent,
                    Stroke = ListBrush[i],
                };
                chart.Series.Add(linSerie);
                i++;
            }



            if (times.Count() > 1)
            {

                double dec = Convert.ToDouble(Convert.ToDecimal(times[times.Count() - 1]));
                //xAxisDSeparator.Step = 0.1;
                xAxis.MaxValue = times.Count() - 1;
                xAxis.Separator.Step = times.Count() - 1;
                xAxis.Labels = times;
            }


            DataContext = this;
        }

        public void fillSelectedList()
        {
            brushesPos = new List<int>();
            int i = 0;
            while (i < nozzlesim.GetDivisions())
            {
                brushesPos.Add(-1);
                i++;
            }
        }


        public void DataGrid1_Loaded()
        {
            List<Position> una_lista = nozzlesim.position_initial_conditions;
            DataGrid1.ItemsSource = una_lista;
        }



        /// Controls of the STEP AUTO PAUSE CLOCK...
        

        private void NextStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (steps < 1401)
            {
                steps = steps + 1;
                textStep.Content = steps.ToString();
                textTime.Content = nozzlesim.getTimeList()[steps].ToString() + " sec";
                CreateNozzle(nozzlesim, steps);
                plotChanged = true;
                SetChart();
                
            }
            else
            {
                MessageBox.Show("The simulation is ended, the maximum time is reached ");
            }
        }


        private void clock_time_Tick(object sender, EventArgs e)
        {
            if (auto == true && steps<1401)
            {
                steps = steps + 1;
                clockTime = clockTime + clock.Interval;
                textStep.Content = steps.ToString();
                textTime.Content = nozzlesim.getTimeList()[steps].ToString() + " sec";
                CreateNozzle(nozzlesim, steps);
                plotChanged = true;
                WriteTable(sender);
            }
            else if (auto == true && steps >= 1401)
            {
                clock.Stop();
                textStep.Content = steps.ToString();
                textTime.Content = nozzlesim.getTimeList()[steps].ToString() + " sec";
                CreateNozzle(nozzlesim, steps);
                plotChanged = true;
                SetChart();
                Auto();
                MessageBox.Show("The simulation is ended, the maximum time is reached ");
            }
            
            lastChartUpdate += clock.Interval;
            ParpadeoLabels();
            SetChart();
        }
        public void ParpadeoLabels()
        {
            lastLabeTick += clock.Interval;
            // Parpadeo de labels de no hay steps
            if (steps == 0)
            {
                if (lastLabeTick > new TimeSpan(10000000))
                {
                    lastLabeTick = new TimeSpan(0);
                    if (NoPointsLabelD.Visibility == Visibility.Visible)
                    {
                        NoPointsLabelD.Visibility = Visibility.Hidden;
                        NoPointsLabelV.Visibility = Visibility.Hidden;
                        NoPointsLabelT.Visibility = Visibility.Hidden;
                        NoPointsLabelP.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        NoPointsLabelD.Visibility = Visibility.Visible;
                        NoPointsLabelV.Visibility = Visibility.Visible;
                        NoPointsLabelT.Visibility = Visibility.Visible;
                        NoPointsLabelP.Visibility = Visibility.Visible;
                    }
                }
            }
            else if (steps != 0 && NoPointsLabelD.Visibility == Visibility.Visible)
            {
                lastLabeTick = new TimeSpan(0);
                NoPointsLabelD.Visibility = Visibility.Hidden;
                NoPointsLabelV.Visibility = Visibility.Hidden;
                NoPointsLabelT.Visibility = Visibility.Hidden;
                NoPointsLabelP.Visibility = Visibility.Hidden;
            }
            else if (lastLabeTick > new TimeSpan(10000000))
            {
                lastLabeTick = new TimeSpan(0);
                bool zeroSelected = true;
                foreach (int pos in brushesPos)
                {
                    if (pos != -1)
                    {
                        zeroSelected = false;
                        break;
                    }
                }
                if (zeroSelected == true)
                {
                    if (NoSeriesD.Visibility == Visibility.Visible)
                    {
                        NoSeriesD.Visibility = Visibility.Hidden;
                        NoSeriesV.Visibility = Visibility.Hidden;
                        NoSeriesT.Visibility = Visibility.Hidden;
                        NoSeriesP.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        NoSeriesD.Visibility = Visibility.Visible;
                        NoSeriesV.Visibility = Visibility.Visible;
                        NoSeriesT.Visibility = Visibility.Visible;
                        NoSeriesP.Visibility = Visibility.Visible;
                    }
                }
                else if (NoSeriesD.Visibility == Visibility.Visible)
                {
                    NoSeriesD.Visibility = Visibility.Hidden;
                    NoSeriesV.Visibility = Visibility.Hidden;
                    NoSeriesT.Visibility = Visibility.Hidden;
                    NoSeriesP.Visibility = Visibility.Hidden;
                }
            }

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
                    NextStepButton.IsEnabled = false;
                    auto = true;
                    Color colorset = Color.FromRgb(153, 144, 144);
                    Brush colorBrush = new SolidColorBrush(colorset);
                    AutoButton.Background = colorBrush;
                    AutoButton.Content = "PAUSE";
                }
                else
                {
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
            NextStepButton.IsEnabled = true;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult respuesta = MessageBox.Show(
                "Se va a reiniciar el simulador, se perderán los cambios", "Warning", MessageBoxButton.OKCancel);

            switch (respuesta)
            {
                case MessageBoxResult.OK:
                    Restart();
                    RestartAdvanced();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }

        }
        private void Restart()
        {
            Color colorset = Color.FromRgb(232, 232, 232); //The gray color of some buttons
            Brush colorBrush = new SolidColorBrush(colorset);
            clock.Interval = new TimeSpan(2000000);        // The interval could have other values
            clockTime = new TimeSpan(0);                   // We set clockTime to 0
            clock.Stop();

            if (auto == false)  // If we aro not in auto modde thee is as it should be initially
            {

            }
            else                 // If not, we set  it
            {
                clock.Stop();
                NextStepButton.IsEnabled = true;
                auto = false;
                AutoButton.Background = colorBrush;
                AutoButton.Content = "AUTO";
            }

            nozzlesim = new Nozzle();  // We create an empty class nozzle, so min and max values will be 0, as initiall (unsued situation)
            calculateMinMax();
            fillSelectedList();
            steps = 0;                 // The time step counter is reseted to 0 as initially

            labelStep.Visibility = Visibility.Hidden; textStep.Content = "";    // We clear and hidde the labels of step and time
            labelTime.Visibility = Visibility.Hidden; textTime.Content = "";
            
            RectanglesInitial(Visibility.Visible);         // We make visible the rectangles ( transparent background ) that does not allow us to click anywhere
            List<Brush> ListBrush = new List<Brush>();     // We clear the list of brushes for the plots, to be computed again
            //foreach (int pos in brushesPos)
            //{
            //    ListBrush.Add(Brushes.Transparent);
            //}

            gridRecChart.Children.Clear();                 // The grid that contains the rectangles of different colors on the button's grid of the charts panel are cleared
            if (listaboton != null)                        // And also the buttons that were ona list on of ther charts panel
                listaboton.Clear();
            initiated = false;                             // The boolean that shows that the simulation is initiated alos as initially

            CreateButton.Content = "CREATE";               // The content and properties of the buttons and textBox of the initial setings are seted as initially
            DefaultValuesButton.IsEnabled = true;
            DivisionsTextBox.IsEnabled = true;
            CourantTextBox.IsEnabled = true;
            DivisionsTextBox.Text = "";
            CourantTextBox.Text = "";                       // 

            SetChart();                                     // SetChart deletes the plots of the charts

            if (DimensionlessButton.IsChecked == true)      // We set the charts as dimensionless
            { }
            else
            {
                DimensionlessButton.IsChecked = true;
                setDimensionlessCharts();
            }                                               //

            row0.Height = new GridLength(175);                    // Maximize and minimize charts are candelled and seted as initialy seting the 
            chartD.Height = 150;                                  // height and width of each grid again and  
            chartV.Height = 150;
            chartT.Height = 150;
            chartP.Height = 150;
            col0.Width = new GridLength(245.2);
            row1.Height = new GridLength(175);
            col1.Width = new GridLength(245.2);                   //

            MaxDensity.Visibility = Visibility.Visible;           // we make Visible the icone realted to maximize action and hide the one of minimize avction
            MinDensity.Visibility = Visibility.Hidden;
            MaxVelocity.Visibility = Visibility.Visible;
            MinVelocity.Visibility = Visibility.Hidden;
            MaxTemperature.Visibility = Visibility.Visible;
            MinTemperature.Visibility = Visibility.Hidden;
            MaxPressure.Visibility = Visibility.Visible;
            MinPressure.Visibility = Visibility.Hidden;            //

            rayahorizontal.Visibility = Visibility.Hidden;         // We hidde the orizonthal longitudinal x variable on the nozzle panel
            Indicator.Visibility = Visibility.Hidden;
            panelShow.Visibility = Visibility.Hidden;
            fixedIndicator.Visibility = Visibility.Hidden;         //

            fixTable = false;                                      // The bolean that informs that that the informtion table on the left buttom corner is setted to false

            NozzleCanvas.Children.Clear();                         // The nozzle rectangles are cleared
            gridRecChart.Children.Clear();               
            gridRecChart.RowDefinitions.Clear();
            gridButtChart.Children.Clear();
            gridButtChart.RowDefinitions.Clear();                  //

            PropertiesBoxSelection.SelectedIndex = -1;             // The propierties box selection is cleared
            //nozzlesim = new Nozzle(3, 800, 0.5, 0.5, 31);
            //nozzlesim.ComputeUntilPos(1401);
            //calculateMinMax();
        }

        public void RectanglesInitial(Visibility vis)
        {
            rectangleCharts.Visibility = vis;
            rectangleNozzle.Visibility = vis;
            rectangleAutoStep.Visibility = vis;
        }

        

        private void WriteIndicatorMaxMin(int propind)
        {
            if (propind == 0)
            {
                MaxLabel.Content = Convert.ToString(Math.Round(maxT, 1));
                MinLabel.Content = Convert.ToString(Math.Round(minT, 1));
                MedLabel.Content = Convert.ToString(Math.Round((maxT + minT) / (double)2, 1));
            }
            else if (propind == 1)
            {
                MaxLabel.Content = Convert.ToString(Math.Round(maxV, 1));
                MinLabel.Content = Convert.ToString(Math.Round(minV, 1));
                MedLabel.Content = Convert.ToString(Math.Round((maxV + minV) / (double)2, 1));
            }
            else if (propind == 2)
            {
                MaxLabel.Content = Convert.ToString(Math.Round(maxD, 1));
                MinLabel.Content = Convert.ToString(Math.Round(minD, 1));
                MedLabel.Content = Convert.ToString(Math.Round((maxD + minD) / (double)2, 1));
            }
            else if (propind == 3)
            {
                MaxLabel.Content = Convert.ToString(Math.Round(maxP, 1));
                MinLabel.Content = Convert.ToString(Math.Round(minP, 1));
                MedLabel.Content = Convert.ToString(Math.Round((maxP + minP) / (double)2, 1));
            }
        }

        private void AutoSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double periodo = 1000000 / AutoSlider.Value;
            clock.Interval = new TimeSpan((long)periodo);
        }


        // ADVANCED STUDY

        private void buttonAdvanced_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult respuesta = MessageBox.Show(
                "The advanced study is getting started, the simulator will be reset" + "\n" + 
                "to create a new nozzle, so the info will be lost. Do you want to coninue?", "Advanced study", MessageBoxButton.OKCancel);

            switch (respuesta)
            {
                case MessageBoxResult.OK:

                    RestartAdvanced();
                    
                    Restart();
                    buttonAdvanced.Visibility = Visibility.Hidden;
                    panelAdvanced2.Visibility = Visibility.Visible;
                    advanced = true;
                    rectangleAdvanced.Visibility = Visibility.Hidden;
                    
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }
        public void RestartAdvanced()
        {
            textNewA.Text = "";
            buttonAdvanced.Visibility = Visibility.Hidden;
            panelAdvanced2.Visibility = Visibility.Visible;
            buttCheckNewA.Background = new SolidColorBrush(Color.FromArgb(204, 255, 9, 0));
            buttCheckNewA.Visibility = Visibility.Hidden;
            alertRateA.Visibility = Visibility.Hidden;
            recOld.Visibility = Visibility.Hidden;
            recNew.Visibility = Visibility.Hidden;
            rectangleAdvanced.Visibility = Visibility.Hidden;
            
            panelAdvanced2.Visibility = Visibility.Hidden;
            buttonAdvanced.Visibility = Visibility.Visible;
            buttonNewAdvanced.Visibility = Visibility.Hidden;
            chartA.Visibility = Visibility.Hidden;
            chartA.Series.Clear();
            xAxisA.Labels=new string[0];

            advanced = false;
        }

        private void textNewA_TextChanged(object sender, TextChangedEventArgs e)
        {
            SolidColorBrush red= new SolidColorBrush(Color.FromArgb(204,255,9,0));
            buttCheckNewA.Background = red;
            buttCheckNewA.Visibility = Visibility.Visible;
            buttCheckNewA.Content = "check value";
            buttCheckNewA.IsEnabled = true;
            alertRateA.Visibility = Visibility.Hidden;
            chartA.Visibility = Visibility.Hidden;
            recNew.Visibility = Visibility.Hidden;
            recOld.Visibility = Visibility.Hidden;
            DefaultValuesClear();
        }

        private void buttCheckNewA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textNewA.Text != "")
                {
                    decimal decRes = decimal.Parse(textNewA.Text.Split(':')[0].Replace('.', ','));
                    decimal decThr = decimal.Parse(textNewA.Text.Split(':')[1].Replace('.', ','));

                    
                    double Res = Convert.ToDouble(decRes);
                    double Thr = Convert.ToDouble(decThr);

                    if (Thr !=0)
                    {
                        if (Res > Thr && Res / Thr != 5.95 && Res / Thr <= 8)
                        {
                            Res = Math.Round(Res / Thr,2);
                            
                            textNewA.Text = ((Convert.ToDecimal(Res)).ToString() + " : 1").ToString().Replace(',', '.');
                            buttCheckNewA.Background = Brushes.GreenYellow;
                            buttCheckNewA.Content = "checked";
                            buttCheckNewA.IsEnabled = false;
                            alertRateA.Visibility = Visibility.Hidden;

                            chartA.Visibility = Visibility.Visible;
                            CreateButtonAdvStudy.IsEnabled = true;
                            chartA.Series = new SeriesCollection();
                            int N = 31;
                            List<double> listA = new List<double>();
                            List<double> listAold = new List<double>();
                            List<double> listAM = new List<double>();
                            List<double> listAoldM = new List<double>();
                            int i = 0;
                            double Res0 = 2.2 * Math.Pow(0.0 - (N - 1)*0.1 / 2, 2);
                            double Correction = (Res-1) / Res0;
                            string[] x = new string[N];
                            while (i < N)
                            {
                                double xi = i * 0.1;
                                double A = (1 + 2.2 * Correction * Math.Pow(xi - (N - 1)*0.1 / 2, 2));
                                listA.Add(A/2);
                                double Aminus = -1*(1 + 2.2 * Correction * Math.Pow(xi - (N - 1) * 0.1 / 2, 2));
                                listAM.Add(Aminus/2);
                                double Aold = 1+2.2 * Math.Pow(xi - (N - 1)*0.1 / 2, 2);
                                listAold.Add(Aold/2);
                                double Aoldminus = -1*(1 + 2.2 * Math.Pow(xi - (N - 1) * 0.1 / 2, 2));
                                listAoldM.Add(Aoldminus/2);
                                x[i] = (i * 0.1).ToString();
                                i++;
                            }
                            LineSeries linSerie = new LineSeries
                            {
                                Title = textNewA.Text.ToString(),
                                Name = "lineChartNewA",
                                Values = new ChartValues<double>(listA),
                                PointGeometry = null,
                                Fill = Brushes.Transparent,
                                Stroke = Brushes.Green,
                            };
                            chartA.Series.Add(linSerie);
                            LineSeries linSerieM = new LineSeries
                            {
                                Title = textNewA.Text.ToString(),
                                Name = "lineChartNewAM",
                                Values = new ChartValues<double>(listAM),
                                PointGeometry = null,
                                Fill = Brushes.Transparent,
                                Stroke = Brushes.Green,
                            };
                            chartA.Series.Add(linSerieM);
                            LineSeries linSerie1 = new LineSeries
                            {
                                Title = "5.95 : 1",
                                Name = "lineChartOldA",
                                Values = new ChartValues<double>(listAold),
                                PointGeometry = null,
                                Fill = Brushes.Transparent,
                                Stroke = Brushes.Black,
                            };
                            chartA.Series.Add(linSerie1);
                            LineSeries linSerie1M = new LineSeries
                            {
                                Title = "5.95 : 1",
                                Name = "lineChartOldAM",
                                Values = new ChartValues<double>(listAoldM),
                                PointGeometry = null,
                                Fill = Brushes.Transparent,
                                Stroke = Brushes.Black,
                            };
                            chartA.Series.Add(linSerie1M) ;
                            xAxisA.Labels = x;
                            xAxisA.MaxValue = x.Count() - 1;
                            SepA.Step= (x.Count() - 1)/2;
                            if (listA[0]>listAold[0])
                            {
                                yAxisA.MaxValue = listA[0] * 1.1;
                                yAxisA.MinValue = listA[0] * (-1.1);
                            }
                            else
                            {
                                yAxisA.MaxValue = listAold[0] * (1.1);
                                yAxisA.MinValue = listAold[0] * (-1.1);
                            }
                            DataContext = this;
                            chartA.Visibility = Visibility.Visible;
                            recNew.Visibility = Visibility.Visible;
                            recOld.Visibility = Visibility.Visible;
                            if (DivisionsTextBox.Text =="" && CourantTextBox.Text=="")
                                DefaultValues();
                        }
                        else if (Res / Thr > 8)
                        {
                            alertRateA.Visibility = Visibility.Visible;
                            CreateButtonAdvStudy.IsEnabled = false;
                            textAlertA.Text = "The maximum allowed rate is 8 : 1 ";
                        }
                        else if (Res < Thr)
                        {
                            alertRateA.Visibility = Visibility.Visible;
                            CreateButtonAdvStudy.IsEnabled = false;
                            textAlertA.Text = "Only convergent-divergent nozzle is possibe ";
                        }
                        else if (Res == Thr)
                        {
                            CreateButtonAdvStudy.IsEnabled = false;
                            alertRateA.Visibility = Visibility.Visible;
                            textAlertA.Text = "Cannot create the same nozzle as initialy";
                        }
                        else
                        {
                            CreateButtonAdvStudy.IsEnabled = false;
                            alertRateA.Visibility = Visibility.Visible;
                            textAlertA.Text = "Error in text format, check it!";
                        }
                    }
                }
                else
                {
                    CreateButtonAdvStudy.IsEnabled = false;
                    alertRateA.Visibility = Visibility.Visible;
                    textAlertA.Text = "Error in text format, there is a null value";
                }
            }
            catch (FormatException)
            {
                CreateButtonAdvStudy.IsEnabled = false;
                alertRateA.Visibility = Visibility.Visible;
                textAlertA.Text = "Error in text format, check it!";
            }
           
        }

        private void CreateButtonAdvStudy_Click(object sender, RoutedEventArgs e)
        {
            decimal Ratio = decimal.Parse(textNewA.Text.Split(':')[0].Replace('.', ','));


            double new_Ratio = Convert.ToDouble(Ratio);
            Create(true, new_Ratio);
            CreateButtonAdvStudy.IsEnabled = false;
            buttCheckNewA.Visibility = Visibility.Hidden;
            buttonNewAdvanced.Visibility = Visibility.Visible;
            rectangleAdvanced.Visibility = Visibility.Visible;

        }

        private void buttonNewAdvanced_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult respuesta = MessageBox.Show(
                "A new advanced study is getting started, the simulator will be reset" + "\n" +
                "to create a new nozzle, so the info will be lost. Do you want to coninue?", "Advanced study", MessageBoxButton.OKCancel);

            switch (respuesta)
            {
                case MessageBoxResult.OK:
                    RestartAdvanced();

                    Restart();
                    buttonAdvanced.Visibility = Visibility.Hidden;
                    panelAdvanced2.Visibility = Visibility.Visible;
                    advanced = true;
                    rectangleAdvanced.Visibility = Visibility.Hidden;
                    buttonNewAdvanced.Visibility = Visibility.Hidden;


                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
            
        }
    } 
}





