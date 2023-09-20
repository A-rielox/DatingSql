namespace API.Entities;

public class Message
{
    public int Id { get; set; }
    /////////////////////
    public int SenderId { get; set; }
    public string SenderUsername { get; set; } // nullable
    //public AppUser Sender { get; set; }
    /////////////////////
    public int RecipientId { get; set; }
    public string RecipientUsername { get; set; } // nullable
    //public AppUser Recipient { get; set; }
    /////////////////////
    public string Content { get; set; } // nullable
    public DateTime? DateRead { get; set; } // nullable
    public DateTime MessageSent { get; set; } = DateTime.UtcNow;
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }
}

// on delete: restricted, para q no se borre si se borra el usuario