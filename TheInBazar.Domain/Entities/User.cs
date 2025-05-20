using TheInBazar.Domain.Commons;

namespace TheInBazar.Domain.Entities;

public class User :Auditable
{
    public string Firstname {  get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
