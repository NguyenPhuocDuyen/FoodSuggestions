﻿using Project.DataAccess.Data;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DataAccess.Repository
{
    public class RoomChatRepository : Repository<RoomChat>, IRoomChatRepository
    {
        private readonly ApplicationDbContext _db;

        public RoomChatRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(RoomChat roomChat)
        {            
        }
    }
}
