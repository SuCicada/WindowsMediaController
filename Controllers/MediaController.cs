using Microsoft.AspNetCore.Mvc;
using WindowsMediaController.Models;
using WindowsMediaController.Services;

namespace WindowsMediaController.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    private readonly ILogger<MediaController> _logger;

    public MediaController(IMediaService mediaService, ILogger<MediaController> logger)
    {
        _mediaService = mediaService;
        _logger = logger;
    }

    [HttpGet("status")]
    public async Task<ActionResult<ApiResponse<MediaStatus>>> GetStatus()
    {
        try
        {
            var status = await _mediaService.GetStatusAsync();
            return Ok(ApiResponse<MediaStatus>.Ok(status));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media status");
            return StatusCode(500, ApiResponse<object?>.Fail(ex.Message));
        }
    }

    [HttpPost("play-pause")]
    public async Task<ActionResult<ApiResponse<object?>>> PlayPause()
    {
        try
        {
            await _mediaService.PlayPauseAsync();
            return Ok(ApiResponse<object?>.Ok(null, "Play/Pause signal sent"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Play/Pause");
            return StatusCode(500, ApiResponse<object?>.Fail(ex.Message));
        }
    }
    [HttpPost("play")]
    public async Task<ActionResult<ApiResponse<object?>>> Play()
    {

        var status = await _mediaService.GetStatusAsync();
        if (!status.IsPlaying)
        {
            await _mediaService.PlayPauseAsync();
            return Ok(ApiResponse<object?>.Ok(null, "Play signal sent"));
        }
        else
        {
            return Ok(ApiResponse<object?>.Ok(null, "Already playing"));
        }

    }
    [HttpPost("pause")]
    public async Task<ActionResult<ApiResponse<object?>>> Pause()
    {
        var status = await _mediaService.GetStatusAsync();
        if (status.IsPlaying)
        {
            await _mediaService.PlayPauseAsync();
            return Ok(ApiResponse<object?>.Ok(null, "Pause signal sent"));
        }
        else
        {
            return Ok(ApiResponse<object?>.Ok(null, "Already paused"));
        }
    }

    [HttpPost("stop")]
    public async Task<ActionResult<ApiResponse<object?>>> Stop()
    {
        try
        {
            await _mediaService.StopAsync();
            return Ok(ApiResponse<object?>.Ok(null, "Stop signal sent"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Stop");
            return StatusCode(500, ApiResponse<object?>.Fail(ex.Message));
        }
    }

    [HttpPost("next")]
    public async Task<ActionResult<ApiResponse<object?>>> Next()
    {
        try
        {
            await _mediaService.NextAsync();
            return Ok(ApiResponse<object?>.Ok(null, "Next signal sent"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Next");
            return StatusCode(500, ApiResponse<object?>.Fail(ex.Message));
        }
    }

    [HttpPost("previous")]
    public async Task<ActionResult<ApiResponse<object?>>> Previous()
    {
        try
        {
            await _mediaService.PreviousAsync();
            return Ok(ApiResponse<object?>.Ok(null, "Previous signal sent"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Previous");
            return StatusCode(500, ApiResponse<object?>.Fail(ex.Message));
        }
    }
}
