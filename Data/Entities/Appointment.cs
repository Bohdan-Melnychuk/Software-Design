using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Clinic_BD.Data.Entities;

namespace Clinic_BD.Data.Entities
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        [Column("appointment_id")]
        public int AppointmentId { get; set; }

        [Column("patient_id")]
        public int PatientId { get; set; }

        [Column("doctor_id")]
        public int DoctorId { get; set; }

        [Column("referral_id")]
        public int? ReferralId { get; set; }

        [Column("appointment_date")]
        public DateTime? AppointmentDate { get; set; }

        [Column("appointment_time")]
        public TimeSpan? AppointmentTime { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime? CreateAt { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
        
        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }
        
        [ForeignKey("ReferralId")]
        public virtual Referral? Referral { get; set; }
    }
}