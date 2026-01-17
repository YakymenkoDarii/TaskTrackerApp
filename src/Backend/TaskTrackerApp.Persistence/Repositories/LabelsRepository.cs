using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.Interfaces.Repositories;
using TaskTrackerApp.Domain.Entities;
using TaskTrackerApp.Persistence.Contexts;

namespace TaskTrackerApp.Persistence.Repositories;

public class LabelsRepository : Repository<Label, int>, ILabelsRepository
{
    public LabelsRepository(TaskTrackerDbContext context) : base(context)
    {
    }
}