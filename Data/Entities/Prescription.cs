using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("Prescriptions")]
    public class Prescription
    {
        [Key]
        [Column("prescription_id")]
        public int PrescriptionId { get; set; }

        [Column("visit_id")]
        public int VisitId { get; set; }

        [Column("medication")]
        [DisplayName("Ліки")]
        [MaxLength(200)]
        public string Medication { get; set; } = null!;

        [Column("dosage")]
        [DisplayName("Дозування")]
        [MaxLength(50)]
        public string Dosage { get; set; } = null!;

        [Column("frequency")]
        [DisplayName("Частота")]
        [MaxLength(50)]
        public string? Frequency { get; set; }

        [Column("duration_days")]
        [DisplayName("Тривалість (дні)")]
        public int? DurationDays { get; set; }

        [Column("start_date")]
        [DisplayName("Початок")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        [DisplayName("Кінець")]
        public DateTime? EndDate { get; set; }

        [Column("instructions")]
        [DisplayName("Інструкції")]
        public string? Instructions { get; set; }

        [Column("refills_allowed")]
        [DisplayName("Дозволено повторів")]
        public int RefillsAllowed { get; set; }

        [Column("refills_used")]
        [DisplayName("Використано повторів")]
        public int RefillsUsed { get; set; }

        [Column("prescribed_by")]
        public int PrescribedBy { get; set; }

        [Browsable(false)]
        [ForeignKey("VisitId")]
        public virtual Visit? Visit { get; set; }

        [Browsable(false)]
        [ForeignKey("PrescribedBy")]
        public virtual Doctor? PrescribingDoctor { get; set; }
    }
}