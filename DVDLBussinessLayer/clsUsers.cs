using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DVLDdataAccessLayer;

namespace DVDLBussinessLayer
{
    public class clsUsers
    {
        public int PersonID {  get; set; }
        public int UserID { get; set; }
        public string UserName {  get; set; }
        public string UserPassword { get; set; }
        public bool IsActive {  get; set; }

        enum enMode { AddNewUser=0, UpdateUser=1 }
        enMode _Mode;

        static public DataTable GetAllUsersList()
        {
            return clsUsersDataLayer.GetUsersList();
        }
        public clsUsers()
        {
            PersonID = -1;
            UserID = -1;
            UserName = "";
            UserPassword = "";
            IsActive = false;
            _Mode=enMode.AddNewUser;
        }
        private clsUsers(int UserID,int PersonID,string UserName,string Password,
            bool IsActive)
        {
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.UserName = UserName;
            this.UserPassword = Password;
            this.IsActive = IsActive;
            _Mode = enMode.UpdateUser;
        }
        bool _UpdateUser()
        {
            return clsUsersDataLayer.UpdateUserIsActive(this.UserID,
                this.IsActive,this.UserName,this.UserPassword);
        }

        bool  _AddNewUser()
        {
            this.UserID = clsUsersDataLayer.AddNewUser(this.PersonID,
                this.UserName, this.UserPassword, this.IsActive);
            return this.UserID != -1;
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNewUser:
                    if (_AddNewUser())
                    {
                        _Mode = enMode.UpdateUser;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.UpdateUser:
                    if(_UpdateUser())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                   
            }

            return false;

        }

        public static bool DeleteUser(int UserID) 
        {
          
            return clsUsersDataLayer.DeleteUser(UserID);
        
        }

        public static clsUsers FindUser(int UserID)
        {
            string Password = "", UserName = "";
            bool IsActive = false;
            int PersonID = -1;
            if (clsUsersDataLayer.FindUserByUserID(UserID,ref PersonID,
                ref UserName,ref Password,ref IsActive))
            {
                return new clsUsers(UserID, PersonID, UserName, Password, IsActive);

            }
            else
            {
                return null;
            }
        }
        public static clsUsers FindUser(string UserName,string Password)
        {
            
            int  PersonID = -1,UserID=-1;
            bool IsActive = false;

            if (clsUsersDataLayer.FindUserByUserNameAndPassword(ref UserID, ref PersonID,
                 UserName,  Password, ref IsActive))
            {
                return new clsUsers(UserID, PersonID, UserName, Password, IsActive);

            }
            else
            {
                return null;
            }
        }

        public static bool GetUserNameByUserID(int UserId,ref string UserName)
        {
            return clsUsersDataLayer.GetUserNameByUserID(UserId, ref UserName);

        }
        public static void AddInfoToEventViewer(string Message)
        {
            Settings.AddInfoToEventViewer(Message);
        }
        public static string HashPassWord(string Password)
        {
            return Settings.ComputeHash(Password);
        }
    }
}
