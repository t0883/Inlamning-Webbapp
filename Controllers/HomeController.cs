using Inlamning_Webbapp.Data;
using Inlamning_Webbapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Identity.Client;

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

            var movies = await _context.Movie.ToListAsync();
            if(!movies.Any())
            {
                await GenerateMovies();
            }
            var actors = await _context.Actor.ToListAsync();
            if (!actors.Any())
            {
                await GenerateActors();
            }

            return View();
        }

        private async Task GenerateActors()
        {
            DateTime[] dateTimes = new DateTime[]
            {
                new DateTime(2020-09-21),
                new DateTime(2022-01-20),
                new DateTime(2021-12-01),
                new DateTime(2019-06-14),
                new DateTime(2014-07-31)
            };

            string[,] actors = { { "Göran", "Svensson", "19" }, { "Karl", "Larsson", "56" }, { "Morgan", "Gustavsson", "35" }, { "Hejdar", "Fredriksson", "89" } };

            for(int i = 0; i < 4; i++) 
            {
                Actor actor = new Actor
                {
                    FirstName = actors[i, 0],
                    LastName = actors[i, 1],
                    Age = int.Parse(actors[i, 2])
                };

                _context.Actor.Add(actor);
                await _context.SaveChangesAsync();
            }

        }

        private async Task GenerateMovies()
        {
            DateTime[] dateTimes = new DateTime[]
            {
                new DateTime(2020-09-21),
                new DateTime(2022-01-20),
                new DateTime(2021-12-01),
                new DateTime(2019-06-14),
                new DateTime(2014-07-31)
            };

            string[,] movies = { { "James Bond", "Action", "110" }, { "Harry Potter", "Fantasy", "110" } };
            for(int i = 0; i < 2; i++)
            {
                Movie movie = new Movie
                {
                    Title = movies[i, 0],
                    ReleaseDate = dateTimes[i],
                    Genre = movies[i, 1],
                    Price = int.Parse(movies[i, 2])
                };

                _context.Movie.Add(movie);
                await _context.SaveChangesAsync();
            }
        }


        private async Task PopulateDB()
        {
            //Starta process för att populate DB
            var roleStore = new RoleStore<IdentityRole>(_context);
            var userStore = new UserStore<IdentityUser>(_context);

            string[] roles = { "Admin", "Moderator", "User" };
            foreach (string role in roles)
            {
                //Skapa en role i DB med namnet från 'role' variabel
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