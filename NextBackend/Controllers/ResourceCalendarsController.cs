using Microsoft.AspNetCore.Mvc;
using NextBackend.DAL;

namespace NextBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourceCalendarsController : Controller
    {
        [HttpGet]
        public IEnumerable<ResourceCalendar> Read()
        {
            using var dbContext = new NmaContext();
            return dbContext.ResourceCalendars.ToList();
        }

        [HttpPost]
        [Route("CreateResourceCalendar/{resourceGuid:guid}/{calendarTemplateGuid:guid}/{fromTime:datetime}")]
        public async Task<ResourceCalendar> Create(Guid resourceGuid, Guid calendarTemplateGuid, DateTime fromTime)
        {
            using var dbContext = new NmaContext();
            if (!dbContext.Resources.Any(res => res.Guid == resourceGuid))
                throw new ArgumentException("Illegal resource", nameof(resourceGuid));
            if (!dbContext.CalendarTemplates.Any(ct => ct.Guid == calendarTemplateGuid))
                throw new ArgumentException("Illegal calendar template", nameof(calendarTemplateGuid));
            var resourceCalendar = dbContext.ResourceCalendars.FirstOrDefault(rc => (rc.ResourceGuid == resourceGuid) && (rc.FromTime == fromTime));
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
                dbContext.ResourceCalendars.Add(resourceCalendar);
            }
            await dbContext.SaveChangesAsync();
            return resourceCalendar;
        }

        [HttpPost]
        [Route("UpdateResourceCalendar/{guid:guid}/{calendarTemplateGuid:guid}/{fromTime:datetime}")]
        public async Task<ResourceCalendar> Update(Guid guid, Guid calendarTemplateGuid, DateTime fromTime)
        {
            using var dbContext = new NmaContext();
            var resourceCalendar = dbContext.ResourceCalendars.FirstOrDefault(rc => rc.Guid == guid) ??
                throw new ArgumentException("Record not found", nameof(guid));
            if (!dbContext.CalendarTemplates.Any(ct => ct.Guid == calendarTemplateGuid))
                throw new ArgumentException("Illegal calendar template", nameof(calendarTemplateGuid));
            var duplicateResourceCalendar = dbContext.ResourceCalendars
                .FirstOrDefault(rc => (rc.ResourceGuid == resourceCalendar.ResourceGuid) && (rc.FromTime == fromTime));
            if (duplicateResourceCalendar != null)
            {
                dbContext.ResourceCalendars.Remove(duplicateResourceCalendar);
            }
            resourceCalendar.CalendarTemplateGuid = calendarTemplateGuid;
            resourceCalendar.FromTime = fromTime;
            await dbContext.SaveChangesAsync();
            return resourceCalendar;
        }

        [HttpDelete]
        [Route("DeleteResourceCalendar/{guid:guid}")]
        public async Task<bool> Delete(Guid guid)
        {
            using var dbContext = new NmaContext();
            var resourceCalendar = dbContext.ResourceCalendars.FirstOrDefault(rc => rc.Guid == guid);
            if (resourceCalendar == null)
                return false;
            dbContext.ResourceCalendars.Remove(resourceCalendar);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
