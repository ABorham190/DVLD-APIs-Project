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
            CreateMap<AppointmentDTO, clsAppointments>();
            
        }
    }
}
