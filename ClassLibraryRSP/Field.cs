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

            // Если "будущего" нет — делаем обычный ход и сохраняем его
            Save();

            MoveShapes();
            NewShapes();
            FindAndRemoveGroups();

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
        /// Сбор очков за группы из 3+ одинаковых фигур и их удаление
        /// </summary>
        private void FindAndRemoveGroups()
        {
            bool[,] visited = new bool[SIZE, SIZE];
            List<(int, int)> group = new List<(int, int)>();

            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    if (field[i, j] == null || visited[i, j])
                        continue;

                    group.Clear();
                    DFS(i, j, field[i, j], visited, group);

                    if (group.Count >= 3)
                    {
                        foreach (var (x, y) in group)
                            field[x, y] = null;

                        points += group.Count;
                    }
                }
            }
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
