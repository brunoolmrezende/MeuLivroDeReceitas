using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebApi.Test
{
    public class MyRecipeBookClassFixture : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public MyRecipeBookClassFixture(CustomWebApplicationFactory factory) => _httpClient = factory.CreateClient();

        protected async Task<HttpResponseMessage> DoPost(
            string endpoint, 
            object request, 
            string token = "", 
            string culture = "en")
        {
            ChangeRequestCulture(culture);
            AuthorizeRequest(token);

            return await _httpClient.PostAsJsonAsync(endpoint, request);
        }

        protected async Task<HttpResponseMessage> DoGet(string endpoint, string token = "", string culture = "en")
        {
            ChangeRequestCulture(culture);
            AuthorizeRequest(token);

            return await _httpClient.GetAsync(endpoint);
        }

        protected async Task<HttpResponseMessage> DoPut(string endpoint, object request, string token = "", string culture = "en")
        {
            ChangeRequestCulture(culture);
            AuthorizeRequest(token);

            return await _httpClient.PutAsJsonAsync(endpoint, request);
        }

        protected async Task<HttpResponseMessage> DoDelete(string endpoint, string token = "", string culture = "en")
        {
            ChangeRequestCulture(culture);
            AuthorizeRequest(token);

            return await _httpClient.DeleteAsync(endpoint);
        }

        protected async Task<HttpResponseMessage> DoPostFormData(
            string endpoint,
            object request,
            string token = "",
            string culture = "en")
        {
            ChangeRequestCulture(culture);
            AuthorizeRequest(token);

            var multipartContent = new MultipartFormDataContent();

            var requestProperties = request.GetType().GetProperties().ToList();

            foreach (var property in requestProperties)
            {
                var propertyValue = property.GetValue(request);

                if (string.IsNullOrWhiteSpace(propertyValue?.ToString()))
                    continue;

                if (propertyValue is System.Collections.IList list)
                {
                    AddListToMultipartContent(multipartContent, property.Name, list);
                }
                else
                {
                    multipartContent.Add(new StringContent(propertyValue.ToString()!), property.Name);
                }
            }

            return await _httpClient.PostAsync(endpoint, multipartContent);
        }

        private void ChangeRequestCulture(string culture)
        {
            if (_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
                _httpClient.DefaultRequestHeaders.Remove("Accept-Language");

            _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);
        }

        private void AuthorizeRequest(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private void AddListToMultipartContent(
            MultipartFormDataContent multipartContent,
            string propertyName,
            System.Collections.IList list)
        {
            var itemType = list.GetType().GetGenericArguments().Single();

            if (itemType.IsClass && itemType != typeof(string))
            {
                AddClassListToMultipartContent(multipartContent, propertyName, list);
            }
            else
            {
                foreach (var item in list)
                {
                    multipartContent.Add(new StringContent(item.ToString()!), propertyName);
                }
            }
        }

        private void AddClassListToMultipartContent(
            MultipartFormDataContent multipartContent,
            string propertyName,
            System.Collections.IList list)
        {
            var index = 0;

            foreach (var item in list)
            {
                var classPropertiesInfo = item.GetType().GetProperties().ToList();

                foreach (var prop in classPropertiesInfo)
                {
                    var value = prop.GetValue(item, null);
                    multipartContent.Add(new StringContent(value!.ToString()!), $"{propertyName}[{index}][{prop.Name}]");
                }

                index++;
            }
        }
    }
}
