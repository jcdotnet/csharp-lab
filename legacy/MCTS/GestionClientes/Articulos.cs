using System;
using System.Collections.Generic;
using System.Text;

namespace GestionClientes
{
    public class Articulo
    {
        private string producto;

        public string Producto
        {
            get { return producto; }
            set { producto = value; }
        }
        private float precio;

        public float Precio
        {
            get { return precio; }
            set { precio = value; }
        }
        public Articulo(string producto, float valor)
        {
            this.producto = producto;
            this.precio = valor;
        }

    }

    public interface IPedidos
    {
        void AñadirPedido(Articulo articulo);
    }
    public interface IFacturas
    {
        float RealizarFactura();
    }
    public interface IRecibos
    {
        void EntregarRecibo(float cantidad);
    }

}
