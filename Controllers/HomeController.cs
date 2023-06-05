using Inlamning_Webbapp.Data;
using Inlamning_Webbapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Inlamning_Webbapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public ApplicationDbContext _context {  get; set; }

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            if(!_context.Roles.Any(m => m.Name == "Admin"))
            {
                await PopulateDB();
            }

            return View();
        }

        private async Task PopulateDB()
        {
            //Starta process för att populate DB
            var roleStore = new RoleStore<IdentityRole>(_context);
            var userStore = new UserStore<IdentityUser>(_context);

            string[] roles = { "Admin", "Moderator", "User" };
            foreach (string role in roles)
            {
                //Skapa en role i DB med namnet frpn 'role' variabel
                roleStore.CreateAsync(new IdentityRole(role)).Wait();

                var newRole = _context.Roles.Where(m => m.Name == role).FirstOrDefault();
                newRole.NormalizedName = role.ToUpper();

                _context.Roles.Update(newRole);
                await _context.SaveChangesAsync();
            }

            //Skapa nya Users
            string[] users = { "Tobias", "Superadmin", "Madeleine", "Sarvenaz" };
            string ePostHandler = "@app.se";

            foreach (string user in users)
            {

                //Skapa ett nytt IdentityUser objekt
                var newUser = new IdentityUser();
                newUser.UserName = user + ePostHandler;
                newUser.Email = user + ePostHandler;

                newUser.NormalizedUserName = (user + ePostHandler).ToUpper();
                newUser.NormalizedEmail = (user + ePostHandler).ToUpper();

                newUser.EmailConfirmed = true;

                var password = "12345";
                var hasher = new PasswordHasher<IdentityUser>();
                newUser.PasswordHash = hasher.HashPassword(newUser, password);

                //Add user to DB
                userStore.CreateAsync(newUser).Wait();
            }

            //Koppla rollen Admin till user Superadmin
            var adminUser = _context.Users.SingleOrDefault(n => n.UserName == "Superadmin@app.se");
            if(!userStore.IsInRoleAsync(adminUser,"Admin").Result)
            {
                await userStore.AddToRoleAsync(adminUser, "Admin");
                await _context.SaveChangesAsync();
            }


        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}