using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Domain.Dtos
{
    public record GeneratedRecipeDto
    {
        public string Title { get; set; } = string.Empty;
        public CookingTime CookingTime { get; set; }
        public IList<string> Ingredients { get; set; } = [];
        public IList<GeneratedInstructionDto> Instructions { get; set; } = [];
    }
}
