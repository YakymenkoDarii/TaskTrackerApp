using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardMembers;
using TaskTrackerApp.Frontend.Domain.DTOs.Columns;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.BoardMembers;

public class BoardMembersService : IBoardMembersService
{
    private readonly IBoardMembersApi _boardMembersApi;

    public BoardMembersService(IBoardMembersApi boardMembersApi)
    {
        _boardMembersApi = boardMembersApi;
    }

    public async Task<Result<IEnumerable<BoardMemberDto>>> GetMembersAsync(int boardId)
    {
        try
        {
            var response = await _boardMembersApi.GetMembersAsync(boardId);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<IEnumerable<BoardMemberDto>>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<BoardMemberDto>>.Failure(new Error("UnknownError", ex.Message));
        }
    }
}