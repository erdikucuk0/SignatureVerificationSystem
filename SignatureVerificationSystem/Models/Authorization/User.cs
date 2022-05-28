using SignatureVerificationSystem.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SignatureVerificationSystem.Models.Authorization.AuthorizationModel;

namespace SignatureVerificationSystem.Models.Authorization
{
    public class User
    {
        #region Member Fields

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; } = UserRole.Admin;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        #endregion

        #region Helper Procedures

      
        #endregion
    }
}