using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NextBackend.DAL;

namespace NextBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourceCalendarsController : Controller
    {
        private readonly NmaContext _dbContext;
        private readonly IStringLocalizer<ResourceCalendarsController> _localizer;

        public ResourceCalendarsController(NmaContext dbContext, IStringLocalizer<ResourceCalendarsController> localizer)
        {
            _dbContext = dbContext;
            _localizer = localizer;
        }

        [HttpGet]
        public IEnumerable<ResourceCalendar> Read()
        {
            return _dbContext.ResourceCalendars.ToList();
        }

        [HttpPost]
        [Route("CreateResourceCalendar/{resourceGuid:guid}/{calendarTemplateGuid:guid}/{fromTime:datetime}")]
        public async Task<ResourceCalendar> Create(Guid resourceGuid, Guid calendarTemplateGuid, DateTime fromTime)
        {
            if (!_dbContext.Resources.Any(res => res.Guid == resourceGuid))
                throw new ArgumentException("Illegal resource", nameof(resourceGuid));
            if (!_dbContext.CalendarTemplates.Any(ct => ct.Guid == calendarTemplateGuid))
                throw new ArgumentException("Illegal calendar template", nameof(calendarTemplateGuid));
            var resourceCalendar = _dbContext.ResourceCalendars.FirstOrDefault(rc => (rc.ResourceGuid == resourceGuid) && (rc.FromTime == fromTime));
            if (resourceCalendar != null)
            {
                resourceCalendar.CalendarTemplateGuid = calendarTemplateGuid;
            }
            else
            {
                resourceCalendar = new ResourceCalendar()
                {
                    Guid = Guid.NewGuid(),
                    ResourceGuid = resourceGuid,
                    CalendarTemplateGuid = calendarTemplateGuid,
                    FromTime = fromTime
                };
                _dbContext.ResourceCalendars.Add(resourceCalendar);
            }
            await _dbContext.SaveChangesAsync();
            return resourceCalendar;
        }

        [HttpPost]
        [Route("UpdateResourceCalendar/{guid:guid}/{calendarTemplateGuid:guid}/{fromTime:datetime}")]
        public async Task<ResourceCalendar> Update(Guid guid, Guid calendarTemplateGuid, DateTime fromTime)
        {
            var resourceCalendar = _dbContext.ResourceCalendars.FirstOrDefault(rc => rc.Guid == guid) ??
                throw new ArgumentException("Record not found", nameof(guid));
            if (!_dbContext.CalendarTemplates.Any(ct => ct.Guid == calendarTemplateGuid))
                throw new ArgumentException("Illegal calendar template", nameof(calendarTemplateGuid));
            var duplicateResourceCalendar = _dbContext.ResourceCalendars
                .FirstOrDefault(rc => (rc.ResourceGuid == resourceCalendar.ResourceGuid) && (rc.FromTime == fromTime));
            if (duplicateResourceCalendar != null)
            {
                _dbContext.ResourceCalendars.Remove(duplicateResourceCalendar);
            }
            resourceCalendar.CalendarTemplateGuid = calendarTemplateGuid;
            resourceCalendar.FromTime = fromTime;
            await _dbContext.SaveChangesAsync();
            return resourceCalendar;
        }

        [HttpDelete]
        [Route("DeleteResourceCalendar/{guid:guid}")]
        public async Task<bool> Delete(Guid guid)
        {
            var resourceCalendar = _dbContext.ResourceCalendars.FirstOrDefault(rc => rc.Guid == guid);
            if (resourceCalendar == null)
                return false;
            _dbContext.ResourceCalendars.Remove(resourceCalendar);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
