using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tetris.Items;

namespace Tetris
{
    public partial class MainWindow: Window
    {
        private const int CELL_SIZE = 25;
        private readonly int framesPerSecond = 50;
        private TimeSpan CalculateDelay => TimeSpan.FromMilliseconds(1000.0 / framesPerSecond);

        GameGrid gameGrid;

        private readonly Image[,] imageControls;
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/gg.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
        };
        private readonly ImageSource[] button_images = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/icon_play.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative))
        };

        public MainWindow()
        {
            InitializeComponent();
            gameGrid = new GameGrid(rows: 20, columns: 10);
            imageControls = InitGameCanvas();
            StartGameLoop();
        }
        private Image[,] InitGameCanvas()
        {
            int[][] matrix = gameGrid.Grid;
            Image[,] imageControls = new Image[matrix.Length, matrix[0].Length];

            for (int r = 0; r < matrix.Length; r++)
            {
                for (int c = 0; c < matrix[0].Length; c++)
                {
                    Image imageControl = new()
                    {
                        Width = CELL_SIZE,
                        Height = CELL_SIZE
                    };
                    Canvas.SetTop(imageControl, ((r - 2) * CELL_SIZE) + 50);
                    Canvas.SetLeft(imageControl, (c * CELL_SIZE) + 0);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileImages[0];
                }
            }

            return imageControls;
        }
        private void DrawNextFigureCanvas()
        {
            nextShapeCanvas.Children.Clear();

            Figure? next_figure = gameGrid.NextFigure;
            if (next_figure is null) return;
            Square[] container = next_figure.container;

            foreach (Square item in container)
            {
                int r = item.x;
                int c = item.y;

                Image imageControl = new()
                {
                    Width = CELL_SIZE,
                    Height = CELL_SIZE
                };
                Canvas.SetTop(imageControl, ((r - 2) * CELL_SIZE) + 50);
                Canvas.SetLeft(imageControl, (c * CELL_SIZE) + 0);
                nextShapeCanvas.Children.Add(imageControl);
                imageControl.Source = tileImages[item.Type];
            }
        }

        private async void StartGameLoop()
        {
            while (true)
            {
                while (!gameGrid.GameEnded && gameGrid.IsPlaying)
                {
                    await Task.Delay(CalculateDelay);
                    gameGrid.Update(CalculateDelay);
                    Draw();
                }
                await Task.Delay(100);
            }
        }               
        private void Draw()
        {
            var matrix = gameGrid.Grid;
            for (int r = 0; r < matrix.Length; r++)
            {
                for (int c = 0; c < matrix[0].Length; c++)
                {
                    int id = matrix[r][c];
                    imageControls[r, c].Source = tileImages[id];
                }
            }
            var figure = gameGrid.ActiveFigure;
            foreach (var item in figure.container)
            {
                imageControls[item.x, item.y].Source = tileImages[item.Type];
            }

            lines.Text = $"Burned lines: {gameGrid.BurnedLines}";
            level.Text = $"Level: {gameGrid.GameSpeed}";
            score.Text = gameGrid.Scores.ToString();

            DrawNextFigureCanvas();
        }

        private void GridKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                case Key.S:
                    gameGrid.TryMoveFigureDownFast();
                    break;
                case Key.Left:
                case Key.A:
                    gameGrid.TryMoveFigureLeft();
                    break;
                case Key.Right:
                case Key.D:
                    gameGrid.TryMoveFigureRight();
                    break;
                case Key.W:
                case Key.Up:
                    gameGrid.TryRotateFigure();
                    break;
            }
        }
        private void MainButtonClick(object sender, RoutedEventArgs e)
        {
            if (gameGrid.GameEnded)
            {
                gameGrid.StartNewGame();
            }
            else
            {
                gameGrid.IsPlaying = !gameGrid.IsPlaying;
            }
            img.Source = button_images[!gameGrid.GameEnded && gameGrid.IsPlaying ? 0 : 1];
        }
    }
}