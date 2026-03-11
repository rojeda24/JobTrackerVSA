using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using static JobTrackerVSA.Web.Features.JobApplications.Edit.EditModel;

namespace JobTrackerVSA.UnitTests.Features.JobApplications.Edit;

public class EditJobApplicationInputModelValidationTests
{
    [Fact]
    public void InputModel_WithValidData_ShouldBeValid()
    {
        // Arrange
        var model = new InputModel
        {
            CompanyName = "Valid Company",
            Position = "Valid Position"
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void InputModel_WithoutCompanyName_ShouldBeInvalid()
    {
        // Arrange
        var model = new InputModel
        {
            CompanyName = null!,
            Position = "Valid Position"
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("CompanyName") && r.ErrorMessage == "Company Name is required.");
    }

    [Fact]
    public void InputModel_WithCompanyNameLongerThan150Characters_ShouldBeInvalid()
    {
        // Arrange
        var model = new InputModel
        {
            CompanyName = new string('A', 151),
            Position = "Valid Position"
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("CompanyName") && r.ErrorMessage == "Company Name cannot exceed 150 characters.");
    }

    [Fact]
    public void InputModel_WithoutPosition_ShouldBeInvalid()
    {
        // Arrange
        var model = new InputModel
        {
            CompanyName = "Valid Company",
            Position = null!
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("Position") && r.ErrorMessage == "Position is required.");
    }

    [Fact]
    public void InputModel_WithPositionLongerThan150Characters_ShouldBeInvalid()
    {
        // Arrange
        var model = new InputModel
        {
            CompanyName = "Valid Company",
            Position = new string('A', 151)
        };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(model, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("Position") && r.ErrorMessage == "Position cannot exceed 150 characters.");
    }
}
