using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dto;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonorsController : Controller
{
    private readonly IDonorServise donorsService;
    private readonly ILogger<DonorsController> logger;
    public DonorsController(IDonorServise donorsService, ILogger<DonorsController> logger)
    {
        this.donorsService = donorsService;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<DtoDonors>>> GetAllDonors()
    {
        try {
            var donors = await donorsService.GetAllDonors();
            return Ok(donors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }        
    }

    [HttpGet]
    [Route("getByName")]
    public async Task<ActionResult<DonorCreateDto>> GetByName(string name)
    {
        try
        {
            var d = await donorsService.GetByName(name);
            return Ok(d);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }

    }

    [HttpGet]
    [Route("GetByEmail")]
    public async Task<ActionResult<DtoDonors>> GetByEmail(string email)
    {
        try
        {
            var d = await donorsService.GetByEmail(email);
            return Ok(d);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpGet]
    [Route("GetByGift")]
    public async Task<ActionResult<DtoDonors>> GetByGift(string giftName)
    {
        try
        {
            var d = await donorsService.GetByGift(giftName);
            return Ok(d);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> createDonor(DonorCreateDto d)
    {
        try
        {
            await donorsService.CreateDonorAsync(d);
            return Ok(new { success = true, message = "succed" });
        }
        catch (ArgumentNullException argEx)
        {
            return BadRequest("Bad request: " + argEx.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpDelete]
    [Route("deleteDonor")]
    public async Task<ActionResult> deleteDonor(int id)
    {
        try
        {
            await donorsService.DeleteDonorAsync(id);
            return Ok(new { success = true, message = "succed" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult> UpdateDonor(DtoDonors donor, int id)
    {
        try
        {
            await donorsService.UpdateDonorAsync(donor, id);
            return Ok(new { success = true, message = "succed" });
        }
        catch (ArgumentNullException argEx)
        {
            return BadRequest("Bad request: " + argEx.Message);
        }
        catch (ArgumentException ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
        
        catch (KeyNotFoundException ex)
        {
        return NotFound("Not found: " + ex.Message);
        }
    }
    [HttpPut]
    [Route("AddDonation")]
    public async Task<ActionResult<bool>> AddDonation(int id, int giftId)
    {
        try
        {
            var result = await donorsService.AddDonation(id, giftId);
            return Ok(result);
        }
        catch (ArgumentException argEx)
        {
            return BadRequest("Bad request: " + argEx.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}


