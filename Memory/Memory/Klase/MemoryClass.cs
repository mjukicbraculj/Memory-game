using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Memory.Klase
{
    class MemoryClass
    {
        public int Dim { get; set; }
        public List<int> Pozicije;
        private static Random rng = new Random();
        public double VisinaKucice { get; set; }
        public double SirinaKucice { get; set; }

        public string Igrac1 { get; set; }
        public string Igrac2 { get; set; }

        public int Pogodak1 { get; set; }
        public int Pogodak2 { get; set; }

        public int Brojac { get; set; }
        public int oldValue { get; set; }
        public int oldPosition { get; set; }
        public int newValue { get; set; }
        public int newPosition { get; set; }
        public MouseButtonEventHandler klik { get; set; }

        public MemoryClass(int dimenzija, string igrac1, string igrac2)
        {
            Dim = dimenzija;
            Igrac1 = igrac1;
            Igrac2 = igrac2;
            Pogodak1 = 0;
            Pogodak2 = 0;
            Pozicije = Enumerable.Range(0, 50).ToList();
            for (int i = 0; i < 50; ++i)
                Pozicije.Add(i);
            Pozicije = shuffle(Pozicije);
            Brojac = 0;
        }

        public List<Line> nacrtajPlocu(double visina, double sirina)
        {
            List<Line> myLines = new List<Line>();
            VisinaKucice = visina / Dim;
            SirinaKucice = sirina / Dim;
            for (int i = 0; i < Dim + 1; ++i)
            {
                Line myLine = new Line();
                myLine.Stroke = Brushes.Red;
                myLine.StrokeThickness = 2;
                myLine.X1 = 0;
                myLine.Y1 = i * VisinaKucice;
                myLine.X2 = sirina;
                myLine.Y2 = myLine.Y1;
                myLines.Add(myLine);
            }

            for (int i = 0; i < Dim + 1; ++i)
            {
                Line myLine = new Line();
                myLine.Stroke = Brushes.Red;
                myLine.StrokeThickness = 2;
                myLine.Y1 = 0;
                myLine.X1 = i * SirinaKucice;
                myLine.Y2 = visina;
                myLine.X2 = myLine.X1;
                myLines.Add(myLine);
            }
            return myLines;
        }

        public List<int> shuffle(List<int> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public bool krajIgre()
        {
            for (int i = 0; i < Pozicije.Count(); ++i)
                if (Pozicije[i] != -1)
                    return false;
            return true;
        }
    }
    
}
