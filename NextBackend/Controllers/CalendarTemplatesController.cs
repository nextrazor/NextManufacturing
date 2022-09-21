using Microsoft.AspNetCore.Mvc;
using NextBackend.DAL;

namespace NextBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarTemplatesController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<CalendarTemplate> Read()
        {
            using var dbContext = new NmaContext();
            return dbContext.CalendarTemplates.ToList();
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
            using var dbContext = new NmaContext();
            if (dbContext.CalendarTemplates.Any(ct => ct.Name == name))
                throw new ArgumentException("Duplicate name", nameof(name));
            if (!dbContext.CalendarStates.Any(cs => cs.Guid == defaultStateGuid))
                throw new ArgumentException("Illegal default state", nameof(defaultStateGuid));
            var calendarTemplate = new CalendarTemplate()
            {
                Guid = Guid.NewGuid(),
                Name = name,
                DefaultStateGuid = defaultStateGuid,
                PeriodDuration = TimeSpan.FromDays(periodDuration),
                ReferenceDate = referenceDate
            };
            dbContext.CalendarTemplates.Add(calendarTemplate);
            await dbContext.SaveChangesAsync();
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
            using var dbContext = new NmaContext();
            var calendarTemplate = dbContext.CalendarTemplates.FirstOrDefault(ct => ct.Guid == guid) ??
                throw new ArgumentException("Record not found", nameof(guid));
            if (!dbContext.CalendarStates.Any(cs => cs.Guid == defaultStateGuid))
                throw new ArgumentException("Illegal default state", nameof(defaultStateGuid));
            if (dbContext.CalendarTemplateSpans.Any(cts => (cts.CalendarTemplateGuid == guid) && (cts.ToTime > pd)))
                throw new ArgumentException("There are calendar spans later that a new period duration", nameof(periodDuration));
            calendarTemplate.Name = name;
            calendarTemplate.DefaultStateGuid = defaultStateGuid;
            calendarTemplate.PeriodDuration = pd;
            calendarTemplate.ReferenceDate = referenceDate;
            await dbContext.SaveChangesAsync();
            return calendarTemplate;
        }

        [HttpDelete]
        [Route("DeleteCalendarTemplate/{guid:guid}")]
        public async Task<bool> Delete(Guid guid)
        {
            using var dbContext = new NmaContext();
            var calendarTemplate = dbContext.CalendarTemplates.FirstOrDefault(ct => ct.Guid == guid);
            if (calendarTemplate == null)
                return false;
            dbContext.CalendarTemplates.Remove(calendarTemplate);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
