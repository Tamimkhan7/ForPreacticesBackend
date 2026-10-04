using Xunit;

public class BrokenCiTest
{
    [Fact]
    public void ShouldFail()
    {
        Assert.Equal(1, 2);
    }
}
