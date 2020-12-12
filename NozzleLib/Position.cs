using System;

namespace NozzleLib
{
    public class Position
    {
        double x;       //value of the position of the cell in x    
        double T;       //temperature in the cell
        double ro;      //density in the cell
        double V;       //velocity in the cell
        double p;       //pressure in the cell
        double A;       //area in the position of the cell
        double M;
        double R = 286;
        //CONSTRUCTORS
        public Position(double x, double T, double ro, double V, double A)
        {
            this.x = x;
            this.T = T;
            this.ro = ro;
            this.V = V;
            this.p = T*ro*R;
            this.A = A;
            this.M = MachNumber();
        }



        //FUNCTIONS
        //Variable Extraction
        public double GetX()
        {
            return this.x;
        }
        public double GetTemperature()
        {
            return this.T;
        }
        public double GetDensity()
        {
            return this.ro;
        }
        public double GetVelocity()
        {
            return this.V;
        }
        public double GetPressure()
        {
            return this.p;
        }
        public double GetArea()
        {
            return this.A;
        }

        //Variable Setting
        public void SetX(double x)
        {
            this.x = x;
        }
        public void SetDensity(double newro)
        {
            this.ro = newro;
        }
        public void SetTemperature(double newT)
        {
            this.T = newT;
            M = MachNumber();
        }
        public void SetVelocity(double newV)
        {
            this.V = newV;
            M = MachNumber();
        }
        public void SetPressure(double newP)
        {
            this.p = newP;
        }
        public void SetA(double A)
        {
            this.A = A;
        }

        //Math
        public double LnA()
        {
            return Math.Log(this.A);
        }
        public double Speedofsound()
        {
            double a = Math.Sqrt(this.T);
            return a;
        }
        public double Deltatime(double C, double deltax)
        {
            double deltatime = C * deltax / (this.Speedofsound() + this.V);
            return deltatime;
        }
        public double MachNumber()
        {
            double Mach = this.V / this.Speedofsound();
            return Mach;
        }
    }
}
