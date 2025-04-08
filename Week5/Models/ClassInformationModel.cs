using System.ComponentModel.DataAnnotations;

namespace YourProjectName.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class name is required.")]
        public string ClassName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Student count must be greater than 0.")]
        public int StudentCount { get; set; }

        public string? Description { get; set; }
    }
}
