using System;
using System.Collections.Generic;
using System.Text;

namespace NozzleLib
{
    public class Nozzle
    {
        double gamma = 1.4;
        double R = 286;
        double deltax;
        double deltatime;
        List<double> TimeList= new List<double> { 0 };
        double C;                       //Courant value
        Position[,] malla;              //Position matrix rows=time steps and columns=space divisions
        int N;                     //Number of space divisions, 31 by default (Anderson value)
        double[] dimensionalvalues;     //Initial values to obtain dimensional values to magnitudes [L, T0, a0, p0, ro0]
        double throatposition;          //where is the throat

        public Nozzle(double L, double T0, double ro0, double C, int N)
        {
            SetDimensionalValues(L, T0, ro0);
            this.throatposition = L / 2;
            this.deltax = L / (N-1);
            this.N = N;
            this.C = C;
            this.malla = new Position[1401, N];
            int i = 0;
            while (i < N)
            {


                double xi = 0 + i * deltax;
                double temp = 1 - 0.2314 * xi;
                Position pos = new Position(xi, temp, 1 - 0.3146 * xi, (0.1 + 1.09 * xi) * Math.Sqrt(temp), 1 + 2.2 * Math.Pow(xi - throatposition, 2));
                SetPosition(0, i, pos);
                i++;
            }
        }

        //FUNCTIONS
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
        public List<double> getTimeList()
        {
            return TimeList;
        }
        public List<double> getTimeList(int steps)
        {
            List<double> Times = new List<double>();
            int i = 0;
            int initStep = 0;
            while (i < this.TimeList.Count)
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
        public void SetDimensionalValues(double L, double T0, double ro0)
        {
            dimensionalvalues = new double[5];
            dimensionalvalues[0] = L;
            dimensionalvalues[1] = T0;
            dimensionalvalues[2] = Math.Sqrt(gamma * R * T0);
            dimensionalvalues[3] = T0*ro0*R;
            dimensionalvalues[4] = ro0;
        }
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
        public List<double> GetRowPar(int row, string parameter)
        {
            List<double> fila = new List<double>();
            int i = 0;
            while (i < N)
            {
                Position pos = GetPosition(row, i);
                if (pos != null)
                {
                    double value;
                    if (parameter == "x")
                        value = pos.GetX();
                    else if (parameter == "T")
                        value = pos.GetTemperature();
                    else if (parameter == "D")
                        value = pos.GetDensity();
                    else if (parameter == "V")
                        value = pos.GetVelocity();
                    else if (parameter == "P")
                        value = pos.GetPressure();
                    else if (parameter == "A")
                        value = pos.GetArea();
                    else if (parameter == "M")
                        value = pos.MachNumber();
                    else
                        value = -2;

                    if (value != -2)
                        fila.Add(value);
                    i++;
                }
                else
                {
                    break;
                }
            }
            return fila;
        }
        public List<double> GetColumnPar(int col, string parameter, int steps)
        {
            List<double> columna = new List<double>();
            int i = 0;
            int initStep = 0;
            while (i < malla.GetLength(0))
            {
                if (initStep == 0)
                {
                    Position pos = GetPosition(i, col);
                    if (pos != null)
                    {
                        double value;
                        if (parameter == "x")
                            value = Math.Round(pos.GetX(), 4);
                        else if (parameter == "T")
                            value = Math.Round(pos.GetTemperature(), 4);
                        else if (parameter == "D")
                            value = Math.Round(pos.GetDensity(), 4);
                        else if (parameter == "V")
                            value = Math.Round(pos.GetVelocity(), 4);
                        else if (parameter == "P")
                            value = Math.Round(pos.GetPressure(), 4);
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
                if (initStep == steps)
                {
                    initStep = -1;
                }
                initStep++;
                i++;
            }
            return columna;
        }
        public List<Position> GetColumn(int col)
        {
            List<Position> columna = new List<Position>();
            int i = 0;
            while (i < malla.GetLength(0))
            {
                if (i == col)
                {
                    int j = 0;
                    while (j < malla.GetLength(1))
                    {
                        columna[j] = malla[j, i];
                        j++;
                    }
                    return columna;
                }
                i++;
            }
            return null;
        }
        public Position[,] Getmalla()
        {
            return this.malla;
        }
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
        public void SetDeltaTime(double deltatime)
        {
            this.deltatime = deltatime;
            this.TimeList.Add(Math.Round(TimeList[TimeList.Count-1]+deltatime,3));
        }
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
            SetDeltaTime(ComputeDeltaTime(t-1));
            // List<double> ro_0, List<double> V0, List<double> E0, List<double> A0
            List<double> ro_0 = createListDensity(t-1);
            List<double> V0 = createListVelocity(t-1);
            List<double> E0 = createListTemperature(t-1);
            List<double> A0 = createListArea(t-1);

            List<double> continuity = new List<double>();
            List<double> momentum = new List<double>();
            List<double> energy = new List<double>();
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
                    DoAv.Add(1);
                    EoAv.Add(1);
                    VoAv.Add(1);
                }
                else if (i == ro_0.Count - 1)
                {
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
                Position actualPos = new Position(i * deltax, E[i], D[i], V[i], A0[i]);
                SetPosition(t, i, actualPos);
                i++;
            }
        }
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
