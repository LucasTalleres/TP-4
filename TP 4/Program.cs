using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

[Serializable]
public class AerolineaData
{
    public string RazonSocial { get; set; }
    public string Telefono { get; set; }
    public string Domicilio { get; set; }
    public List<Vuelo> Vuelos { get; set; }

    public AerolineaData()
    {
        Vuelos = new List<Vuelo>();
    }
}

[Serializable]
public class Vuelo
{
    public string CodigoVuelo { get; set; }
    public DateTime FechaHoraSalida { get; set; }
    public DateTime FechaHoraLlegada { get; set; }
    public string Piloto { get; set; }
    public string Copiloto { get; set; }
    public int CapacidadMaxima { get; set; }
    public int PasajerosRegistrados { get; set; }

    public double PorcentajeOcupacion()
    {
        return (PasajerosRegistrados / (double)CapacidadMaxima) * 100;
    }
}

class Program
{
    public static void Main(string[] args)
    {
        AerolineaData aerolinea = new AerolineaData();
        string rutaArchivo = "aerolinea.xml";

        // Cargar datos desde archivo XML al comenzar el programa 
        CargarDatosXML(rutaArchivo, aerolinea);

        int opcion;
        do
        {
            MostrarOpciones();
            Console.Write("Seleccione una opción: ");
            opcion = int.Parse(Console.ReadLine());

            switch (opcion)
            {
                case 1:
                    AgregarVuelo(aerolinea);
                    break;
                case 2:
                    RegistrarPasajeros(aerolinea);
                    break;
                case 3:
                    Console.WriteLine($"Ocupación media de la flota: {CalcularOcupacionMedia(aerolinea):F2}%");
                    break;
                case 4:
                    Vuelo vueloMayorOcupacion = ObtenerVueloMayorOcupacion(aerolinea);
                    if (vueloMayorOcupacion != null)
                    {
                        Console.WriteLine($"Vuelo con mayor ocupación: {vueloMayorOcupacion.CodigoVuelo} - {vueloMayorOcupacion.PorcentajeOcupacion():F2}%");
                    }
                    else
                    {
                        Console.WriteLine("No hay vuelos registrados.");
                    }
                    break;
                case 5:
                    BuscarVuelo(aerolinea);
                    break;
                case 6:
                    ListarVuelosOrdenadosPorOcupacion(aerolinea);
                    break;
                case 7:
                    GuardarDatosXML(aerolinea, rutaArchivo);
                    return;
                default:
                    Console.WriteLine("Opción inválida. Intente nuevamente.");
                    break;
            }
        } while (opcion != 7);
    }

    public static void MostrarOpciones()
    {
        Console.WriteLine("\n Menú de Aerolínea");
        Console.WriteLine("1. Agregar vuelo");
        Console.WriteLine("2. Registrar pasajeros");
        Console.WriteLine("3. Calcular ocupación media");
        Console.WriteLine("4. Vuelo con mayor ocupación");
        Console.WriteLine("5. Buscar vuelo por código");
        Console.WriteLine("6. Listar vuelos ordenados por ocupación");
        Console.WriteLine("7. Salir");
    }

    public static void CargarDatosXML(string archivo, AerolineaData aerolinea)
    {
        if (File.Exists(archivo))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AerolineaData));
            using (FileStream fileStream = new FileStream(archivo, FileMode.Open))
            {
                AerolineaData data = (AerolineaData)serializer.Deserialize(fileStream);
                aerolinea.RazonSocial = data.RazonSocial;
                aerolinea.Telefono = data.Telefono;
                aerolinea.Domicilio = data.Domicilio;
                aerolinea.Vuelos = data.Vuelos;
            }
        }
    }

    public static void GuardarDatosXML(AerolineaData aerolinea, string archivo)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(archivo))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AerolineaData));
                serializer.Serialize(writer, aerolinea);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al guardar los datos: " + ex.Message);
        }
    }

    public static void AgregarVuelo(AerolineaData aerolinea)
    {
        Vuelo nuevoVuelo = new Vuelo();
        Console.Write("Código de vuelo: ");
        nuevoVuelo.CodigoVuelo = Console.ReadLine();
        Console.Write("Fecha y hora de salida (aaaa-MM-dd HH:mm): ");
        nuevoVuelo.FechaHoraSalida = DateTime.Parse(Console.ReadLine());
        Console.Write("Fecha y hora de llegada (aaaa-MM-dd HH:mm): ");
        nuevoVuelo.FechaHoraLlegada = DateTime.Parse(Console.ReadLine());
        Console.Write("Nombre del piloto: ");
        nuevoVuelo.Piloto = Console.ReadLine();
        Console.Write("Nombre del copiloto: ");
        nuevoVuelo.Copiloto = Console.ReadLine();
        Console.Write("Capacidad máxima: ");
        nuevoVuelo.CapacidadMaxima = int.Parse(Console.ReadLine());
        aerolinea.Vuelos.Add(nuevoVuelo);
    }

    public static void RegistrarPasajeros(AerolineaData aerolinea)
    {
        Console.Write("Código de vuelo: ");
        string codigo = Console.ReadLine();
        Console.Write("Cantidad de pasajeros: ");
        int cantidad = int.Parse(Console.ReadLine());
        Vuelo vuelo = aerolinea.Vuelos.FirstOrDefault(vuelo => vuelo.CodigoVuelo == codigo);
        if (vuelo != null)
        {
            vuelo.PasajerosRegistrados += cantidad;
        }
        else
        {
            Console.WriteLine("Vuelo no encontrado.");
        }
    }

    public static double CalcularOcupacionMedia(AerolineaData aerolinea)
    {
        if (aerolinea.Vuelos.Count == 0) return 0;
        return aerolinea.Vuelos.Average(v => v.PorcentajeOcupacion());
    }

    public static Vuelo ObtenerVueloMayorOcupacion(AerolineaData aerolinea)
    {
        return aerolinea.Vuelos.OrderByDescending(vuelo => vuelo.PorcentajeOcupacion()).FirstOrDefault();
    }

    public static void BuscarVuelo(AerolineaData aerolinea)
    {
        Console.Write("Código de vuelo: ");
        string codigoBuscar = Console.ReadLine();
        Vuelo vueloBuscado = aerolinea.Vuelos.FirstOrDefault(vuelo => vuelo.CodigoVuelo == codigoBuscar);
        if (vueloBuscado != null)
        {
            Console.WriteLine($"Detalles del vuelo: {vueloBuscado.CodigoVuelo}, Ocupación: {vueloBuscado.PorcentajeOcupacion():F2}%");
        }
        else
        {
            Console.WriteLine("Vuelo no encontrado.");
        }
    }

    public static void ListarVuelosOrdenadosPorOcupacion(AerolineaData aerolinea)
    {
        var vuelosOrdenados = aerolinea.Vuelos.OrderByDescending(vuelo => vuelo.PorcentajeOcupacion()).ToList();
        foreach (var vueloOrd in vuelosOrdenados)
        {
            Console.WriteLine($"{vueloOrd.CodigoVuelo} - Ocupación: {vueloOrd.PorcentajeOcupacion():F2}%");
        }
    }
}