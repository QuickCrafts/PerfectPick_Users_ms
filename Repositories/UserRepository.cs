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

        public UserModel GetUser(int userID)
        {

            using (var _context = new PerfectPickUsersDbContext())
            {
                var user = _context.Users.FindAsync(userID);
                return new UserModel
                {
                    Email = user.Result.Email,
                    Password = user.Result.Password,
                    FirstName = user.Result.FirstName,
                    LastName = user.Result.LastName,
                    Birthdate = user.Result.Birthdate,
                    Gender = user.Result.Gender,
                    CreatedTime = user.Result.CreatedTime,
                    IdCountry = user.Result.IdCountry
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
                        Email = user.Email,
                        Password = user.Password,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Birthdate = user.Birthdate,
                        Gender = user.Gender,
                        CreatedTime = user.CreatedTime,
                        IdCountry = user.IdCountry
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
                        Email = user.Email,
                        Password = user.Password,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Birthdate = user.Birthdate,
                        Gender = user.Gender,
                        CreatedTime = user.CreatedTime,
                        IdCountry = user.IdCountry
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
                    var userToUpdate = _context.Users.Find(userID);
                    userToUpdate.FirstName = user.FirstName ?? userToUpdate.FirstName;
                    userToUpdate.LastName = user.LastName ?? userToUpdate.LastName;
                    userToUpdate.Birthdate = user.Birthdate ?? userToUpdate.Birthdate;
                    userToUpdate.Gender = user.Gender ?? userToUpdate.Gender;
                    userToUpdate.IdCountry = user.IdCountry ?? userToUpdate.IdCountry;

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
                    var user = _context.Users.Find(userID);
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while deleting user: " + e);
            }
        }

        public bool UserExists(int userID)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                return _context.Users.Find(userID) != null;
            }
        }

        public bool UserExists(string email)
        {
            using (var _context = new PerfectPickUsersDbContext())
            {
                return _context.Users.Find(email) != null;
            }
        }
    }
}
