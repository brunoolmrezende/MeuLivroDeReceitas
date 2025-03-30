using Moq;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories
{
    public class UserReadOnlyRepositoryBuilder
    {
        private readonly Mock<IUserReadOnlyRepository> _readOnlyRepository;
        public UserReadOnlyRepositoryBuilder() => _readOnlyRepository = new Mock<IUserReadOnlyRepository>();

        public void ExistActiveUserWithEmail(string email)
        {
            _readOnlyRepository.Setup(repository => repository.ExistActiveUserWithEmail(email)).ReturnsAsync(true);
        }

        public void GetByEmailAndPassword(User user)
        {
            _readOnlyRepository.Setup(repository => repository.GetByEmailAndPassword(user.Email, user.Password)).ReturnsAsync(user);
        }

        public void GetByEmail(User user)
        {
            _readOnlyRepository.Setup(repository => repository.GetByEmail(user.Email)).ReturnsAsync(user);
        }

        public IUserReadOnlyRepository Build() => _readOnlyRepository.Object;
    }
}
