using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacao.DTOs.Carrapato
{
    public class CarrapatoMongoCriarDto
    {
        public string CodigoSerial { get; set; } = string.Empty;
        public int IdPatio { get; set; }

        // Campos opcionais só pro Mongo
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
