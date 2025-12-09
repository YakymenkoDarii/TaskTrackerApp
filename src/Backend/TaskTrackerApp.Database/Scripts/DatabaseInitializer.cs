using DbUp;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Database.Scripts
{
    public class DatabaseInitializer
    {

        public static void  Initialize(string connectionString)
        {
            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(typeof(DatabaseInitializer).Assembly)
                    .LogToConsole()
                    .Build();

            if (upgrader.IsUpgradeRequired())
            {
                var result = upgrader.PerformUpgrade();

                if (!result.Successful)
                {
                    throw result.Error;
                }
            }
        }
    }
}
