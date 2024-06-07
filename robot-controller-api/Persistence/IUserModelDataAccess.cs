using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public interface IUserModelDataAccess
    {
        List<UserModel> GetAllUsers();
        List<UserModel> GetAllAdmins();
        UserModel GetUserById(int id);
        UserModel GetUserByEmail(string email);
        UserModel AddUser(UserModel newUser);
        void UpdateUser(UserModel updatedUser);
        void DeleteUser(int id);
        void PatchUser(int id, LoginModel loginModel);
    }
}
