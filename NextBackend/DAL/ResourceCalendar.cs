using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextBackend.DAL
{
    /// <summary>
    /// Заданный для ресурса календарь
    /// </summary>
    public class ResourceCalendar
    {
        /// <summary>
        /// Guid
        /// </summary>
        [Key]
        [Column("guid")]
        public Guid Guid { get; set; }
        /// <summary>
        /// Ресурс
        /// </summary>
        public Resource Resource { get; set; } = null!;
        /// <summary>
        /// Guid ресурса
        /// </summary>
        [Column("resource_guid")]
        public Guid ResourceGuid { get; set; }
        /// <summary>
        /// Календарный шаблон
        /// </summary>
        public CalendarTemplate CalendarTemplate { get; set; } = null!;
        /// <summary>
        /// Guid календарного шаблона
        /// </summary>
        [Column("calendar_template_guid")]
        public Guid CalendarTemplateGuid { get; set; }
        /// <summary>
        /// Начало периода действия календаря для ресурса
        /// </summary>
        [Column("from_time")]
        public DateTimeOffset FromTime { get; set; }
    }
}
