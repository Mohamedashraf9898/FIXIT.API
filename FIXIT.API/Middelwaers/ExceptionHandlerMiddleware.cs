using System.Net;
using FIXIT.API.Erorrs;
using FIXIT.API.Erorrs.Exceptions;

namespace FIXIT.API.Midelwaers
{
    public class ExceptionHandlerMiddlewares
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlerMiddlewares> logger;
        private readonly IWebHostEnvironment environment;

        public ExceptionHandlerMiddlewares(RequestDelegate next, ILogger<ExceptionHandlerMiddlewares> logger, IWebHostEnvironment environment)
        {
            this.next = next;
            this.logger = logger;
            this.environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
                if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    var response = new ApiResponse((int)HttpStatusCode.NotFound, $"The Requsted Endpoint : {context.Request.Path} is Not Found. ");
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(response.ToString());
                }
            }
            catch (Exception ex)
            {
                #region loggin
                if (environment.IsDevelopment())
                {
                    logger.LogError(ex, ex.Message, ex.StackTrace!.ToString());

                }
                else
                {
                    //log exption in extirnal resorse 
                }

                ApiResponse response;
                #endregion
                switch (ex)
                {
                    case NotFoundException:
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        response = new ApiResponse((int)HttpStatusCode.NotFound, ex.Message);
                        break;
                    case BadRequestException:
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response = new ApiResponse((int)HttpStatusCode.BadRequest, ex.Message);
                        break;
                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response = environment.IsDevelopment() ?
                     new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace!.ToString()) :
                     new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message);
                        break;
                }


                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(response.ToString());
            }

        }



    }
}
