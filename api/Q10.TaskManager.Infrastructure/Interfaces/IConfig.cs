using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q10.TaskManager.Infrastructure.Interfaces
{
    public interface IConfig
    {
        string GetValue(string key);
    }
}
