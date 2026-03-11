using FluentAssertions;
using JobTrackerVSA.Web.Features.JobApplications.Add;
using System.ComponentModel.DataAnnotations;

namespace JobTrackerVSA.UnitTests.Features.JobApplications.Add;

public class AddJobApplicationCommandValidationTests
{
    [Fact]
    public void Command_WithValidData_ShouldBeValid()
    {
        // Arrange
        var command = new AddJobApplicationCommand
        {
            CompanyName = "Valid Company",
            Position = "Valid Position"
        };
        var context = new ValidationContext(command);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(command, context, results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void Command_WithoutCompanyName_ShouldBeInvalid()
    {
        // Arrange
        var command = new AddJobApplicationCommand
        {
            CompanyName = null!,
            Position = "Valid Position"
        };
        var context = new ValidationContext(command);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(command, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("CompanyName") && r.ErrorMessage == "Company Name is required.");
    }

    [Fact]
    public void Command_WithCompanyNameLongerThan150Characters_ShouldBeInvalid()
    {
        // Arrange
        var command = new AddJobApplicationCommand
        {
            CompanyName = new string('A', 151),
            Position = "Valid Position"
        };
        var context = new ValidationContext(command);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(command, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("CompanyName") && r.ErrorMessage == "Company Name cannot exceed 150 characters.");
    }

    [Fact]
    public void Command_WithoutPosition_ShouldBeInvalid()
    {
        // Arrange
        var command = new AddJobApplicationCommand
        {
            CompanyName = "Valid Company",
            Position = null!
        };
        var context = new ValidationContext(command);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(command, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("Position") && r.ErrorMessage == "Position is required.");
    }

    [Fact]
    public void Command_WithPositionLongerThan150Characters_ShouldBeInvalid()
    {
        // Arrange
        var command = new AddJobApplicationCommand
        {
            CompanyName = "Valid Company",
            Position = new string('A', 151)
        };
        var context = new ValidationContext(command);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(command, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("Position") && r.ErrorMessage == "Position cannot exceed 150 characters.");
    }
}
