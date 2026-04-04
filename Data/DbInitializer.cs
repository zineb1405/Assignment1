using Assignment1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context,
                                            UserManager<IdentityUser> userManager,
                                            RoleManager<IdentityRole> roleManager)
        {
            context.Database.Migrate();

            // ROLES
            if (!await roleManager.RoleExistsAsync("Organizer"))
                await roleManager.CreateAsync(new IdentityRole("Organizer"));

            if (!await roleManager.RoleExistsAsync("Attendee"))
                await roleManager.CreateAsync(new IdentityRole("Attendee"));

            //  USERS 
            var organizerEmail = "organizer@test.com";
            var attendeeEmail = "attendee@test.com";

            if (await userManager.FindByEmailAsync(organizerEmail) == null)
            {
                var organizer = new IdentityUser
                {
                    UserName = organizerEmail,
                    Email = organizerEmail
                };

                await userManager.CreateAsync(organizer, "Password123!");
                await userManager.AddToRoleAsync(organizer, "Organizer");
            }

            if (await userManager.FindByEmailAsync(attendeeEmail) == null)
            {
                var attendee = new IdentityUser
                {
                    UserName = attendeeEmail,
                    Email = attendeeEmail
                };

                await userManager.CreateAsync(attendee, "Password123!");
                await userManager.AddToRoleAsync(attendee, "Attendee");
            }

            // EVENTS 
            if (!context.Events.Any())
            {
                var events = new List<Event>
                {
                    new Event
                    {
                        Title = "Career Fair",
                        Description = "Meet employers and explore job opportunities.",
                        Date = new DateTime(2026, 2, 1),
                        Location = "Gym",
                        BannerUrl = ""
                    },
                    new Event
                    {
                        Title = "Tech Talk",
                        Description = "A talk about modern technology trends.",
                        Date = new DateTime(2026, 2, 8),
                        Location = "Auditorium",
                        BannerUrl = ""
                    },
                    new Event
                    {
                        Title = "Hack Night",
                        Description = "Collaborative coding and problem-solving event.",
                        Date = new DateTime(2026, 2, 15),
                        Location = "Library",
                        BannerUrl = ""
                    }
                };

                context.Events.AddRange(events);
                context.SaveChanges();

                // ATTENDEES 
                var attendees = new List<Attendee>
                {
                    new Attendee { Name = "Alice Johnson", Email = "alice@example.com", EventId = events[0].Id },
                    new Attendee { Name = "Bob Smith", Email = "bob@example.com", EventId = events[0].Id },
                    new Attendee { Name = "Charlie Brown", Email = "charlie@example.com", EventId = events[1].Id },
                    new Attendee { Name = "Diana Prince", Email = "diana@example.com", EventId = events[1].Id },
                    new Attendee { Name = "Ethan Lee", Email = "ethan@example.com", EventId = events[2].Id },
                    new Attendee { Name = "Fiona White", Email = "fiona@example.com", EventId = events[2].Id }
                };

                context.Attendees.AddRange(attendees);
                context.SaveChanges();
            }
        }
    }
}