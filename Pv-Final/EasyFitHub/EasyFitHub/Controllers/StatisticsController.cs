using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyFitHub.Data;
using EasyFitHub.Models.Statistics;
using Microsoft.Extensions.Logging;

namespace EasyFitHub.Controllers
{
    /// <summary>
    /// AUTHOR: Francisco Silva
    /// Controller responsible for handling statistics-related actions in the application.
    /// </summary>
    public class StatisticsController : Controller
    {
        private readonly StatisticsInfo _statisticsInfo;
        private readonly ILogger<StatisticsController> _logger;


        public StatisticsController(EasyFitHubContext context, ILogger<StatisticsController> logger)
        {
            _logger = logger;
            _statisticsInfo = new StatisticsInfo(context, logger);
        }

        /// <summary>
        /// Retrieves platform-wide statistics.
        /// </summary>
        /// <returns>Returns a view displaying platform statistics.</returns>
        public async Task<IActionResult> PlatformStats()
        {
            var platformStats = await _statisticsInfo.GetPlatformStats();
            return View(platformStats);
        }

        /// <summary>
        /// Retrieves statistics for a specific gym.
        /// </summary>
        /// <param name="gymId">The ID of the gym.</param>
        /// <returns>Returns a view displaying statistics for the specified gym.</returns>
        public async Task<IActionResult> GymStats(int gymId)
        {
            var gymStats = await _statisticsInfo.GetGymStats(gymId);
            return View(gymStats);
        }

        /// <summary>
        /// Retrieves statistics for a specific employee.
        /// </summary>
        /// <param name="clientId">The ID of the employee.</param>
        /// <returns>Returns a view displaying statistics for the specified employee.</returns>
        public async Task<IActionResult> EmployeeStats(int clientId)
        {
            var employeeStats = await _statisticsInfo.GetEmployeeStats(clientId);
            return View(employeeStats);
        }

        /// <summary>
        /// Retrieves statistics for a specific client.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <returns>Returns a view displaying statistics for the specified client.</returns>
        public async Task<IActionResult> ClientStats(int clientId)
        {
            var clientStats = await _statisticsInfo.GetClientStats(clientId);
            return View(clientStats);
        }

        /// <summary>
        /// Creates platform-wide statistics.
        /// </summary>
        /// <returns>Returns the elapsed time in milliseconds for creating the statistics.</returns>
        [HttpPost]
        public async Task<ActionResult<double>> CreatePlatformStats()
        {
            try
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                await _statisticsInfo.CreateStats();

                stopwatch.Stop();
                double elapsedTime = stopwatch.Elapsed.TotalMilliseconds;

                _logger.LogInformation($"\n\n\n\nElapsed Time in Miliseconds: {elapsedTime}\n\n\n\n");

                return Ok(elapsedTime);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating platform statistics: {ex.Message}");
                return StatusCode(500, "An error occurred while creating platform statistics.");
            }
        }
    }
}
