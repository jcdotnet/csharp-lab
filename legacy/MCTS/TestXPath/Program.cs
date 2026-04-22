namespace TestXPath
{
    class Program
    {
        static void SelectNodo()
        {

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load("clientes.xml");
            System.Xml.XmlNodeList nodos = doc.SelectNodes("child::clientes/cliente/nombre");
            //System.Xml.XmlNodeList nodos = doc.SelectNodes("clientes/cliente[starts-with(nombre,'nom')]");
            foreach (System.Xml.XmlNode nodo in nodos)
            {
                System.Console.WriteLine("nodo: {0}, valor: {1}", nodo.Name, nodo.InnerText);
            }
        }

        static void NavegarNodos()
        {
            //cojo el dom del xptah en vez del xmldocument
            System.Xml.XPath.XPathDocument doc = new System.Xml.XPath.XPathDocument("clientes.xml");

            System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();

            System.Xml.XPath.XPathNodeIterator it = nav.Select("clientes/cliente"); //elemento a iterar

            while (it.MoveNext())
            {
                System.Console.WriteLine("nodo: {0}, valor: {1}", it.Current.Name, it.Current.Value);

            }
        }

        static void InsertarUpdate()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load("clientes.xml");
            System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();
            nav.MoveToChild("clientes", "");
            nav.MoveToChild("cliente", "");
            nav.InsertBefore("<cliente id='0'><nombre>Nuevo Nombre</nombre><apellidos>Nuevos apellidos</apellidos><importe>10</importe></cliente>");
            nav.InsertAfter("<cliente id='10'><nombre>Otro Nombre</nombre><apellidos>Otros apellidos</apellidos><importe>10</importe></cliente>");
            nav.ReplaceSelf("<cliente id='11'><nombre>nombre modificado</nombre><apellidos>apellidos modificados</apellidos><importe>10</importe></cliente>");

            nav.MoveToParent();


            nav.AppendChild("<cliente id='11'><nombre>primero</nombre><apellidos>apellidos</apellidos><importe>10</importe></cliente>");
            System.Console.WriteLine(nav.OuterXml);
        }
        static void CogerSumaImporte()
        {
            System.Xml.XPath.XPathDocument doc = new System.Xml.XPath.XPathDocument("clientes.xml");
            System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();
            System.Xml.XPath.XPathExpression exp = nav.Compile("sum(clientes/cliente/importe)");
            System.Console.WriteLine(nav.Evaluate(exp));
        }

        static void Main(string[] args)
        {
            // navegar con Xpath
            Program.SelectNodo();
            Program.NavegarNodos();
            Program.InsertarUpdate();
            Program.CogerSumaImporte();

            // trasformar documentos con xslt
            System.Xml.Xsl.XslCompiledTransform tran = new System.Xml.Xsl.XslCompiledTransform();
            tran.Load("clientes.xsl");

            tran.Transform("clientes.xml", "clientes.cvs");



            System.Xml.Xsl.XslCompiledTransform tran2 = new System.Xml.Xsl.XslCompiledTransform();
            tran2.Load("clientesHtml.xslt");

            System.Xml.Xsl.XsltArgumentList parametros = new System.Xml.Xsl.XsltArgumentList();
            parametros.AddParam("fechaActual", "", System.DateTime.Now.ToString());
            parametros.AddExtensionObject("urn:pie", new Pie());  // interactuamos con objetos

            System.Xml.XmlTextWriter salida = new System.Xml.XmlTextWriter("clientes.html", System.Text.Encoding.UTF8);
            salida.Formatting = System.Xml.Formatting.Indented;
            tran2.Transform("clientes.xml", parametros, salida);
            salida.Close();

            System.Xml.Xsl.XslCompiledTransform tran3 = new System.Xml.Xsl.XslCompiledTransform();
            tran3.Load("clientesXML.xslt");


            tran3.Transform("clientes.xml", "clientesAtr.xml");


            System.Console.ReadLine();
        }
    }

    public class Pie
    {
        public string Fecha()
        {
            return System.DateTime.Now.ToString();
        }

    }
    //class Program
    //{

    //    static void SumaEad()
    //    {
    //        System.Xml.XPath.XPathDocument doc = new System.Xml.XPath.XPathDocument("datos.xml");
    //        System.Xml.XPath.XPathNavigator nav = doc.CreateNavigator();
    //        System.Xml.XPath.XPathExpression ex = nav.Compile("sum(//edad)");

    //        System.Console.WriteLine(nav.Evaluate(ex));
    //    }

    //    static void SelecionarConjuntoNodos()
    //    {
    //        System.Xml.XmlDocument doc1 = new System.Xml.XmlDocument();
    //        doc1.Load("datos.xml");
    //        System.Xml.XmlNodeList nodos = doc1.SelectNodes("clientes/cliente[starts-with(nombre, 'pe')]");

    //        //child::*                                  //Busqueda        
    //        //child::clientes/cliente                   // clientes/cliente[last()]
    //        //descendant::*                             // clientes/cliente[position()=2]
    //        //descendant:clientes/clientes              // clientes/cliente[position()=2]            
    //        //clientes/cliente[starts-with(nombre, 'pe')]
    //        foreach (System.Xml.XmlNode nodo in nodos)
    //        {
    //            System.Console.WriteLine("nodo: {0} valor: {1}", nodo.Name, nodo.InnerText);
    //        }

    //    }

    //    static void NavegarCojuntoNodos()
    //    {
    //        System.Xml.XmlDocument doc1 = new System.Xml.XmlDocument();
    //        doc1.Load("datos.xml");

    //        System.Xml.XPath.XPathNavigator nav = doc1.CreateNavigator();

    //        System.Xml.XPath.XPathNodeIterator it = nav.Select("root/clientes/nombre");

    //        while (it.MoveNext())
    //        {
    //            System.Console.WriteLine(it.Current.Name);

    //        }

    //    }

    //    static void InsetarUpdateNodo()
    //    {
    //        System.Xml.XmlDocument doc1 = new System.Xml.XmlDocument();
    //        doc1.Load("datos.xml");

    //        System.Xml.XPath.XPathNavigator nav = doc1.CreateNavigator();

    //        nav.MoveToChild("clientes", "www.forman.es");
    //        nav.MoveToChild("cliente", "www.forman.es");

    //        nav.InsertBefore("<cliente id='10'><nombre>primero</nombre><apellidos>apellidos</apellidos><importe>10,20</importe></cliente>");

    //        nav.InsertAfter("<cliente id='11'><nombre>primero</nombre><apellidos>apellidos</apellidos><importe>10,20</importe></cliente>");

    //        nav.ReplaceSelf("<cliente id='11'><nombre>modificadoprimero</nombre><apellidos>apellidos</apellidos><importe>10,20</importe></cliente>");


    //        nav.MoveToParent();


    //        nav.AppendChild("<cliente id='11'><nombre>primero</nombre><apellidos>apellidos</apellidos><importe>10,20</importe></cliente>");


    //        System.Console.WriteLine(nav.OuterXml);

    //    }

    //    static void Main(string[] args)
    //    {

    //        Program.SumaEad();

    //        Program.SelecionarConjuntoNodos();

    //        Program.InsetarUpdateNodo();

    //        System.Console.ReadLine();


    //    }
    }
