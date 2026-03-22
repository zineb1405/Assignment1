using Assignment1.Models;

namespace Assignment1.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Events.Any())
            {
                return;
            }

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
                },
                new Attendee
                {
                    Name = "Diana Prince",
                    Email = "diana@example.com",
                    EventId = events[1].Id
                },
                new Attendee
                {
                    Name = "Ethan Lee",
                    Email = "ethan@example.com",
                    EventId = events[2].Id
                },
                new Attendee
                {
                    Name = "Fiona White",
                    Email = "fiona@example.com",
                    EventId = events[2].Id
                }
            };

            context.Attendees.AddRange(attendees);
            context.SaveChanges();
        }
    }
}