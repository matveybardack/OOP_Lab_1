using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryRSP
{
    // Треугольника бьет Круг
    public class Triangle : Shape
    {
        public Triangle() { }

        public override Shape GetShape(Shape other)
        {
            if (other is Circle)
                return other;

            return this;
        }
    }

    // Круга бьет Квадрат
    public class Circle : Shape
    {
        public Circle() { }

        public override Shape GetShape(Shape other)
        {
            if (other is Square)
                return other;

            return this;
        }
    }

    // Квадрата бьет Треугольник
    public class Square : Shape
    {
        public Square() { }

        public override Shape GetShape(Shape other)
        {
            if (other is Triangle)
                return other;

            return this;
        }
    }
}
