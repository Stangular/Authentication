using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AuthUser
{

    public class AUser : IdentityUser
    {
        public virtual AdditionalUserInformation AdditionalUserInformation { get; set; }
    }

    public class AdditionalUserInformation
    {
        public int Id { get; set; }
        public string PreferredName { get; set; }
        public string FamilyName { get; set; }
        public int Status { get; set; }
    }
}
