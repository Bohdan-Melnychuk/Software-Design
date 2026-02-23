using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_BD.Data.Entities
{
    [Table("Specialties")]
    public class Specialty
    {
        [Column("specialty_id")] 
        public int SpecialtyId { get; set; }

        [Column("code")] 
        public string Code { get; set; } = null!;

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("is_family")]
        public bool IsFamily { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Browsable(false)]
        public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}