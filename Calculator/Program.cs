using System;
using NSubstitute;
using Xunit;

namespace Calculator
{
    public interface ICalculator
    {
        int Add(int a, int b);
        string Mode { get; set; }
        event EventHandler PoweringUp;
    }

    class Program
    {
        static void Main(string[] args)
        {
            // We can ask NSubstitute to create a substitute instance for this
            // type:
            var calculator = Substitute.For<ICalculator>();

            // Now we can tell our substitute to return a value for a call:
            calculator.Add(1, 2).Returns(3);
            var expected1 = 3;
            var actual1 = calculator.Add(1, 2);
            Assert.Equal(actual1, expected1);

            // We can check that our substitute received a call, and did not
            // receive others:
            calculator.Add(1, 2);
            calculator.Received().Add(1, 2);
            calculator.DidNotReceive().Add(5, 7);

            // We can also work with properties using the Returns syntax we use
            // for methods:
            calculator.Mode.Returns("DEC");
            var expected2 = "DEC";
            var actual2 = calculator.Mode;
            Assert.Equal(actual2, expected2);

            // Or just stick with plain old property setters (for
            // read/write properties)
            calculator.Mode = "HEX";
            var expected3 = "HEX";
            var actual3 = calculator.Mode;
            Assert.Equal(actual3, expected3);

            // NSubstitute supports argument matching for setting return values
            // and asserting a call was received:
            calculator.Add(10, -5);
            calculator.Received().Add(10, Arg.Any<int>());
            calculator.Received().Add(10, Arg.Is<int>(x => x < 0));

            // We can use argument matching as well as passing a function to
            // Returns() to get some more behaviour out of our substitute
            // (possibly too much, but that’s your call):

            calculator
                .Add(Arg.Any<int>(), Arg.Any<int>())
                .Returns(x => (int)x[0] + (int)x[1]);
            var expected4 = 15;
            var actual4 = calculator.Add(5, 10);
            Assert.Equal(actual4, expected4);
            
            // Returns() can also be called with multiple arguments to set up
            // a sequence of return values.
            calculator.Mode.Returns("HEX", "DEC", "BIN");
            var expected5 = "HEX";
            var actual5 = calculator.Mode;
            Assert.Equal(actual5, expected5);
            var expected6 = "DEC";
            var actual6 = calculator.Mode;
            Assert.Equal(actual6, expected6);
            var expected7 = "BIN";
            var actual7 = calculator.Mode;
            Assert.Equal(actual7, expected7);

            // Finally, we can raise events on our substitutes (unfortunately
            // C# dramatically restricts the extent to which this syntax can
            // be cleaned up):
            bool eventWasRaised = false;
            calculator.PoweringUp += (sender, args) => eventWasRaised = true;
            calculator.PoweringUp += Raise.Event();
            var condition = eventWasRaised;
            Assert.True(condition);

            Console.WriteLine("Hello World!");
        }
    }
}
