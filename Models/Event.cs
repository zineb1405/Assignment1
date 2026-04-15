using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Location { get; set; } = "";

        public string? BannerUrl { get; set; } = "";

        public string? OrganizerUserId { get; set; }

        public List<Attendee> Attendees { get; set; } = new List<Attendee>();
    }
}