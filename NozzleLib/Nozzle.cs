﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NozzleLib
{
    public class Nozzle
    {
        double gamma = 1.4;             //Heat capacity ratio
        double R = 286;                 //Gas constant
        double deltax;                  //distance between positions in the nozzle    
        double deltatime;               //time-step of the simulation
        List<double> TimeList = new List<double> { 0 };
        double C;                       //Courant value
        Position[,] malla;              //Position matrix rows=time steps and columns=space divisions
        int N;                     //Number of space divisions, 31 by default (Anderson value)
        double[] dimensionalvalues;     //Initial values to obtain dimensional values to magnitudes [L, T0, a0, p0, ro0]
        Position dimensionalPos;         
        double throatposition;          //where is the throat

        //CONSTRUCTORS
        public Nozzle(double L, double T0, double ro0, double A0, double C, int N) //A0 is the area of the throat
        {
            SetDimensionalValues(L, T0, ro0, A0);
            this.throatposition = L / 2;
            this.deltax = L / (N-1);                //Depending on the ammount of divisions the nozzle has, distance between positions will vary
            this.N = N;
            this.C = C;
            this.malla = new Position[1401, N];
            int i = 0;
            while (i < N)
            {
                double xi = 0 + i * deltax;
                double temp = 1 - 0.2314 * xi;
                Position pos = new Position(xi, temp, 1 - 0.3146 * xi, (0.1 + 1.09 * xi) * Math.Sqrt(temp), 1 + 2.2 * Math.Pow(xi - throatposition, 2), i + 1); //nozzle initial conditions
                SetPosition(0, i, pos);
                i++;
            }
        }
        // New constructor used to set a new Area profile different to the initial one
        public Nozzle(double L, double T0, double ro0, double A0, double C, int N, double Res) //A0 is the area of the throat, 
        {
            SetDimensionalValues(L, T0, ro0, A0);
            this.throatposition = L / 2;
            this.deltax = L / (N - 1);
            this.N = N;
            this.C = C;
            this.malla = new Position[1401, N];
            int i = 0;
            while (i < N)
            {
                double xi = 0 + i * deltax;
                double temp = 1 - 0.2314 * xi;
                double Res0 = 2.2 * Math.Pow(0.0 - (N - 1) * 0.1 / 2, 2);
                double Correction = (Res - 1) / Res0;
                Position pos = new Position(xi, temp, 1 - 0.3146 * xi, (0.1 + 1.09 * xi) * Math.Sqrt(temp), 1 + 2.2 * Correction * Math.Pow(xi - throatposition, 2), i + 1);
                SetPosition(0, i, pos);
                i++;
            }
        }
        public Nozzle()
        {

        }
        //FUNCTIONS
            //Extraction and setting of variables
        public Position GetPosition(int t, int i)
        {
            Position pos = this.malla[t, i];
            return pos;
        }
        public int GetDivisions()
        {
            return N;
        }
        public void SetPosition(int t, int i, Position pos)
        {
            this.malla[t, i] = pos;
        }
        public int getN()
        {
            return this.N;
        }
        public double getCourant()
        {
            return this.C;
        }
        public List<double> getTimeList()
        {
            return TimeList;
        }
        public double[] getDimensionalValues()
        {
            return dimensionalvalues;
        }
        public Position getDimensionalPosition()
        {
            return dimensionalPos;
        }
        public Position[,] Getmalla()
        {
            return this.malla;
        }
        
        //Function to get alternate time values for the charts, creates a list of time values depending on the number of time steps the simulation has run
        public List<double> getTimeList(int steps, int finalStep)
        {
            List<double> Times = new List<double>();
            int i = 0;
            int initStep = 0;
            while (i < this.TimeList.Count && i<=finalStep)
            {
                if (initStep == 0)
                {
                    Times.Add(TimeList[i]);
                    
                }
                if (initStep == steps)
                {
                    initStep = -1;
                }
                initStep++;
                i++;
            }
            return Times;
        }
        //Function to obtain a list of the area values the nozzle has
        public List<double> createListArea(int t)
        {
            List<double> Area = new List<double>();
            if (1401 >= t && GetPosition(t, 0) != null)
            {
                int i = 0;
                while (i < N)
                {
                    Area.Add(GetPosition(t, i).GetArea());
                    i++;
                }
            }
            return Area;
        }
        //Function to obtain a list of the density values the nozzle has for a certain value of time
        public List<double> createListDensity(int t)
        {
            List<double> Density = new List<double>();
            if (1401 >= t && GetPosition(t, 0) != null)
            {
                int i = 0;
                while (i < N)
                {
                    Density.Add(GetPosition(t, i).GetDensity());
                    i++;
                }

            }
            return Density;
        }
        //Function to obtain a list of the velocity values the nozzle has for a certain value of time
        public List<double> createListVelocity(int t)
        {
            List<double> Velocity = new List<double>();
            if (1401 >= t && GetPosition(t, 0) != null)
            {
                int i = 0;
                while (i < N)
                {
                    Velocity.Add(GetPosition(t, i).GetVelocity());
                    i++;
                }

            }
            return Velocity;
        }
        //Function to obtain a list of the temeperature values the nozzle has for a certain value of time
        public List<double> createListTemperature(int t)
        {
            List<double> Temperature = new List<double>();
            if (1401 >= t && GetPosition(t, 0) != null)
            {
                int i = 0;
                while (i < N)
                {
                    Temperature.Add(GetPosition(t, i).GetTemperature());
                    i++;
                }

            }
            return Temperature;
        }
        //Function to set the dimensional values of the nozzle
        public void SetDimensionalValues(double L, double T0, double ro0, double A0)
        {
            dimensionalvalues = new double[5];
            dimensionalvalues[0] = L;
            dimensionalvalues[1] = T0;
            dimensionalvalues[2] = Math.Sqrt(gamma * R * T0);
            dimensionalvalues[3] = T0*ro0*R;
            dimensionalvalues[4] = ro0;
            dimensionalPos = new Position(0, T0,ro0, Math.Sqrt(gamma * R * T0),A0, N);

        }
        //Obtain a certain row of 
        public List<Position> GetRow (int row)
        {
            List<Position> fila = new List<Position>();
            int i = 0;
            while (i < N)
            {
                Position pos = GetPosition(row, i);
                fila.Add(pos);
                i++;
            }
            return fila;
        }
        
        //GetColumnPar creates a list of values of some property identified as an string  
        public List<double> GetColumnPar(int col, string parameter, int steps, Position dimens, int finStep)
        {
            List<double> columna = new List<double>();
            int i = 0;
            int initStep = 0;
            while (i < malla.GetLength(0) && i <= finStep)                       // steps are the number of dismissed position
            {                                                                    // dimens is an object position that contains the dimensional values of each property
                if (initStep == 0) // only initStep = 0 is added, others are dismiis, later some condition will make initStep return to 0
                {
                    Position pos = GetPosition(i, col);
                    if (pos != null)
                    {
                        double value;
                        if (parameter == "x")
                            value = Math.Round(pos.GetX(), 4);
                        else if (parameter == "T")
                            value = Math.Round(pos.GetTemperature() * dimens.GetTemperature(), 4);
                        else if (parameter == "D")
                            value = Math.Round(pos.GetDensity() * dimens.GetDensity(), 4);
                        else if (parameter == "V")
                            value = Math.Round(pos.GetVelocity() * dimens.GetVelocity(), 4);
                        else if (parameter == "P")                                                             // dimensional P units are hPa
                        {
                            if (dimens.GetPressure() != 1)
                                value = Math.Round(pos.GetPressure() * dimens.GetPressure(), 4) * dimens.R / 100;
                            else
                                value = Math.Round(pos.GetPressure() * dimens.GetPressure(), 4);
                        }
                        else if (parameter == "A")
                            value = Math.Round(pos.GetArea(), 4);
                        else if (parameter == "M")
                            value = Math.Round(pos.MachNumber(), 4);
                        else
                            value = -2;

                        if (value != -2)
                            columna.Add(value);
                    }
                    else
                    {
                        break;
                    }
                }
                if (initStep == steps)      // here we code the way we dismiss values, the next position of the while will has initStep = 0, that are the only cases added
                {
                    initStep = -1;
                }
                initStep++;
                i++;
            }
            return columna;
        }
        //Function to obtain a column of the matrix, belonging to the values of a certain position along time
        public List<Position> GetColumn(int col)
        {
            List<Position> columna = new List<Position>();
            int j = 0;
            while (j < 1401)
            {
                if (malla[j, col] != null)
                    columna.Add(malla[j, col]);
                else
                    break;
                j++;
            }
            return columna;
                
            
        }
        //Computation of the time-step each step of the simulation has. Each position has a certain value, and for each step, the simulation picks the minimum value
        public double ComputeDeltaTime(int t)
        {
            List<Position> fila = this.GetRow(t);
            List<double> lista_tirar = new List<double>();
            double variable_tirada;
            int ii = 0;
            while (ii < fila.Count)
            {
                variable_tirada = fila[ii].Deltatime(C, deltax);
                lista_tirar.Add(variable_tirada);
                ii++;
            }
            int jj = 0;
            int tt = 0;
            while (tt < lista_tirar.Count)
            {
                if (lista_tirar[jj]>=lista_tirar[tt])
                {
                    jj = tt;
                    deltatime = lista_tirar[jj];
                }
                tt++;
            }
            return deltatime;
        }
        //Function that, once the time-step is computed, it's added to the property of the nozzle and to the chart time list 
        public void SetDeltaTime(double deltatime)
        {
            this.deltatime = deltatime;
            this.TimeList.Add(Math.Round(TimeList[TimeList.Count-1]+deltatime,3));
        }
        //Function to compute the next values of Positions in the nozzle, using MacCormack method
        public void ComputeNextTime()
        {
            int t=0;
            int i = 0;
            while (t == 0)
            {
                if (GetPosition(i, 0) == null)
                    t = i;
                i++;
            }
            SetDeltaTime(ComputeDeltaTime(t-1));                    //Computation of the time-step value

            //Extraction of the properties' values
            List<double> ro_0 = createListDensity(t-1);
            List<double> V0 = createListVelocity(t-1);
            List<double> E0 = createListTemperature(t-1);
            List<double> A0 = createListArea(t-1);

            //Definition of the differences' guesses lists
            List<double> continuity = new List<double>();
            List<double> momentum = new List<double>();
            List<double> energy = new List<double>();

            //Computation of the first guess of the derivatives' values (forward differences)
            i = 0;
            while (i < ro_0.Count)
            {
                if (i == ro_0.Count - 1)
                {
                    continuity.Add(1.0);
                    momentum.Add(1.0);
                    energy.Add(1.0);
                }
                else
                {
                    double DifRo = -(ro_0[i] * (V0[i + 1] - V0[i])/ deltax) - (ro_0[i] * V0[i] * (Math.Log(A0[i + 1]) - Math.Log(A0[i])) / deltax) - (V0[i] * (ro_0[i + 1] - ro_0[i]) / deltax);
                    double DifV = -V0[i] * (V0[i + 1] - V0[i]) / deltax - (1 / gamma) * ((E0[i + 1] - E0[i]) / deltax + (E0[i] / ro_0[i]) * (ro_0[i + 1] - ro_0[i]) / deltax);
                    double DifE = -V0[i] * (E0[i + 1] - E0[i]) / deltax - (gamma - 1) * E0[i] * ((V0[i + 1] - V0[i]) / deltax + V0[i] * (Math.Log(A0[i + 1]) - Math.Log(A0[i])) / deltax);

                    continuity.Add(DifRo);
                    momentum.Add(DifV);
                    energy.Add(DifE);
                }
                i = i + 1;
            }
            List<double> DoAv = new List<double>();
            List<double> VoAv = new List<double>();
            List<double> EoAv = new List<double>();
            i = 0;
            while (i < ro_0.Count)
            {
                if (i == 0)
                {
                    //Inflow boundary conditions
                    DoAv.Add(1);
                    EoAv.Add(1);
                    VoAv.Add(1);
                }
                else if (i == ro_0.Count - 1)
                {
                    //Outflow boundary conditions (+ velocity inflow condition)
                    VoAv[0] = 2 * VoAv[1] - VoAv[2];
                    DoAv.Add(2 * VoAv[i - 1] - VoAv[i - 2]);
                    EoAv.Add(2 * EoAv[i - 1] - EoAv[i - 2]);
                    VoAv.Add(2 * DoAv[i - 1] - DoAv[i - 2]);
                }
                else
                {
                    DoAv.Add(ro_0[i] + continuity[i] * deltatime);
                    VoAv.Add(V0[i] + momentum[i] * deltatime);
                    EoAv.Add(E0[i] + energy[i] * deltatime);
                }
                i++;
            }

            List<double> continuityProm = new List<double>();
            List<double> momentumProm = new List<double>();
            List<double> energyProm = new List<double>();

            //Computation of the second guess of the derivatives' values (backwards differences)
            i = 0;
            while (i < ro_0.Count)
            {
                if (i == 0 || i== ro_0.Count-1)
                {
                    continuityProm.Add(1.0);
                    momentumProm.Add(1.0);
                    energyProm.Add(1.0);
                }
                else
                {
                    double DifRo = -(DoAv[i] * (VoAv[i] - VoAv[i-1]) / deltax) - (DoAv[i] * VoAv[i] * (Math.Log(A0[i]) - Math.Log(A0[i-1])) / deltax) - (VoAv[i] * (DoAv[i] - DoAv[i-1]) / deltax);
                    double DifV = -VoAv[i] * (VoAv[i] - VoAv[i-1]) / deltax - (1 / gamma) * ((EoAv[i] - EoAv[i-1]) / deltax + (EoAv[i] / DoAv[i]) * (DoAv[i] - DoAv[i-1]) / deltax);
                    double DifE = -VoAv[i] * (EoAv[i] - EoAv[i-1]) / deltax - (gamma - 1) * EoAv[i] * ((VoAv[i] - VoAv[i-1]) / deltax + VoAv[i] * (Math.Log(A0[i]) - Math.Log(A0[i-1])) / deltax);

                    continuityProm.Add(DifRo);
                    momentumProm.Add(DifV);
                    energyProm.Add(DifE);
                }
                i = i + 1;
            }
            List<double> continuityAv = new List<double>();
            List<double> momentumAv = new List<double>();
            List<double> energyAv = new List<double>();

            //Computation of the average between the two guesses
            i = 0;
            while (i < continuity.Count)
            {
                if (i == 0 || i == continuity.Count - 1)
                {
                    continuityAv.Add(1);
                    momentumAv.Add(1);
                    energyAv.Add(1);
                }
                else
                {
                    continuityAv.Add(0.5 * (continuity[i] + continuityProm[i]));
                    momentumAv.Add(0.5 * (momentum[i] + momentumProm[i]));
                    energyAv.Add(0.5 * (energy[i] + energyProm[i]));
                }
                i = i + 1;
            }
            List<double> D = new List<double>();
            List<double> V = new List<double>();
            List<double> E = new List<double>();
            i = 0;
            while (i < ro_0.Count)
            {
                if (i == 0)
                {
                    D.Add(1);
                    E.Add(1);
                    V.Add(1);
                }
                else if (i == ro_0.Count - 1)
                {
                    V[0] = 2 * V[1] - V[2];
                    D.Add(2 * D[i - 1] - D[i - 2]);
                    E.Add(2 * E[i - 1] - E[i - 2]);
                    V.Add(2 * V[i - 1] - V[i - 2]);
                }
                else
                {
                    D.Add(ro_0[i] + continuityAv[i] * deltatime);
                    V.Add(V0[i] + momentumAv[i] * deltatime);
                    E.Add(E0[i] + energyAv[i] * deltatime);
                }
                i++;
            }
            i = 0;
            
            while (i < D.Count)
            {
                Position actualPos = new Position(i * deltax, E[i], D[i], V[i], A0[i], i + 1);
                SetPosition(t, i, actualPos);
                i++;
            }
        }

        //Function to compute as many steps as one wants
        public void ComputeUntilPos(int finalPos)
        {
            int i = 1;
            while (i < finalPos)
            {
                ComputeNextTime();
                i++;
            }
        }


    }
}
