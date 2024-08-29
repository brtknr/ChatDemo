using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace ChatDemo.API.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public string SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string? RecieverId { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("Id")]
        public int GroupId { get; set; }

        public virtual Group Group { get; set; }

    }
}
