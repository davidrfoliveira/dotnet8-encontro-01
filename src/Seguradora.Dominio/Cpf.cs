public readonly struct Cpf
{
    public string Numero { get; }

    public Cpf(string numero)
    {
        var digitos = new string((numero ?? string.Empty).Where(char.IsDigit).ToArray());

        if (digitos.Length != 11)
            throw new ArgumentException("CPF deve conter 11 dígitos.", nameof(numero));

        if (digitos.Distinct().Count() == 1)
            throw new ArgumentException("CPF com todos os dígitos iguais é inválido.", nameof(numero));

        Numero = digitos;
    }


    public string Formatado => $" {Numero[..3]}.{Numero[3..6]}-{Numero[9..]}";
    public string Mascarado => $"***.{Numero[3..6]}.{Numero[6..9]}-**";

}