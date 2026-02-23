using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("TestAppointments")]
    public class TestAppointment
    {
        [Key]
        [Column("test_app_id")]
        public int TestAppId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("room_id")]
        public int RoomId { get; set; }

        [Column("patient_id")]
        public int PatientId { get; set; }

        [Column("scheduled_date")]
        [DisplayName("Дата")]
        public DateTime ScheduledDate { get; set; }

        [Column("scheduled_time")]
        [DisplayName("Час")]
        public TimeSpan ScheduledTime { get; set; }

        [Column("status")]
        [DisplayName("Статус")]
        [MaxLength(20)]
        public string Status { get; set; } = "заплановано";

        [Column("technician_id")]
        public int? TechnicianId { get; set; }

        [Column("actual_start_time")]
        [DisplayName("Початок")]
        public DateTime? ActualStartTime { get; set; }

        [Column("actual_end_time")]
        [DisplayName("Кінець")]
        public DateTime? ActualEndTime { get; set; }

        [Column("notes")]
        [DisplayName("Нотатки")]
        public string? Notes { get; set; }

        [Browsable(false)]
        [ForeignKey("OrderId")]
        public virtual TestOrder? TestOrder { get; set; }

        [Browsable(false)]
        [ForeignKey("RoomId")]
        public virtual ExaminationRoom? ExaminationRoom { get; set; }

        [Browsable(false)]
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        [Browsable(false)]
        [ForeignKey("TechnicianId")]
        public virtual Doctor? Technician { get; set; }
    }
}