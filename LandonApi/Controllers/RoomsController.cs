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


        // GET /rooms?offet=1&limit=2&orderBy=rate desc
        [HttpGet(Name = nameof(GetAllRooms))]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Collection<Room>>> GetAllRooms(
            [FromQuery] PagingOptions pagingOptions = null,
            [FromQuery] SortOptions<Room, RoomEntity> sortOptions = null,
            [FromQuery] SearchOptions<Room,RoomEntity> searchOptions = null)
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

            var rooms = await _roomService.GetPagedRoomsAsync(pagingOptions, sortOptions, searchOptions);

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

        // GET /rooms/openings/{?offset=1&limit=2&orderBy=name(rate,startAt,endAt){ desc}}
        /*
            Search functionality:
                    ?search={field} {operator: [eq (numeric, string), 
                        lt (numeric, date), lte, gt, gte],
                        sw (startsWith), co (contains) - for String Search Pattern
                        } {value}
                    + multiple search:
                    GET /rooms/openings?search=rate lt 199.99&search=startAt gte 2022-01-10
         */
        [HttpGet("openings", Name = nameof(GetAllRoomOpenings))]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<Collection<Opening>>> GetAllRoomOpenings(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Opening, OpeningEntity> sortOptions,
            [FromQuery] SearchOptions<Opening, OpeningEntity> searchOptions)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            var openings = await _openingService.GetOpeningsAsync(
                pagingOptions, sortOptions, searchOptions);

            var collection = PagedCollection<Opening>.Create(
                Link.ToCollection(nameof(GetAllRoomOpenings)),
                openings.Items.ToArray(),
                openings.TotalSize,
                pagingOptions);

            return collection;
        }
    }
}
