using System.Text.RegularExpressions;

namespace Practice.Gateway.Dto
{
    public class Properties
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Personas { get; set; }
        public int Camas { get; set; }
        public int Habitaciones { get; set; }

        public string TipoPropiedad { get; set; }
    }
}
