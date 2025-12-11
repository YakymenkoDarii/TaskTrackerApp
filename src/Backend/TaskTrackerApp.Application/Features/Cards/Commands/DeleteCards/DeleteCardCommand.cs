using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Application.Features.Cards.Commands.DeleteCard;
public class DeleteCardCommand : IRequest
{
    public int Id { get; set; }
}
