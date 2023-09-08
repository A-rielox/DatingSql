using Dapper.Contrib.Extensions;

namespace API.Entities;

// p' especificar el nombre q va a tener en la db
[System.ComponentModel.DataAnnotations.Schema.Table("Photos")]
public class Photo
{
    [Key]
    public int Id { get; set; }
    public string Url { get; set; }
    public int IsMain { get; set; }
    public string PublicId { get; set; }

    // p'q ocupe la id del AppUser como foreign-key, y paq la prop AppUserId NO sea nullable ( NO puede
    // haber fotos q no esten relacionadas a un AppUser )
    // p' decirle a Dapper que esta prop no es writeable ( esta prop NO va a la tabla )
    [Write(false)]
    public AppUser AppUser { get; set; }
    
    public int AppUserId { get; set; }
}
