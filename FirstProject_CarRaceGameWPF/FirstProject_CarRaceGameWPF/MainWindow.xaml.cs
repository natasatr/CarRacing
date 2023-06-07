using System;
using System.Collections.Generic;
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
using System.Windows.Threading; // importovanje dispatcher timer-a

namespace FirstProject_CarRaceGameWPF
{
   
    public partial class MainWindow : Window
    {
        //globalne varijable 

        DispatcherTimer tajmer = new DispatcherTimer();
        List<Rectangle> itemRemover = new List<Rectangle>();

        Random rand = new Random();
        ImageBrush igracSlika = new ImageBrush();
        ImageBrush dijamantSlika = new ImageBrush();
        ImageBrush bombaSlika = new ImageBrush();

        Rect playerHitBox;

        int brzina = 15;
        int brzinaIgraca = 10;
        int carNum;
        int diamondCounter = 30;
        int bombCounter = 10;
        int powerModeCounter = 200;


        double score;
        double i;

        bool moveLeft, moveRight, gameOver, powerMode;


        public MainWindow()
        {
            InitializeComponent();
            roadCanvas.Focus();// dozvoljava nam da osluskujemo dogadjaje keyUp i keyDown kada je igrica pokrenuta 
            tajmer.Tick += GameLoop;
            tajmer.Interval = TimeSpan.FromMilliseconds(20);

            pocetakIgre();
            
        }

        private void GameLoop(object sender, EventArgs e)
        {
            score += .05;
            diamondCounter -= 1;
            bombCounter -= 1;
            vrijemeText.Content = " Vrijeme " + score.ToString("#.#") + "sec";
            playerHitBox = new Rect(Canvas.GetLeft(igrac), Canvas.GetTop(igrac), igrac.Width, igrac.Height);

            if (moveLeft == true && Canvas.GetLeft(igrac) > 0) {
                Canvas.SetLeft(igrac, Canvas.GetLeft(igrac) - brzinaIgraca);
            }

            if (moveRight == true && Canvas.GetLeft(igrac) + 90 < Application.Current.MainWindow.Width) {
                Canvas.SetLeft(igrac, Canvas.GetLeft(igrac) + brzinaIgraca);

            }

            if (diamondCounter < 1) {
                kreirajDijamant();
                diamondCounter = rand.Next(600, 900);
            }
            if (bombCounter < 1)
            {
                kreirajBombu();
                bombCounter = rand.Next(600, 900);
            }

            foreach (var i in roadCanvas.Children.OfType<Rectangle>()) {
                if ((string)i.Tag == "linije") {
                    Canvas.SetTop(i, Canvas.GetTop(i) + brzina);

                    if (Canvas.GetTop(i) > 510) {
                        Canvas.SetTop(i, -152);
                    }
                }

                if ((string)i.Tag == "Auto") {

                    Canvas.SetTop(i, Canvas.GetTop(i) + brzina);

                    if (Canvas.GetTop(i) > 500) {

                        promjenaVozila(i);
                    }

                    Rect autoHitBox = new Rect(Canvas.GetLeft(i), Canvas.GetTop(i), i.Width, i.Height);
                    //kada nije ubrzna
                    if (playerHitBox.IntersectsWith(autoHitBox) && powerMode == true)
                    {

                        promjenaVozila(i);
                    }
                    else if (playerHitBox.IntersectsWith(autoHitBox) && powerMode == false) {

                        tajmer.Stop();
                        //vrijemeText.Content += " Press enter to replay!";
                        MessageBoxResult result = MessageBox.Show("Da li zelite opet igrati?",
                                    "Loser", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            vrijemeText.Content = "";
                            gameOver = true;
                        }
                        else
                        {
                            StartWindow sw = new StartWindow();
                            this.Close();
                            sw.ShowDialog();
                        }

                    }
                }

                if ((string)i.Tag == "dijamant") {

                    Canvas.SetTop(i, Canvas.GetTop(i) + 5);
                    Rect dijamantHit = new Rect(Canvas.GetLeft(i), Canvas.GetTop(i), i.Width, i.Height);
                    if (playerHitBox.IntersectsWith(dijamantHit))
                    {
                        itemRemover.Add(i);
                        powerMode = true;
                        powerModeCounter = 200;
                    }

                    if (Canvas.GetTop(i) > 400) {

                        itemRemover.Add(i);
                    }
                }

                if ((string)i.Tag == "bomba")
                {

                    Canvas.SetTop(i, Canvas.GetTop(i) + 5);
                    Rect bomaHit = new Rect(Canvas.GetLeft(i), Canvas.GetTop(i), i.Width, i.Height);
                    if (playerHitBox.IntersectsWith(bomaHit))
                    {
                        itemRemover.Add(i);
                        tajmer.Stop();
                        //vrijemeText.Content += " Press enter to replay!";
                        MessageBoxResult result = MessageBox.Show("Da li zelite opet igrati?",
                                    "Loser", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            vrijemeText.Content = "";
                            gameOver = true;
                        }
                        else {
                            StartWindow sw = new StartWindow();
                            this.Close();
                            sw.ShowDialog();
                        }
                        
                    }

                    if (Canvas.GetTop(i) > 400)
                    {

                        itemRemover.Add(i);
                    }
                }
            }

            if (powerMode == true)
            {
                powerModeCounter -= 1;
                powerUp();

                if (powerModeCounter < 1)
                {
                    powerMode = false;
                }
            }
            else {
                roadCanvas.Background = Brushes.Black ;
            }

            foreach (Rectangle r in itemRemover) {

                roadCanvas.Children.Remove(r);
                    
             }




            if (score > 20 && score < 40) {
                brzina = 15;
            }
           
        }

