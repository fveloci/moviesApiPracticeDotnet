using Microsoft.AspNetCore.Mvc.Filters;

namespace APIPeliculas.Filters
{
    public class MyActionFilter : IActionFilter
    {
        private readonly ILogger<MyActionFilter> logger;

        public MyActionFilter(ILogger<MyActionFilter> logger)
        {
            this.logger = logger;
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Antes de ejecutar accion");
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("Despues de ejecutar accion");
        }
    }
}
