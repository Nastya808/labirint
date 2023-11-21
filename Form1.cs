using System;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace MazeGame
{
    public partial class LoadMapForm : Form
    {
        private int[,] array2D;

        public LoadMapForm()
        {
            InitializeForm();
            AddControls();
        }

        private void InitializeForm()
        {
            Size = new Size(400, 400);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
        }

        private void AddControls()
        {
            Label label = new Label
            {
                Text = "Выберите файл для загрузки лабиринта",
                Font = new Font("Verdana", 12),
                AutoSize = true
            };

            Controls.Add(label);
            CenterControl(label);

            Button loadMapButton = new Button
            {
                Text = "Загрузить",
                Font = new Font("Verdana", 12),
                AutoSize = true
            };

            Controls.Add(loadMapButton);
            CenterControl(loadMapButton, label);

            Label labelInfo = new Label
            {
                Font = new Font("Verdana", 12),
                AutoSize = true
            };

            Controls.Add(labelInfo);
            CenterControl(labelInfo, loadMapButton);

            Button startGameButton = new Button
            {
                Text = "Начать игру",
                Font = new Font("Verdana", 16),
                AutoSize = true,
                Enabled = false
            };

            Controls.Add(startGameButton);
            CenterControl(startGameButton, labelInfo, 5);

            loadMapButton.Click += (sender, e) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "txt files (*.txt)|*.txt"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        LoadMapFromFile(openFileDialog.FileName);
                        labelInfo.Text = $"Файл загружен: {Path.GetFileName(openFileDialog.FileName)}";
                        CenterControl(labelInfo, loadMapButton);
                        startGameButton.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки файла: {ex.Message}");
                    }
                }
            };

            startGameButton.Click += (sender, e) =>
            {
                GameForm gameForm = new GameForm(array2D);
                gameForm.ShowDialog();
            };
        }

        private void LoadMapFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            array2D = new int[lines.Length, lines[0].Split(',').Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');

                for (int j = 0; j < values.Length; j++)
                {
                    array2D[i, j] = int.Parse(values[j].Trim());
                }
            }
        }

        private void CenterControl(Control currentControl, Control lastControl = null, int koeff = 1)
        {
            int centerX = (ClientSize.Width - currentControl.Width) / 2;
            int centerY = lastControl != null ? lastControl.Bottom : currentControl.Height;

            if (lastControl != null)
            {
                currentControl.Location = new Point(centerX, centerY + lastControl.Height * koeff);
            }
            else
            {
                currentControl.Location = new Point(centerX, centerY);
            }
        }
    }

    public partial class GameForm : Form
    {
        private readonly int CellSize;
        private readonly int MazeSize;

        private int playerX = 1;
        private int playerY = 0;

        private int[,] array2D;

        public GameForm(int[,] array2D)
        {
            InitializeComponent();
            this.array2D = array2D;
            CellSize = array2D.GetLength(0) * 2;
            MazeSize = array2D.GetLength(1);
            InitializeMaze();
        }

        private void InitializeMaze()
        {
            ClientSize = new Size(MazeSize * CellSize, MazeSize * CellSize);
            Text = "Labirint Game";
            Paint += GameForm_Paint;
            KeyDown += GameForm_KeyDown;
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int i = 0; i < array2D.GetLength(0); i++)
            {
                for (int j = 0; j < array2D.GetLength(1); j++)
                {
                    Brush brush = array2D[i, j] == 1 ? Brushes.Black : Brushes.White;
                    g.FillRectangle(brush, j * CellSize, i * CellSize, CellSize, CellSize);
                }
            }

            g.FillRectangle(Brushes.Red, playerX * CellSize, playerY * CellSize, CellSize, CellSize);
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            int newX = playerX;
            int newY = playerY;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    newY = Math.Max(0, playerY - 1);
                    break;
                case Keys.Down:
                    newY = Math.Min(MazeSize - 1, playerY + 1);
                    break;
                case Keys.Left:
                    newX = Math.Max(0, playerX - 1);
                    break;
                case Keys.Right:
                    newX = Math.Min(MazeSize - 1, playerX + 1);
                    break;
            }

            if (array2D[newY, newX] == 0)
            {
                playerX = newX;
                playerY = newY;
                Invalidate();
            }

            if (playerX == array2D.GetLength(1) - 1 || playerY == array2D.GetLength(0) - 1)
            {
                MessageBox.Show("Лабиринт пройден!");
                Close();
            }
        }
    }
}
