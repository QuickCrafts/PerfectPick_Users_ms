using _PerfectPickUsers_MS.Exceptions;
using _PerfectPickUsers_MS.Functions;
using _PerfectPickUsers_MS.Models.Email;
using _PerfectPickUsers_MS.Models.Login;
using _PerfectPickUsers_MS.Models.User;
using _PerfectPickUsers_MS.Repositories;



namespace _PerfectPickUsers_MS.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly BCryptEncryptor _Encryptor;
        private readonly EmailSender _EmailSender;
        private readonly TokenModule _TokenModule;

        public UserService()
        {
            try
            {
                _EmailSender = new EmailSender();
                _userRepository = new UserRepository();
                _TokenModule = new TokenModule();
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
                user.Password = _Encryptor.Encrypt(user.Password);

                _userRepository.CreateUser(user);
            }
            catch (Exception e)
            {
                throw new Exception("Error while creating user on Database " + e.Message);
            }

            try
            {
                int userID = _userRepository.GetUserIDFromEmail(user.Email);
                EmailDTO emailToSend = new EmailDTO
                {
                    To = user.Email,
                    Subject = "Welcome to PerfectPick!",
                    Body = "Welcome to PerfectPick! Your account has been created successfully. To activate it, please click on the link below: " + "http://localhost:8080/Users/verify?token=" + _TokenModule.GenerateToken(userID, false)
                };

                _EmailSender.SendEmail(emailToSend);
            }
            catch (Exception e)
            {
                var userID = _userRepository.GetUserIDFromEmail(user.Email);
                _userRepository.DeleteUser(userID);
                throw new Exception("Error while creating user email " + e.Message);
            }

            return true;
           
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

        public void VerifyUser(string userToken)
        {
            int? userID;
            try
            {
                userID = _TokenModule.ValidateToken(userToken, false);
                if (userID == null)
                {
                    throw new UserNotFoundException("Invalid token");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            try
            {
                _userRepository.VerifyUser(userID.Value);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void SetupUser(int userID)
        {
            try
            {
                _userRepository.SetupUser(userID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string Login(LoginModel loginInfo)
        {
            try
            {
                int userID = _userRepository.GetUserIDFromEmail(loginInfo.Email);
                if (!_userRepository.UserIsVerified(userID))
                {
                    throw new UserNotVerifiedException("User not verified");
                }
                UserModel user = _userRepository.GetUser(userID);
                bool isRegistered = _Encryptor.Verify(loginInfo.Password, user.Password);
                if(isRegistered)
                {
                    bool isAdmin = _userRepository.UserIsAdmin(userID);
                    return _TokenModule.GenerateToken(userID, true);
                }
                else
                {
                    throw new Exception("Invalid password");
                }
            }catch(UserNotVerifiedException e)
            {
                throw new UserNotVerifiedException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool UserIsAdmin(int userID)
        {
            try
            {
                return _userRepository.UserIsAdmin(userID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public int GetUserIDFromEmail(string email)
        {
            try
            {
                return _userRepository.GetUserIDFromEmail(email);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void ChangePassword(int userID,string currentPassword ,string newPassword)
        {
            try
            {
                string userPassword = _userRepository.GetUser(userID).Password;
                if (!_Encryptor.Verify(currentPassword, userPassword))
                {
                    throw new Exception("Invalid password");
                }
                _userRepository.ChangePassword(userID, _Encryptor.Encrypt(newPassword));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
