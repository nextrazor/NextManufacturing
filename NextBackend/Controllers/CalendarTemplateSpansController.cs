using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NextBackend.DAL;

namespace NextBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarTemplateSpansController : Controller
    {
        private readonly NmaContext _dbContext;
        private readonly IStringLocalizer<CalendarTemplateSpansController> _localizer;

        public CalendarTemplateSpansController(NmaContext dbContext, IStringLocalizer<CalendarTemplateSpansController> localizer)
        {
            _dbContext = dbContext;
            _localizer = localizer;
        }

        [HttpGet]
        public IEnumerable<CalendarTemplateSpan> Read()
        {
            return _dbContext.CalendarTemplateSpans.ToList();
        }

        [HttpGet]
        [Route("GetSpansByTemplate/{calendarTemplateGuid:guid}")]
        public IEnumerable<CalendarTemplateSpan> Read(Guid calendarTemplateGuid)
        {
            return _dbContext.CalendarTemplateSpans.Where(el => el.CalendarTemplateGuid == calendarTemplateGuid).ToList();
        }

        [HttpPost]
        [Route("CreateCalendarTemplateSpan/{calendarTemplateGuid:guid}/{stateGuid:guid}/{fromTime:double}/{toTime:double}")]
        public async Task<CalendarTemplateSpan> Create(Guid calendarTemplateGuid, Guid stateGuid, double fromTime, double toTime)
        {
            if (fromTime < 0)
                throw new ArgumentException(_localizer["Negative start time"], nameof(fromTime));
            if (toTime <= fromTime)
                throw new ArgumentException(_localizer["End time too early"], nameof(toTime));
            TimeSpan ft = TimeSpan.FromDays(fromTime);
            TimeSpan tt = TimeSpan.FromDays(toTime);
            CalendarTemplate calendarTemplate = _dbContext.CalendarTemplates.FirstOrDefault(ct => ct.Guid == calendarTemplateGuid) ??
                throw new ArgumentException(_localizer["Illegal calendar template"], nameof(calendarTemplateGuid));
            if (!_dbContext.CalendarStates.Any(cs => cs.Guid == stateGuid))
                throw new ArgumentException(_localizer["Illegal calendar state"], nameof(stateGuid));
            if (tt > calendarTemplate.PeriodDuration)
                throw new ArgumentException(_localizer["End time is bigger that template period"], nameof(toTime));
            if (_dbContext.CalendarTemplateSpans.Any(cts => (cts.CalendarTemplateGuid == calendarTemplateGuid) &&
                (cts.FromTime < tt) && (cts.ToTime > ft)))
                throw new ArgumentException(_localizer["There is an overlapped span in this template"], nameof(fromTime));
            var calendarTemplateSpan = new CalendarTemplateSpan()
            {
                Guid = Guid.NewGuid(),
                CalendarTemplateGuid = calendarTemplateGuid,
                StateGuid = stateGuid,
                FromTime = ft,
                ToTime = tt
            };
            _dbContext.CalendarTemplateSpans.Add(calendarTemplateSpan);
            await _dbContext.SaveChangesAsync();
            return calendarTemplateSpan;
        }

        [HttpPost]
        [Route("UpdateCalendarTemplateSpan/{guid:guid}/{stateGuid:guid}/{fromTime:double}/{toTime:double}")]
        public async Task<CalendarTemplateSpan> Update(Guid guid, Guid stateGuid, double fromTime, double toTime)
        {
            if (fromTime < 0)
                throw new ArgumentException(_localizer["Negative start time"], nameof(fromTime));
            if (toTime <= fromTime)
                throw new ArgumentException(_localizer["End time too early"], nameof(toTime));
            TimeSpan ft = TimeSpan.FromDays(fromTime);
            TimeSpan tt = TimeSpan.FromDays(toTime);
            var calendarTemplateSpan = _dbContext.CalendarTemplateSpans.FirstOrDefault(cts => cts.Guid == guid) ??
                throw new ArgumentException(_localizer["Record not found"], nameof(guid));
            if (!_dbContext.CalendarStates.Any(cs => cs.Guid == stateGuid))
                throw new ArgumentException(_localizer["Illegal calendar state"], nameof(stateGuid));
            if (tt > calendarTemplateSpan.CalendarTemplate.PeriodDuration)
                throw new ArgumentException(_localizer["End time is bigger that template period"], nameof(toTime));
            if (_dbContext.CalendarTemplateSpans.Any(cts => (cts.CalendarTemplateGuid == calendarTemplateSpan.CalendarTemplateGuid) &&
                (cts.Guid != guid) && (cts.FromTime < tt) && (cts.ToTime > ft)))
                throw new ArgumentException(_localizer["There is an overlapped span in this template"], nameof(fromTime));
            calendarTemplateSpan.StateGuid = stateGuid;
            calendarTemplateSpan.FromTime = ft;
            calendarTemplateSpan.ToTime = tt;
            await _dbContext.SaveChangesAsync();
            return calendarTemplateSpan;
        }

        [HttpDelete]
        [Route("DeleteCalendarTemplateSpan/{guid:guid}")]
        public async Task<bool> Delete(Guid guid)
        {
            var calendarTemplateSpan = _dbContext.CalendarTemplateSpans.FirstOrDefault(cts => cts.Guid == guid);
            if (calendarTemplateSpan == null)
                return false;
            _dbContext.CalendarTemplateSpans.Remove(calendarTemplateSpan);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
