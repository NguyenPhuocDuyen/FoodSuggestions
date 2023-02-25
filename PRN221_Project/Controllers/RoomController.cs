using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PRN221_Project.Hubs;
using PRN221_Project.ViewModels;
using Project.DataAccess.Data;
using Project.DataAccess.Repository.IRepository;
using Project.Models;

namespace PRN221_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public RoomController(ApplicationDbContext context,
            IMapper mapper,
            IHubContext<ChatHub> hubContext, IUnitOfWork unitOfWork)
        {            
            _mapper = mapper;
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomViewModel>>> Get()
        {
            var rooms = _unitOfWork.RoomChat.GetAll();

            var roomsViewModel = _mapper.Map<IEnumerable<RoomChat>, IEnumerable<RoomViewModel>>(rooms);

            return Ok(roomsViewModel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomChat>> Get(int id)
        {
            var room = _unitOfWork.RoomChat.GetFirstOrDefault(x=>x.Id == id);
            if (room == null)
                return NotFound();

            var roomViewModel = _mapper.Map<RoomChat, RoomViewModel>(room);
            return Ok(roomViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<RoomChat>> Create(RoomViewModel roomViewModel)
        {
            if (_unitOfWork.RoomChat.GetFirstOrDefault(r => r.Name == roomViewModel.Name) != null)
                return BadRequest("Invalid room name or room already exists");

            var user = _unitOfWork.User.GetFirstOrDefault(filter: u => u.UserName == User.Identity.Name);
            var room = new RoomChat()
            {
                Name = roomViewModel.Name,
                Admin = user
            };

            _unitOfWork.RoomChat.Add(room);
            _unitOfWork.Save();

            await _hubContext.Clients.All.SendAsync("addChatRoom", new { id = room.Id, name = room.Name });

            return CreatedAtAction(nameof(Get), new { id = room.Id }, new { id = room.Id, name = room.Name });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, RoomViewModel roomViewModel)
        {
            if (_unitOfWork.RoomChat.GetFirstOrDefault(r => r.Name == roomViewModel.Name) != null)
                return BadRequest("Invalid room name or room already exists");

            var room = _unitOfWork.RoomChat.GetFirstOrDefault(includeProperties: "Admin", filter: r => r.Id == id && r.Admin.UserName == User.Identity.Name);                                             

            if (room == null)
                return NotFound();

            room.Name = roomViewModel.Name;
            _unitOfWork.Save();

            await _hubContext.Clients.All.SendAsync("updateChatRoom", new { id = room.Id, room.Name });

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = _unitOfWork.RoomChat.GetFirstOrDefault(includeProperties: "Admin", filter: r => r.Id == id && r.Admin.UserName == User.Identity.Name);

            if (room == null)
                return NotFound();

            _unitOfWork.RoomChat.Remove(room);
            _unitOfWork.Save();

            await _hubContext.Clients.All.SendAsync("removeChatRoom", room.Id);
            await _hubContext.Clients.Group(room.Name).SendAsync("onRoomDeleted", string.Format("Room {0} has been deleted.\nYou are moved to the first available room!", room.Name));

            return Ok();
        }
    }
}
