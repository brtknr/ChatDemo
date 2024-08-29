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
        public string? Title { get; set; }
        public string AdminId { get; set; }
        public string AvatarUrl { get; set; }
        public string AdminUsername { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<AppUser> GroupUsers { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
