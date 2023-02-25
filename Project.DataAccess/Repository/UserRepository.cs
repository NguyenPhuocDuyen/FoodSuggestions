using Project.DataAccess.Data;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DataAccess.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(User user)
        {
            //var objFromDb = _db.User.FirstOrDefault(u => u.Id == user.Id);
            //objFromDb.Id = user.Id;
            //objFromDb.FirstName = user.FirstName;
            //objFromDb.LastName = user.LastName;
            //objFromDb.Address = user.Address;
            //objFromDb.PhoneNumber = user.PhoneNumber;
            //objFromDb.Birthday = user.Birthday;
            //objFromDb.UserImage = user.UserImage;

            _db.Update(user);
        }
    }
}
