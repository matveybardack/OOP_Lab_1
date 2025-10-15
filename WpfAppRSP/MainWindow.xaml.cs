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
using ClassLibraryRSP;

namespace WpfAppRSP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Размер поля
        private const int FieldSize = 128;

        private Field _Field;
        private double zoomSpeed = 1.1;
        public MainWindow()
        {
            InitializeComponent();
            _Field = new Field(FieldSize);
            UpdateCanvas();
        }

        private void border_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoom = e.Delta > 0 ? zoomSpeed : 1 / zoomSpeed;
            viewbox.Width *= zoom;
            viewbox.Height *= zoom;
        }

        /// <summary>
        /// Создание фигуры для отображения на Canvas
        /// </summary>
        /// <param name="figure">тип фигуры</param>
        /// <returns>фигура System.Windows.Shapes.Shape</returns>
        private System.Windows.Shapes.Shape CreateShape(ClassLibraryRSP.Shape figure)
        {
            System.Windows.Shapes.Shape shape;

            if (figure is Circle)
                shape = new Ellipse();
            else if (figure is Square)
                shape = new Rectangle();
            else // Triangle
            {
                shape = new Polygon
                {
                    Points = new PointCollection
                    {
                        new Point(0.5, 0), // Top-center
                        new Point(0, 1),   // Bottom-left
                        new Point(1, 1)    // Bottom-right
                    },
                    Stretch = Stretch.Fill
                };
            }

            Color fillColor = System.Windows.Media.Colors.Black;
            switch (figure.Color)
            {
                case ClassLibraryRSP.Colors.Red: fillColor = System.Windows.Media.Colors.Red; break;
                case ClassLibraryRSP.Colors.Yellow: fillColor = System.Windows.Media.Colors.Yellow; break;
                case ClassLibraryRSP.Colors.Blue: fillColor = System.Windows.Media.Colors.Blue; break;
            }
            shape.Fill = new SolidColorBrush(fillColor);

            shape.Stroke = new SolidColorBrush(System.Windows.Media.Colors.Black);
            shape.StrokeThickness = 1;

            return shape;
        }

        /// <summary>
        /// Отображение поля на Canvas
        /// </summary>
        private void GetField()
        {
            RSPCanvas.Children.Clear();

            double cellSize = RSPCanvas.Width / _Field.InnerField.GetLength(0);

            for (int x = 0; x < _Field.InnerField.GetLength(0); x++)
            {
                for (int y = 0; y < _Field.InnerField.GetLength(1); y++)
                {
                    ClassLibraryRSP.Shape figure = _Field.InnerField[x, y];
                    if (figure != null)
                    {
                        System.Windows.Shapes.Shape shape = CreateShape(figure);

                        Canvas.SetLeft(shape, x * cellSize);
                        Canvas.SetTop(shape, y * cellSize);

                        shape.Width = cellSize;
                        shape.Height = cellSize;

                        RSPCanvas.Children.Add(shape);
                    }
                }
            }
        }

        /// <summary>
        /// Обновление поля и очков
        /// </summary>
        private void UpdateCanvas()
        {
            GetField();
            lbScore.Content = $"Очки: {_Field.Points}";
        }

        /// <summary>
        /// Вперед на один ход
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _Field.MoveNext();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (ex.Message == "Game Over")
                {
                    btnForward.IsEnabled = false;
                    btnBack.IsEnabled = false;
                }
            }

            UpdateCanvas();
        }

        /// <summary>
        /// Назад на один ход
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _Field.Load();
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message); 
            }

            UpdateCanvas();
        }
    }
}
