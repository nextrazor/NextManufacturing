using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NextBackend.DAL
{
    /// <summary>
    /// Календарное состояние
    /// </summary>
    public class CalendarState
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
    }
}
