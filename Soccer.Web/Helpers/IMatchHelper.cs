using System.Threading.Tasks;

namespace Soccer.Web.Helpers
{
    public interface IMatchHelper
    {
        Task CloseMatchAsync(int matchid, int goalslocal, int goalsvisitor);
    }
}
