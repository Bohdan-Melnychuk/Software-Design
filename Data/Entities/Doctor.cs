using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("Doctors")]
    public class Doctor
    {
        [Key]
        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Column("full_name")]
        [DisplayName("ПІБ Лікаря")]
        public string FullName { get; set; } = null!;

        [Column("specialty_id")]
        public int SpecialtyId { get; set; }

        [Column("room_number")]
        [DisplayName("Кабінет")]
        public string RoomNumber { get; set; } = null!;

        [Column("work_phone")]
        [DisplayName("Робочий тел.")]
        public string? WorkPhone { get; set; }

        [Column("personal_phone")]
        [DisplayName("Особистий тел.")]
        public string? PersonalPhone { get; set; }

        [Column("email")]
        [DisplayName("Email")]
        public string? Email { get; set; }
        
        [Column("password")]
        [Browsable(false)] 
        public string Password { get; set; } = null!;

        [Browsable(false)]
        [ForeignKey("SpecialtyId")]
        public virtual Specialty Specialty { get; set; } = null!;
        
        [Column("is_accepting_new_patients")]
        [DisplayName("Приймає нових")]
        public bool IsAcceptingNewPatients { get; set; } = true;

        [NotMapped] 
        [DisplayName("Код спец.")] 
        public string SpecialtyCode => Specialty?.Code ?? "???";
        
        [Column("created_at")]
        [DisplayName("Дата створення")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreateAt { get; set; } = DateTime.Now;
    }
}