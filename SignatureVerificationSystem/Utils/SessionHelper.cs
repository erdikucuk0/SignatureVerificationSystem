using SignatureVerificationSystem.Models.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignatureVerificationSystem.Utils
{
    internal class SessionHelper
    {
        public static User CurrentUser;

        public static bool IsLoggedIn => CurrentUser != null;
    }
}