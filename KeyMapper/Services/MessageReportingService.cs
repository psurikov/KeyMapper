using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyMapper.Services
{
    public interface IMessageReportingService
    {
        void ReportError(string message);
    }

    public class MessageReportingService : IMessageReportingService
    {
        public void ReportError(string message)
        {
            MessageBox.Show(message);
        }
    }
}
