using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YourProjectName.Models;
using System.Collections.Generic;

namespace YourProjectName.Pages
{
    public class IndexModel : PageModel
    {
        // In-memory storage
        private static List<ClassInformationModel> _classes = new List<ClassInformationModel>();
        private static int _nextId = 1;

        public List<ClassInformationModel> Classes => _classes;

        [BindProperty]
        // Nullable olarak işaretlendi
        public ClassInformationModel? ClassInformation { get; set; }

        public IActionResult OnPostAdd()
        {
            // Validate the form data and add the new class to the list
            if (ModelState.IsValid && ClassInformation != null)
            {
                ClassInformation.Id = _nextId++;
                _classes.Add(ClassInformation);
                return RedirectToPage();
            }

            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            // Find the class by Id and remove it from the list
            var classToDelete = _classes.Find(c => c.Id == id);
            if (classToDelete != null)
            {
                _classes.Remove(classToDelete);
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            if (ModelState.IsValid && ClassInformation != null)
            {
                // Find the class to update by Id
                var classToEdit = _classes.Find(c => c.Id == ClassInformation.Id);
                if (classToEdit != null)
                {
                    // Update class properties
                    classToEdit.ClassName = ClassInformation.ClassName;
                    classToEdit.StudentCount = ClassInformation.StudentCount;
                    classToEdit.Description = ClassInformation.Description;
                }
                return RedirectToPage();
            }
            return Page();
        }
    }
}
