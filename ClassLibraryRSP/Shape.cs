using System;
using System.Collections.Generic;

namespace ClassLibraryRSP
{
    /// <summary>
    /// Абстрактный класс "Фигура"
    /// </summary>
    public abstract class Shape
    {
        protected uint age = 0; // возраст фигуры
        protected Colors color; // цвет фигуры
        private Random rnd_1 = new Random();

        public Colors Color { get { return color; } }

        protected Shape() 
        {
            color = (Colors)rnd_1.Next(3);
        }

        /// <summary>
        /// Сравнение фигуры "камень-ножницы-бумага"
        /// </summary>
        /// <param name="other">передаваемая фигура</param>
        /// <returns>сильнейшую из двух фигур</returns>
        public abstract Shape GetShape(Shape other);

        /// <summary>
        /// Выбор случайного цвета
        /// </summary>
        protected void RandColor()
        {
            Random rnd = new Random();

            double probability = (double)age / (age + 10);
            if (rnd.NextDouble() < probability)
            {
                color = (Colors)rnd.Next(3);
            }
        }

        /// <summary>
        /// Следующий ход (увеличение возраста и возможная смена цвета)
        /// </summary>
        public void Next()
        {
            this.age++;
            RandColor();
        }
    }

    public enum Colors
    {
        Blue,
        Red,
        Yellow,
    }
}
