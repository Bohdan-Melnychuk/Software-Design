using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("Diagnoses")]
    public class Diagnosis
    {
        [Key]
        [Column("diagnosis_id")]
        public int DiagnosisId { get; set; }

        [Column("icd_code")]
        [DisplayName("Код ICD")]
        [MaxLength(10)]
        public string IcdCode { get; set; } = null!;

        [Column("name")]
        [DisplayName("Назва")]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [Column("specialty_id")]
        public int SpecialtyId { get; set; }

        [Column("description")]
        [DisplayName("Опис")]
        public string? Description { get; set; }

        [Column("symptoms")]
        [DisplayName("Симптоми")]
        public string? Symptoms { get; set; }

        [Column("typical_treatment")]
        [DisplayName("Типове лікування")]
        public string? TypicalTreatment { get; set; }

        [Browsable(false)]
        [ForeignKey("SpecialtyId")]
        public virtual Specialty? Specialty { get; set; }
    }
}