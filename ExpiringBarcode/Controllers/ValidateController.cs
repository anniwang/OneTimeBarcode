using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Shared;

namespace ExpiringBarcode.Controllers
{
    public class ValidateController : BaseController
    {
        // GET: Validate
        public string Index(string barcode)
        {
            if (barcode.Length != 12 + TOTP.digits || barcode.Any(a=>!"0123456789".Contains(a)))
            {
                return "data not in correct format";
            }

            var memberId = barcode.Substring(0, 12);
            var code = barcode.Substring(12);
            var member = UserManager.Users.FirstOrDefault(a => a.MembershipNumber.Equals(memberId));
            if (member == null)
            {
                return "Not Authorized";
            }
            if (member.SharedBarcodeSecret == null)
            {
                return "Not Authorized";
            }
            var totp = new TOTP(member.SharedBarcodeSecret);
            if (totp.ConfirmCode(code))
            {
                return "Authorized! \n User: " + member.Email;
            }
            return "Not Authorizd";
        }
    }
}