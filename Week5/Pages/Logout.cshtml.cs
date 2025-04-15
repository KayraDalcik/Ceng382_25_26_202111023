using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        // Oturumu temizle
        HttpContext.Session.Clear();

        // Girişle ilgili çerezleri sil
        Response.Cookies.Delete("username");
        Response.Cookies.Delete("token");
        Response.Cookies.Delete("session_id");

        // Login sayfasına yönlendir
        return RedirectToPage("/Login");
    }
}
