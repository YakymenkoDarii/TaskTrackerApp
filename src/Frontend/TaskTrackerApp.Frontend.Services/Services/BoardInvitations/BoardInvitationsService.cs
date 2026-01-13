using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTrackerApp.Frontend.Domain.DTOs.BoardInvitations;
using TaskTrackerApp.Frontend.Domain.DTOs.Columns;
using TaskTrackerApp.Frontend.Domain.Errors;
using TaskTrackerApp.Frontend.Domain.Results;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.APIs;
using TaskTrackerApp.Frontend.Services.Abstraction.Interfaces.Services;

namespace TaskTrackerApp.Frontend.Services.Services.BoardInvitations;

public class BoardInvitationsService : IBoardInvitationsService
{
    private readonly IBoardInvitationsApi _api;

    public BoardInvitationsService(IBoardInvitationsApi api)
    {
        _api = api;
    }

    public async Task<Result<IEnumerable<BoardInvitationDto>>> GetPendingInvitesAsync(int boardId)
    {
        try
        {
            var response = await _api.GetPendingInvitesAsync(boardId);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<IEnumerable<BoardInvitationDto>>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<BoardInvitationDto>>.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> RevokeInviteAsync(int invitationId)
    {
        try
        {
            var response = await _api.RevokeInviteAsync(invitationId);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> SendInviteAsync(SendBoardInvitationRequestDto request)
    {
        try
        {
            var response = await _api.SendInviteAsync(request);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result> RespondToInvite(RespondToInvitationRequestDto request)
    {
        try
        {
            var response = await _api.RespondToInvite(request);
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("UnknownError", ex.Message));
        }
    }

    public async Task<Result<IEnumerable<MyInvitationDto>>> GetMyPendingInvitations()
    {
        try
        {
            var response = await _api.GetMyPendingInvitations();
            return response.ToResult();
        }
        catch (ApiException ex)
        {
            return Result<IEnumerable<MyInvitationDto>>.Failure(new Error(ClientErrors.NetworkError.Code, ex.Message));
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<MyInvitationDto>>.Failure(new Error("UnknownError", ex.Message));
        }
    }
}