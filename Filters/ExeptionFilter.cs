using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webAPIAutores.Filters
{
    public class ExeptionFilter: ExceptionFilterAttribute
    {
        private readonly ILogger<ExeptionFilter> logger;

        public ExeptionFilter(ILogger<ExeptionFilter> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);

            base.OnException(context);
        }
    }
}
