using System;
using System.Collections.Generic;

namespace ECommerce.Models 
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }
        public string Name {get; set; }
        public string EmailAddress {get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<Connections> Connections {get; set; }

        public List<Invitations> Invitations {get; set; }


        public User()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

    }

}