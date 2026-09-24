public record SeguradoResponse(
    string Id,
    string Nome,
    string Cpf,
    DateTime DataNascimento,
    int Idade)
{
    public static SeguradoResponse De(Segurado s) => new(
        s.Id,
        s.Nome,
        s.Documento.Mascarado,
        s.DataNascimento,
        s.Idade);
}
