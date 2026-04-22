#define Depuracion
namespace Misc
{
    class Program
    {
        #region Tipos

        public const float PI = 2.1416f;
        public const System.Int16 VALOR10 = 10;



        public struct TiposPrimitivos
        {

            readonly int decremento;

            public byte miByte;  // CTS System.Byte
            public sbyte miSByte;
            public System.Boolean booleano;
            public short corto;  // CTS System.Int16
            public ushort uCorto;
            public int entero; // CTS System.Int32
            public uint uEntero;
            public long largo;    // CTS System.Int64
            public ulong uLargo;
            public float flotante; // CTS System.Single
            public System.Double doble;
            public System.Decimal miDecimal;
            public System.Char caracter;


            public TiposPrimitivos(string mensaje)
            {
                System.Console.WriteLine(mensaje);
                decremento = 15;
                miByte = 0;
                miSByte = 1;
                booleano = true;
                corto = -10;
                uCorto = 10;
                entero = -22;
                uEntero = (22 - uint.Parse("10"));
                flotante = 111.5f - System.Convert.ToSingle(decremento);
                doble = 0.00001d;
                largo = (long)doble;
                uLargo = 100000;
                miDecimal = 1234567890;
                caracter = 'B';
            }
            public override string ToString()
            {
                return miByte + ", " + booleano + ", " + entero + ", " + uEntero + ", " + flotante + ", " + doble + ", " + largo + ", " + uLargo + ", " + miDecimal + ", " + caracter;
            }
        }

        #endregion Tipos

        #region Metodos
        public void Operadores(ref TiposPrimitivos tipos)
        {

            int valor10 = Program.VALOR10;
            try
            {
                // conversion de double a char
                System.Console.WriteLine(System.Convert.ToChar(tipos.doble));
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            // operadores
            try
            {
                tipos.booleano = false;
                // unarios
                System.Console.WriteLine("Valor:" + valor10);
                System.Console.WriteLine("INCREMENTO POSTERIOR ++:" + ++valor10);
                System.Console.WriteLine("INCREMENTO ANTERIOR ++: " + valor10++);
                System.Console.WriteLine("DECREMENTO POSTERIOR --:" + --valor10);
                System.Console.WriteLine("DECREMENTO ANTERIOR--" + valor10--);
                System.Console.WriteLine("OPERADOR NOT:" + !tipos.booleano);
                // binarios
                System.Console.WriteLine("\nOperaciones con 4,4 y 4");
                System.Console.WriteLine("SUMA: " + (4.4 + 4));
                System.Console.WriteLine("RESTA: " + (4.4 - 4));
                System.Console.WriteLine("MULTIPLICACION: " + (4.4 * 4));
                System.Console.WriteLine("DIVISION:" + (4.4 / 4));
                System.Console.WriteLine("MODULO: " + ((short)4.4 % 4));
                System.Console.WriteLine("DESPLAZAMIENTO IZQUIERDA 100<<2: " + (4 << 2));
                System.Console.WriteLine("DESPLAZAMIENTO DERECHA 100>>2: " + (4 >> 2));
                System.Console.WriteLine("AND CORTOCIRCUITO: " + (tipos.booleano & true));
                System.Console.WriteLine("AND:  " + (tipos.booleano && true));
                System.Console.WriteLine("OR CORTOCIRCUITO: " + (tipos.booleano || true));
                System.Console.WriteLine("OR:  " + (tipos.booleano || true));
                // ternarios
                System.Console.Write("Nuestra variable booleana vale: ");
                System.Console.WriteLine(tipos.booleano ? "valor true" : "valor false");

                System.Console.WriteLine("Pulse Enter para continuar..");
                System.Console.ReadLine();
            }
            catch (System.DivideByZeroException e)
            {
                System.Console.WriteLine(e.Message);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        public void Sentencias(TiposPrimitivos tipos)
        {
            string[] escalaMusical = { "do", "re", "mi", "fa", "sol", "la", "si" };

            if (tipos.booleano)
                tipos.ToString();
            switch (tipos.caracter)
            {
                case 'a': goto default;
                case 'b':
                case 'c': System.Console.WriteLine(tipos.caracter); break;
                default: System.Console.WriteLine("Estoy en el default de la sentencia switch"); break;
            }

            int cuenta = 0;
            while (tipos.booleano)
            {
                System.Console.WriteLine(++cuenta);
                if (cuenta == 10) break;
            }

            do
            {
                System.Console.WriteLine(--cuenta);
            } while (cuenta > -10);

            for (int i = 0; i < escalaMusical.Length; i += 2)
            {
                System.Console.WriteLine(escalaMusical[i]);
            }
            System.Console.WriteLine("Pulse Enter para continuar..");
            System.Console.ReadLine();
        }
        #endregion Metodos

        static void Main(string[] args)
        {
            Program test = new Program();
            #if Depuracion
               System.Console.WriteLine(" Modo depuración");
            #else
                System.Console.WriteLine(" Modo release");
            #endif
            TiposPrimitivos tipos = new TiposPrimitivos("Empezamos..");
            test.Operadores(ref tipos);
            test.Sentencias(tipos);
        }
    }
}
