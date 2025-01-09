using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestDoLoginJsonBuilder
    {
        public static RequestDoLoginJson Build()
        {
            return new Faker<RequestDoLoginJson>()
                .RuleFor(user => user.Email, (f) => f.Internet.Email())
                .RuleFor(user => user.Password, (f) => f.Internet.Password());
        }
    }
}
