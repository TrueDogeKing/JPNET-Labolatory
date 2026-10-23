namespace LabIoC.UnitTests;

using FluentAssertions;
using LabIoC.Core;
using LabIoC.UnitTests.Fakes;
using Xunit;

public class WorkerTests
{
    [Fact]
    public void Execute_ValidCount_ShouldReturnExpectedResultAndLogMessages()
    {
        // Arrange
        var spy = new SpyLogger();
        var worker = new Worker(spy);

        // Act
        int result = worker.Execute(3);

        // Assert (FluentAssertions)
        result.Should().Be(30);
        spy.Messages.Should().HaveCount(2)
            .And.Contain("Zakończono pomyślnie.");
    }

    [Fact]
    public void Execute_NegativeCount_ShouldThrowArgumentException()
    {
        // Arrange
        var spy = new SpyLogger();
        var worker = new Worker(spy);

        // Act
        Action act = () => worker.Execute(-1);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*nie może być ujemna*");
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(5, 50)]
    [InlineData(10, 100)]
    public void Execute_VariousCounts_ShouldCalculateScoreProperly(int input, int expected)
    {
        var spy = new SpyLogger();
        var worker = new Worker(spy);

        int result = worker.Execute(input);

        result.Should().Be(expected);
    }
}