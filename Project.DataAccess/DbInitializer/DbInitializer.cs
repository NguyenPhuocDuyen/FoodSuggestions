using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.DataAccess.Data;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

using Microsoft.AspNetCore.Builder;

using Project.Utility;


namespace Project.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }

            if (!_roleManager.RoleExistsAsync(SD.AdminRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.UserRole)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new User
                {
                    UserName = "admin@master.com",
                    UserImage = "/images/Avatars/avt.png",
                    Email = "admin@master.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "Master",
                    Date_Create = DateTime.Now,
                }, "Admin123*").GetAwaiter().GetResult();

                User user = _db.User.FirstOrDefault(u => u.Email == "admin@master.com");

                _userManager.AddToRoleAsync(user, SD.AdminRole).GetAwaiter().GetResult();
            }

            //Thêm database mẫu khác sau đây:
            if (_db.Feedback.Any())
            {
                return;
            }

            Randomizer.Seed = new Random(8675309);

            var userSetDefaults = new Faker<User>();
            userSetDefaults.RuleFor(a => a.UserName, f => "userdefault@gmail.com");
            userSetDefaults.RuleFor(a => a.UserImage, f => "/images/Avatars/avt.png");
            userSetDefaults.RuleFor(a => a.Email, f => "userdefault@gmail.com");
            userSetDefaults.RuleFor(a => a.FirstName, f => f.Lorem.Sentence(1, 1));
            userSetDefaults.RuleFor(a => a.LastName, f => f.Lorem.Sentence(1, 1));

            for (int i = 0; i < 333; i++)
            {
                User u = userSetDefaults.Generate();
                _userManager.CreateAsync(new User
                {
                    UserName = u.UserName+i,
                    UserImage = u.UserImage,
                    Email = u.Email+i,
                    EmailConfirmed = true,
                    FirstName = u.FirstName+i,
                    LastName = u.LastName+i,
                    Date_Create = DateTime.Now,
                }, "User123*").GetAwaiter().GetResult();

                User user = _db.User.FirstOrDefault(a => a.Email == u.Email+i);

                _userManager.AddToRoleAsync(user, SD.UserRole).GetAwaiter().GetResult();
            }


            User user2 = _db.User.FirstOrDefault(u => u.Email == "admin@master.com");

            var fakeRestaurant = new Faker<Restaurant>();
            fakeRestaurant.RuleFor(a => a.UserId, f => user2.Id);
            fakeRestaurant.RuleFor(a => a.Name, f => f.Lorem.Sentence(3, 3));
            fakeRestaurant.RuleFor(a => a.RestaurantImage, f => "/img/restaurant/restaurantdefault.jpg");
            fakeRestaurant.RuleFor(a => a.Phone, f => "0123456789");
            fakeRestaurant.RuleFor(a => a.Address, f => f.Lorem.Sentence(5, 10));
            fakeRestaurant.RuleFor(a => a.Date_Create, f => f.Date.Between(new DateTime(2000, 1, 1), new DateTime(2005, 1, 1)));
            fakeRestaurant.RuleFor(a => a.Date_Edit, f => f.Date.Between(new DateTime(2005, 1, 1), new DateTime(2006, 1, 1)));
            fakeRestaurant.RuleFor(a => a.Description, f => f.Lorem.Sentence(20, 20));
            fakeRestaurant.RuleFor(a => a.IsDeleted, f => false);
            for (int i = 0; i < 10; i++)
            {
                Restaurant restaurant = fakeRestaurant.Generate();
                _db.Restaurant.Add(restaurant);
            }
            _db.SaveChanges();

            string imgaddress = "/images/foods/";
            Food[] foodList = new Food[]{
                new Food{  Name = "Phở", FoodImage = imgaddress+"pho.jpg", UserId = user2.Id, 
                    RestaurantId  = 1, Description = "Thành phần chính của phở là bánh phở và nước dùng cùng với thịt bò hoặc thịt gà cắt lát mỏng. Thịt bò thích hợp nhất để nấu phở là thịt, xương từ các giống bò ta (bò nội, bò vàng).",
                    Price = 23000, 
                    Date_Create = new DateTime(2022, 10, 1), Date_Edit = DateTime.Now, 
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Chả cá", FoodImage = imgaddress+"chaca.jpg", UserId = user2.Id,
                    RestaurantId  = 2, Description = "Chả cá thát lát là một loại chả cá làm từ thịt của cá thát lát, thường là lóc thịt, bỏ da và xương rồi giã hay xay nhuyễn thành chả viên, rồi đem xào, chiên hay làm lẩu.",
                    Price = 12000,
                    Date_Create = new DateTime(2022, 10, 2), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Bánh xèo", FoodImage = imgaddress+"banhxeo.jpg", UserId = user2.Id,
                    RestaurantId  = 3, Description = "Nhân bánh được biến tấu đa dạng với đủ loại thịt heo, gà, tôm, tép, đu đủ, đậu xanh, giá… cộng thêm cách làm bột và đổ bánh khác nhau của mỗi người đầu bếp đã làm nên những hương vị riêng biệt nhưng rất hòa hợp và trọn vẹn.",
                    Price = 25000,
                    Date_Create = new DateTime(2022, 10, 3), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Cao lầu", FoodImage = imgaddress+"caolau.jpg", UserId = user2.Id,
                    RestaurantId  = 4, Description = "Giống như mì Quảng, cao lầu Hội An không cần nước lèo, nhưng nước xíu (nước sốt làm xá xíu), thịt xíu và tép mỡ thì không thể thiếu.",
                    Price = 44000,
                    Date_Create = new DateTime(2022, 10, 4), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Rau muống xào tỏi", FoodImage = imgaddress+"raumuongxaotoi.jpg", UserId = user2.Id,
                    RestaurantId  = 5, Description = "Rau muống là loại rau quen thuộc trong bữa ăn của người Việt. Với vị đặc trưng ngọt, giòn, mát, được chế biến thành nhiều món ăn. ",
                    Price = 23000,
                    Date_Create = new DateTime(2022, 10, 5), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Nem rán chả giò", FoodImage = imgaddress+"chagio.jpg", UserId = user2.Id,
                    RestaurantId  = 6, Description = "Nem rán là món ăn rất quen thuộc với nhiều người, một phần vì hương vị rất hấp dẫn một phần vì nguyên liệu chính là thịt heo nên rất dễ mua và chế biến.",
                    Price = 3000,
                    Date_Create = new DateTime(2022, 10, 6), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Gỏi cuốn", FoodImage = imgaddress+"goicuon.jpg", UserId = user2.Id,
                    RestaurantId  = 7, Description = "nguyên liệu gồm rau xà lách, húng quế, tía tô, tôm khô, rau thơm, thịt luộc, tôm tươi.. tất cả được cuộn trong vỏ bánh tráng",
                    Price = 5000,
                    Date_Create = new DateTime(2022, 10, 7), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Bún bò Huế", FoodImage = imgaddress+"bunbohue.jpg", UserId = user2.Id,
                    RestaurantId  = 8, Description = "Bún bò Huế không những là đặc sản của xứ Huế mộng mơ mà còn của nền ẩm thực Việt Nam. Sự kết hợp tuyệt vời giữa nước dùng đậm đà hoà cùng chút cay nồng của sa tế và mùi vị đặc biệt của mắm ruốc tạo nên hương vị ngon khó cưỡng cho thực khách ngay lần đầu thưởng thức.",
                    Price = 34000,
                    Date_Create = new DateTime(2022, 10, 8), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Bánh Khọt", FoodImage = imgaddress+"banhkhot.jpg", UserId = user2.Id,
                    RestaurantId  = 9, Description = "Bánh khọt là loại bánh Việt Nam (chính xác là loại bánh đặc trưng của miền nam Việt Nam[1]) làm từ bột gạo hoặc bột sắn, có nhân tôm, được nướng và ăn kèm với rau sống, ớt tươi, thường ăn với nước mắm pha ngọt",
                    Price = 6000,
                    Date_Create = new DateTime(2022, 10, 9), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Gà tần", FoodImage = imgaddress+"gatan.jpg", UserId = user2.Id,
                    RestaurantId  = 10, Description = "Gà tần thuốc bắc hay cũng chính là gà tiềm, gà hầm thuốc bắc lâu nay đã là một trong những món ăn vừa bổ dưỡng, vừa ngon mà từ người lớn cho đến trẻ em đều ưa thích. ",
                    Price = 123000,
                    Date_Create = new DateTime(2022, 10, 10), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Nộm hoa chuối", FoodImage = imgaddress+"nomhoachuoi.jpg", UserId = user2.Id,
                    RestaurantId  = 1, Description = "Nộm hoa chuối là một món ăn ngon, dân dã và được rất nhiều người yêu thích. Với hàng trăm phiên bản chế biến, nhìn chung nộm hoa chuối rất dễ làm, chỉ cần bạn nắm bắt được cách giữ hoa chuối không bị thâm đen là được.",
                    Price = 12000,
                    Date_Create = new DateTime(2022, 10, 11), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Bún bò nam bộ", FoodImage = imgaddress+"bunbonambo.jpg", UserId = user2.Id,
                    RestaurantId  = 2, Description = "đơn giản với sự kết hợp giữa bún, thịt bò, nước mắm chua ngọt, các loại rau thơm, nhưng lại vừa ngon vừa thanh mát, hấp dẫn vô cùng khó cưỡng",
                    Price = 32000,
                    Date_Create = new DateTime(2022, 10, 12), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Hoa quả dầm", FoodImage = imgaddress+"hoaquadam.jpg", UserId = user2.Id,
                    RestaurantId  = 3, Description = "Ngày hè nóng bức mà có một tô hoa quả dầm thì còn gì thích bằng. Ngày hè nóng bức mà có một tô hoa quả dầm thì còn gì thích bằng. Cách làm hoa quả dầm chả có gì khó nhưng bạn đã biết hết các biến thể của nó chưa?\r\n\r\nĐúng như tên gọi, món ăn này chỉ đơn giản là các loại trái cây mix với nhau tùy ý mỗi người nhưng luôn rất được yêu thích. ",
                    Price = 18000,
                    Date_Create = new DateTime(2022, 10, 13), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Phở cuốn", FoodImage = imgaddress+"phocuon.jpg", UserId = user2.Id,
                    RestaurantId  = 4, Description = "Phở cuốn có nguồn gốc từ món phở truyền thống, được biết món này xuất hiện lần đầu tiên ở ngã tư phố Ngũ Xã và đường Nguyễn Khắc Hiếu, Hà Nội.",
                    Price = 26000,
                    Date_Create = new DateTime(2022, 10, 14), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Gà nướng", FoodImage = imgaddress+"ganuong.jpg", UserId = user2.Id,
                    RestaurantId  = 5, Description = "Chỉ cần nghe đến tên, chiếc bụng bé xinh của bạn bỗng đói cồn cào bởi độ ngon của món ăn này. Dù là công thức nào, cách làm ra sao, món gà nướng muối ớt luôn mang đến hương vị thơm ngon, hấp dẫn khó có thể chối từ.",
                    Price = 156000,
                    Date_Create = new DateTime(2022, 10, 15), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Phở xào", FoodImage = imgaddress+"phoxao.jpg", UserId = user2.Id,
                    RestaurantId  = 6, Description = "Công thức hướng dẫn chi tiết cách làm món Phở xào giòn và phở xào mềm thơm ngon hấp dẫn, vô cùng đơn giản.",
                    Price = 29000,
                    Date_Create = new DateTime(2022, 10, 16), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Cà phê trứng", FoodImage = imgaddress+"caphetrung.jpg", UserId = user2.Id,
                    RestaurantId  = 7, Description = "cà phê cũng được biến tấu không ngừng bằng cách pha chế với các nguyên liệu khác như: cà phê cốt dừa, cà phê ngũ cốc, cà phê muối,… Tuy nhiên, cà phê trứng vẫn là một thức uống khó lòng thay thế",
                    Price = 65000,
                    Date_Create = new DateTime(2022, 10, 17), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Bò lá lốt", FoodImage = imgaddress+"bolalot.jpg", UserId = user2.Id,
                    RestaurantId  = 8, Description = "Bò nướng lá lốt là món ăn vặt đường phố vô cùng nổi tiếng mà chắc chắn rằng rất nhiều người trong chúng ta đã từng thử qua. ",
                    Price = 40000,
                    Date_Create = new DateTime(2022, 10, 18), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Xôi", FoodImage = imgaddress+"xoi.jpg", UserId = user2.Id,
                    RestaurantId  = 9, Description = "Xôi là đồ ăn thông dụng được làm từ nguyên liệu chính là gạo nếp, đồ/hấp chín bằng hơi nước, thịnh hành trong ẩm thực của nhiều nước châu Á.",
                    Price = 7000,
                    Date_Create = new DateTime(2022, 10, 19), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                },
                new Food{  Name = "Bánh cuốn nóng", FoodImage = imgaddress+"banhcuon.jpg", UserId = user2.Id,
                    RestaurantId  = 10, Description = "Bánh cuốn nhân thịt nấm dai mềm, vị mằn mặn ngọt ngọt từ thịt, giòn giòn từ nấm ăn cùng với nước chấm chua ngọt chắc hẳn là một món hấp mà ai cũng thích.",
                    Price = 33000,
                    Date_Create = new DateTime(2022, 10, 20), Date_Edit = DateTime.Now,
                    IsBlackList = false, IsDeleted = false
                }
            };
            _db.Food.AddRange(foodList);
            _db.SaveChanges();

            var fakeFood = new Faker<Food>();
            fakeFood.RuleFor(a => a.Name, f => f.Lorem.Sentence(3, 3));
            fakeFood.RuleFor(a => a.FoodImage, f => "/img/food/fooddefault");
            fakeFood.RuleFor(a => a.UserId, f => user2.Id);
            fakeFood.RuleFor(a => a.RestaurantId, f => f.Random.Number(1, 10));
            fakeFood.RuleFor(a => a.Description, f => f.Lorem.Sentence(20, 20));
            fakeFood.RuleFor(a => a.Price, f => f.Random.Number(10000, 10000000));
            fakeFood.RuleFor(a => a.Date_Create, f => f.Date.Between(new DateTime(2005, 1, 1), new DateTime(2010, 1, 1)));
            fakeFood.RuleFor(a => a.Date_Edit, f => f.Date.Between(new DateTime(2010, 1, 1), new DateTime(2011, 1, 1)));
            fakeFood.RuleFor(a => a.IsBlackList, f => false);
            fakeFood.RuleFor(a => a.IsDeleted, f => false);
            for (int i = 0; i < 2563; i++)
            {
                Food food = fakeFood.Generate();
                var numberImg = i % 10;
                food.FoodImage += numberImg + ".jpg";
                _db.Food.Add(food);

                FoodOfUser foodOfUser = new FoodOfUser();
                foodOfUser.FoodId = i+1;
                foodOfUser.UserId = user2.Id;
                _db.FoodOfUser.Add(foodOfUser);
            }
            _db.SaveChanges();

            int foodCount = _db.Food.Count();

            var fakeFeedback = new Faker<Feedback>();
            fakeFeedback.RuleFor(a => a.UserId, f => user2.Id);
            fakeFeedback.RuleFor(a => a.Star, f => f.Random.Number(1, 5));
            fakeFeedback.RuleFor(a => a.Description, f => f.Lorem.Sentence(20, 20));
            fakeFeedback.RuleFor(a => a.Date, f => f.Date.Between(new DateTime(2010, 1, 1), new DateTime(2015, 1, 1)));
            for (int i = 0; i < foodCount-1249; i++)
            {
                Feedback feedback = fakeFeedback.Generate();
                feedback.FoodId = i + 1;
                _db.Feedback.Add(feedback);
            }
            _db.SaveChanges();

            var fakeReport = new Faker<Report>();
            fakeReport.RuleFor(a => a.UserId, f => user2.Id);
            fakeReport.RuleFor(a => a.Reason, f => f.Lorem.Sentence(20, 20));
            fakeReport.RuleFor(a => a.Date, f => f.Date.Between(new DateTime(2010, 1, 1), new DateTime(2015, 1, 1)));
            fakeReport.RuleFor(a => a.Status, f => 0);
            for (int i = 0; i < foodCount-2173; i++)
            {
                Report report = fakeReport.Generate();
                report.FoodId = i + 1;
                _db.Report.Add(report);
            }
            _db.SaveChanges();

            var fakeFoodEaten = new Faker<FoodEaten>();
            fakeFoodEaten.RuleFor(a => a.UserId, f => user2.Id);
            fakeFoodEaten.RuleFor(a => a.IsDeleted, f => false);

            for (var i = 0; i < foodCount-897; i++)
            {
                FoodEaten foodEaten = fakeFoodEaten.Generate();
                foodEaten.FoodId = i + 1;
                _db.FoodEaten.Add(foodEaten);
            }
            _db.SaveChanges();

        }
    }
}
