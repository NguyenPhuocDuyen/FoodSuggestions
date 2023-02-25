using AutoMapper;
using PRN221_Project.ViewModels;
using Project.Models;
using static PRN221_Project.Controllers.MessageController;

namespace PRN221_Project.Mapping
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<RoomChat, RoomViewModel>();
            CreateMap<RoomViewModel, RoomChat>();
        }
    }
}
