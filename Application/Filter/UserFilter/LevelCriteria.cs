using Application.Interfaces;
using Domain.Entities;

namespace Application.Filter.UserFilter
{
    public class LevelCriteria : ICriterias<User>
    {
        private string searchCriteria;
        public LevelCriteria(string searchCriteria)
        {
            this.searchCriteria = searchCriteria;
        }
        public List<User> MeetCriteria(List<User> users)
        {
            if (searchCriteria != null)
            {
                List<User> userData = new List<User>();
                foreach (User user in users)
                {
                    if (string.IsNullOrEmpty(user.Level) || user.Level.ToLower().Equals(searchCriteria.ToLower()))
                    {
                        userData.Add(user);
                    }
                }
                return userData;
            }
            else
                return users;
        }
    }
}
