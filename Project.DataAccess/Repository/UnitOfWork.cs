using Project.DataAccess.Data;
using Project.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Food = new FoodRepository(_dbContext);
            Restaurant = new RestaurantRepository(_dbContext);
            User = new UserRepository(_dbContext);
            Message = new MessageRepository(_dbContext);
            RoomChat = new RoomChatRepository(_dbContext);
            FoodOfUser = new FoodOfUserRepository(_dbContext);
            Feedback = new FeedbackRepository(_dbContext);
            FoodEaten = new FoodEatenRepository(_dbContext);
            Report = new ReportRepository(_dbContext);
        }

        public IFoodRepository Food { get; private set; }
        public IRestaurantRepository Restaurant { get; private set; }
        public IUserRepository User { get; private set; }
        public IMessageRepository Message { get; private set; }
        public IRoomChatRepository RoomChat { get; private set; }
        public IFoodOfUserRepository FoodOfUser { get; private set; }
        public IFeedbackRepository Feedback { get; private set; }
        public IFoodEatenRepository FoodEaten { get; private set; }
        public IReportRepository Report { get; private set; }

        public void Dispose()
        {
            _dbContext.Dispose();
        
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

    }
}
