using _PerfectPickUsers_MS.DB;
using _PerfectPickUsers_MS.Exceptions;
using _PerfectPickUsers_MS.Models.User;
using Microsoft.EntityFrameworkCore;

namespace _PerfectPickUsers_MS.Repositories
{
    public class UserRepository
    {

        public UserRepository()
        {

        }

        public UserModel GetUser(string userEmail)
        {

            using (var _context = new PerfectPickUsersDbContext())
            {
                var user = _context.Users.FindAsync(userEmail);
                return new UserModel
                {
                    Email = user.Result.UserEmail,
                    FirstName = user.Result.UserName,
                    LastName = user.Result.UserSurname,
                    IsAdmin = user.Result.UserIsAdmin
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
                        Email = user.UserEmail,
                        FirstName = user.UserName,
                        LastName = user.UserSurname,
                        IsAdmin = user.UserIsAdmin
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
                    if (_context.Users.Find(user.Email) != null)
                    {
                        return false;
                    }
                    var newUser = new User
                    {
                        UserEmail = user.Email,
                        UserName = user.FirstName,
                        UserSurname = user.LastName,
                        UserPassword = user.Password,
                        UserIsAdmin = false
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

        public void UpdateUser(UserDTO user)
        {
            try
            {
                using (var _context = new PerfectPickUsersDbContext())
                {
                    var userToUpdate = _context.Users.Find(user.Email);
                    userToUpdate.UserName = user.FirstName ?? userToUpdate.UserName;
                    userToUpdate.UserSurname = user.LastName ?? userToUpdate.UserSurname;
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while updating user: " + e);
            }

        }

        public void DeleteUser(string userEmail)
        {
            try
            {
                using (var _context = new PerfectPickUsersDbContext())
                {
                    var user = _context.Users.Find(userEmail);
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while deleting user: " + e);
            }
        }

        public bool UserExists(string userEmail)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                return _context.Users.Find(userEmail) != null;
            }
        }
    }
}
