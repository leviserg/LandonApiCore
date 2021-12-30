using LandonApi.Models;
using LandonApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IOpeningService _openingService;
        private readonly PagingOptions _defaultPagingOptions;

        public RoomsController(
            IRoomService roomService,
            IOpeningService openingService,
            IOptions<PagingOptions> defaultPagingOptionsWrapper)
        {
            _roomService = roomService;
            _openingService = openingService;
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
        }

        [HttpGet(Name = nameof(GetAllRooms))]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Collection<Room>>> GetAllRooms(
            [FromQuery] PagingOptions pagingOptions = null,
            [FromQuery] SortOptions<Room, RoomEntity> sortOptions = null)
        {
            /*
            var rooms = await _roomService.GetRoomsAsync();

            var collection = new Collection<Room>
            {
                Selfhref = Link.ToCollection(nameof(GetAllRooms)),
                Value = rooms.ToArray()
            };
            */
            
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var rooms = await _roomService.GetPagedRoomsAsync(pagingOptions, sortOptions);

            var collection = PagedCollection<Room>.Create(
                Link.ToCollection(nameof(GetAllRooms)),
                rooms.Items.ToArray(),
                rooms.TotalSize,
                pagingOptions);
            
            return collection;
        }

        // GET /rooms/{roomId}
        [HttpGet("{roomId}", Name = nameof(GetRoomById))]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Room>> GetRoomById(Guid roomId)
        {
            var room = await _roomService.GetRoomAsync(roomId);
            if (room == null) return NotFound();

            return room;
        }

        // GET /rooms/openings
        [HttpGet("openings", Name = nameof(GetAllRoomOpenings))]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Collection<Opening>>> GetAllRoomOpenings(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Opening, OpeningEntity> sortOptions)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var openings = await _openingService.GetOpeningsAsync(pagingOptions, sortOptions);

            var collection = PagedCollection<Opening>.Create(
                Link.ToCollection(nameof(GetAllRoomOpenings)),
                openings.Items.ToArray(),
                openings.TotalSize,
                pagingOptions);

            return collection;
        }
    }
}
