
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Domain.Services.ServiceBus;

namespace MyRecipeBook.Application.UseCases.User.Delete.Request
{
    public class RequestDeleteUserUseCase : IRequestDeleteUserUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IUserUpdateOnlyRepository _updateOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeleteUserQueue _queue;

        public RequestDeleteUserUseCase(
            ILoggedUser loggedUser,
            IUserUpdateOnlyRepository updateOnlyRepository,
            IUnitOfWork unitOfWork,
            IDeleteUserQueue queue)
        {
            _loggedUser = loggedUser;
            _updateOnlyRepository = updateOnlyRepository;
            _unitOfWork = unitOfWork;
            _queue = queue;
        }

        public async Task Execute()
        {
            var loggedUser = await _loggedUser.User();

            var user = await _updateOnlyRepository.GetById(loggedUser.Id);

            user.Active = false;
            _updateOnlyRepository.Update(user);

            await _unitOfWork.Commit();

            await _queue.SendMessage(user);
        }
    }
}
