using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("DiagnosisTests")]
    public class DiagnosisTest
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("diagnosis_id")]
        public int DiagnosisId { get; set; }

        [Column("test_type_id")]
        public int TestTypeId { get; set; }

        [Column("is_mandatory")]
        [DisplayName("Обов'язковий")]
        public bool IsMandatory { get; set; } = true;

        [Column("description")]
        [DisplayName("Опис")]
        public string? Description { get; set; }

        [Column("recommended_frequency")]
        [DisplayName("Рекомендована частота")]
        [MaxLength(50)]
        public string? RecommendedFrequency { get; set; }

        [Browsable(false)]
        [ForeignKey("DiagnosisId")]
        public virtual Diagnosis? Diagnosis { get; set; }

        [Browsable(false)]
        [ForeignKey("TestTypeId")]
        public virtual TestType? TestType { get; set; }
    }
}