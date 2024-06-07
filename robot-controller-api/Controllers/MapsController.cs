using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using robot_controller_api.Models;
using Microsoft.AspNetCore.Authorization;

namespace robot_controller_api.Controllers
{
    [ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        private readonly IMapDataAccess mapRepo; // Instance of IMapDataAccess

        public MapsController(IMapDataAccess repo)
        {
            mapRepo = repo; // Instantiate IMapDataAccess
        }

        [HttpGet]
        public IEnumerable<Map> GetMaps()
        {
            return mapRepo.GetMaps();
        }

        [HttpGet("square-maps")]
        public IEnumerable<Map> GetSquareMapsOnly()
        {
            return mapRepo.GetMaps().Where(x => x.Columns == x.Rows);
        }

        [HttpGet("{id}", Name = "GetMapById")]
        public IActionResult GetMapById(int id)
        {
            var map = mapRepo.GetMapById(id);
            if (map == null)
            {
                return NotFound("Map not found.");
            }
            return Ok(map);
        }

        [HttpPost]
        public IActionResult AddMap(Map newMap)
        {
            if (newMap.Name == null || newMap.Rows <= 0 || newMap.Columns <= 0)
            {
                return BadRequest("Invalid map data.");
            }

            if (mapRepo.MapExists(newMap.Name))
            {
                return Conflict("Map with the same name already exists.");
            }

            newMap.CreatedDate = newMap.ModifiedDate = DateTime.Now;
            mapRepo.InsertMap(newMap);

            return CreatedAtRoute("GetMapById", new { id = newMap.Id }, newMap);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMap(int id, Map updatedMap)
        {
            var existingMap = mapRepo.GetMapById(id);
            if (existingMap == null)
            {
                return NotFound("Map not found.");
            }

            if (updatedMap == null || updatedMap.Rows <= 0 || updatedMap.Columns <= 0)
            {
                return BadRequest("Invalid map data.");
            }

            existingMap.Name = updatedMap.Name;
            existingMap.Description = updatedMap.Description;
            existingMap.Rows = updatedMap.Rows;
            existingMap.Columns = updatedMap.Columns;
            existingMap.ModifiedDate = DateTime.Now;

            mapRepo.UpdateMap(existingMap);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMap(int id)
        {
            var mapToDelete = mapRepo.GetMapById(id);
            if (mapToDelete == null)
            {
                return NotFound("Map not found.");
            }

            mapRepo.DeleteMap(id);

            return NoContent();
        }

        [HttpGet("{id}/{x}-{y}")]
        public IActionResult CheckCoordinate(int id, int x, int y)
        {
            var map = mapRepo.GetMapById(id);
            if (map == null)
            {
                return NotFound("Map not found.");
            }

            if (x < 0 || x >= map.Columns || y < 0 || y >= map.Rows)
            {
                return NotFound("Coordinate out of map boundaries.");
            }

            // Logic to check whether the coordinate is on the map here
            bool isOnMap = true;

            return Ok(isOnMap);
        }
    }
}
