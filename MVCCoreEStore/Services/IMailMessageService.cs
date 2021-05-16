using System.Threading.Tasks;

namespace MVCCoreEStore.Services
{
    public interface IMailMessageService
    {
        Task<bool> Send(string to, string subject, string body);
    }
}
