namespace API.Helpers;

public class UserParams : PaginationParams
{
    // este no me los envia, yo lo pongo en el controller
    public string CurrentUsername { get; set; }
    public string Gender { get; set; }
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;
    public string OrderBy { get; set; } = "lastActive";
}
