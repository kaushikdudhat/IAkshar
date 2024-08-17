using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iAkshar.Models;
using iAkshar.Dto;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

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
                    return NotFound();
                }
                return await _context.Users.Where(x => x.Sabhaid == sabhaId && x.Iskaryakarta==true).Select(x => new
                {
                    Id = x.UserId,
                    Name = x.Firstname + " " + (!string.IsNullOrEmpty(x.Middlename) ? x.Middlename[0] : "") + " " + x.Lastname
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [HttpGet("Login")]
        public async Task<ActionResult> Login(string Username, string password)
        {

            var tokenString = string.Empty;
            if (_context.Users == null)
            {
                return NotFound();
            }
            try
            {


                var user = _context.Users.FirstOrDefault(x => x.Mobile == Username && x.Password == password);
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
                return Ok(user);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AksharUser>> GetUser(int id)
        {
            //try
            //{
                if (_context.Users == null)
                {
                    return NotFound();
                }
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

            //}
            //catch (Exception ex)
            //{

            //    return new AksharUser();
            //}

            return user;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("ChangeUser/{id}")]
        public async Task<IActionResult> ChangeUser(int id, [FromForm] AksharUSerDto userDto, IFormFile? profileImage)
        {

            var user = _context.Users.FirstOrDefault(x => x.UserId == id);
            if (user == null)
            {
                return BadRequest();
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
                user.ProfileImagePath = await uploadFile(profileImage, id);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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
                return Path.Combine("Image", uniqueFileName);
            }
            return "";
        }

        [HttpPost("ChangePassword/{id}")]
        public async Task<IActionResult> ChangePassword(int id, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == id);

            if (user != null && user.Password == currentPassword)
            {
                user.Password = newPassword;
            }
            else
            {
                return BadRequest("Current Password is not match");
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message != null ? ex.Message : ex.InnerException?.Message);
            }

            return Ok("Password Change successfully");
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostUser([FromForm] UserDto user, IFormFile? profileImage)
        {
            try
            {


                if (_context.Users == null)
                {
                    return Problem("Entity set 'AskharyatraContext.Users'  is null.");
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

                entity.Followupby = user.Followupby;
                entity.Referenceby = user.Referenceby;
                entity.Role = _context.Roles.FirstOrDefault(x => x.Roleid == user.Roleid);
                entity.Bloodgroup = user.Bloodgroup;
                entity.Emailid = user.Emailid;
                entity.Sabhaid = user.Sabhaid;

                //entity.Isattending = user.Isattending;
                //entity.Isirregular = user.Isirregular;
                //entity.Ispresent = user.Ispresent;

                _context.Users.Add(entity);
                await _context.SaveChangesAsync();

                entity.ProfileImagePath = await uploadFile(profileImage, entity.UserId);
                await _context.SaveChangesAsync();

                return Ok("User Inserted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
