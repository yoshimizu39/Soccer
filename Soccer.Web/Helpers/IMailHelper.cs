using Soccer.Common.Models;

namespace Soccer.Web.Helpers
{
    public interface IMailHelper
    {
        //devuelve un response
        Response SendMail(string to, string subject, string body);
    }
}
