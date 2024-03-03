using _PerfectPickUsers_MS.Models.User;
using _PerfectPickUsers_MS.Repositories;

namespace _PerfectPickUsers_MS.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService()
        {
            _userRepository = new UserRepository();
        }

        public UserModel? GetUser(string userEmail)
        {
            try
            {
                return _userRepository.GetUser(userEmail);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public List<UserModel> GetAllUsers()
        {
            try
            {
                return _userRepository.GetAllUsers();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public bool CreateUser(UserModel user)
        {
            try
            {
                return _userRepository.CreateUser(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void UpdateUser(UserDTO user)
        {
            try
            {
                _userRepository.UpdateUser(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void DeleteUser(string userEmail)
        {
            try
            {
                _userRepository.DeleteUser(userEmail);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public bool UserExists(string userEmail)
        {
            try
            {
                return _userRepository.UserExists(userEmail);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
    }
}
