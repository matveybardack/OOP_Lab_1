using System;
using ClassLibraryRSP;
using Xunit;

namespace TestProjectRSP
{
    public class UnitTestField
    {
        [Fact]
        public void Field_InitialState_IsEmpty()
        {
            var field = new Field(8);
            var inner = field.InnerField;
            int count = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (inner[i, j] != null) count++;
            Assert.Equal(0, count);
            Assert.Equal(0, field.Points);
        }

        [Fact]
        public void Field_MoveNext_AddsShapesOrChangesState()
        {
            var field = new Field(8);
            field.MoveNext();
            var inner = field.InnerField;
            int count = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (inner[i, j] != null) count++;
            Assert.True(count > 0);
        }

        [Fact]
        public void Field_IsGameOver_IfAllSameColor()
        {
            var field = new Field(3);
            var inner = field.InnerField;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Shape s = j % 2 == 0 ? (Shape)new Triangle() : new Square();
                    // Принудительно задаём цвет
                    typeof(Shape).GetField("color", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(s, Colors.Red);
                    inner[i, j] = s;
                }
            // Сохраняем состояние после ручного изменения
            var saveMethod = typeof(Field).GetMethod("Save", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            saveMethod.Invoke(field, null);
            Assert.Throws<Exception>(() => field.MoveNext());
        }
    }
}
