namespace PokeAPIService.Exceptions;

//Custom exception
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
