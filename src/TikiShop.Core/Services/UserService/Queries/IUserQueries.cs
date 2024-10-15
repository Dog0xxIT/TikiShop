namespace TikiShop.Core.Services.UserService.Queries;

public interface IUserQueries
{
    Task<dynamic> GetAllAddress(int userId);
}