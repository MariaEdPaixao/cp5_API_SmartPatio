using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.ValueObjects
{
    public class Localizacao
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        protected Localizacao() { } 

        public Localizacao(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude deve estar entre -90 e 90 graus.");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude deve estar entre -180 e 180 graus.");

            Latitude = latitude;
            Longitude = longitude;
        }

        public override string ToString() => $"{Latitude}, {Longitude}";

    }
}
