using Xunit;

public class PortValidationTests
{
    [Theory]
    [InlineData(3999, false)]
    [InlineData(4000, true)]
    [InlineData(4500, true)]
    [InlineData(5001, false)]
    public void Port_ShouldBeWithinValidRange(int port, bool expected)
    {
        int portMin = 4000;
        int portMax = 5000;
        bool result = port >= portMin && port <= portMax;

        Assert.Equal(expected, result);
    }
}
