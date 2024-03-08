using _PerfectPickUsers_MS.Functions;
using _PerfectPickUsers_MS.Models.Email;
using _PerfectPickUsers_MS.Models.User;
using _PerfectPickUsers_MS.Repositories;



namespace _PerfectPickUsers_MS.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly BCryptEncryptor _Encryptor;
        private readonly EmailSender _EmailSender;

        public UserService()
        {
            try
            {
                _EmailSender = new EmailSender();
                _userRepository = new UserRepository();
            }
            catch (Exception e)
            {
                throw new Exception("Error while creating UserRepository: " + e.Message);
            }

            try
            {
                _Encryptor = new BCryptEncryptor();
            }
            catch (Exception e)
            {
                throw new Exception("Error while creating Encryptor: " + e.Message);
            }


        }

        public UserModel? GetUser(int userID)
        {
            try
            {
                return _userRepository.GetUser(userID);
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
                EmailDTO emailToSend = new EmailDTO
                {
                    To = user.Email,
                    Subject = "Welcome to PerfectPick!",
                    Body = "Welcome to PerfectPick! Your account has been created successfully."
                };

                _EmailSender.SendEmail(emailToSend);
            } catch (Exception e)
            {
                throw new Exception("Error while creating user email "+ e.Message);
            }
            try
            {
                user.Password = _Encryptor.Encrypt(user.Password);

                return _userRepository.CreateUser(user);
            }
            catch (Exception e)
            {
                throw new Exception("Error while creating user on Database "+ e.Message);
            }

        }

        public void UpdateUser(UserDTO user, int userID)
        {
            try
            {
                _userRepository.UpdateUser(user, userID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void DeleteUser(int userID)
        {
            try
            {
                _userRepository.DeleteUser(userID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public bool UserIDExists(int userID)
        {
            try
            {
                return _userRepository.UserIDExists(userID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public bool UserEmailExists(string email)
        {
            try
            {
                return _userRepository.UserEmailExists(email);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
