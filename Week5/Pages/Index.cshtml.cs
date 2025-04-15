using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YourProjectName.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace YourProjectName.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> _classes = new List<ClassInformationModel>();
        private static int _nextId = 1;

        public int SayfaBoyutu { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Sayfa { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedColumns { get; set; } = new();

        public int ToplamSayfa { get; set; }

        public List<ClassInformationModel> Classes { get; set; } = new();

        [BindProperty]
        public ClassInformationModel? ClassInformation { get; set; }

        public IActionResult OnGet()
        {
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionId = HttpContext.Session.GetString("session_id");

            var cookieToken = Request.Cookies["token"];
            var cookieUsername = Request.Cookies["username"];
            var cookieSessionId = Request.Cookies["session_id"];

            if (string.IsNullOrEmpty(sessionToken) ||
                string.IsNullOrEmpty(sessionUsername) ||
                sessionToken != cookieToken ||
                sessionUsername != cookieUsername ||
                sessionId != cookieSessionId)
            {
                return RedirectToPage("/Login");
            }

            if (_classes.Count == 0)
            {
                for (int i = 1; i <= 100; i++)
                {
                    _classes.Add(new ClassInformationModel
                    {
                        Id = _nextId++,
                        ClassName = $"Sınıf {i}",
                        StudentCount = 10 + (i % 5),
                        Description = i % 2 == 0 ? "Sayısal" : "Sözel"
                    });
                }
            }

            if (SelectedColumns == null || !SelectedColumns.Any())
            {
                SelectedColumns = new List<string> { "ClassName", "StudentCount", "Description" };
            }

            IEnumerable<ClassInformationModel> sorgu = _classes;

            if (!string.IsNullOrEmpty(SearchName))
            {
                sorgu = sorgu.Where(c => c.ClassName.Contains(SearchName));
            }

            int toplamKayit = sorgu.Count();
            ToplamSayfa = (int)System.Math.Ceiling(toplamKayit / (double)SayfaBoyutu);

            Classes = sorgu
                .Skip((Sayfa - 1) * SayfaBoyutu)
                .Take(SayfaBoyutu)
                .ToList();

            return Page();
        }

        public IActionResult OnPostAdd()
        {
            if (ModelState.IsValid && ClassInformation != null)
            {
                ClassInformation.Id = _nextId++;
                _classes.Add(ClassInformation);
                return RedirectToPage(new { Sayfa, SearchName, SelectedColumns });
            }

            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToDelete = _classes.Find(c => c.Id == id);
            if (classToDelete != null)
            {
                _classes.Remove(classToDelete);
            }

            return RedirectToPage(new { Sayfa, SearchName, SelectedColumns });
        }

        public IActionResult OnPostEdit()
        {
            if (ModelState.IsValid && ClassInformation != null)
            {
                var classToEdit = _classes.Find(c => c.Id == ClassInformation.Id);
                if (classToEdit != null)
                {
                    classToEdit.ClassName = ClassInformation.ClassName;
                    classToEdit.StudentCount = ClassInformation.StudentCount;
                    classToEdit.Description = ClassInformation.Description;
                }

                return RedirectToPage(new { Sayfa, SearchName, SelectedColumns });
            }

            return Page();
        }

        public IActionResult OnPostExportJson([FromForm] List<string> SelectedColumns)
        {
            this.SelectedColumns = SelectedColumns ?? new();

            var selectedClasses = _classes.Select(c =>
            {
                var dict = new Dictionary<string, object>();
                if (SelectedColumns.Contains("ClassName")) dict["ClassName"] = c.ClassName;
                if (SelectedColumns.Contains("StudentCount")) dict["StudentCount"] = c.StudentCount;
                if (SelectedColumns.Contains("Description")) dict["Description"] = c.Description;
                return dict;
            }).ToList();

            var jsonData = JsonSerializer.Serialize(selectedClasses);
            return File(System.Text.Encoding.UTF8.GetBytes(jsonData), "application/json", "classes.json");
        }
    }
}
