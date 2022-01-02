using System;
using System.ComponentModel.DataAnnotations;

namespace LandonApi.Models
{
    public class BookingForm
    {
        [Required]
        [Display(Name = "startAt", Description = "Booking start time")]
        public DateTimeOffset? StartAt { get; set; }


        [Required]
        [Display(Name = "endAt", Description = "Booking end time")]
        public DateTimeOffset? EndAt { get; set; }

    }
}
