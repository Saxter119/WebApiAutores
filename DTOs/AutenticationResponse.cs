
namespace webAPIAutores.DTOs
{
    public class AutenticationResponse 
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}