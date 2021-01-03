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
        int i;          //divisions of the nozzle
        double M;
        public double R = 286;

        public int I { get; set; }
        public double Position_x { get; set; }
        public double Area { get; set; }
        public double Density { get; set; }
        public double Velocity { get; set; }
        public double Temperature { get; set; }
        public double Pressure { get; set; }
        public double Mach { get; set; }

        //CONSTRUCTORS
        public Position(double x, double T, double ro, double V, double A, int i)
        {
            this.x = x;
            this.T = T;
            this.ro = ro;
            this.V = V;
            this.p = T*ro;
            this.A = A;
            this.M = MachNumber();

            this.I = i;
            this.Position_x = x;
            this.Area = A;
            this.Density = ro;
            this.Velocity = V;
            this.Temperature = T;
            this.Pressure = T * ro;
            this.Mach = V / Math.Sqrt(T);
        }

        public Position()
        { }

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
