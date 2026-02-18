using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ImportacaoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public Guid IdUsuario { get; set; }

    public DateTime DataProcessamento { get; set; }

    public string Status { get; set; } = string.Empty;

    public int TotalParaProcessar { get; set; }

    public int TotalProcessadoSucesso { get; set; }

    public int TotalProcessadoErro { get; set; }

    public string Mensagem { get; set; } = string.Empty;
}
