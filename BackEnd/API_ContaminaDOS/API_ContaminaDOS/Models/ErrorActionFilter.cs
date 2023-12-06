using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using API_ContaminaDOS.Models.DTO;

namespace API_ContaminaDOS.Models
{
    public class ErrorActionFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is BadRequestObjectResult badRequestObjectResult)
            {
                if (badRequestObjectResult.Value is ValidationProblemDetails)
                {
                    var error = new ErrorResponse
                    {
                        status = 400,
                        msg = "Bad Payload"
                    };
                    context.Result = new BadRequestObjectResult(error);
                }
                else if (badRequestObjectResult.Value is UnsupportedMediaTypeResult)
                {
                    var error = new ErrorResponse
                    {
                        status = 415,
                        msg = "Unsupported Media Type"
                    };
                    context.Result = new ObjectResult(error)
                    {
                        StatusCode = 415
                    };
                }
            }

            base.OnResultExecuting(context);
        }
    }

}
