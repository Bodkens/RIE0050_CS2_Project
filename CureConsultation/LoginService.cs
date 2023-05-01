using DatabaseLayer;
namespace CureConsultation
{
    public class LoginService
    {
        public LoginService()
        {

        }

        public User? TryLogin(string email, string password)
        {
            List<User> userList = DatabaseManager.Select<User>("SELECT * FROM User WHERE Email = @email AND Password = @password",
                new Dictionary<string, object?>() { { "@email", email }, { "@password", password } });

            if (userList.Count != 0)
            {

                return userList.First();
            }
            return null;
        }


    }
}
