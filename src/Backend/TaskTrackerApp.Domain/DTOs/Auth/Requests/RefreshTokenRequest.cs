using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackerApp.Domain.DTOs.Auth.Requests;

public class RefreshTokenRequest
{
    public string? RefreshToken { get; set; } = default!;
    public string Tag { get; set; } = default!;
}