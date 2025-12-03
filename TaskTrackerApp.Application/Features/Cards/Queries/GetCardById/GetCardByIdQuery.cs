using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Application.DTOs;

namespace TaskTrackerApp.Application.Features.Cards.Queries.GetCardById
{
    public class GetCardByIdQuery : IRequest<CardDto>
    {
        public int Id { get; set; }

        public GetCardByIdQuery(int id)
        {
            Id = id;
        }
    }
}
