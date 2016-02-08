using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tsosii_lab1
{
    class ObjectParameters
    {
        public int Perimeter { get; set; }          // Периметр
        public int Square { get; set; }             // Площадь
        public double Compactness { get; set; }     // Компактность

        public double Xmas { get; set; }            // Координата X центра масс
        public double Ymas { get; set; }            // Координата Y центра масс

        // Дискретные центральные моменты
        public double m20 { get; set; }
        public double m02 { get; set; }
        public double m11 { get; set; } 

        public double elongation { get; set; }              // Удлинённость
        public double OrientationOfMainAxis { get; set; }   // Ориентация главной оси


        public ObjectParameters()
        {
            Perimeter = Square = 0;
            Compactness = Xmas = Ymas = elongation = 0.0;
            m20 = m02 = m11 = elongation = OrientationOfMainAxis = 0.0;
        }

        public ObjectParameters(ObjectParameters obj)
        {
            Perimeter = obj.Perimeter;
            Square = obj.Square;
            Compactness = obj.Compactness;
            Xmas = obj.Xmas;
            Ymas = obj.Ymas;
            m20 = obj.m20;
            m02 = obj.m02;
            m11 = obj.m11;
            elongation = obj.elongation;
            OrientationOfMainAxis = obj.OrientationOfMainAxis;
        }
    }
}
