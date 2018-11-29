using System;
using System.Collections.Generic;
using System.Text;

namespace Servinte.Framework.Clinic.BasicInformation.Infraestructure
{
    public class Patient
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string TipoIdentificacion { get; set; }
        public long NumeroIdentificacion { get; set; }
   
        public int Edad { get; set; }

        public decimal Peso { get; set; }

        public decimal MasaCorporal { get; set; }

        public decimal SuperficieCorporal { get; set; }

        public string Genero { get; set; }

        public long Identificador { get; set; }

        public decimal Talla { get; set; }

        
        public string GrupoSanguineo { get; set; }

    }
}
