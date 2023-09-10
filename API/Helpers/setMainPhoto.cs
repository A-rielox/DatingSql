namespace API.Helpers;

public class SetMainPhoto
{
    public int oldMainId { get; set; }
    public int newMainId { get; set; }

    public SetMainPhoto(int oldMainId, int newMainId)
    {
        this.oldMainId = oldMainId;
        this.newMainId = newMainId;
    }    
}
