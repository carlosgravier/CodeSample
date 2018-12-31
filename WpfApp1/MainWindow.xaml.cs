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
using Battleship;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SolidColorBrush _blackBrush = new SolidColorBrush(Colors.Black);
        SolidColorBrush _blueBrush = new SolidColorBrush(Colors.LightBlue);
        SolidColorBrush _grayBrush = new SolidColorBrush(Colors.DarkGray);
        SolidColorBrush _yellowBrush = new SolidColorBrush(Colors.Yellow);
        SolidColorBrush _redBrush = new SolidColorBrush(Colors.Red);
        SolidColorBrush _whiteBrush = new SolidColorBrush(Colors.White);
        List<Rectangle> _rectangles = new List<Rectangle>();
        List<TextBlock> _textblocks = new List<TextBlock>();
        Game game;
        TextBlock GameStateText;
        TextBlock HelpText;
        Color previouscolor;

        string[] labelsArray = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

        public MainWindow()
        {
            InitializeComponent();

            SetupNewGame();

            //ShowEnemyShipLocations();
        }

        private void SetupNewGame()
        {
            game = new Game();
            game.StartGame();

            CreateHelpText();
            CreateGameStateText();

            var width = 25;
            var height = 25;
            var marginleft = 50;
            var margintop = 75;
            var griddistance = 0;


            CreateGrid("target", width, height, marginleft, margintop, griddistance);
            CreateTargetGridLabels(width, marginleft, margintop, griddistance);
            griddistance = 300;
            CreateGrid("fleet", width, height, marginleft, margintop, griddistance);
            CreateTargetGridLabels(width, marginleft, margintop, griddistance);

            foreach (var rect in _rectangles)
            {
                GameCanvas.Children.Add(rect);
            }
        }

        private void CreateGameStateText()
        {
            ScrollViewer scrollviewer = new ScrollViewer();
            scrollviewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            GameStateText = new TextBlock();
            GameStateText.Width = 250;
            GameStateText.Height = 300;
            GameStateText.Foreground = Brushes.Navy;

            GameStateText.FontFamily = new FontFamily("Calibri");
            GameStateText.FontSize = 12;
            GameStateText.FontWeight = FontWeights.UltraBold;

            GameStateText.TextAlignment = TextAlignment.Center;
            GameStateText.TextWrapping = TextWrapping.Wrap;
            GameStateText.Typography.SlashedZero = true;
            GameStateText.Text = $"Placing ship : {game.CurrentShipToPlace.ShipName} \r\nSize : {game.CurrentShipToPlace.Size}";
            Canvas.SetLeft(scrollviewer, 325);
            Canvas.SetTop(scrollviewer, 200);

            scrollviewer.Content = GameStateText;
            GameCanvas.Children.Add(scrollviewer);
        }

        private void CreateHelpText()
        {
            HelpText = new TextBlock();
            HelpText.Text = "Place your ships on the game board by clicking on the lower grid.\r\n\r\nLeft click to add a ship horizontally.\r\nRight click to add a ship vertically.";
            HelpText.Width = 250;
            HelpText.TextWrapping = TextWrapping.Wrap;
            HelpText.TextAlignment = TextAlignment.Center;
            HelpText.FontSize = 12;
            HelpText.FontWeight = FontWeights.Bold;
            Canvas.SetLeft(HelpText, 325);
            Canvas.SetTop(HelpText, 75);
            GameCanvas.Children.Add(HelpText);
        }

        private void CreateTargetGridLabels(int width, int marginleft, int margintop, int griddistance)
        {
            var offset = width / 2;


            int i = 0;
            for (int j = 0; j < 10 * width; j += width)
            {
                var textBlock = new TextBlock();
                textBlock.Text = labelsArray[i];
                textBlock.TextAlignment = TextAlignment.Center;
                Canvas.SetLeft(textBlock, j + marginleft + offset);
                Canvas.SetTop(textBlock, 50 + griddistance);
                GameCanvas.Children.Add(textBlock);
                i++;
            }

            i = 1;
            for (int j = 0; j < 10 * width; j += width)
            {
                var textBlock = new TextBlock();
                textBlock.Text = i.ToString();
                textBlock.TextAlignment = TextAlignment.Right;
                Canvas.SetTop(textBlock, j + margintop + 5 + griddistance);
                Canvas.SetLeft(textBlock, 30);
                GameCanvas.Children.Add(textBlock);
                i++;
            }
        }

        private void CreateGrid(string gridname, int width, int height, int marginleft, int margintop, int griddistance)
        {
            int a = 0;
            int b = 0;
            for (int j = 0; j < 10 * height; j += height)
            {
                for (int i = 0; i < 10 * width; i += width)
                {

                    Rectangle rect = new Rectangle();
                    HitState.HitValue hitvalue = HitState.HitValue.Empty;
                    var gridSquare = new GridSquare(rect, hitvalue);
                    game.GridSquares.Add(gridSquare);
                    rect.StrokeThickness = 1;
                    rect.Stroke = _blackBrush;
                    rect.Width = width;
                    rect.Height = height;
                    rect.Fill = _blueBrush;
                    rect.Name = $"{gridname}_{b}_{a}";
                    if (gridname == "fleet")
                    {
                        rect.MouseLeftButtonDown += FleetGrid_MouseLeftButtonDown;
                        rect.MouseRightButtonDown += FleetGrid_MouseRightButtonDown;
                    }
                    Canvas.SetLeft(rect, i + marginleft);
                    Canvas.SetTop(rect, j + griddistance + margintop);
                    _rectangles.Add(rect);

                    b++;
                }
                a++;
                b = 0;
            }
        }

        private void ShowEnemyShipLocations()
        {
            foreach (Coordinate c in game.Enemy.PlayerOccupiedCoordinates)
            {
                Rectangle occupiedCoordinate = GameCanvas.Children.OfType<Rectangle>().Where(x => x.Name == $"target_{c.X}_{c.Y}").First();

                if (occupiedCoordinate != null)
                    occupiedCoordinate.Fill = _grayBrush;
            }
        }

        private void FleetGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle clickedRectangle = sender as Rectangle;
            var nameArray = clickedRectangle.Name.Split('_');
            var xcoord = Int32.Parse(nameArray[1]);
            var ycoord = Int32.Parse(nameArray[2]);
            game.SetupGameboardClick(xcoord, ycoord, Ship.ShipDirection.Vertical);

            DrawPlayer1Ships();
        }

        private void FleetGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle clickedRectangle = sender as Rectangle;
            var nameArray = clickedRectangle.Name.Split('_');
            var xcoord = Int32.Parse(nameArray[1]);
            var ycoord = Int32.Parse(nameArray[2]);
            game.SetupGameboardClick(xcoord, ycoord, Ship.ShipDirection.Horizontal);

            DrawPlayer1Ships();
        }

        private void DrawPlayer1Ships()
        {
            foreach (Coordinate c in game.Player1.PlayerOccupiedCoordinates)
            {
                Rectangle wantedNode = GameCanvas.Children.OfType<Rectangle>().Where(x => x.Name == $"fleet_{c.X}_{c.Y}").First();

                if (wantedNode != null)
                    wantedNode.Fill = _grayBrush;
            }

            if (game.CurrentGameMode == GameModes.PlayerFleetPlacement)
                GameStateText.Text = $"Placing ship : {game.CurrentShipToPlace.ShipName} \r\nSize : {game.CurrentShipToPlace.Size}";

            if (game.CurrentGameMode == GameModes.Battle)
            {
                GameStateText.Text = "";
                IEnumerable<Rectangle> fleetGridRectangles = GameCanvas.Children.OfType<Rectangle>().Where(x => x.Name.StartsWith("fleet"));
                foreach (Rectangle r in fleetGridRectangles)
                {
                    r.MouseLeftButtonDown -= FleetGrid_MouseLeftButtonDown;
                    r.MouseRightButtonDown -= FleetGrid_MouseRightButtonDown;
                }

                IEnumerable<Rectangle> targetGridRectangles = GameCanvas.Children.OfType<Rectangle>().Where(x => x.Name.StartsWith("target"));
                foreach (Rectangle r in targetGridRectangles)
                {
                    r.MouseLeftButtonDown += TargetGrid_MouseLeftButtonDown;
                    r.MouseEnter += TargetGridMouseEnter;
                    r.MouseLeave += TargetGridMouseLeave;
                }

                HelpText.Text = "Click on the top grid to fire at the enemy.\r\nA hit will appear red.\r\nA miss will appear white.";
            }
        }

        private void TargetGridMouseLeave(object sender, MouseEventArgs e)
        {
            Rectangle entered = sender as Rectangle;
            entered.Fill = new SolidColorBrush(previouscolor);
        }

        private void TargetGridMouseEnter(object sender, MouseEventArgs e)
        {
            Rectangle entered = sender as Rectangle;
            SolidColorBrush b = (SolidColorBrush)entered.Fill;
            previouscolor = b.Color;
            entered.Fill = _yellowBrush;
        }

        private void TargetGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle clicked = sender as Rectangle;
            var gridSquare = game.GridSquares.Where(x => x._rectangle == clicked).First();

            if (gridSquare.WasHit != HitState.HitValue.Empty)
                return;

            var IsGameOver = UpdateGameStateAfterFiring(game.Enemy, gridSquare);

            if (IsGameOver)
                GameOver();

            if (!IsGameOver)
            {
                gridSquare = game.EnemyGuess();
                IsGameOver = UpdateGameStateAfterFiring(game.Player1, gridSquare);


                if (IsGameOver)
                    GameOver();
            }
        }

        private void GameOver()
        {
            //remove all the click events from the target grid
            IEnumerable<Rectangle> targetGridRectangles = GameCanvas.Children.OfType<Rectangle>().Where(x => x.Name.StartsWith("target"));
            foreach (Rectangle r in targetGridRectangles)
            {
                r.MouseLeftButtonDown -= TargetGrid_MouseLeftButtonDown;
                r.MouseEnter -= TargetGridMouseEnter;
                r.MouseLeave -= TargetGridMouseLeave;
            }

            HelpText.Text = String.Empty;
            Button button = new Button();
            button.Width = 250;
            button.Height = 50;
            button.Content = "Play Again";
            button.Click += Button_Click;
            Canvas.SetLeft(button, 325);
            Canvas.SetTop(button, 75);
            GameCanvas.Children.Add(button);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GameCanvas.Children.Clear();
            SetupNewGame();
        }

        private bool UpdateGameStateAfterFiring(Player enemyPlayer, GridSquare gridSquare)
        {
            Player inversePlayer = enemyPlayer.PlayerName == "Player 1" ? game.Enemy : game.Player1;

            var nameArray = gridSquare._rectangle.Name.Split('_');
            var xcoord = Int32.Parse(nameArray[1]);
            var ycoord = Int32.Parse(nameArray[2]);

            Tuple<bool, bool, string, bool> wasHit = game.CheckFireResult(enemyPlayer, xcoord, ycoord);
            GameStateText.Text = $"{inversePlayer.PlayerName} fires at {labelsArray[xcoord]}, {ycoord + 1}\r\n" + GameStateText.Text;

            if (wasHit.Item1)
            {
                gridSquare.WasHit = HitState.HitValue.Hit;
                GameStateText.Text = "HIT!\r\n" + GameStateText.Text;
                gridSquare._rectangle.Fill = _redBrush;
                if (inversePlayer == game.Player1)
                    previouscolor = _redBrush.Color;
                if (wasHit.Item2)
                {
                    GameStateText.Text = $"{enemyPlayer.PlayerName} {wasHit.Item3} was sunk!\r\n" + GameStateText.Text;
                    game.Enemy.Fleet.Remove(enemyPlayer.Fleet.Where(x => x.ShipName == wasHit.Item3).First());

                    if (wasHit.Item4)
                    {
                        //the game is over
                        GameStateText.Text = $"{inversePlayer.PlayerName} WINS THE GAME!\r\n" + GameStateText.Text;
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
            {
                gridSquare._rectangle.Fill = _whiteBrush;
                if (inversePlayer == game.Player1)
                    previouscolor = _whiteBrush.Color;
                gridSquare.WasHit = HitState.HitValue.Missed;
                GameStateText.Text = "Missed.\r\n" + GameStateText.Text;
                return false;
            }
        }
    }
}