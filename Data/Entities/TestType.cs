using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("TestTypes")]
    public class TestType
    {
        [Key]
        [Column("test_type_id")]
        public int TestTypeId { get; set; }

        [Column("code")]
        [DisplayName("Код")]
        [MaxLength(20)]
        public string Code { get; set; } = null!;

        [Column("name")]
        [DisplayName("Назва")]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Column("category")]
        [DisplayName("Категорія")]
        [MaxLength(50)]
        public string Category { get; set; } = null!;

        [Column("description")]
        [DisplayName("Опис")]
        public string? Description { get; set; }

        [Column("preparation")]
        [DisplayName("Підготовка")]
        public string? Preparation { get; set; }

        [Column("duration_min")]
        [DisplayName("Тривалість (хв)")]
        public int? DurationMin { get; set; }

        [Column("cost")]
        [DisplayName("Вартість")]
        public decimal Cost { get; set; } = 0.00m;

        [Column("normal_range")]
        [DisplayName("Нормальний діапазон")]
        public string? NormalRange { get; set; }
    }
}