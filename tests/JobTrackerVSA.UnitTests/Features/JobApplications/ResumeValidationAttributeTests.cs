using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using JobTrackerVSA.Web.Features.JobApplications;
using NSubstitute;

namespace JobTrackerVSA.UnitTests.Features.JobApplications;

public class ResumeValidationAttributeTests
{
    private readonly ResumeValidationAttribute _attribute;
    private readonly ValidationContext _validationContext;

    public ResumeValidationAttributeTests()
    {
        _attribute = new ResumeValidationAttribute();
        _validationContext = new ValidationContext(new object());
    }

    [Fact]
    public void IsValid_NullFile_ReturnsSuccess()
    {
        // Arrange
        IFormFile? file = null;

        // Act
        var result = _attribute.GetValidationResult(file, _validationContext);

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void IsValid_FileLargerThan2MB_ReturnsFailure()
    {
        // Arrange
        var mockFile = Substitute.For<IFormFile>();
        mockFile.Length.Returns(3 * 1024 * 1024); // 3 MB
        mockFile.FileName.Returns("resume.pdf");

        // Act
        var result = _attribute.GetValidationResult(mockFile, _validationContext);

        // Assert
        result.Should().NotBeNull();
        result!.ErrorMessage.Should().Contain("exceeds the maximum allowed size");
    }

    [Theory]
    [InlineData("image.jpg")]
    [InlineData("video.mp4")]
    [InlineData("script.exe")]
    [InlineData("resume.txt")]
    public void IsValid_InvalidExtension_ReturnsFailure(string fileName)
    {
        // Arrange
        var mockFile = Substitute.For<IFormFile>();
        mockFile.Length.Returns(1024); // 1 KB
        mockFile.FileName.Returns(fileName);

        // Act
        var result = _attribute.GetValidationResult(mockFile, _validationContext);

        // Assert
        result.Should().NotBeNull();
        result!.ErrorMessage.Should().Contain("Only .pdf, .doc, and .docx");
    }

    [Theory]
    [InlineData("resume.pdf", new byte[] { 0x25, 0x50, 0x44, 0x46, 0x00, 0x00, 0x00, 0x00 })] // Valid PDF
    [InlineData("resume.docx", new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x00, 0x00, 0x00, 0x00 })] // Valid DOCX
    [InlineData("resume.doc", new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 })] // Valid DOC
    public void IsValid_ValidExtensionAndValidSignature_ReturnsSuccess(string fileName, byte[] fileBytes)
    {
        // Arrange
        var stream = new MemoryStream(fileBytes);
        var mockFile = Substitute.For<IFormFile>();
        mockFile.Length.Returns(stream.Length);
        mockFile.FileName.Returns(fileName);
        mockFile.OpenReadStream().Returns(stream);

        // Act
        var result = _attribute.GetValidationResult(mockFile, _validationContext);

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData("resume.pdf", new byte[] { 0x4D, 0x5A, 0x90, 0x00, 0x03, 0x00, 0x00, 0x00 })] // Fake PDF (Actually an EXE)
    [InlineData("resume.docx", new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46 })] // Fake DOCX (Actually a JPEG)
    [InlineData("resume.doc", new byte[] { 0x49, 0x44, 0x33, 0x03, 0x00, 0x00, 0x00, 0x00 })] // Fake DOC (Actually an MP3)
    public void IsValid_FakeExtensionValidSizeButInvalidSignature_ReturnsFailure(string fileName, byte[] fileBytes)
    {
        // Arrange
        var stream = new MemoryStream(fileBytes);
        var mockFile = Substitute.For<IFormFile>();
        mockFile.Length.Returns(stream.Length);
        mockFile.FileName.Returns(fileName);
        mockFile.OpenReadStream().Returns(stream);

        // Act
        var result = _attribute.GetValidationResult(mockFile, _validationContext);

        // Assert
        result.Should().NotBeNull();
        result!.ErrorMessage.Should().Contain("signature does not match");
    }
}
