using System;
using System.Collections.Generic;
using System.Text;

namespace GestionClientes
{
    public class Cliente : IPedidos, IFacturas, IRecibos
    {
        private string nombre;

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        private string dni;

        public string Dni
        {
            get { return dni; }
        }
        protected System.Collections.Generic.List <Articulo> lista;

        public Cliente(string nombre, string dni)
        {
            this.nombre = nombre;
            this.dni = dni;
            this.lista= new System.Collections.Generic.List <Articulo>();

        }


        public virtual void AñadirPedido(Articulo articulo)
        {
            this.lista.Add(articulo);
        }


        public float RealizarFactura()
        {
            float total=0;
            System.Console.WriteLine("FACTURA, Nombre: {0}, DNI: {1} , Fecha: {2}\nLista Productos:\n", this.nombre, this.dni, System.DateTime.Now.ToString("dd/MM/yyyy"));
            foreach (Articulo articulo in this.lista)
            {
                total += articulo.Precio;
                System.Console.WriteLine("Producto: {0},  Precio: {1}", articulo.Producto, articulo.Precio);
            }
            System.Console.WriteLine("               TOTAL FACTURA:  " + total);
            System.Console.ReadLine();
            return total;
        }

       
        public void EntregarRecibo(float cantidad)
        {
            System.Console.WriteLine(this.nombre+ " "+cantidad + " euros PAGADOS");
            System.Console.ReadLine();
        }
    }



    class ClienteEspecial : Cliente
    {
        readonly float descuento;
        public ClienteEspecial(string nombre, string dni) : base(nombre, dni) {
            this.descuento = 0.10f;
        }
        public override void AñadirPedido(Articulo articulo)
        {
            articulo.Precio-= (this.descuento*articulo.Precio);
            base.lista.Add(articulo);
        }
        

    }
}
