using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.Errors.Label;

public class LabelErrors
{
    public static readonly Error NotFound = new(
    "Label.NotFound", "Label not found.");
}