﻿using Microsoft.AspNetCore.Identity;

namespace Cream.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Nickname { get; set; }


        public string FullName
        {
            get
            {
                return $"{LastName} {FirstName[0]}.{MiddleName?[0]}.";
            }
        }

    }
}
