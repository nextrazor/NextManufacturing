using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextBackend.DAL
{
    /// <summary>
    /// Шаблон календаря
    /// </summary>
    public class CalendarTemplate
    {
        /// <summary>
        /// Guid
        /// </summary>
        [Key]
        [Column("guid")]
        public Guid Guid { get; set; }
        /// <summary>
        /// Уникальное название
        /// </summary>
        [Column("name")]
        [MaxLength(255)]
        public string Name { get; set; } = String.Empty;
        /// <summary>
        /// Состояние шаблона по умолчанию
        /// </summary>
        /// <remarks>
        /// Состояние применяется для всех дипазонов шаблона, для которых состояние не указано явно через CalendarTemplateSpan
        /// Значение null не считается корректным. В коде оставлен nullable-тип, потому что нельзя задать default-значение, гарантирующее ссылочную целостность
        /// </remarks>
        public CalendarState? DefaultState { get; set; }
        /// <summary>
        /// Guid состояния шаблона по умолчанию
        /// </summary>
        [Column("default_state_guid")]
        public Guid DefaultStateGuid { get; set; }
        /// <summary>
        /// Точка отсчета шаблона
        /// </summary>
        [Column("reference_date")]
        public DateTimeOffset ReferenceDate { get; set; }
        /// <summary>
        /// Длительность периода шаблона
        /// </summary>
        [Column("period_duration")]
        public TimeSpan PeriodDuration { get; set; }
        /// <summary>
        /// Диапазоны в шаблоне календаря
        /// </summary>
        public List<CalendarTemplateSpan> Spans {get; set; } = new List<CalendarTemplateSpan>();
    }
}
