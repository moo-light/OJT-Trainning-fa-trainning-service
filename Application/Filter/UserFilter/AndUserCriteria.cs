using Application.Interfaces;
using Domain.Entities;

namespace Application.Filter.UserFilter
{
    public class AndUserCriteria : ICriterias<User>
    {
        private ICriterias<User> firstCriteria;
        private ICriterias<User> secondCriteria;
        private ICriterias<User> thirdCriteria;

        public AndUserCriteria(ICriterias<User> firstCriteria, ICriterias<User> secondCriteria, ICriterias<User> thirdCriteria)
        {
            this.firstCriteria = firstCriteria;
            this.secondCriteria = secondCriteria;
            this.thirdCriteria = thirdCriteria;
        }

        public List<User> MeetCriteria(List<User> users)
        {
            List<User> firstCriteriaUsers = firstCriteria.MeetCriteria(users);
            List<User> secondCriteriaUsers = secondCriteria.MeetCriteria(firstCriteriaUsers);
            return thirdCriteria.MeetCriteria(secondCriteriaUsers);
        }
    }
}
