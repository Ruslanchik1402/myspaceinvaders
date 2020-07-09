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

using System.Windows.Threading; /// таймер

namespace myspaceinvaders
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool goLeft, goRight = false;
        List<Rectangle> itemstoremove = new List<Rectangle>();
        int enemyImages = 0;
        int bulletTimer;
        int bulletTimerLimit = 90;
        int totalEnemeis;
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        ImageBrush playerSkin = new ImageBrush();
        int enemySpeed = 6;
        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer.Tick += gameEngine;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(20);
            dispatcherTimer.Start();
            playerSkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player.png"));
            player1.Fill = playerSkin;
            makeEnemies(15);
        }

        private void Canvas_KeyisDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = true;
            }
            if (e.Key == Key.Right)
            {
                goRight = true;
            }
        }

        private void Canvas_KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = false;
            }

            ///генерируем выстрел при нажатии на пробел
            if (e.Key == Key.Space)
            {
                
                itemstoremove.Clear();

              
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };
                ///пуля расположена там же, где и игрок
                Canvas.SetTop(newBullet, Canvas.GetTop(player1) - newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player1) + player1.Width / 2);
                myCanvas.Children.Add(newBullet);

            }
        }

        private void enemyBulletMaker(double x, double y)
        {
            Rectangle newEnemyBullet = new Rectangle
            {
                Tag = "enemyBullet",
                Height = 40,
                Width = 15,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                StrokeThickness = 5

            };
            Canvas.SetTop(newEnemyBullet, y);
            Canvas.SetLeft(newEnemyBullet, x);
            myCanvas.Children.Add(newEnemyBullet);
        }

        private void makeEnemies(int limit)
        {
           
            int left = 0;
            totalEnemeis = limit;


            for (int i = 0; i < limit; i++)
            {

                ImageBrush enemySkin = new ImageBrush();
                Rectangle newEnemy = new Rectangle
                {
                    Tag = "enemy",
                    Height = 45,
                    Width = 45,
                    Fill = enemySkin,

                };

              
                Canvas.SetTop(newEnemy, 10); 
                Canvas.SetLeft(newEnemy, left); 
                myCanvas.Children.Add(newEnemy);
                left -= 60;

                enemyImages++;

                if (enemyImages > 8)
                {
                    enemyImages = 1;
                }
                switch (enemyImages)
                {
                    case 1:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader1.gif"));
                       
                        break;
                    case 2:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader2.gif"));
                        
                        break;
                    case 3:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader3.gif"));
                        break;
                    case 4:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader4.gif"));
                        break;
                    case 5:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader5.gif"));
                        break;
                    case 6:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader6.gif"));
                        break;
                    case 7:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader7.gif"));
                        break;
                    case 8:
                        enemySkin.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/invader8.gif"));
                        break;
                }
            }
        }


        //срабатывает каждые 20 мс.
        private void gameEngine(object sender, EventArgs e)
        {
            //создаем объект с игроком
            Rect player = new Rect(Canvas.GetLeft(player1), Canvas.GetTop(player1), player1.Width, player1.Height);
            //заполняем лейбл числом оставшихся врагов
            enemiesLeft.Content = "Invaders Left: " + totalEnemeis;
            //проверяем есть ли возможность движения вправо или влево, в зависимости от прожатой кнопки перемещаем объект игрока
            if (goLeft && Canvas.GetLeft(player1) > 0)
            {
                Canvas.SetLeft(player1, Canvas.GetLeft(player1) - 10);
            }
            else if (goRight && Canvas.GetLeft(player1) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player1, Canvas.GetLeft(player1) + 10);
            }

            bulletTimer -= 3;


            if (bulletTimer < 0)
            {
                enemyBulletMaker((Canvas.GetLeft(player1) + 20), 10);
                bulletTimer = bulletTimerLimit;
            }

            if (totalEnemeis < 10)
            {
                enemySpeed = 10;
            }


            //цикл для детей канваса
            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                //если объект - пуля
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    //перемещаем пулю игрока по направлению к врагам
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);
                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    //если пуля достигла верхнего предела, убираем ее в контейнер для ненужных объектов
                    if (Canvas.GetTop(x) < 10)
                    {
                        itemstoremove.Add(x);
                    }
                    //проверяем попадание по врагу
                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemy = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
                            //если пуля попадает во врага, то отправляем и пулю, и врага в контейнер для удаления
                            if (bullet.IntersectsWith(enemy))
                            {
                                itemstoremove.Add(x);
                                itemstoremove.Add(y);
                                totalEnemeis -= 1;
                            }
                        }


                    }

                }

                //если объект - враг
                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    //передвигаем вправо 
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + enemySpeed);
                    //если враги достигли правого края, смещаем их ниже, они начнут двигаться слева снова
                    if (Canvas.GetLeft(x) > 820)
                    {
                        Canvas.SetLeft(x, -80);
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
                    }
                    Rect enemy = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    //если же враг добрался до игрока, то пользователь проиграл
                    if (player.IntersectsWith(enemy))
                    {
                        dispatcherTimer.Stop();
                        MessageBox.Show("Wasted");
                    }
                }
                //если объект - пуля врага
                if (x is Rectangle && (string)x.Tag == "enemyBullet")
                {
                    //перемещаем пулю ниже, к игроку
                    Canvas.SetTop(x, Canvas.GetTop(x) + 10);
                    if (Canvas.GetTop(x) > 480)
                    //если пуля дошла до нижнего края канваса, но не настигла игрока, перемещаем в контейнер для удаления
                    {
                        itemstoremove.Add(x);

                    }
                    Rect enemyBullets = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    //если же пуля настигла игрока, то он проиграл
                    if (enemyBullets.IntersectsWith(player))

                    {

                        dispatcherTimer.Stop();
                        MessageBox.Show("Wasted");
                    }

                }
            }
            //очищаем контейнер
            foreach (Rectangle y in itemstoremove)
            {
                myCanvas.Children.Remove(y);
            }
            //если же врагов не осталось, игрок одерживает победу
            if (totalEnemeis < 1)
            {
                dispatcherTimer.Stop();
                MessageBox.Show("Win");
            }
        }
    }
}
