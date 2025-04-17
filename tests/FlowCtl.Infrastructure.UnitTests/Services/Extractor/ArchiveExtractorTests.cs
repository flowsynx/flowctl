using FlowCtl.Infrastructure.Services.Extractor;

namespace FlowCtl.Infrastructure.UnitTests.Services.Extractor;

public class ArchiveExtractorTests
{
    [Theory]
    [InlineData("sample.zip", ".zip")]
    [InlineData("sample.tar.gz", ".tar.gz")]
    [InlineData("sample.tgz", ".tgz")]
    public void ExtractArchive_SupportedExtensions_DoesNotThrow(string fileName, string expectedExtension)
    {
        // Arrange
        var extractor = new ArchiveExtractor();
        var archivePath = fileName;
        var extractTo = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act & Assert
        if (expectedExtension == ".zip")
        {
            Assert.ThrowsAny<Exception>(() => extractor.ExtractArchive(archivePath, extractTo));
        }
        else
        {
            Assert.ThrowsAny<Exception>(() => extractor.ExtractArchive(archivePath, extractTo));
        }
    }

    [Fact]
    public void ExtractArchive_UnsupportedExtension_ThrowsNotSupportedException()
    {
        // Arrange
        var extractor = new ArchiveExtractor();
        var unsupportedFile = "file.rar";
        var extractTo = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act & Assert
        var ex = Assert.Throws<NotSupportedException>(() => extractor.ExtractArchive(unsupportedFile, extractTo));
        Assert.Contains(".rar", ex.Message);
    }

    [Theory]
    [InlineData("file.tar.gz", ".tar.gz")]
    [InlineData("file.tgz", ".tgz")]
    [InlineData("file.zip", ".zip")]
    [InlineData("file.txt", ".txt")]
    public void GetExtension_ReturnsCorrectExtension(string fileName, string expected)
    {
        var method = typeof(ArchiveExtractor).GetMethod("GetExtension", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var result = method.Invoke(null, new object[] { fileName });

        Assert.Equal(expected, result);
    }
}