using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Application.Features.Columns.Commands.DeleteColumns;
public class DeleteColumnCommand : IRequest
{
    public int Id { get; set; }
}
