using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestChangePasswordJsonBuilder
    {
        public static RequestChangePasswordJson Build(int passwordLength = 10)
        {
            return new Faker<RequestChangePasswordJson>()
                .RuleFor(x => x.Password, (f) => f.Internet.Password())
                .RuleFor(x => x.NewPassword, (f) => f.Internet.Password(length: passwordLength));
        }
    }
}
