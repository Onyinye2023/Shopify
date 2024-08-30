using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Domain.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Admin> Admins { get; set; } = new List<Admin>();


    }
}
