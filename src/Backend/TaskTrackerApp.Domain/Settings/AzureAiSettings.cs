using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.Settings;

public class AzureAiSettings
{
    public string Endpoint { get; set; }

    public string ApiKey { get; set; }

    public string SearchEndpoint { get; set; }

    public string SearchApiKey { get; set; }
}