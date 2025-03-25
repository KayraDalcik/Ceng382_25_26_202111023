namespace YourProjectName.Models

{
    public class ClassInformationModel
    {
        public int Id { get; set; }

        public required string ClassName { get; set; }

        public int StudentCount { get; set; }

        // nullable yapın
        public string? Description { get; set; }
    }
}
