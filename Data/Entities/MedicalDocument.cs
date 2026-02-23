using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("MedicalDocuments")]
    public class MedicalDocument
    {
        [Key]
        [Column("document_id")]
        public int DocumentId { get; set; }

        [Column("patient_id")]
        public int PatientId { get; set; }

        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Column("document_type")]
        [DisplayName("Тип")]
        [MaxLength(50)]
        public string DocumentType { get; set; } = null!;

        [Column("issue_date")]
        [DisplayName("Дата видачі")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? IssueDate { get; set; }

        [Column("file_path")]
        [DisplayName("Файл")]
        [MaxLength(500)]
        public string? FilePath { get; set; }

        [Column("document_text")]
        [DisplayName("Текст")]
        public string? DocumentText { get; set; }

        [Column("notes")]
        [DisplayName("Нотатки")]
        public string? Notes { get; set; }

        [Browsable(false)]
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        [Browsable(false)]
        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }
    }
}