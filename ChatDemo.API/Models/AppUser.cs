using Microsoft.AspNetCore.Identity;

namespace ChatDemo.API.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            Groups = new List<Group>();
        }
        public virtual ICollection<Group> Groups { get; set; }
    }
}
