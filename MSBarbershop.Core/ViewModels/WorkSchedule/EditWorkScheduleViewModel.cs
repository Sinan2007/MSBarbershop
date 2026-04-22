using System;
using System.ComponentModel.DataAnnotations;

namespace MSBarbershop.Core.ViewModels.WorkSchedule
{
    public class EditWorkScheduleViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
    }
}