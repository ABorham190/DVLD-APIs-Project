using System.ComponentModel.DataAnnotations;

namespace dvld_api.models
{
    public class DetainLicenseDTO
    {
        public int DetainID { get;  }
        [Required]
        public int LicenseID { get; set; }
        [Required]
        public DateTime DetainDate { get; set; }
        [Required]
        public Decimal FineFees { get; set; }
        [Required]
        public int CreatedByUserID { get; set; }
        public bool? IsRelease { get;  }
        public DateTime? ReleaseDate { get;  }
        public int? ReleasedByUserId { get; }
        public int? ReleasedAppID { get;  }
    }
}
