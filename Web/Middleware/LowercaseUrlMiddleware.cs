namespace Web.Middleware
{
    public class LowercaseUrlMiddleware
    {
        private readonly RequestDelegate _next;

        public LowercaseUrlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (!string.IsNullOrEmpty(path) && path != path.ToLowerInvariant())
            {
                var query = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "";
                var newUrl = path.ToLowerInvariant() + query;
                context.Response.Redirect(newUrl, permanent: true);
                return;
            }

            await _next(context);
        }
    }
}
