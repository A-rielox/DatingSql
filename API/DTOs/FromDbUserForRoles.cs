namespace API.DTOs;

public class FromDbUserForRoles
{
    public int Id { get; set; }
    public string UserName { get; set; }

    public List<string> Roles { get; set; }
}
