using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NextBackend.DAL;

namespace NextBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarStatesController : ControllerBase
    {
        private readonly NmaContext _dbContext;
        private readonly IStringLocalizer<CalendarStatesController> _localizer;

        public CalendarStatesController(NmaContext dbContext, IStringLocalizer<CalendarStatesController> localizer)
        {
            _dbContext = dbContext;
            _localizer = localizer;
        }

        [HttpGet]
        public IEnumerable<CalendarState> Read()
        {
            return _dbContext.CalendarStates.ToList();
        }

        [HttpPost]
        [Route("CreateCalendarState/{name}")]
        public async Task<CalendarState> Create(string name)
        {
            name = name.Trim();
            if (name == string.Empty)
                throw new ArgumentException("Empty name", nameof(name));
            if (_dbContext.CalendarStates.Any(cs => cs.Name == name))
                throw new ArgumentException("Duplicate name", nameof(name));
            var calendarState = new CalendarState()
            {
                Guid = Guid.NewGuid(),
                Name = name
            };
            _dbContext.CalendarStates.Add(calendarState);
            await _dbContext.SaveChangesAsync();
            return calendarState;
        }

        [HttpDelete]
        [Route("DeleteCalendarState/{guid:guid}")]
        public async Task<bool> Delete(Guid guid)
        {
            var calendarState = _dbContext.CalendarStates.FirstOrDefault(cs => cs.Guid == guid);
            if (calendarState == null)
                return false;
            _dbContext.CalendarStates.Remove(calendarState);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}