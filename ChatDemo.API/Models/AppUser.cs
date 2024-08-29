using Microsoft.AspNetCore.Identity;

namespace ChatDemo.API.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
            Groups = new List<Group>();
        }

        public string Token { get; set; }
        public string ConnectionId { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
    }
}
