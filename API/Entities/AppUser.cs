using API.Extensions;
using Dapper.Contrib.Extensions;

namespace API.Entities;

// p'q Dapper sepa el nombre de la tabla, ya que buscaba a "AppUsers" xdefault
[Dapper.Contrib.Extensions.Table("Users")]
public class AppUser
{
    [Key]
    public int Id { get; set; }
    public string UserName { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }



    public DateTime DateOfBirth { get; set; } = new DateTime(1901, 01, 01, 00, 00, 00);
    public string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime LastActive { get; set; } = DateTime.Now;
    public string Gender { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    // relacion one-to-many ( un user many photos )
    // p' decirle a Dapper que esta prop no es writeable ( esta prop NO va a la tabla )
    // xq al hacer el update le paso a dapper un AppUser y me trataba de hacer update a esta prop
    [Write(false)]
    public List<Photo> Photos { get; set; } = new();
    // el Photo hago
    // p'q ocupe la id del AppUser como foreign-key
    // public AppUser AppUser { get; set; }
    // public int AppUserId { get; set; }
    // asi las fotos quedan ligadas a un AppUser, y cuando se borre un user se van a borrar las fotos
    // el cascade delete

    ////////////////////////////
    ///public List<UserLike> LikedByUsers { get; set; } // los q te dan like
    //public List<UserLike> LikedUsers { get; set; } // a quienes les doy like
    public List<AppUser> LikedByUsers { get; set; } // los q te dan like
    public List<AppUser> LikedUsers { get; set; } // a quienes les doy like


    ////////////////////////////

    //public List<Message> MessagesSent { get; set; }
    //public List<Message> MessagesReceived { get; set; }

    ////////////////////////////
    // es la misma navigation-property hacia la join-table en AppUser.cs y AppRole.cs
    //public ICollection<AppUserRole> UserRoles { get; set; }

}
