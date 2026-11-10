using System;
using System.Collections.Generic;
using System.Text;

namespace DecoArte.Modelos
{
    public class Cliente
    {
        // Atributos privados (Ocultos)
        private string idCliente;
        private string nombreApellido;
        private string telefono;
        private string direccion;

        // Propiedades públicas (Encapsulamiento con validación)
        public string IdCliente
        {
            get { return idCliente; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El ID del cliente no puede estar vacío.");
                idCliente = value;
            }
        }

        public string NombreApellido
        {
            get { return nombreApellido; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El nombre del cliente no puede estar vacío.");
                nombreApellido = value;
            }
        }

        public string Telefono
        {
            get { return telefono; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("El teléfono no puede estar vacío.");
                telefono = value;
            }
        }

        public string Direccion
        {
            get { return direccion; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("La dirección no puede estar vacía.");
                direccion = value;
            }
        }
    }
}
