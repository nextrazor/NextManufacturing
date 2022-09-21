using Microsoft.AspNetCore.Mvc;
using NextBackend.DAL;

namespace NextBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarStatesController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<CalendarState> Read()
        {
            using var dbContext = new NmaContext();
            return dbContext.CalendarStates.ToList();
        }

        [HttpPost]
        [Route("CreateCalendarState/{name}")]
        public async Task<CalendarState> Create(string name)
        {
            name = name.Trim();
            if (name == string.Empty)
                throw new ArgumentException("Empty name", nameof(name));
            using var dbContext = new NmaContext();
            if (dbContext.CalendarStates.Any(cs => cs.Name == name))
                throw new ArgumentException("Duplicate name", nameof(name));
            var calendarState = new CalendarState()
            {
                Guid = Guid.NewGuid(),
                Name = name
            };
            dbContext.CalendarStates.Add(calendarState);
            await dbContext.SaveChangesAsync();
            return calendarState;
        }

        [HttpDelete]
        [Route("DeleteCalendarState/{guid:guid}")]
        public async Task<bool> Delete(Guid guid)
        {
            using var dbContext = new NmaContext();
            var calendarState = dbContext.CalendarStates.FirstOrDefault(cs => cs.Guid == guid);
            if (calendarState == null)
                return false;
            dbContext.CalendarStates.Remove(calendarState);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}