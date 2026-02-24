using TaskTrackerApp.Frontend.Domain.Enums;

namespace TaskTrackerApp.Frontend.Services.Helpers;

public static class BoardHex
{
    public static string GetThemeHex(BoardThemeColor color) => color switch
    {
        BoardThemeColor.EmeraldGreen => "#10B981",
        BoardThemeColor.SunsetOrange => "#F59E0B",
        BoardThemeColor.RubyRed => "#EF4444",
        BoardThemeColor.AmethystPurple => "#8B5CF6",
        BoardThemeColor.SlateGray => "#64748B",
        _ => "#264575"
    };
}