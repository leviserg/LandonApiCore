using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LandonApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LandonApi.Services
{
    public class DefaultRoomService : IRoomService
    {
        private readonly HotelApiDbContext _context;
        private readonly IConfigurationProvider _mappingConfiguration;

        public DefaultRoomService(
            HotelApiDbContext context,
            IConfigurationProvider mappingConfiguration)
        {
            _context = context;
            _mappingConfiguration = mappingConfiguration;
        }

        public async Task<Room> GetRoomAsync(Guid id)
        {
            var entity = await _context.Rooms
                .SingleOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return null;
            }

            var mapper = _mappingConfiguration.CreateMapper();
            return mapper.Map<Room>(entity);
        }

        public async Task<IEnumerable<Room>> GetRoomsAsync()
        {
            var query = _context.Rooms
                .ProjectTo<Room>(_mappingConfiguration);

            return await query.ToArrayAsync();
        }

        public async Task<PagedResults<Room>> GetPagedRoomsAsync(
            PagingOptions pagingOptions,
            SortOptions<Room, RoomEntity> sortOptions,
            SearchOptions<Room, RoomEntity> searchOptions
        )
        {
            /*
            var rooms = await _context.Rooms.ProjectTo<Room>(_mappingConfiguration).ToArrayAsync();

            var pagedRooms = rooms
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value).ToArray();

            return new PagedResults<Room>
            {
                Items = pagedRooms,
                TotalSize = rooms.Count()
            };
            */

            IQueryable<RoomEntity> query = _context.Rooms;
            query = searchOptions.Apply(query); // filter before sort
            query = sortOptions.Apply(query);

            var size = await query.CountAsync();
            var items = await query
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ProjectTo<Room>(_mappingConfiguration)
                .ToArrayAsync();

            return new PagedResults<Room>
            {
                Items = items,
                TotalSize = size
            };
        }



    }
}
