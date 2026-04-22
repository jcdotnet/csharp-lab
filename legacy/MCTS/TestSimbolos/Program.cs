namespace TestSimbolos
{
    class Simbolo
    {
        static char[] simbolo = { 'A', 'T', 'ß', 'o' };

        private char valor;

        public char Valor
        {
            get { return valor; }

        }

        public Simbolo()
        {
            valor = simbolo[0];
        }

        private static int Posicion(char caracter)
        {
            for (int pos = 0; pos < simbolo.Length; pos++)
            {
                if (simbolo[pos] == caracter)
                {
                    return pos;
                }
            }
            return -1;
        }

        public static Simbolo operator +(Simbolo simbolo, int incremento) // nos da el siguiente simbolo
        {
            int pos = Posicion(simbolo.valor);
            if (pos + incremento > Simbolo.simbolo.Length) throw new Exception("Fuera de rango");


            Simbolo valor = new Simbolo();
            valor.valor = Simbolo.simbolo[pos + incremento];
            return valor;


        }
        public static Simbolo operator -(Simbolo simbolo, int decremento) // nos da el siguiente simbolo
        {
            int pos = Posicion(simbolo.valor);
            if (pos - decremento < 0) throw new Exception("Fuera de rango");


            Simbolo valor = new Simbolo();
            valor.valor = Simbolo.simbolo[pos - decremento];
            return valor;


        }
        public static Simbolo operator ++(Simbolo simbolo) // nos da el siguiente simbolo
        {
            int pos = Posicion(simbolo.valor);
            if (pos + 1 > Simbolo.simbolo.Length) throw new Exception("Fuera de rango");


            Simbolo valor = new Simbolo();
            valor.valor = Simbolo.simbolo[pos + 1];
            return valor;


        }
        public static Simbolo operator --(Simbolo simbolo) // nos da el siguiente simbolo
        {
            int pos = Posicion(simbolo.valor);
            if (pos - 1 < 0) throw new Exception("Fuera de rango");


            Simbolo valor = new Simbolo();
            valor.valor = Simbolo.simbolo[pos - 1];
            return valor;


        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Simbolo simbolo1 = new Simbolo();
            Simbolo resultado = simbolo1 + 2;

            System.Console.WriteLine(resultado.Valor);
            resultado -= 1;
            System.Console.WriteLine(resultado.Valor);
            resultado--;
            System.Console.WriteLine(resultado.Valor);
            System.Console.ReadLine();

        }
    }
}
