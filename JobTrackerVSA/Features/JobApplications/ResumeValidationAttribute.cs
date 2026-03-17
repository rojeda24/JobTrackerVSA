using System.ComponentModel.DataAnnotations;

namespace JobTrackerVSA.Web.Features.JobApplications;

public class ResumeValidationAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions = [".pdf", ".doc", ".docx"];
    private const int MaxFileSizeInBytes = 2 * 1024 * 1024; // 2 MB

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IFormFile file)
        {
            return ValidationResult.Success; // Not required, handled by RequiredAttribute if needed
        }

        if (file.Length > MaxFileSizeInBytes)
        {
            return new ValidationResult("The file exceeds the maximum allowed size of 2 MB.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            return new ValidationResult("Only .pdf, .doc, and .docx files are allowed.");
        }

        // Validate Magic Numbers (File Signatures)
        using var stream = file.OpenReadStream();
        var header = new byte[8];
        int offset = 0;
        while (offset < header.Length)
        {
            int bytesRead = stream.Read(header, offset, header.Length - offset);
            if (bytesRead == 0)
            {
                break;
            }
            offset += bytesRead;
        }

        bool isValidSignature = false;

        // PDF: 25 50 44 46 (%PDF)
        if (extension == ".pdf" && header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46)
        {
            isValidSignature = true;
        }
        // DOCX (ZIP archive): 50 4B 03 04 (PK..)
        else if (extension == ".docx" && header[0] == 0x50 && header[1] == 0x4B && header[2] == 0x03 && header[3] == 0x04)
        {
            isValidSignature = true;
        }
        // DOC (Compound File Binary Format): D0 CF 11 E0 A1 B1 1A E1
        else if (extension == ".doc" && header[0] == 0xD0 && header[1] == 0xCF && header[2] == 0x11 && header[3] == 0xE0 && header[4] == 0xA1 && header[5] == 0xB1 && header[6] == 0x1A && header[7] == 0xE1)
        {
            isValidSignature = true;
        }

        if (!isValidSignature)
        {
            return new ValidationResult("The file signature does not match the extension. Invalid file format.");
        }

        return ValidationResult.Success;
    }
}
