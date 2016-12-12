using Memory.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Memory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MemoryClass myMemory = null;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void play(object sender, EventArgs args)
        {
            plocaCanvas.Children.Clear();
            pocetakSP.Visibility = Visibility.Hidden;
            igraUTijeku.Visibility = Visibility.Visible;
            myMemory = new MemoryClass(10, name1TB.Text, name2TB.Text);
            name1.Text = myMemory.Igrac1;
            name2.Text = myMemory.Igrac2;
            igracNaPotezu.Text = myMemory.Igrac1;
            pogodak1.Text = Convert.ToString(myMemory.Pogodak1);
            pogodak2.Text = Convert.ToString(myMemory.Pogodak2);
            foreach (Line child in myMemory.nacrtajPlocu(plocaCanvas.ActualHeight, plocaCanvas.ActualWidth))
                plocaCanvas.Children.Add(child);
            myMemory.klik = new MouseButtonEventHandler(klikNaCanvas);
            plocaCanvas.MouseDown += myMemory.klik;
        }

        private void klikNaCanvas(object sender, MouseButtonEventArgs args)
        {
            Canvas myCanvas = sender as Canvas;
            var pos = args.GetPosition(myCanvas);
            bool success = true;
            if (myMemory.Brojac == 2)
                myMemory.Brojac = 0;

            if (myMemory.Brojac == 0)
                success = nacrtaj(pos);
            else
            {
                success = nacrtaj(pos);
                if (success)
                {
                    plocaCanvas.MouseDown -= myMemory.klik;
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(1000);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            ponistiRundu();
                            plocaCanvas.MouseDown += myMemory.klik;
                        }));

                    });
                }
            }
            if (success)
                ++myMemory.Brojac;
        }

        private bool nacrtaj(Point pos)
        {
            int selectedRedak = Convert.ToInt32(Math.Floor(pos.Y / myMemory.VisinaKucice));
            int selectedStupac = Convert.ToInt32(Math.Floor(pos.X / myMemory.SirinaKucice));
            int index = selectedRedak * 10 + selectedStupac;
            string imeSlicice = "Images/" + myMemory.Pozicije[index] + ".ico";
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(imeSlicice, UriKind.Relative));
            img.Width = myMemory.SirinaKucice;
            img.Height = myMemory.VisinaKucice;

            if (myMemory.Pozicije[index] == -1)
                return false;
            if (myMemory.Brojac == 0)
            {
                myMemory.oldValue = myMemory.Pozicije[index];
                myMemory.oldPosition = index;
                Canvas.SetTop(img, selectedRedak * myMemory.VisinaKucice);
                Canvas.SetLeft(img, selectedStupac * myMemory.SirinaKucice);
            }
            else
            {
                myMemory.newValue = myMemory.Pozicije[index];
                myMemory.newPosition = index;
                if (myMemory.newPosition == myMemory.oldPosition)
                    return false;
                Canvas.SetTop(img, selectedRedak * myMemory.VisinaKucice);
                Canvas.SetLeft(img, selectedStupac * myMemory.SirinaKucice);

            }
            plocaCanvas.Children.Add(img);
            return true;
        }

        private void ponistiRundu()
        {
            List<Image> images = plocaCanvas.Children.OfType<Image>().ToList();
            foreach (var image in images)
                plocaCanvas.Children.Remove(image);
            if (myMemory.oldValue == myMemory.newValue && myMemory.oldPosition != myMemory.newPosition)
            {
                nacrtajPravokutnik((myMemory.oldPosition / myMemory.Dim) * myMemory.VisinaKucice,
                                        (myMemory.oldPosition % myMemory.Dim) * myMemory.SirinaKucice);
                nacrtajPravokutnik((myMemory.newPosition / myMemory.Dim) * myMemory.VisinaKucice,
                                        (myMemory.newPosition % myMemory.Dim) * myMemory.SirinaKucice);
                myMemory.Pozicije[myMemory.oldPosition] = -1;
                myMemory.Pozicije[myMemory.newPosition] = -1;
                if (myMemory.Igrac1 == igracNaPotezu.Text)
                {
                    myMemory.Pogodak1++;
                    pogodak1.Text = Convert.ToString(myMemory.Pogodak1);
                }
                else
                {
                    myMemory.Pogodak2++;
                    pogodak2.Text = Convert.ToString(myMemory.Pogodak2);
                }
                //for (int i = 0; i < myMemory.Pozicije.Count; ++i)
                //    myMemory.Pozicije[i] = -1;
                provjeriKrajIgre();
            }
            else
            {
                if (myMemory.Igrac1 == igracNaPotezu.Text)
                    igracNaPotezu.Text = myMemory.Igrac2;
                else
                    igracNaPotezu.Text = myMemory.Igrac1;
            }

        }

        private void provjeriKrajIgre()
        {
            if (myMemory.krajIgre())
            {
                krajIgre.Visibility = Visibility.Visible;
                igraUTijeku.Visibility = Visibility.Hidden;
                pocetakSP.Visibility = Visibility.Hidden;
            }
        }


        private void nacrtajPravokutnik(double x, double y)
        {
            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Black);
            rect.Fill = new SolidColorBrush(Colors.Black);
            rect.Width = myMemory.SirinaKucice;
            rect.Height = myMemory.VisinaKucice;
            Canvas.SetLeft(rect, y);
            Canvas.SetTop(rect, x);
            plocaCanvas.Children.Add(rect);
        }

        private void crtajCanvasOpet(object sender, EventArgs args)
        {
            if (myMemory != null)
            {
                myMemory.Brojac = 0;
                List<Image> stareStvari = plocaCanvas.Children.OfType<Image>().ToList();
                plocaCanvas.Children.Clear();
                double staraVisina = myMemory.VisinaKucice;
                double staraSirina = myMemory.SirinaKucice;
                foreach (Line child in myMemory.nacrtajPlocu(plocaCanvas.ActualHeight, plocaCanvas.ActualWidth))
                    plocaCanvas.Children.Add(child);
                for (int i = 0; i < myMemory.Pozicije.Count(); ++i)
                {
                    if (myMemory.Pozicije[i] == -1)
                        nacrtajPravokutnik((i / myMemory.Dim) * myMemory.VisinaKucice, (i % myMemory.Dim) * myMemory.SirinaKucice);
                }
            }

        }

        private void igraJedanIgrac(object sender, EventArgs args)
        {
            drugiIgracSP.Visibility = Visibility.Hidden;
        }

        private void igraDvaIgraca(object sender, EventArgs args)
        {
            drugiIgracSP.Visibility = Visibility.Visible;
        }

        private void startNewGame(object sender, EventArgs args)
        {
            plocaCanvas.MouseDown -= myMemory.klik;
            myMemory = new MemoryClass(10, myMemory.Igrac1, myMemory.Igrac2);
            myMemory.klik = new MouseButtonEventHandler(klikNaCanvas);
            plocaCanvas.MouseDown += myMemory.klik;
            crtajCanvasOpet(null, null);
            igraUTijeku.Visibility = Visibility.Visible;
            krajIgre.Visibility = Visibility.Hidden;
        }

        private void goHome(object sender, EventArgs ergs)
        {
            plocaCanvas.MouseDown -= myMemory.klik;
            krajIgre.Visibility = Visibility.Hidden;
            igraUTijeku.Visibility = Visibility.Hidden;
            pocetakSP.Visibility = Visibility.Visible;
        }

    }
}
