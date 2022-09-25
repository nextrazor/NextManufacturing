using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NextBackend.DAL;

namespace NextBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarTemplatesController : ControllerBase
    {
        private readonly NmaContext _dbContext;
        private readonly IStringLocalizer<CalendarTemplatesController> _localizer;

        public CalendarTemplatesController(NmaContext dbContext, IStringLocalizer<CalendarTemplatesController> localizer)
        {
            _dbContext = dbContext;
            _localizer = localizer;
        }

        [HttpGet]
        public IEnumerable<CalendarTemplate> Read()
        {
            return _dbContext.CalendarTemplates.ToList();
        }

        [HttpPost]
        [Route("CreateCalendarTemplate/{name}/{defaultStateGuid:guid}/{periodDuration:double}/{referenceDate:datetime}")]
        public async Task<CalendarTemplate> Create(string name, Guid defaultStateGuid, double periodDuration, DateTime referenceDate)
        {
            name = name.Trim();
            if (name == string.Empty)
                throw new ArgumentException("Empty name", nameof(name));
            if (periodDuration <= 0)
                throw new ArgumentException("Negative or zero period duration", nameof(periodDuration));
            if (_dbContext.CalendarTemplates.Any(ct => ct.Name == name))
                throw new ArgumentException("Duplicate name", nameof(name));
            if (!_dbContext.CalendarStates.Any(cs => cs.Guid == defaultStateGuid))
                throw new ArgumentException("Illegal default state", nameof(defaultStateGuid));
            var calendarTemplate = new CalendarTemplate()
            {
                Guid = Guid.NewGuid(),
                Name = name,
                DefaultStateGuid = defaultStateGuid,
                PeriodDuration = TimeSpan.FromDays(periodDuration),
                ReferenceDate = referenceDate
            };
            _dbContext.CalendarTemplates.Add(calendarTemplate);
            await _dbContext.SaveChangesAsync();
            return calendarTemplate;
        }

        [HttpPost]
        [Route("UpdateCalendarTemplate/{guid:guid}/{name}/{defaultStateGuid:guid}/{periodDuration:double}/{referenceDate:datetime}")]
        public async Task<CalendarTemplate> Update(Guid guid, string name, Guid defaultStateGuid, double periodDuration, DateTimeOffset referenceDate)
        {
            name = name.Trim();
            if (name == string.Empty)
                throw new ArgumentException("Empty name", nameof(name));
            if (periodDuration <= 0)
                throw new ArgumentException("Negative period duration", nameof(periodDuration));
            TimeSpan pd = TimeSpan.FromDays(periodDuration);
            var calendarTemplate = _dbContext.CalendarTemplates.FirstOrDefault(ct => ct.Guid == guid) ??
                throw new ArgumentException("Record not found", nameof(guid));
            if (!_dbContext.CalendarStates.Any(cs => cs.Guid == defaultStateGuid))
                throw new ArgumentException("Illegal default state", nameof(defaultStateGuid));
            if (_dbContext.CalendarTemplateSpans.Any(cts => (cts.CalendarTemplateGuid == guid) && (cts.ToTime > pd)))
                throw new ArgumentException("There are calendar spans later that a new period duration", nameof(periodDuration));
            calendarTemplate.Name = name;
            calendarTemplate.DefaultStateGuid = defaultStateGuid;
            calendarTemplate.PeriodDuration = pd;
            calendarTemplate.ReferenceDate = referenceDate;
            await _dbContext.SaveChangesAsync();
            return calendarTemplate;
        }

        [HttpDelete]
        [Route("DeleteCalendarTemplate/{guid:guid}")]
        public async Task<bool> Delete(Guid guid)
        {
            var calendarTemplate = _dbContext.CalendarTemplates.FirstOrDefault(ct => ct.Guid == guid);
            if (calendarTemplate == null)
                return false;
            _dbContext.CalendarTemplates.Remove(calendarTemplate);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
