using System;
using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models
{
    public class Attendee
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        public int EventId { get; set; }

        public Event? Event { get; set; }
    }
}