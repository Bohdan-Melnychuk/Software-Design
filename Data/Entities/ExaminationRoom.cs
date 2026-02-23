using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("ExaminationRooms")]
    public class ExaminationRoom
    {
        [Key]
        [Column("room_id")]
        public int RoomId { get; set; }

        [Column("room_number")]
        [DisplayName("Номер")]
        [MaxLength(10)]
        public string RoomNumber { get; set; } = null!;

        [Column("room_type")]
        [DisplayName("Тип")]
        [MaxLength(50)]
        public string RoomType { get; set; } = null!;

        [Column("description")]
        [DisplayName("Опис")]
        public string? Description { get; set; }

        [Column("responsible_doctor_id")]
        public int? ResponsibleDoctorId { get; set; }

        [Column("schedule_json")]
        [Browsable(false)]
        public string? ScheduleJson { get; set; }

        [Column("equipment_list")]
        [DisplayName("Обладнання")]
        public string? EquipmentList { get; set; }

        [Column("max_patients_per_day")]
        [DisplayName("Макс. пацієнтів/день")]
        public int MaxPatientsPerDay { get; set; } = 20;

        [Browsable(false)]
        [ForeignKey("ResponsibleDoctorId")]
        public virtual Doctor? ResponsibleDoctor { get; set; }
    }
}