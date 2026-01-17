using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Frontend.Domain.DTOs.Labels;

public class LabelDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Color { get; set; }
}