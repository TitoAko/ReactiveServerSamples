using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Interfaces
{
    public interface IMessageType
    {
        void ProcessMessage(string sender, string content);
    }
}