        private void keyDownEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left) {
                moveLeft = true;
            }
            if (e.Key == Key.Right) {
                moveRight = true;
            }
        }

        private void keyUpEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight = false;
            }

            if (e.Key == Key.Enter && gameOver == true) {
                pocetakIgre();
            }
        }

        private void pocetakIgre() {
            brzina = 8;
            tajmer.Start();
            moveLeft = false;
            moveRight = false;
            gameOver = false;
            powerMode = false;
            score = 0;
            vrijemeText.Content = "Vrijeme: 0sec";
            igracSlika.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/mainPlayer.png"));
            dijamantSlika.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/dijamant.png"));
            bombaSlika.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/head1.png"));

            igrac.Fill = igracSlika;

            roadCanvas.Background = Brushes.Black;

            foreach (var i in roadCanvas.Children.OfType<Rectangle>())
            {
                if ((string)i.Tag == "Auto") { 
                    Canvas.SetTop(i, (rand.Next(100, 400)* -1));
                    Canvas.SetLeft(i, rand.Next(0,430));
                    promjenaVozila(i);

                }

                if ((string)i.Tag == "dijamant") {
                    itemRemover.Add(i);
                }
                
                if ((string)i.Tag == "bomba") {
                    itemRemover.Add(i);
                }
            }

            itemRemover.Clear();//brise item-e ukljujuci i srcek
           
        }

        private void promjenaVozila(Rectangle rec)
        {

            carNum = rand.Next(1, 4);
            ImageBrush carImage = new ImageBrush();

            switch (carNum)
            {

                case 1:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/carGreen.png"));
                    break;
                case 2:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/carYellow.png"));
                    break;
                case 3:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/CarRed.png"));
                    break;

                case 4:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/carOrange.png"));
                    break;
              
            }

            rec.Fill = carImage;

            Canvas.SetTop(rec, (rand.Next(100, 400) * -1));
            Canvas.SetLeft(rec, rand.Next(0, 430));

        }

        private void powerUp() {
            i += .5;
            if (i > 4) 
                i = 1;

            igracSlika.ImageSource = new BitmapImage(new Uri("pack://application:,,,/resources/mainPlayer.png"));
            roadCanvas.Background = Brushes.LightCoral;
      }
        private void kreirajDijamant() {
            Rectangle dij = new Rectangle {
                Height = 50,
                Width = 50,
                Tag = "dijamant",
                Fill = dijamantSlika
            };

            Canvas.SetLeft(dij, rand.Next(0, 430));
            Canvas.SetTop(dij, (rand.Next(100, 400) * -1));
            roadCanvas.Children.Add(dij);
           

        }

        private void kreirajBombu()
        {
            Rectangle bomba = new Rectangle
            {
                Height = 50,
                Width = 50,
                Tag = "bomba",
                Fill = bombaSlika
            };

            Canvas.SetLeft(bomba, rand.Next(0, 430));
            Canvas.SetTop(bomba, (rand.Next(100, 400) * -1));
            roadCanvas.Children.Add(bomba);


        }
    }
}
