using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("TestOrders")]
    public class TestOrder
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("visit_id")]
        public int? VisitId { get; set; }

        [Column("test_type_id")]
        public int TestTypeId { get; set; }

        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Column("patient_id")]
        public int PatientId { get; set; }

        [Column("order_date")]
        [DisplayName("Дата направлення")]
        public DateTime? OrderDate { get; set; }

        [Column("priority")]
        [DisplayName("Пріоритет")]
        [StringLength(20)]
        public string Priority { get; set; } = "плановий";

        [Column("status")]
        [DisplayName("Статус")]
        [StringLength(20)]
        public string Status { get; set; } = "призначено";

        [Column("notes")]
        [DisplayName("Нотатки")]
        public string? Notes { get; set; }

        [Column("required_for_diagnosis")]
        [DisplayName("Для діагнозу")]
        public bool RequiredForDiagnosis { get; set; } = false;

        [ForeignKey("VisitId")]
        public virtual Visit? Visit { get; set; }

        [ForeignKey("TestTypeId")]
        public virtual TestType? TestType { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
    }
}