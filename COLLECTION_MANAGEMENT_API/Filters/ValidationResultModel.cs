using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace COLLECTION_MANAGEMENT_API.Filters
{
    public class ValidationResultModel
    {
        public string status_code { get; set; }
        public object data { get; set; } = new object();
        public List<ErrorResponseData> error_messages { get; set; } = new List<ErrorResponseData>();
        public class ErrorResponseData
        {
            public string error_code { get; set; }
            public string error_message { get; set; }
            public ErrorResponseData(string errorCode = "", string errorMessage = "")
            {
                error_code = errorCode;
                error_message = errorMessage;
            }
        }

        public ValidationResultModel(ModelStateDictionary modelState, HttpContext httpContext)
        {

            try
            {
                string? languageHeader = httpContext.Request.Headers["language"];
                status_code = StatusCodes.Status400BadRequest.ToString();
                var keyList = modelState.Keys.ToList();
                using (var db = new am_dbcontext())
                {
                    var errorMessageData = db.ResponseMessages.ToList();
                    var errorKeys = modelState.Keys.SelectMany(key => modelState[key].Errors.Select(a => key)).ToList();
                    foreach (var item in errorKeys)
                    {
                        if (errorMessageData.Any(a => a.Key == item))
                        {
                            var mainData = errorMessageData.FirstOrDefault(a => a.Key == item);
                            if (mainData != null)
                            {
                                error_messages.Add(new ErrorResponseData
                                {
                                    error_code = mainData.StatusCode ?? string.Empty,
                                    error_message = !string.IsNullOrWhiteSpace(languageHeader) ? languageHeader == "bn" ? mainData.MessageBn ?? string.Empty : mainData.MessageEn : mainData.MessageEn ?? string.Empty
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}
