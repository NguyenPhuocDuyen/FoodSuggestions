using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DataAccess.Repository.IRepository
{
    public interface IFoodOfUserRepository : IRepository<FoodOfUser>
    {
        void Update(FoodOfUser foodOfUser);
    }
}
