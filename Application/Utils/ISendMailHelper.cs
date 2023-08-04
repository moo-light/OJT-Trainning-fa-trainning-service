using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public interface ISendMailHelper
    {
        public Task<bool> SendMailAsync(string email, string subject, string message);
        public Task<bool> SendMailAsync(List<string> email, string subject, string message);
    }
}
