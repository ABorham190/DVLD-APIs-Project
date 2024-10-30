using DVDLBussinessLayer;

namespace dvld_api.models
{
    public static class HandleAddNewDetain
    {
        public static async Task< int> AddNewDetain(DetainLicenseDTO detainLicenseDTO)
        {
            clsDetain detain = new clsDetain
            {
                LicenseID = detainLicenseDTO.LicenseID,
                CreatedByUserID = detainLicenseDTO.CreatedByUserID,
                DetainDate = detainLicenseDTO.DetainDate,
                FineFees = detainLicenseDTO.FineFees,
                IsRelease = false,
            };

            if (await detain.Save())
            {
                return detain.DetainID;
            }
            else
            {
                return -1;
            }
        }
    }
}
