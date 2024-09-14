using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalStructs
{
    public class General
    {
        
    }
    public struct GeneralStructs
    {

        public int AppointmentID;
        public int DLAppID;
        public string ApplicantName;
        public string ClassName;
        public int Trial;
        public Decimal Fees;
        public DateTime Date;
        public int ApplicationID;
        public int RetakeTestAppID;

    }
    public struct ScheduleTestInfo
    {

        int _DLAppID ;
        string _ClassTypeName ;
        string _Name ;
        int _TestTrials ;
        DateTime _Date ;
        Decimal _Fees ;
        Decimal _RetakeTestAppFees ;
        Decimal _TotalFees ;
        int _RetakeTestAppID ;

    }
}
