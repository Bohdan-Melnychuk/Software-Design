using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("Visits")]
    public class Visit
    {
        [Key]
        [Column("visit_id")]
        public int VisitId { get; set; }

        [Column("appointment_id")]
        public int AppointmentId { get; set; }

        [Column("diagnosis_id")]
        public int? DiagnosisId { get; set; }

        [Column("symptoms")]
        [DisplayName("Симптоми")]
        public string? Symptoms { get; set; }

        [Column("examination")]
        [DisplayName("Обстеження")]
        public string? Examination { get; set; }

        [Column("treatment_plan")]
        [DisplayName("План лікування")]
        public string? TreatmentPlan { get; set; }

        [Column("next_visit_date")]
        [DisplayName("Наступний візит")]
        public DateTime? NextVisitDate { get; set; }

        [Column("referral_needed")]
        [DisplayName("Потрібне направлення")]
        public bool ReferralNeeded { get; set; }

        [Column("visit_notes")]
        [DisplayName("Нотатки")]
        public string? VisitNotes { get; set; }

        [Column("visit_date")]
        [DisplayName("Дата візиту")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? VisitDate { get; set; }

        [Browsable(false)]
        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }

        [Browsable(false)]
        [ForeignKey("DiagnosisId")]
        public virtual Diagnosis? Diagnosis { get; set; }
    }
}