using Dominio.Enumeradores;
using Dominio.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class CarrapatoMongo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string CodigoSerial { get; set; }
    public StatusBateriaEnum StatusBateria { get; set; }
    public StatusDeUsoEnum StatusDeUso { get; set; }
    public int IdPatio { get; set; }

    // Value Object
    public Localizacao? UltimaLocalizacao { get; private set; }

    public CarrapatoMongo(string codigoSerial, int idPatio)
    {
        CodigoSerial = codigoSerial;
        IdPatio = idPatio;
        StatusBateria = StatusBateriaEnum.Alta;
        StatusDeUso = StatusDeUsoEnum.Disponivel;
    }
    public void AtualizarLocalizacao(double latitude, double longitude)
    {
        UltimaLocalizacao = new Localizacao(latitude, longitude);
    }
}