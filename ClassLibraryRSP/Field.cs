using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRSP
{
    public struct ShapeInField
    {
        public Shape shape;
        public (int, int) position;
    }

    public class Field
    {
        public Field(uint size)
        {
            SIZE = size;
            field = new Shape[size, size];
            Save(); // Сохраняем начальное состояние поля
        }

        private uint SIZE; // размер игрового поля
        private Shape[,] field; // само поле

        private int story = 0;
        private List<List<ShapeInField>> storyFields = new List<List<ShapeInField>>(); //история игры

        private int points = 0; // очки
        private List<int> storyPoints = new List<int>() { 0 }; // история очков

        public int Points { get { return points; } }
        public Shape[,] InnerField { get { return field; } }
            
        /// <summary>
        /// Сохранение поля ()
        /// </summary>
        private void Save()
        {
            storyPoints.Add(points);

            List<ShapeInField> _field = new List<ShapeInField>();

            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    if (field[i, j] != null)
                        _field.Add(new ShapeInField
                        {
                            shape = field[i, j],
                            position = (i, j),
                        });
                }
            }

            storyFields.Add(_field);
            story++;
        }

        /// <summary>
        /// Выгрузка поля (ход назад)
        /// </summary>
        public void Load()
        {
            if (story == 0)
                return;

            story--;
            if (story < 0 || story >= storyFields.Count)
                throw new Exception("Некорректный индекс истории!"); // На всякий случай (недостижимо должго быть)

            List<ShapeInField> obj = storyFields[story];
            points = storyPoints[story];

            field = new Shape[SIZE, SIZE];

            foreach (ShapeInField _field in obj)
                field[_field.position.Item1, _field.position.Item2] = _field.shape;
        }

        /// <summary>
        /// Ход вперёд
        /// </summary>
        public void MoveNext()
        {
            // Если есть сохранённое "будущее" состояние — просто загружаем его
            if (story < storyFields.Count - 1)
            {
                story++;
                List<ShapeInField> obj = storyFields[story];
                points = storyPoints[story];

                field = new Shape[SIZE, SIZE];
                foreach (ShapeInField _field in obj)
                    field[_field.position.Item1, _field.position.Item2] = _field.shape;

                if (IsGameOver())
                    throw new Exception("Game Over");

                return;
            }

            MoveShapes();
            NewShapes();
            FindAndRemoveGroups();

            Save(); // Сохраняем состояние поля и очков ПОСЛЕ изменений

            if (IsGameOver())
                throw new Exception("Game Over");
        }

        /// <summary>
        /// Появление новых фигур в пустых клетках
        /// </summary>
        private void NewShapes()
        {
            Random rnd = new Random();

            int null_count = 0;

            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                    if (field[i, j] is null)
                        null_count++;

            int newShapesCount = rnd.Next(0, null_count + 1);

            for (int n = 0; n < newShapesCount; n++)
            {
                int x, y;
                do
                {
                    x = rnd.Next((int)SIZE);
                    y = rnd.Next((int)SIZE);
                } while (field[x, y] != null);

                Shape shape;
                int type = rnd.Next(3);
                switch (type)
                {
                    case 0: shape = new Triangle(); break;
                    case 1: shape = new Circle(); break;
                    default: shape = new Square(); break;
                }
                field[x, y] = shape;
            }
        }

        /// <summary>
        /// Движение фигур по полю
        /// </summary>
        private void MoveShapes()
        {
            // Для предотвращения двойного движения
            Shape[,] newField = new Shape[SIZE, SIZE];
            Random rnd = new Random();

            // Фигуры стареют
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                    if (field[i, j] != null)
                        field[i, j].Next();

            // Перебираем все клетки и двигаем фигуры
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    if (field[i, j] == null)
                        continue;

                    // Случайное направление: 0-вверх, 1-вниз, 2-влево, 3-вправо, 4-остаться
                    int dir = rnd.Next(5);
                    int ni = i, nj = j;
                    switch (dir)
                    {
                        case 0: ni = i > 0 ? i - 1 : i; break;
                        case 1: ni = i < SIZE - 1 ? i + 1 : i; break;
                        case 2: nj = j > 0 ? j - 1 : j; break;
                        case 3: nj = j < SIZE - 1 ? j + 1 : j; break;
                        case 4: break; // не двигаем
                    }

                    // Применяем правила замены
                    newField[ni, nj] = field[i, j].GetShape(newField[ni, nj]);
                }
            }

            field = newField;
        }

        /// <summary>
        /// Сбор очков за линии из 3+ одинаковых фигур по типу или цвету (по строкам и столбцам)
        /// </summary>
        private void FindAndRemoveGroups()
        {
            bool[,] toRemove = new bool[SIZE, SIZE];
            int removedCount = 0;

            // Проверка по строкам
            for (int i = 0; i < SIZE; i++)
            {
                int j = 0;
                while (j < SIZE)
                {
                    int start = j;
                    if (field[i, j] == null)
                    {
                        j++;
                        continue;
                    }
                    // Поиск по типу
                    Type type = field[i, j].GetType();
                    int k = j + 1;
                    while (k < SIZE && field[i, k] != null && field[i, k].GetType() == type)
                        k++;
                    if (k - j >= 3)
                        for (int m = j; m < k; m++)
                            toRemove[i, m] = true;
                    // Поиск по цвету
                    Colors color = field[i, j].Color;
                    k = j + 1;
                    while (k < SIZE && field[i, k] != null && field[i, k].Color == color)
                        k++;
                    if (k - j >= 3)
                        for (int m = j; m < k; m++)
                            toRemove[i, m] = true;
                    j = Math.Max(k, start + 1);
                }
            }

            // Проверка по столбцам
            for (int j = 0; j < SIZE; j++)
            {
                int i = 0;
                while (i < SIZE)
                {
                    int start = i;
                    if (field[i, j] == null)
                    {
                        i++;
                        continue;
                    }
                    // Поиск по типу
                    Type type = field[i, j].GetType();
                    int k = i + 1;
                    while (k < SIZE && field[k, j] != null && field[k, j].GetType() == type)
                        k++;
                    if (k - i >= 3)
                        for (int m = i; m < k; m++)
                            toRemove[m, j] = true;
                    // Поиск по цвету
                    Colors color = field[i, j].Color;
                    k = i + 1;
                    while (k < SIZE && field[k, j] != null && field[k, j].Color == color)
                        k++;
                    if (k - i >= 3)
                        for (int m = i; m < k; m++)
                            toRemove[m, j] = true;
                    i = Math.Max(k, start + 1);
                }
            }

            // Удаление и подсчёт очков
            for (int i = 0; i < SIZE; i++)
                for (int j = 0; j < SIZE; j++)
                    if (toRemove[i, j] && field[i, j] != null)
                    {
                        field[i, j] = null;
                        removedCount++;
                    }

            points += removedCount;
        }

        /// <summary>
        /// Поиск группы одинаковых фигур с помощью DFS
        /// </summary>
        /// <param name="i">строка</param>
        /// <param name="j">столбец</param>
        /// <param name="shape">фигура</param>
        /// <param name="visited">история посещений</param>
        /// <param name="group">собранная группа</param>
        private void DFS(int i, int j, Shape shape, bool[,] visited, List<(int, int)> group)
        {
            if (i < 0 || i >= SIZE || j < 0 || j >= SIZE)
                return;
            if (visited[i, j] || field[i, j] == null)
                return;
            if (field[i, j].GetType() != shape.GetType() || field[i, j].Color != shape.Color)
                return;

            visited[i, j] = true;
            group.Add((i, j));

            DFS(i - 1, j, shape, visited, group);
            DFS(i + 1, j, shape, visited, group);
            DFS(i, j - 1, shape, visited, group);
            DFS(i, j + 1, shape, visited, group);
        }

        /// <summary>
        /// Проверка завершения игры: все фигуры одного цвета или одного типа
        /// </summary>
        private bool IsGameOver()
        {
            Type firstType = null;
            Colors? firstColor = null;
            bool found = false;

            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    if (field[i, j] == null)
                        continue;

                    if (!found)
                    {
                        firstType = field[i, j].GetType();
                        firstColor = field[i, j].Color;
                        found = true;
                    }
                    else
                    {
                        if (field[i, j].GetType() != firstType)
                            firstType = typeof(object); // разные типы

                        if (field[i, j].Color != firstColor)
                            firstColor = null; // разные цвета
                    }
                }
            }

            // Если на поле нет фигур — игра не окончена
            if (!found)
                return false;

            // Игра окончена, если все фигуры одного цвета или одного типа
            return firstType != typeof(object) || firstColor != null;
        }
    }
}
