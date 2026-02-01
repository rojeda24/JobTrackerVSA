using MediatR;

namespace JobTrackerVSA.Web.Infrastructure.MediatR
{
    public class UnhandledExceptionBehavior<TRequest, TResponse>(ILogger<TRequest> logger) 
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                
                logger.LogError(ex, "JobTrackerVSA Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);

                throw;
            }
        }
    }
}
