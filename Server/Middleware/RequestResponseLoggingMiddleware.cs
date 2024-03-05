using CUnity.Server.Data;
using CUnity.Server.Models;
using CUnity.Shared;
using CUnity.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
namespace CUnity.Server.Middleware
{
    public class RequestResponseLoggingMiddleware :BaseMiddleware
    {
        private IServiceScopeFactory _scopeFactory;
        private readonly bool _enableAPILogging;
        private List<string> _ignorePaths;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, IConfiguration configuration) : base(next, logger)
        {
            _enableAPILogging = configuration.GetSection("CUnity:Api:Logging:Enabled").Get<bool>();
            _ignorePaths = configuration.GetSection("CUnity:Api:Logging:IgnorePaths").Get<List<string>>() ?? new List<string>();
        }

        public async Task Invoke(HttpContext httpContext, IServiceScopeFactory scopeFactory, UserManager<ApplicationUser> userManager)
        {
            _scopeFactory = scopeFactory;
            try
            {
                var request = httpContext.Request;
                if (!request.Path.StartsWithSegments(new PathString("/api")))
                {
                    await _next(httpContext);
                }
                else
                {
                    Stopwatch stopWatch = Stopwatch.StartNew();
                    var requestTime = DateTime.UtcNow;

                    var formattedRequest = await FormatRequest(request);
                    var originalBodyStream = httpContext.Response.Body;

                    using (var responseBody = new MemoryStream())
                    {
                        try
                        {

                            string responseBodyContent = null;

                            var response = httpContext.Response;

                            if (new string[] { "/api/localization", "/api/data", "/api/externalauth" }.Any(e => request.Path.StartsWithSegments(new PathString(e.ToLower()))))
                                await _next.Invoke(httpContext);
                            else
                            {
                                response.Body = responseBody;

                                await _next.Invoke(httpContext);
                                if (httpContext.Response.StatusCode == Status200OK)
                                {
                                    //wrap response in ApiResponse
                                    responseBodyContent = await FormatResponse(response);
                                }
                            }


                            stopWatch.Stop();

                            #region Log Request / Response
                            //Search the Ignore paths from appsettings to ignore the loggin of certian api paths
                            if (_enableAPILogging && _ignorePaths.All(e => !request.Path.StartsWithSegments(new PathString(e.ToLower()))))
                            {
                                try
                                {


                                    //User id = "sub" y default
                                    ApplicationUser user = null;
                                    if (httpContext.User.Identity.IsAuthenticated)
                                    {
                                        user = await userManager.FindByNameAsync(httpContext.User.Identity.Name);
                                    }
                                    await SafeLog(requestTime,
                                        stopWatch.ElapsedMilliseconds,
                                        response.StatusCode,
                                        request.Method,
                                        request.Path,
                                        request.QueryString.ToString(),
                                        formattedRequest,
                                        responseBodyContent,
                                        httpContext.Connection.RemoteIpAddress.ToString(),
                                        user);

                                    //await responseBody.CopyToAsync(originalBodyStream);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning("An Inner Middleware exception occurred on SafeLog: " + ex.Message);
                                }
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning("An Inner Middleware exception occurred: " + ex.Message);
                            //Console.WriteLine($"Exception: {ex.Message}, Inner: {ex.InnerException.Message}");
                            //await HandleExceptionAsync(httpContext, ex);
                        }
                        finally
                        {
                            responseBody.Seek(0, SeekOrigin.Begin);
                            await responseBody.CopyToAsync(originalBodyStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // We can't do anything if the response has already started, just abort.
                if (httpContext.Response.HasStarted)
                {
                    _logger.LogWarning("A Middleware exception occurred, but response has already started!");
                    Console.WriteLine($"Exception: {ex.Message}, Inner: {ex.InnerException.Message}");
                    throw;
                }

                //await HandleExceptionAsync(httpContext, ex);
                throw;
            }


        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return $"{request.Method} {request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var plainBodyText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return plainBodyText;
        }
        private async Task SafeLog(DateTime requestTime,
                             long responseMillis,
                             int statusCode,
                             string method,
                             string path,
                             string queryString,
                             string requestBody,
                             string responseBody,
                             string ipAddress,
                             ApplicationUser user)
        {
            if (requestBody.Length > 256)
                requestBody = $"(Truncated to 200 chars) {requestBody.Substring(0, 200)}";

            // If the response body was an ApiResponse we should just save the Result object
            if (responseBody != null && responseBody.Contains("\"result\":"))
            {
                try
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);
                    responseBody = Regex.Replace(apiResponse.Result.ToString(), @"(""[^""\\]*(?:\\.[^""\\]*)*"")|\s+", "$1");
                }
                catch { }
            }

            if (responseBody != null && responseBody.Length > 256)
                responseBody = $"(Truncated to 200 chars) {responseBody.Substring(0, 200)}";

            if (queryString.Length > 256)
                queryString = $"(Truncated to 200 chars) {queryString.Substring(0, 200)}";



            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                dbContext.ApiLogs.Add(new ApiLog
                {
                    RequestTime = requestTime,
                    ResponseMillis = responseMillis,
                    StatusCode = statusCode,
                    Method = method,
                    Path = path,
                    QueryString = queryString,
                    RequestBody = requestBody,
                    ResponseBody = responseBody ?? String.Empty,
                    IPAddress = ipAddress,
                    ApplicationUserId = new Guid()
                });

                await dbContext.SaveChangesAsync();
            }
        }

    }
}
