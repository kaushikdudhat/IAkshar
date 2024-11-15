using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iAkshar.Models;
using iAkshar.Dto;

namespace iAkshar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AskharyatraContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public UserController(AskharyatraContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: api/User

        //  [Authorize]
        [HttpGet]
        public async Task<object> GetUsers()
        {
            try
            {
                if (_context.Users == null)
                {
                    return NotFound();
                }
                return await _context.Users.ToListAsync();

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [HttpGet("GetUsersForDropdown")]
        public async Task<object> GetUsersForDropdown(int sabhaId)
        {
            try
            {
                if (_context.Users == null)
                {
                    return Common.Common.GenerateError("Users Not Found");
                }
                var data = await _context.Users.Where(x => x.Sabhaid == sabhaId && x.Iskaryakarta == true).Select(x => new
                {
                    Id = x.UserId,
                    Name = x.Firstname + " " + (!string.IsNullOrEmpty(x.Middlename) ? x.Middlename[0] : "") + " " + x.Lastname
                }).ToListAsync();

                return Common.Common.GenerateSuccResponse(data);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        [HttpGet("GetDefaultBdayMessage")]
        public async Task<object> GetDefaultBdayMessage(int userId)
        {
            try
            {
                string message = "જય સ્વામિનારાયણ... દાસના દાસ..\r\n🙏" +
                    "જન્મદિવસની શુભકામનાઓ!💐\r\n\r\n" +
                    "🎂 Wish you Happy Birthday to {YuvakName}, 🎉\r\n\r\n" +
                    "બસ એક ‘તું રાજી થા’.\r\n\r\n" +
                    "From :  {UserName}.";
                return Common.Common.GenerateSuccResponse(null, message);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [HttpGet("Login")]
        public async Task<object> Login(string Username, string password)
        {

            var tokenString = string.Empty;

            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Mobile == Username && x.Password == password);

                if (user == null)
                {
                    return Common.Common.GenerateError("User Not Found");
                }
                //    if (user != null)
                //    {
                //        var tokenHandler = new JwtSecurityTokenHandler();
                //        var key = Encoding.ASCII.GetBytes("your-super-secret-key-key-key-key-prabodham");
                //        var tokenDescriptor = new SecurityTokenDescriptor
                //        {
                //            Subject = new ClaimsIdentity(new Claim[]
                //            {
                //new Claim(ClaimTypes.Name, "username"),
                //                // Add more claims as needed
                //            }),
                //            Expires = DateTime.UtcNow.AddDays(7),
                //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                //        };
                //        var token = tokenHandler.CreateToken(tokenDescriptor);
                //        tokenString = tokenHandler.WriteToken(token);
                //    }
                return Common.Common.GenerateSuccResponse(user);
            }
            catch (Exception ex)
            {

                return Common.Common.GenerateError(ex.Message);
            }
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUser(int id)
        {
            //try
            //{
            if (_context.Users == null)
            {
                return Common.Common.GenerateError("User Not Found");
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return Common.Common.GenerateError("User Not Found");
            }

            AksharUserDto yuvakData = new AksharUserDto()
            {
                Address= user.Address,
                BirthDate= user.BirthDate,
                Emailid= user.Emailid,
                Firstname = user.Firstname,
                Followupby = user.Followupby,
                Bloodgroup = user.Bloodgroup,
                Education= user.Education,
                Educationstatus= user.Educationstatus,
                Gender= user.Gender,
                IsAmbrish = user.IsAmbrish,
                Isattending= user.Isattending,
                Isirregular= user.Isirregular,
                Iskaryakarta= user.Iskaryakarta,
                Ispresent= user.Ispresent,
                JoiningDate= user.JoiningDate,
                Lastname= user.Lastname,
                Maritalstatus= user.Maritalstatus,
                Middlename= user.Middlename,
                Mobile= user.Mobile,
                ProfileImagePath= user.ProfileImagePath,
                Pincode= user.Pincode,
                Referenceby= user.Referenceby,
                Roleid = user.Roleid,
                Sabhaid = user.Sabhaid,
                UserId= user.UserId
            };

            var followupYuvakObject = _context.Users.FirstOrDefault(x => x.UserId == user.Followupby);
            yuvakData.FollowupName = followupYuvakObject?.Firstname + " " + followupYuvakObject?.Lastname;

            var referenceByupYuvakObject = _context.Users.FirstOrDefault(x => x.UserId == user.Referenceby);
            if (referenceByupYuvakObject != null)
            {
                yuvakData.ReferenceName = referenceByupYuvakObject?.Firstname + " " + referenceByupYuvakObject?.Lastname;
            }

            var attendence = _context.Attendences.Where(x => x.Userid == id).OrderByDescending(x => x.Sabhatrackid).FirstOrDefault();
            if (attendence != null)
            {
                yuvakData.LastSabhaAttended = _context.SabhaTracks.Where(x => x.Sabhatrackid == attendence.Sabhatrackid).Select(x => x.Date).FirstOrDefault(); ;
            }

            var totalAttendences = _context.Attendences.Where(x => x.Userid == id);
            if (totalAttendences.Count() > 0)
            {
                var attended = totalAttendences.Where(x => x.Ispresent == true).Count();
                yuvakData.Percentage = Math.Round((double)attended / totalAttendences.Count() * 100, 2);
            }
            //catch (Exception ex)
            //{

            //    return new AksharUser();
            //}

            return Common.Common.GenerateSuccResponse(yuvakData);
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("ChangeUser/{id}")]
        public async Task<object> ChangeUser(int id, [FromForm] AksharUSerDto userDto, IFormFile? profileImage)
        {

            var user = _context.Users.FirstOrDefault(x => x.UserId == id);
            if (user == null)
            {
                return Common.Common.GenerateError("User Not Found");
            }

            try
            {
                user.Firstname = userDto.Firstname;
                user.Middlename = userDto.Middlename;
                user.Lastname = userDto.Lastname;
                user.Emailid = userDto.Emailid;
                user.Mobile = userDto.Mobile;
                user.BirthDate = userDto.BirthDate;
                user.Address = userDto.Address;
                user.Pincode = userDto.Pincode;
                user.Education = userDto.Education;
                user.Educationstatus = userDto.Educationstatus;
                user.Gender = userDto.Gender;
                user.Maritalstatus = userDto.Maritalstatus;
                user.Iskaryakarta = userDto.Iskaryakarta;
                user.IsAmbrish = userDto.IsAmbrish;
                user.Followupby = userDto.Followupby;
                user.Referenceby = userDto.Referenceby;
                user.Roleid = userDto.Roleid;
                user.Bloodgroup = userDto.Bloodgroup;
                user.Sabhaid = userDto.Sabhaid;
                if (profileImage != null)
                    user.ProfileImagePath = await uploadFile(profileImage, id);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!UserExists(id))
                {
                    return Common.Common.GenerateError("User Not Found");
                }
                else
                {
                    return Common.Common.GenerateError(ex.Message);
                }
            }

            return Common.Common.GenerateSuccResponse(user, "User updated successfully.");
        }

        private async Task<string> uploadFile(IFormFile? profileImage, int userId)
        {
            if (profileImage != null && profileImage.Length > 0)
            {
                // Define a path to save the file (e.g., wwwroot/images/users/)
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Image");
                var uniqueFileName = userId + Path.GetExtension(profileImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Ensure the directory exists
                Directory.CreateDirectory(uploadsFolder);

                // Check if the file already exists, and if so, delete it
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(fileStream);
                }

                // Update the user's image path in the database
                var imgPath = Path.Combine("Image", uniqueFileName);
                return imgPath.Replace("\\", "/");
            }
            return "";
        }

        [HttpPost("ChangePassword/{id}")]
        public async Task<ActionResult<object>> ChangePassword(int id, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == id);

            if (user != null && user.Password == currentPassword)
            {
                user.Password = newPassword;
            }
            else
            {
                return Common.Common.GenerateError("Current Password is not match");
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Common.Common.GenerateError(ex.Message ?? (ex.InnerException?.Message));
            }

            return Common.Common.GenerateSuccResponse(null, "Password Change successfully");
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<object> PostUser([FromForm] UserDto user, IFormFile? profileImage)
        {
            try
            {

                if (_context.Users == null)
                {
                    return Common.Common.GenerateError("User not found");
                }

                AksharUser entity = new AksharUser();
                entity.Firstname = user.Firstname;
                entity.Lastname = user.Lastname;
                entity.Middlename = user.Middlename;
                entity.Mobile = user.Mobile;
                entity.Password = user.Mobile?.Substring((user.Mobile.Length - 4), 4);
                entity.BirthDate = user.BirthDate;
                entity.Address = user.Address;
                entity.Pincode = user.Pincode;
                entity.Education = user.Education;
                entity.Gender = user.Gender;
                entity.Educationstatus = user.Educationstatus;
                entity.Maritalstatus = user.Maritalstatus;
                entity.Iskaryakarta = user.Iskaryakarta;
                entity.JoiningDate = DateTime.Now;
                entity.Followupby = user.Followupby;
                entity.Referenceby = user.Referenceby;
                entity.Roleid = user.Roleid;
                entity.Bloodgroup = user.Bloodgroup;
                entity.Emailid = user.Emailid;
                entity.Sabhaid = user.Sabhaid;

                //entity.Isattending = user.Isattending;
                //entity.Isirregular = user.Isirregular;
                //entity.Ispresent = user.Ispresent;

                _context.Users.Add(entity);
                await _context.SaveChangesAsync();

                if (profileImage != null)
                {
                    entity.ProfileImagePath = await uploadFile(profileImage, entity.UserId);
                    await _context.SaveChangesAsync();
                }

                return Common.Common.GenerateSuccResponse(entity, "User Added Successfully");
            }
            catch (Exception ex)
            {
                return Common.Common.GenerateError(ex.Message ?? (ex.InnerException?.Message));
            }
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
