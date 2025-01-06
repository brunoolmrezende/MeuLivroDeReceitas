using System.Globalization;

namespace MyRecipeBook.API.Middleware
{
    public class CultureMiddleware
    {
        private readonly RequestDelegate _next;
        public CultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var supportedLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures);

            var requestCulture = context.Request.Headers.AcceptLanguage;

            var cultureInfo = new CultureInfo("en");

            if (!string.IsNullOrWhiteSpace(requestCulture)
                && supportedLanguages.Any(x => x.Name.Equals(requestCulture))) 
            {
                cultureInfo = new CultureInfo(requestCulture!);
            }

            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            await _next(context);
        }
    }
}