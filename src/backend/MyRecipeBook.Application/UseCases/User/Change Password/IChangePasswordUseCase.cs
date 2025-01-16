using MyRecipeBook.Communication.Requests;

namespace MyRecipeBook.Application.UseCases.User.Change_Password
{
    public interface IChangePasswordUseCase
    {
        public Task Execute(RequestChangePasswordJson request);
    }
}
