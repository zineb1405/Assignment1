using Assignment1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            context.Database.Migrate();

            if (!await roleManager.RoleExistsAsync("Organizer"))
                await roleManager.CreateAsync(new IdentityRole("Organizer"));

            if (!await roleManager.RoleExistsAsync("Attendee"))
                await roleManager.CreateAsync(new IdentityRole("Attendee"));

            var organizerEmail = "organizer@test.com";
            var attendeeEmail = "attendee@test.com";

            IdentityUser? organizer = await userManager.FindByEmailAsync(organizerEmail);
            if (organizer == null)
            {
                organizer = new IdentityUser
                {
                    UserName = organizerEmail,
                    Email = organizerEmail
                };

                await userManager.CreateAsync(organizer, "Password123!");
                await userManager.AddToRoleAsync(organizer, "Organizer");
            }

            IdentityUser? attendeeUser = await userManager.FindByEmailAsync(attendeeEmail);
            if (attendeeUser == null)
            {
                attendeeUser = new IdentityUser
                {
                    UserName = attendeeEmail,
                    Email = attendeeEmail
                };

                await userManager.CreateAsync(attendeeUser, "Password123!");
                await userManager.AddToRoleAsync(attendeeUser, "Attendee");
            }

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
                        BannerUrl = "",
                        OrganizerUserId = organizer?.Id
                    },
                    new Event
                    {
                        Title = "Tech Talk",
                        Description = "A talk about modern technology trends.",
                        Date = new DateTime(2026, 2, 8),
                        Location = "Auditorium",
                        BannerUrl = "",
                        OrganizerUserId = organizer?.Id
                    },
                    new Event
                    {
                        Title = "Hack Night",
                        Description = "Collaborative coding and problem-solving event.",
                        Date = new DateTime(2026, 2, 15),
                        Location = "Library",
                        BannerUrl = "",
                        OrganizerUserId = organizer?.Id
                    }
                };

                context.Events.AddRange(events);
                context.SaveChanges();

                var attendees = new List<Attendee>
                {
                    new Attendee
                    {
                        Name = "Alice Johnson",
                        Email = "alice@example.com",
                        EventId = events[0].Id
                    },
                    new Attendee
                    {
                        Name = "Bob Smith",
                        Email = "bob@example.com",
                        EventId = events[0].Id
                    },
                    new Attendee
                    {
                        Name = "Charlie Brown",
                        Email = "charlie@example.com",
                        EventId = events[1].Id
                    }
                };

                context.Attendees.AddRange(attendees);
                context.SaveChanges();
            }
        }
    }
}