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
using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;
using System.Text.RegularExpressions;

namespace PRN221_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;
        public Message Message { get; set; }
        public MessageController(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<ChatHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomChat>> Get(int id)
        {
            Message = _unitOfWork.Message.GetFirstOrDefault(u => u.Id == id);

            if (Message is null)
            {
                return NotFound();
            }

            var messageViewModel = _mapper.Map<Message, MessageViewModel>(Message);

            return Ok(messageViewModel);
        }

        [HttpGet("Room/{roomName}")]
        public IActionResult GetMessages(string roomName)
        {
            var room = _unitOfWork.RoomChat.GetFirstOrDefault(r => r.Name == roomName);
            if (room == null)
                return BadRequest();

            var messages = _unitOfWork.Message.GetAll(includeProperties: "FromUser,ToRoom", filter: m => m.ToRoomId == room.Id)
                .OrderByDescending(m => m.Timestamp)
                .Take(20)
                .Reverse();

            var messagesViewModel = _mapper.Map<IEnumerable<Message>, IEnumerable<MessageViewModel>>(messages);

            return Ok(messagesViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<Message>> Create(MessageViewModel messageViewModel)
        {
            var user = _unitOfWork.User.GetFirstOrDefault(u => u.UserName == User.Identity.Name);
            var room = _unitOfWork.RoomChat.GetFirstOrDefault(r => r.Name == messageViewModel.Room);
            if (room == null)
                return BadRequest();

            var msg = new Message()
            {
                Content = Regex.Replace(messageViewModel.Content, @"<.*?>", string.Empty),
                FromUser = user,
                ToRoom = room,
                Timestamp = DateTime.Now
            };

            _unitOfWork.Message.Add(msg);
            _unitOfWork.Save();

            // Broadcast the message
            var createdMessage = _mapper.Map<Message, MessageViewModel>(msg);
            await _hubContext.Clients.Group(room.Name).SendAsync("newMessage", createdMessage);

            return CreatedAtAction(nameof(Get), new { id = msg.Id }, createdMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var message = _unitOfWork.Message.GetFirstOrDefault(includeProperties: "FromUser", filter: m => m.Id == id && m.FromUser.UserName == User.Identity.Name);

            if (message == null)
                return NotFound();

            _unitOfWork.Message.Remove(message);
            _unitOfWork.Save();

            await _hubContext.Clients.All.SendAsync("removeChatMessage", message.Id);

            return Ok();
        }
    }
}
