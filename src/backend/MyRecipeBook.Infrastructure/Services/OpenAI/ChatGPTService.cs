using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Services.OpenAI;
using OpenAI_API;
using OpenAI_API.Chat;

namespace MyRecipeBook.Infrastructure.Services.OpenAI
{
    public class ChatGPTService : IGenerateRecipeAI
    {
        private const string _CHAT_MODEL = "gpt-4o";
        private readonly IOpenAIAPI _openAIAPI;

        public ChatGPTService(IOpenAIAPI openAIAPI)
        {
            _openAIAPI = openAIAPI;
        }

        public async Task<GeneratedRecipeDto> Generate(IList<string> ingredients)
        {
            var conversation = _openAIAPI.Chat.CreateConversation(new ChatRequest { Model = _CHAT_MODEL });

            conversation.AppendSystemMessage(ResourceOpenAI.STARTING_GENERATE_RECIPE);

            conversation.AppendUserInput(string.Join(";", ingredients));

            var response = await conversation.GetResponseFromChatbotAsync();

            var responseList = response
                .Split("\n")
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Replace("[", "").Replace("]", ""))
                .ToList();

            var step = 1;

            return new GeneratedRecipeDto
            {
                Title = responseList[0],
                CookingTime = (CookingTime)Enum.Parse(typeof(CookingTime), responseList[1]),
                Ingredients = responseList[2].Split(";"),
                Instructions = responseList[3].Split("?").Select(instruction => new GeneratedInstructionDto
                {
                    Text = instruction,
                    Step = step++
                }).ToList(),
            };
        }
    }
}
