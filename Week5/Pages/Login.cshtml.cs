using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Week5.Models;

public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");
        var jsonData = await System.IO.File.ReadAllTextAsync(jsonPath);
        var users = JsonSerializer.Deserialize<List<User>>(jsonData);

        var user = users.FirstOrDefault(u => u.Username == Username && u.Password == Password && u.IsActive);

        if (user != null)
        {
            // Token oluştur
            var token = Guid.NewGuid().ToString();

            // Session’a yaz
            HttpContext.Session.SetString("username", Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

            // Cookie’ye yaz
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("username", Username, options);
            Response.Cookies.Append("token", token, options);
            Response.Cookies.Append("session_id", HttpContext.Session.Id, options);

            return RedirectToPage("/Index"); // giriş başarılıysa yönlendirme
        }

        ErrorMessage = "Username or password is incorrect.";
        return Page();
    }
}
