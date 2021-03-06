using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Models
{
    public class BookingEntity : BookingRange
    {
        public Guid Id { get; set; }

        public RoomEntity Room { get; set; }

        [DataType(DataType.DateTime)]
        public DateTimeOffset CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTimeOffset ModifiedAt { get; set; }

        public int Total { get; set; }
    }
}
