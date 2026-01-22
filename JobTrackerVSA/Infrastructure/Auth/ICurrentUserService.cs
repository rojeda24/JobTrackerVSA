namespace JobTrackerVSA.Web.Infrastructure.Auth
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
    }
}
