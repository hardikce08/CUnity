using CUnity.Server.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CUnity.Server.Middleware
{
    public class ExceptionLogging
    {
        private IServiceScopeFactory _scopeFactory;
        public ExceptionLogging(IServiceScopeFactory scopeFactory)
        {
            this._scopeFactory = scopeFactory;
        }
        public static async Task SendExcepToDB(string MethodName, Exception exdb, ApplicationDBContext dbContext)
        {
                dbContext.ErrorLogs.Add(new Shared.Models.ErrorLog
                {
                    ErrorDate = DateTime.Now,
                    ErrorDescription = MethodName + " ErrorMessage: " + exdb.Message,
                    ErrorMessage = exdb.InnerException != null ? exdb.InnerException.Message : "",
                    ErrorType = exdb.GetType().Name,
                    ErrorSource = exdb.StackTrace
                });

                await dbContext.SaveChangesAsync();
        }
        public  async Task SendExcepToDB(string MethodName, Exception exdb)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                dbContext.ErrorLogs.Add(new Shared.Models.ErrorLog
                {
                    ErrorDate = DateTime.Now,
                    ErrorDescription = MethodName + " ErrorMessage: " + exdb.Message,
                    ErrorMessage = exdb.InnerException != null ? exdb.InnerException.Message : "",
                    ErrorType = exdb.GetType().Name,
                    ErrorSource = exdb.StackTrace
                });

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
