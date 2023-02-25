using AutoMapper;
using PRN221_Project.ViewModels;
using Project.Models;
using static PRN221_Project.Controllers.MessageController;

namespace PRN221_Project.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(dst => dst.Username, opt => opt.MapFrom(x => x.UserName));
            CreateMap<UserViewModel, User>();
        }
    }
}
