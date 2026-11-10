using System;
using System.Collections.Generic;
using System.Text;

namespace DecoArte.Modelos
{
    public class ProductoInventario
    {
        public string IdProducto { get; set; }
        public string IdProducto_Servicio { get; set; }
        public decimal Stock { get; set; }
        public decimal Total { get; set; }
    }
}
