using DVDLBussinessLayer;
using DVLDdataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace dvld_api.models
{
    public static class HandleIssueLicense
    {

        public static async Task FillLicenseObject(clsLicenses license,LDLApp ldlApp)
        {

            license.ApplicationID = ldlApp.AppID;
            license.LicenseClassID = ldlApp.LicenseTypeID;
            license.IssueDate = DateTime.Now;
            license.ExpirationDate = license.IssueDate.AddYears(10);
            license.Notes = "No Notes";
            license.PaidFees = 10;
            license.IsActive = true;
            license.IssueReason = 1;
            license.CreatedByUserID = 1;
            license.ImagePath = ldlApp.Application.Person.ImagePath;
            license.ClassName = await LDLApp.GetLicenseTypeByLicenseClassID(ldlApp.LicenseTypeID);
            license.NationalNo = ldlApp.Application.Person.NationalNumber;
            license.IsDetained = false;
            license.DateOfBirth = ldlApp.Application.Person.DateOfBirth;
            license.Name = ldlApp.Application.Person.FullName;
            license.Gender = (byte)ldlApp.Application.Person.Gender;

        }

        public static void FillDriverObject(clsDriver driver, LDLApp ldlApp)
        {
            driver.PersonID = ldlApp.Application.ApplicantID;
            driver.CreatedByUserID = 1;
            driver.CreatedDate = DateTime.Now;

        }

    }

 }

