using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Clinic_BD.Data.Entities
{
    [Table("Referrals")]
    public class Referral
    {
        [Key]
        [Column("referral_id")]
        public int ReferralId { get; set; }
        
        [Column("patient_id")]
        public int PatientId { get; set; }
        
        [Column("from_doctor_id")]
        public int FromDoctorId { get; set; }
        
        [Column("to_specialty_id")]
        public int? ToSpecialtyId { get; set; }
        
        [Column("reason")]
        public string? Reason { get; set; }
        
        [Column("referral_date")]
        public DateTime? ReferralDate { get; set; }
        
        [Column("expiry_date")]
        public DateTime? ExpiryDate { get; set; }
        
        [Column("used")]
        public bool Used { get; set; }
        
        [Column("priority")]
        public string? Priority { get; set; }
        
        [Column("notes")]
        public string? Notes { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; } 
        
        [ForeignKey("FromDoctorId")]
        public virtual Doctor? FromDoctor { get; set; } 
        
        [ForeignKey("ToSpecialtyId")]
        public virtual Specialty? ToSpecialty { get; set; } 
    }
}