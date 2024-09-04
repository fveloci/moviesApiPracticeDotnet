using Microsoft.EntityFrameworkCore;

namespace APIPeliculas.Utils
{
    public static class HttpContextExtensions
    {
        public async static Task InsertHeaderPaginationParams<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            double quantity = await queryable.CountAsync();
            httpContext.Response.Headers.TryAdd("totalRegistersQuantity", quantity.ToString());
        }
    }
}
