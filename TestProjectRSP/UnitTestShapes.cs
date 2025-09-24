using ClassLibraryRSP;
using Xunit;

namespace TestProjectRSP
{
    public class UnitTestShapes
    {
        [Fact]
        public void Triangle_Beats_Square()
        {
            var triangle = new Triangle();
            var square = new Square();
            // “реугольник против квадрата Ч выигрывает треугольник
            Assert.Same(triangle, triangle.GetShape(square));
            Assert.Same(triangle, square.GetShape(triangle));
        }

        [Fact]
        public void Triangle_Loses_To_Circle()
        {
            var triangle = new Triangle();
            var circle = new Circle();
            // “реугольник против круга Ч выигрывает круг
            Assert.Same(circle, triangle.GetShape(circle));
            Assert.Same(circle, circle.GetShape(triangle));
        }

        [Fact]
        public void Circle_Beats_Triangle()
        {
            var circle = new Circle();
            var triangle = new Triangle();
            //  руг против треугольника Ч выигрывает круг
            Assert.Same(circle, circle.GetShape(triangle));
            Assert.Same(circle, triangle.GetShape(circle));
        }

        [Fact]
        public void Circle_Loses_To_Square()
        {
            var circle = new Circle();
            var square = new Square();
            //  руг против квадрата Ч выигрывает квадрат
            Assert.Same(square, circle.GetShape(square));
            Assert.Same(square, square.GetShape(circle));
        }

        [Fact]
        public void Square_Beats_Circle()
        {
            var square = new Square();
            var circle = new Circle();
            //  вадрат против круга Ч выигрывает квадрат
            Assert.Same(square, square.GetShape(circle));
            Assert.Same(square, circle.GetShape(square));
        }

        [Fact]
        public void Square_Loses_To_Triangle()
        {
            var square = new Square();
            var triangle = new Triangle();
            //  вадрат против треугольника Ч выигрывает треугольник
            Assert.Same(triangle, square.GetShape(triangle));
            Assert.Same(triangle, triangle.GetShape(square));
        }

        [Fact]
        public void SameType_Returns_Self()
        {
            var triangle1 = new Triangle();
            var triangle2 = new Triangle();
            var circle1 = new Circle();
            var circle2 = new Circle();
            var square1 = new Square();
            var square2 = new Square();

            Assert.Same(triangle1, triangle1.GetShape(triangle2));
            Assert.Same(circle1, circle1.GetShape(circle2));
            Assert.Same(square1, square1.GetShape(square2));
        }
    }
}