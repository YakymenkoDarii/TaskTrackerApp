using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.DTOs.User;

public class UserSummaryDto
{
    public int Id { get; set; }

    public string DisplayName { get; set; }

    public string Email { get; set; }

    public string Tag { get; set; }

    public string AvatarUrl { get; set; }
}