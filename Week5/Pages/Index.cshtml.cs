using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Week5.Data;
using Week5.Models;

namespace Week5.Pages.Classes
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        public int SayfaBoyutu { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Sayfa { get; set; } = 1;

        public int ToplamSayfa { get; set; }

        public List<Class> ClassList { get; set; } = new();

        // ✅ Bu satır sayfadaki @Model.Classes kullanımını destekler
        public List<Class> Classes => ClassList;

        [BindProperty]
        public Class? ClassInformation { get; set; }

        public async Task<IActionResult> OnGetAsync()
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

            IQueryable<Class> query = _context.Classes;

            if (!string.IsNullOrEmpty(SearchName))
            {
                query = query.Where(c => c.Name.Contains(SearchName));
            }

            int toplamKayit = await query.CountAsync();
            ToplamSayfa = (int)Math.Ceiling(toplamKayit / (double)SayfaBoyutu);

            ClassList = await query
                .Skip((Sayfa - 1) * SayfaBoyutu)
                .Take(SayfaBoyutu)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (ModelState.IsValid && ClassInformation != null)
            {
                _context.Classes.Add(ClassInformation);
                await _context.SaveChangesAsync();
                return RedirectToPage(new { Sayfa, SearchName });
            }

            await OnGetAsync(); // reload data if validation fails
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var classToDelete = await _context.Classes.FindAsync(id);
            if (classToDelete != null)
            {
                _context.Classes.Remove(classToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { Sayfa, SearchName });
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (ModelState.IsValid && ClassInformation != null)
            {
                var classToEdit = await _context.Classes.FindAsync(ClassInformation.Id);
                if (classToEdit != null)
                {
                    classToEdit.Name = ClassInformation.Name;
                    classToEdit.PersonCount = ClassInformation.PersonCount;
                    classToEdit.Description = ClassInformation.Description;
                    classToEdit.IsActive = ClassInformation.IsActive;

                    await _context.SaveChangesAsync();
                }

                return RedirectToPage(new { Sayfa, SearchName });
            }

            await OnGetAsync(); // reload data if validation fails
            return Page();
        }

        public async Task<IActionResult> OnPostExportSelectedAsync([FromForm] string SelectedColumns)
        {
            var selectedList = SelectedColumns?.Split(',').ToList() ?? new List<string>();

            var allClasses = await _context.Classes.ToListAsync();

            var selectedClasses = allClasses.Select(c =>
            {
                var dict = new Dictionary<string, object>();
                foreach (var col in selectedList)
                {
                    switch (col)
                    {
                        case "Id": dict["Id"] = c.Id; break;
                        case "Name": dict["Name"] = c.Name; break;
                        case "PersonCount": dict["PersonCount"] = c.PersonCount; break;
                        case "Description": dict["Description"] = c.Description; break;
                        case "IsActive": dict["IsActive"] = c.IsActive; break;
                    }
                }
                return dict;
            }).ToList();

            var jsonData = JsonSerializer.Serialize(selectedClasses);
            return File(System.Text.Encoding.UTF8.GetBytes(jsonData), "application/json", "classes.json");
        }
    }
}
