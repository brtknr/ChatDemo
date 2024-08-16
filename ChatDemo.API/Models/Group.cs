using System.ComponentModel.DataAnnotations;

namespace ChatDemo.API.Models
{
    public class Group
    {
        public Group()
        {
            GroupUsers = new HashSet<AppUser>();
        }
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<AppUser> GroupUsers { get; set; }
    }
}
