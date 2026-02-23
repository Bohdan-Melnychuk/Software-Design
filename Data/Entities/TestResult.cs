using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("TestResults")]
    public class TestResult
    {
        [Key]
        [Column("result_id")]
        public int ResultId { get; set; }

        [Column("test_app_id")]
        public int TestAppId { get; set; }

        [Column("performed_by")]
        public int PerformedBy { get; set; }

        [Column("result_date")]
        [DisplayName("Дата результату")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ResultDate { get; set; }

        [Column("result_text")]
        [DisplayName("Текст результату")]
        public string? ResultText { get; set; }

        [Column("result_json")]
        [Browsable(false)]
        public string? ResultJson { get; set; }

        [Column("attachment_path")]
        [DisplayName("Файл")]
        [MaxLength(500)]
        public string? AttachmentPath { get; set; }

        [Column("conclusion")]
        [DisplayName("Висновок")]
        public string? Conclusion { get; set; }

        [Column("is_abnormal")]
        [DisplayName("Аномальний")]
        public bool? IsAbnormal { get; set; }

        [Column("reviewed_by_doctor_id")]
        public int? ReviewedByDoctorId { get; set; }

        [Column("review_notes")]
        [DisplayName("Нотатки лікаря")]
        public string? ReviewNotes { get; set; }

        [Browsable(false)]
        [ForeignKey("TestAppId")]
        public virtual TestAppointment? TestAppointment { get; set; }

        [Browsable(false)]
        [ForeignKey("PerformedBy")]
        public virtual Doctor? PerformedByDoctor { get; set; }

        [Browsable(false)]
        [ForeignKey("ReviewedByDoctorId")]
        public virtual Doctor? ReviewedByDoctor { get; set; }
    }
}