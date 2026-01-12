using Microsoft.AspNetCore.Mvc;
using WindowsMediaController.Models;
using WindowsMediaController.Services;

namespace WindowsMediaController.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VolumeController : ControllerBase
{
    private readonly IVolumeService _volumeService;
    private readonly ILogger<VolumeController> _logger;

    public VolumeController(IVolumeService volumeService, ILogger<VolumeController> logger)
    {
        _volumeService = volumeService;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<ApiResponse<float>> GetVolume()
    {
        try
        {
            float volume = _volumeService.GetVolume();
            return Ok(ApiResponse<float>.Ok(volume));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting volume");
            return StatusCode(500, ApiResponse<object?>.Fail(ex.Message));
        }
    }

    [HttpPost("set/{level}")]
    public ActionResult<ApiResponse<float>> SetVolume(float level)
    {
        if (level > 1)
        {
            return StatusCode(400, ApiResponse<object?>.Fail("Level must be between 0 and 1"));
        }

        try
        {
            _volumeService.SetVolume(level);
            return Ok(ApiResponse<float>.Ok(_volumeService.GetVolume()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting volume to {Level}", level);
            return StatusCode(500, ApiResponse<object?>.Fail(ex.Message));
        }
    }

    [HttpPost("up")]
    public ActionResult<ApiResponse<float>> VolumeUp()
    {
        try
        {
            _volumeService.VolumeUp();
            return Ok(ApiResponse<float>.Ok(_volumeService.GetVolume()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error increasing volume");
            return StatusCode(500, ApiResponse<object?>.Fail(ex.Message));
        }
    }

    [HttpPost("down")]
    public ActionResult<ApiResponse<float>> VolumeDown()
    {
        try
        {
            _volumeService.VolumeDown();
            return Ok(ApiResponse<float>.Ok(_volumeService.GetVolume()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decreasing volume");
            return StatusCode(500, ApiResponse<object?>.Fail(ex.Message));
        }
    }

    [HttpPost("mute")]
    public ActionResult<ApiResponse<bool>> ToggleMute()
    {
        try
        {
            if (_volumeService.IsMuted())
            {
                _volumeService.Unmute();
            }
            else
            {
                _volumeService.Mute();
            }
            return Ok(ApiResponse<bool>.Ok(_volumeService.IsMuted(), _volumeService.IsMuted() ? "已静音" : "已取消静音"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling mute");
            return StatusCode(500, ApiResponse<object?>.Fail(ex.Message));
        }
    }
}
