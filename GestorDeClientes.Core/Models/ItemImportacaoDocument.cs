using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GestorDeClientes.Core.Models{
public class ItemImportacaoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public Guid IdImportacao { get; set; }
    public string Status { get; set; } = string.Empty;  
    public string Mensagem { get; set; } = string.Empty; 
}}