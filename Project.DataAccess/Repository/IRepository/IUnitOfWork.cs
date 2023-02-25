using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IFoodRepository Food { get; }
        IRestaurantRepository Restaurant { get; }
        IUserRepository User { get; }
        IMessageRepository Message { get; }
        IRoomChatRepository RoomChat { get; }
        IFoodOfUserRepository FoodOfUser { get; }
        IFeedbackRepository Feedback { get; }
        IFoodEatenRepository FoodEaten { get; }
        IReportRepository Report { get; }
        void Save();
    }
}
