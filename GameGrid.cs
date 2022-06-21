using System;
using System.Linq;
using Tetris.Utilities;

namespace Tetris
{
    class GameGrid
    {
        public bool IsPlaying { get; set; }
        public bool GameEnded { get; private set; }
        public int GameSpeed { get; private set; } = 1;
        private double position;
        public int BurnedLines { get; private set; }
        public int Scores { get; private set; }
        public int[][] Grid { get; }
        public int Rows { get; }
        public int Columns { get; }
        public Figure ActiveFigure { get; private set; }
        public Figure NextFigure { get; private set; }

        #pragma warning disable CS8618
        public GameGrid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Grid = new int[Rows][];
            for (int i = 0; i < Grid.Length; i++)
            {
                Grid[i] = new int[Columns];
            }
            StartNewGame();
        }


        // оновлення
        public void Update(TimeSpan deltaTime)
        {
            position += deltaTime.TotalSeconds * GameSpeed * 3;
            int moveDownOn = (int)Math.Truncate(position);
            position -= moveDownOn;
            for (int i = 0; i < moveDownOn; i++)
            {
                if (ActiveFigure.MoveDown(Grid))
                {
                    GetNextFigure();
                    return;
                }
            }
        }
        private void GetNextFigure()
        {
            ActiveFigure.EndMove(Grid);
            BurnLinesCheck();

            if (!LineUtility.IsEmpty(0, Grid)) // перевырка програшу
            {
                GameEnded = true;
                return;
            }

            // Прискорювати
            GameSpeed = (BurnedLines / 20) + 1;

            // Отримати далі
            ActiveFigure = NextFigure;
            NextFigure = FigureUtility.GenerateRandomFigure();
        }
        private void BurnLinesCheck()
        {
            // перевірити та видалити рядки
            var lines = Enumerable.Range(0, Rows);

            int lines_count = 0;
            bool flag = true;
            while (flag)
            {
                flag = false;
                foreach (int line in lines)
                {
                    if (LineUtility.IsFull(line, Grid))
                    {
                        LineUtility.DeleteLine(line, Grid);
                        lines_count++;
                        flag = true;
                        break;
                    }
                }
            }

            // підрахунок балів
            BurnedLines += lines_count;
            if (lines_count == 1)
            {
                Scores += 100;
            }
            else if (lines_count == 2)
            {
                Scores += 300;
            }
            else if (lines_count == 3)
            {
                Scores += 700;
            }
            else if (lines_count == 4)
            {
                Scores += 1500;
            }
        }


        // контроль
        public void TryMoveFigureRight() => ActiveFigure?.MoveRight(Grid);
        public void TryMoveFigureLeft() => ActiveFigure?.MoveLeft(Grid);
        public void TryMoveFigureDownFast()
        {
            while (true)
            {
                if (ActiveFigure.MoveDown(Grid))
                {
                    GetNextFigure();
                    return;
                }
            }
        }
        public void TryRotateFigure() => ActiveFigure?.Rotate(Grid);
        public void StartNewGame()
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                for (int j = 0; j < Grid[0].Length; j++)
                {
                    Grid[i][j] = 0;
                }
            }
            ActiveFigure = FigureUtility.GenerateRandomFigure();
            NextFigure = FigureUtility.GenerateRandomFigure();

            GameEnded = false;
            IsPlaying = true;

            BurnedLines = 0;
            GameSpeed = 1;
            position = 0;
            Scores = 0;
        }
    }
}