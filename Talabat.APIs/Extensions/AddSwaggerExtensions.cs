namespace Talabat.APIs.Extensions
{
    public static class AddSwaggerExtensions
    {
        public static WebApplication UseSwaggerMiddleWares(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
