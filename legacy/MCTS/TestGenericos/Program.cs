namespace TestGenericos
{
    class Mensaje<T> where T : MensajeBase
    {
        T mensajeLengua;

        public T MensajeLengua
        {
            get { return mensajeLengua; }
            set { mensajeLengua = value; }
        }
        public void VerMensaje()
        {
            System.Console.WriteLine(MensajeLengua.ToString());
        }
    }
    abstract class MensajeBase
    {

    }
    class MensajeEspañol : MensajeBase
    {
        public override string ToString()
        {
            return "Mensaje en Español";
        }
    }

    class MensajeIngles : MensajeBase
    {
        public override string ToString()
        {
            return "English Message";
        }
    }
    class mensajAleman : MensajeBase
    {
        public override string ToString()
        {
            return "Mensaje en alemán";
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Mensaje<MensajeIngles> ingles = new Mensaje<MensajeIngles>();
            ingles.MensajeLengua = new MensajeIngles();
            ingles.VerMensaje();
        }
    }
}
