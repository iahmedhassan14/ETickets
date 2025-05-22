using ETickets.Models;
using ETickets.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ETickets.ViewModels
{
    public class MovieVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public double Price { get; set; }

        public string? ImgUrl { get; set; }

        [Required]
        public string TrailerUrl { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string MovieStatus { get; set; } = null!;

        [Required]
        public string MovieRate { get; set; } = null!;

        [Required]
        public string PgRate { get; set; } = null!;

        public string Tags { get; set; } = null!;
        [Display(Name = "Select Cinemas")]
        public List<int> CinemaIds { get; set; } = new();

        [Display(Name = "Select Categories")]
        public List<int> CategoryIds { get; set; } = new();

        [Display(Name = "Select Actors")]
        public List<int> ActorIds { get; set; } = new();
        public List<Cinema>? Cinemas { get; set; } = new();
        public List<Category>? Categories { get; set; } = new();
        public List<ActorVM>? Actors { get; set; } = new();
    }
}
