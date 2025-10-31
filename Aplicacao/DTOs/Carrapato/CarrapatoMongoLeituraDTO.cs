namespace Aplicacao.DTOs.Carrapato;

public record CarrapatoMongoLeituraDto(
    string? Id,
    string CodigoSerial,
    string StatusBateria,
    string StatusDeUso,
    int IdPatio
);