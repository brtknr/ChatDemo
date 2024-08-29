using ChatDemo.API.Models;

namespace ChatDemo.API.ViewModels
{
    public class GroupResponseModel
    {
        public int GroupId { get; set; }
        public string Title { get; set; }
        public string AvatarUrl { get; set; }
        public string LastMessage { get; set; }
        public List<AppUser> Members { get; set; }
        public string AdminId { get; set; }
        public string AdminUsername { get; set; }
        public List<Message> Messages { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
