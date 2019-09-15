using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Services
{
    public static class JsonFormatter
    {
        public static JsonResult SuccessResponse(object responseData)
        {
            return new JsonResult(new {
                Status = "success",
                Data = responseData
            });
        }
        public static JsonResult FailResponse(object responseData)
        {
            return new JsonResult(new
            {
                Status = "fail",
                Data = responseData
            });
        }
        public static JsonResult ErrorResponse(string responseMessage)
        {

            return new JsonResult(new
            {
                Status = "error",
                Message = responseMessage,
                Data = ""
            });
        }
        public static JsonResult ErrorResponse(string responseMessage, object formattedErrors)
        {

            return new JsonResult(new
            {
                Status = "error",
                Message = responseMessage,
                Data = formattedErrors
            });
        }
        public static JsonResult ErrorResponse(string responseMessage, IDictionary<string,string> errors)
        {
            List<object> responseData = new List<object>();
            foreach(var e in errors)
            {
                responseData.Add(new {
                    Code = e.Key,
                    Description = e.Value
                });
            }
            return new JsonResult(new
            {
                Status = "error",
                Message = responseMessage,
                Data = responseData
            });
        }
        

        internal static IActionResult ValidationProblemResponse(ModelStateDictionary modelState)
        {
            List<object> response = new List<object>();
            var errors = new Hashtable();
            foreach (var pair in modelState)
            {
                if (pair.Value.Errors.Count > 0)
                {
                    foreach (var e in pair.Value.Errors.Select(error => error.ErrorMessage))
                        response.Add(new {
                            code = pair.Key,
                            Description = e
                        });
                }
            }
            
            return ErrorResponse("Validation Problem", response);
        }
    }
}
