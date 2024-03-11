using _PerfectPickUsers_MS.DB;
using _PerfectPickUsers_MS.Exceptions;
using _PerfectPickUsers_MS.Models.User;
using Microsoft.EntityFrameworkCore;

namespace _PerfectPickUsers_MS.Repositories
{
    public class UserRepository
    {
        public UserModel GetUser(int userID)
        {

            using (var _context = new PerfectPickUsersDbContext())
            {
                User? user = _context.Users.Find(userID);
                if (user == null)
                {
                    throw new UserNotFoundException("User not found");
                }
                return new UserModel
                {
                    IdUser = user.IdUser,
                    Email = user.Email,
                    Password = user.Password,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Birthdate = user.Birthdate,
                    Verified = user.Verified,
                    Setup = user.Setup,
                    CreatedTime = user.CreatedTime,
                    Role = user.Role,
                    AvatarUrl = user.AvatarUrl
                    
                };
            }

        }

        public List<UserModel> GetAllUsers()
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                var users = _context.Set<User>().ToList();
                var usersList = new List<UserModel>();
                foreach (var user in users)
                {
                    usersList.Add(new UserModel
                    {
                        IdUser = user.IdUser,
                        Email = user.Email,
                        Password = user.Password,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Birthdate = user.Birthdate,
                        Verified = user.Verified,
                        Setup = user.Setup,
                        CreatedTime = user.CreatedTime,
                        Role = user.Role
                    });
                }
                return usersList;
            }
        }

        public bool CreateUser(UserModel user)
        {
            try
            {
                using (var _context = new PerfectPickUsersDbContext())
                {
                    var newUser = new User
                    {
                        Email = user.Email,
                        Password = user.Password,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Birthdate = user.Birthdate,
                        Verified = false,
                        Setup = false,
                        CreatedTime = DateTime.Now.ToString(),
                        Role = user.Role

                    };
                    _context.Users.Add(newUser);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while creating user: " + e);
            }



        }

        public void UpdateUser(UserDTO user, int userID)
        {
            try
            {
                using (var _context = new PerfectPickUsersDbContext())
                {
                    User userToUpdate = _context.Users.Find(userID) ?? throw new UserNotFoundException("User not found");
                    userToUpdate.FirstName = user.FirstName ?? userToUpdate.FirstName;
                    userToUpdate.LastName = user.LastName ?? userToUpdate.LastName;
                    userToUpdate.Birthdate = user.Birthdate ?? userToUpdate.Birthdate;
                    userToUpdate.Gender = user.Gender ?? userToUpdate.Gender;
                    userToUpdate.IdCountry = user.IdCountry ?? userToUpdate.IdCountry;
                    userToUpdate.AvatarUrl = user.AvatarUrl ?? userToUpdate.AvatarUrl;

                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while updating user: " + e);
            }

        }

        public void DeleteUser(int userID)
        {
            try
            {
                using (var _context = new PerfectPickUsersDbContext())
                {
                    User userToRemove = _context.Users.Find(userID) ?? throw new UserNotFoundException("User not found");
                    _context.Users.Remove(userToRemove);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while deleting user: " + e);
            }
        }

        public void VerifyUser(int userID)
        {
            try
            {
                using (var _context = new PerfectPickUsersDbContext())
                {
                    User userToVerify = _context.Users.Find(userID) ?? throw new UserNotFoundException("User not found");
                    userToVerify.Verified = true;
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while verifying user: " + e);
            }
        }

        public void SetupUser(int userID)
        {
            try
            {
                using (var _context = new PerfectPickUsersDbContext())
                {
                    User userToSetup = _context.Users.Find(userID) ?? throw new UserNotFoundException("User not found");
                    userToSetup.Setup = true;
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while setting up user: " + e);
            }
        }

        public bool UserIDExists(int userID)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                return _context.Users.Find(userID) != null;
            }
        }

        public bool UserEmailExists(string email)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                var user = _context.Users.SingleOrDefault(u => u.Email == email);
                return user != null;
            }
        }

        public bool UserIsVerified(int userID)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                var user = _context.Users.Find(userID) ?? throw new UserNotFoundException("User not found");
                return user.Verified;
            }
        }

        public bool UserIsSetup(int userID)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                var user = _context.Users.Find(userID) ?? throw new UserNotFoundException("User not found");
                return user.Setup;
            }
        }

        public int GetUserIDFromEmail(string email)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                var user = _context.Users.SingleOrDefault(u => u.Email == email);
                if (user == null)
                {
                    throw new UserNotFoundException("User not found");
                }
                return user.IdUser;
            }
        }

        public bool UserIsAdmin(int userID)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                var user = _context.Users.Find(userID) ?? throw new UserNotFoundException("User not found");
                bool userRole = user.Role;
                return userRole;
            }
        }

        public void ChangePassword(int userID, string newPassword)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                var user = _context.Users.Find(userID) ?? throw new UserNotFoundException("User not found");
                user.Password = newPassword;
                _context.SaveChanges();
            }
        }
    }
}
