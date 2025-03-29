using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Application.UseCases.User.Delete.Delete
{
    public class DeleteUserAccountUseCase : IDeleteUserAccountUseCase
    {
        private readonly IBlobStorageService _storageService;
        private readonly IUserDeleteOnlyRepository _deleteOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserAccountUseCase(
            IBlobStorageService storageService,
            IUserDeleteOnlyRepository deleteOnlyRepository,
            IUnitOfWork unitOfWork)
        {
            _storageService = storageService;
            _deleteOnlyRepository = deleteOnlyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(Guid userIdentifier)
        {
            await _storageService.DeleteContainer(userIdentifier);

            await _deleteOnlyRepository.DeleteAccount(userIdentifier);

            await _unitOfWork.Commit();
        }
    }
}
