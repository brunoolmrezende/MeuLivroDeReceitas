using AutoMapper;
using CommonTestUtilities.IdEncrypter;
using MyRecipeBook.Application.Services.AutoMapper;

namespace CommonTestUtilities.AutoMapper
{
    public class MapperBuilder
    {
        public static IMapper Build()
        {
            var sqids = IdEncrypterBuilder.Build();

            return new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapping(sqids));
            }).CreateMapper();
        }
    }
}
