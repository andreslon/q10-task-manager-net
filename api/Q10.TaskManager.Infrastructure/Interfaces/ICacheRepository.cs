using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q10.TaskManager.Infrastructure.Interfaces
{
    public interface ICacheRepository
    {
        string? Get(string key);
        void Set(string key, string? value);
        void Remove(string key);
        bool Exists(string key);
    }
}
