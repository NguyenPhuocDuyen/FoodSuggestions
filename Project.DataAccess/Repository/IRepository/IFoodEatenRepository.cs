using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DataAccess.Repository.IRepository
{
    public interface IFoodEatenRepository : IRepository<FoodEaten>
    {
        void Update(FoodEaten foodEaten);
    }
}
