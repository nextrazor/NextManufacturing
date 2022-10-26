using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NextBackend.DAL
{
    /// <summary>
    /// Диапазон в шаблоне календаря
    /// </summary>
    public class CalendarTemplateSpan
    {
        /// <summary>
        /// Guid
        /// </summary>
        [Key]
        [Column("guid")]
        public Guid Guid { get; set; }
        /// <summary>
        /// Шаблон календаря
        /// </summary>
        [JsonIgnore]
        public CalendarTemplate CalendarTemplate { get; set; } = null!;
        /// <summary>
        /// Guid шаблона календаря
        /// </summary>
        [Column("calendar_template_guid")]
        public Guid CalendarTemplateGuid { get; set; }
        /// <summary>
        /// Смещение начала диапазона от начала периода
        /// </summary>
        [Column("from_time")]
        public TimeSpan FromTime { get; set; }
        /// <summary>
        /// Смещение конца диапазона от начала периода
        /// </summary>
        [Column("to_time")]
        public TimeSpan ToTime { get; set; }
        /// <summary>
        /// Календарное состояние в заданном диапазоне
        /// </summary>
        [JsonIgnore]
        public CalendarState State { get; set; } = null!;
        /// <summary>
        /// Guid календарного состояния в заданном диапазоне
        /// </summary>
        [Column("state_guid")]
        public Guid StateGuid { get; set; }
    }
}
