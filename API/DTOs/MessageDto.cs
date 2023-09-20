namespace API.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername { get; set; }
    public string SenderPhotoUrl { get; set; } // +
    public int RecipientId { get; set; }
    public string RecipientUsername { get; set; }
    public string RecipientPhotoUrl { get; set; } // +
    public string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; }
}

/*                  Message
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername { get; set; } // nullable
    public int RecipientId { get; set; }
    public string RecipientUsername { get; set; } // nullable
    public string Content { get; set; } // nullable
    public DateTime? DateRead { get; set; } // nullable
    public DateTime MessageSent { get; set; } = DateTime.UtcNow;
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }
*/