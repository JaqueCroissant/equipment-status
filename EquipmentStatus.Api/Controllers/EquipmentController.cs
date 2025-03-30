using EquipmentStatus.Api.Dto;
using EquipmentStatus.Service.Core;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentStatus.Api.Controllers;

[ApiController]
[Route("equipment")]
public class EquipmentController(
    IEquipmentRepository equipmentRepository,
    IEquipmentStateParser equipmentStateParser) 
    : ControllerBase
{
        
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetLatest()
    {
        var latestStates = await equipmentRepository.GetLatestStates();
        var dtos = ToEquipmentHistoryDtos(latestStates);
        
        return new OkObjectResult(dtos);
    }
    
        
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Upsert(EquipmentHistoryDto dto)
    {
        var validEquipmentState = equipmentStateParser
            .Validate(dto.Id, dto.State, dto.TimeStamp);
        
        if (validEquipmentState is null)
        {
            return BadRequest();
        }
        
        await equipmentRepository.Upsert(validEquipmentState);
        
        return new CreatedResult();
    }
    
    [HttpGet]
    [Route("history/from/{from}/to/{to}")]
    public async Task<IActionResult> GetEquipmentHistory(DateTimeOffset from, DateTimeOffset to)
    {
        if (from > to)
        {
            return BadRequest();
        }
        
        var equipmentHistory = await equipmentRepository
            .GetHistory(from, to);

        if (equipmentHistory.Count == 0)
        {
            return NotFound();
        }
        
        var dtos = ToEquipmentHistoryDtos(equipmentHistory);
        
        return new OkObjectResult(dtos);
    }
    
    [HttpGet]
    [Route("history/{id}/from/{from}/to/{to}")]
    public async Task<IActionResult> GetEquipmentHistoryById(string id, DateTimeOffset from, DateTimeOffset to)
    {
        if (string.IsNullOrWhiteSpace(id) || from > to)
        {
            return BadRequest();
        }
        
        var equipmentHistory = await equipmentRepository
            .GetHistoryById(id, from, to);

        if (equipmentHistory.Count == 0)
        {
            return NotFound();
        }
        
        var dtos = ToEquipmentHistoryDtos(equipmentHistory);
        
        return new OkObjectResult(dtos);
    }

    private static EquipmentHistoryDto[] ToEquipmentHistoryDtos(
        IEnumerable<EquipmentState> equipmentStatuses)
    {
        return equipmentStatuses
            .Select(x => 
                new EquipmentHistoryDto(
                    x.EquipmentIdentifier, 
                    x.State.ToString(), 
                    x.TimeStamp.ToString()))
            .ToArray();
    }
}