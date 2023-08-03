using Microsoft.AspNetCore.Mvc;
using WaterWatch.Repositories;
using WaterWatch.Models;

namespace WaterWatch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WaterConsumptionController : ControllerBase
    {
        private readonly IWaterConsumptionRepository _waterConsumptionRepository;

        public WaterConsumptionController(IWaterConsumptionRepository waterConsumptionRepository)
        {
            _waterConsumptionRepository = waterConsumptionRepository;
        }

        [HttpGet("/waterconsumption/getall")]
        public async Task<ActionResult<IAsyncEnumerable<WaterConsumption>>> GettAll()
        {
            var wcData = await _waterConsumptionRepository.GetAll();
            return Ok(wcData);
        }

        [HttpGet("/waterconsumption/topten")]
        public async Task<ActionResult<IAsyncEnumerable<WaterConsumption>>> GetTopTen()
        {
            var wcData = await _waterConsumptionRepository.GetTopTenConsumers();
            return Ok(wcData);
        }

    }
}