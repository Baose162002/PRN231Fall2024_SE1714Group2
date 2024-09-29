using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IAuthService
    {
        Task<IActionResult> LoginAsync(User loginUser);
    }
}