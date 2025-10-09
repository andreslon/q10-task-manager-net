using Microsoft.Extensions.Configuration;
using Q10.TaskManager.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Q10.TaskManager.Infrastructure.Repositories
{
    public class SettingsRepository : IConfig
    {
         private IConfiguration Configuration { get; set;} 
                 public SettingsRepository(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string GetValue(string key)
        {
            return Configuration[key];
        }
    }
}
