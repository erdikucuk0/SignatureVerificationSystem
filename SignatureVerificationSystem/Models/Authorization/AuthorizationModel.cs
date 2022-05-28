using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignatureVerificationSystem.Models.Authorization
{    
    public class AuthorizationModel : INotifyPropertyChanged
    {
        #region Member Fields

        public enum UserRole
        {
            Admin,
            Director,
            Operator,
            Control,
            Normal
        }

        private bool settingsButton { get; set; } = false;
        public bool SettingsButton
        {
            get
            {
                return settingsButton;
            }
            set
            {
                if (value != settingsButton)
                {
                    settingsButton = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("SettingsButton"));
                }
            }
        }

        private bool manageUsersButton { get; set; } = false;
        public bool ManageUsersButton
        {
            get
            {
                return manageUsersButton;
            }
            set
            {
                if (value != manageUsersButton)
                {
                    manageUsersButton = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ManageUsersButton"));
                }
            }
        }

        private bool controlButton { get; set; } = false;
        public bool ControlButton
        {
            get
            {
                return controlButton;
            }
            set
            {
                if (value != controlButton)
                {
                    controlButton = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ControlButton"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Helper Procedures

        public void LoginSucceed(User currentUser)
        {
            if (currentUser == null)
            {
                SettingsButton = false;
                ManageUsersButton = false;
                ControlButton = false;

                return;
            }
            
            SettingsButton = true;
            ManageUsersButton = true;
            ControlButton = true;
        }

        #endregion
    }
}