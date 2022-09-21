using Microsoft.AspNetCore.Mvc;
using NextBackend.DAL;

namespace NextBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarTemplateSpansController : Controller
    {
        [HttpGet]
        public IEnumerable<CalendarTemplateSpan> Read()
        {
            using var dbContext = new NmaContext();
            return dbContext.CalendarTemplateSpans.ToList();
        }

        [HttpPost]
        [Route("CreateCalendarTemplateSpan/{calendarTemplateGuid:guid}/{stateGuid:guid}/{fromTime:double}/{toTime:double}")]
        public async Task<CalendarTemplateSpan> Create(Guid calendarTemplateGuid, Guid stateGuid, double fromTime, double toTime)
        {
            if (fromTime < 0)
                throw new ArgumentException("Negative start time", nameof(fromTime));
            if (toTime <= fromTime)
                throw new ArgumentException("End time too early", nameof(toTime));
            TimeSpan ft = TimeSpan.FromDays(fromTime);
            TimeSpan tt = TimeSpan.FromDays(toTime);
            using var dbContext = new NmaContext();
            CalendarTemplate calendarTemplate = dbContext.CalendarTemplates.FirstOrDefault(ct => ct.Guid == calendarTemplateGuid) ??
                throw new ArgumentException("Illegal calendar template", nameof(calendarTemplateGuid));
            if (!dbContext.CalendarStates.Any(cs => cs.Guid == stateGuid))
                throw new ArgumentException("Illegal calendar state", nameof(stateGuid));
            if (tt > calendarTemplate.PeriodDuration)
                throw new ArgumentException("End time is bigger that template period", nameof(toTime));
            if (dbContext.CalendarTemplateSpans.Any(cts => (cts.CalendarTemplateGuid == calendarTemplateGuid) &&
                (cts.FromTime < tt) && (cts.ToTime > ft)))
                throw new ArgumentException("There is an overlapped span in this template", nameof(fromTime));
            var calendarTemplateSpan = new CalendarTemplateSpan()
            {
                Guid = Guid.NewGuid(),
                CalendarTemplateGuid = calendarTemplateGuid,
                StateGuid = stateGuid,
                FromTime = ft,
                ToTime = tt
            };
            dbContext.CalendarTemplateSpans.Add(calendarTemplateSpan);
            await dbContext.SaveChangesAsync();
            return calendarTemplateSpan;
        }

        [HttpPost]
        [Route("UpdateCalendarTemplateSpan/{guid:guid}/{stateGuid:guid}/{fromTime:double}/{toTime:double}")]
        public async Task<CalendarTemplateSpan> Update(Guid guid, Guid stateGuid, double fromTime, double toTime)
        {
            if (fromTime < 0)
                throw new ArgumentException("Negative start time", nameof(fromTime));
            if (toTime <= fromTime)
                throw new ArgumentException("End time too early", nameof(toTime));
            TimeSpan ft = TimeSpan.FromDays(fromTime);
            TimeSpan tt = TimeSpan.FromDays(toTime);
            using var dbContext = new NmaContext();
            var calendarTemplateSpan = dbContext.CalendarTemplateSpans.FirstOrDefault(cts => cts.Guid == guid) ??
                throw new ArgumentException("Record not found", nameof(guid));
            if (!dbContext.CalendarStates.Any(cs => cs.Guid == stateGuid))
                throw new ArgumentException("Illegal calendar state", nameof(stateGuid));
            if (tt > calendarTemplateSpan.CalendarTemplate.PeriodDuration)
                throw new ArgumentException("End time is bigger that template period", nameof(toTime));
            if (dbContext.CalendarTemplateSpans.Any(cts => (cts.CalendarTemplateGuid == calendarTemplateSpan.CalendarTemplateGuid) &&
                (cts.Guid != guid) && (cts.FromTime < tt) && (cts.ToTime > ft)))
                throw new ArgumentException("There is an overlapped span in this template", nameof(fromTime));
            calendarTemplateSpan.StateGuid = stateGuid;
            calendarTemplateSpan.FromTime = ft;
            calendarTemplateSpan.ToTime = tt;
            await dbContext.SaveChangesAsync();
            return calendarTemplateSpan;
        }

        [HttpDelete]
        [Route("DeleteCalendarTemplateSpan/{guid:guid}")]
        public async Task<bool> Delete(Guid guid)
        {
            using var dbContext = new NmaContext();
            var calendarTemplateSpan = dbContext.CalendarTemplateSpans.FirstOrDefault(cts => cts.Guid == guid);
            if (calendarTemplateSpan == null)
                return false;
            dbContext.CalendarTemplateSpans.Remove(calendarTemplateSpan);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
