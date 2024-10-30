using AutoMapper;
using DVDLBussinessLayer;
using DVLDdataAccessLayer;
namespace dvld_api.models
{
    public class MyAutoMapperProfile:Profile
    {
        public MyAutoMapperProfile() {
            
            CreateMap<clsPerson,AddNewPersonDTO>();
            CreateMap<AddNewPersonDTO, clsPerson>();
            CreateMap<clsPerson,PersonDTO>();
            CreateMap<clsOrders,clsApplicationDTO>();
            CreateMap<AddAppointmentDTO, clsAppointments>();
            CreateMap<clsAppointments, GetAppointmentDTO>();
            CreateMap<clsLicenses, LicenseDTO>();
            CreateMap<LicenseDTO, clsLicenses>();
            CreateMap<DetainLicenseDTO, clsDetain>();
            
        }
    }
}
