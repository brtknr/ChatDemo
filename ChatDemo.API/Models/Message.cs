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
        public int SenderId { get; set; }
        public int RecieverId { get; set; }
        public DateTime Date { get; set; }

    }
}
