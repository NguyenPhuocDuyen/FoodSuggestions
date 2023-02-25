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
    public class FoodRepository : Repository<Food>, IFoodRepository
    {
        private readonly ApplicationDbContext _db;

        public FoodRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Food Food)
        {
        }
    }
}
