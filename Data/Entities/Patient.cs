using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("Patients")] 
    public class Patient
    {
        [Key]
        [Column("patient_id")] 
        public int PatientId { get; set; }

        [Required(ErrorMessage = "ПІБ обов'язкове поле")]
        [Column("full_name")]
        [StringLength(100, ErrorMessage = "ПІБ не може перевищувати 100 символів")]
        public string FullName { get; set; } = "Новий пацієнт";

        [Required(ErrorMessage = "Дата народження обов'язкова")]
        [Column("birth_date")]
        public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-30);

        [Column("address")]
        [StringLength(200)]
        public string? Address { get; set; } = "";

        [Column("phone")]
        [StringLength(20)]
        public string? Phone { get; set; } = "";

        [Column("email")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string? Email { get; set; } = "";

        [Column("family_doctor_id")]
        public int? FamilyDoctorId { get; set; }

        [Column("registration_date")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Column("blood_type")]
        [StringLength(5)]
        public string? BloodType { get; set; } = "";

        [Column("allergies")]
        public string? Allergies { get; set; } = "";

        [Required]
        [Column("password")]
        [StringLength(100)]
        public string Password { get; set; } = "12345";

        [ForeignKey("FamilyDoctorId")]
        public virtual Doctor? FamilyDoctor { get; set; }
    }
}