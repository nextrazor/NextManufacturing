using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextBackend.DAL
{
    /// <summary>
    /// Ресурс для планирования
    /// </summary>
    public class Resource
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
        /// Заданные для ресурса календари
        /// </summary>
        public List<ResourceCalendar> CalendarSpans { get; set; } = new List<ResourceCalendar>();
    }
}
