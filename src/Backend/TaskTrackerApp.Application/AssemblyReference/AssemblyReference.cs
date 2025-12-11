using System.Reflection;

namespace TaskTrackerApp.Application.AssemblyReference;

public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}