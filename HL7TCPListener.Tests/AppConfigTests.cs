using Xunit;
using HL7TCPListener;
using System.IO;

public class AppConfigTests
{
    [Fact]
    public void SaveConfig_ShouldWriteFile()
    {
        // Arrange
        var config = new AppConfig { Port = 4040, FolderPath = "C:\\Temp" };
        string path = Path.Combine(Path.GetTempPath(), "test_config.json");

        // Act
        File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(config));

        // Assert
        Assert.True(File.Exists(path));
    }
}
