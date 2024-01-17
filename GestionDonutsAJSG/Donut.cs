using System.Collections.Generic;
using System.IO;
using System.Text;
using Avalonia.Media.Imaging;

namespace GestionDonutsAJSG;

public class Donut
{
    public Donut()
    {
    }

    public Donut(int id, bool inStock, string nombre, Bitmap imagen, double precio, char calidad)
    {
        this.id = id;
        this.inStock = inStock;
        this.nombre = nombre;

        //string absolutePath = Path.Combine(Directory.GetCurrentDirectory(), relativeImagePath);
        //byte[] imageBytes = File.ReadAllBytes(absolutePath);

        this.imagen = imagen;
        this.precio = precio;
        this.calidad = calidad;
    }

    // los atributos tienen que ser publicos, ya que si no, no se puede acceder a los setters
    public int id { get; set; }
    public bool inStock { get; set; }
    public string nombre { get; set; }

    public Bitmap imagen { get; set; }
    public double precio { get; set; }
    public char calidad { get; set; }

    public override string ToString()
    {
        return $"ID: {id} InStock: {inStock} Nombre: {nombre}, Precio: {precio}, Calidad: {calidad}";
    }

    public void SaveToFile(string filePath, List<Donut> lista)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        using (var writer = new BinaryWriter(fileStream))
        {
            // Escribe el numero de Donuts
            writer.Write(lista.Count);

            // Escribe cada donut de la lista
            foreach (var Donut in lista)
            {
                writer.Write(Donut.id);
                writer.Write(Donut.inStock);

                // Escribe la longitud del string y luego el propio string
                var nameBytes = Encoding.UTF8.GetBytes(Donut.nombre);
                writer.Write(nameBytes.Length);
                writer.Write(nameBytes);

                var imageBytes = AvaloniaBitmapToByteArray(Donut.imagen);
                writer.Write(imageBytes.Length);
                writer.Write(imageBytes);

                writer.Write(Donut.precio);
                writer.Write(Donut.calidad);
            }
        }
    }

    private static byte[] AvaloniaBitmapToByteArray(Bitmap avaloniaBitmap)
    {
        using (var stream = new MemoryStream())
        {
            avaloniaBitmap.Save(stream);
            return stream.ToArray();
        }
    }

    public static List<Donut> LoadFromFile(string filePath)
    {
        var loadedDonuts = new List<Donut>();
        using (var fileStream = new FileStream(filePath, FileMode.Open))
        using (var reader = new BinaryReader(fileStream))
        {
            // Leemos el numero de donuts
            var donutCount = reader.ReadInt32();

            // Leemos cada donut del fichero
            for (var i = 0; i < donutCount; i++)
            {
                var loadedDonut = new Donut();
                loadedDonut.id = reader.ReadInt32();
                loadedDonut.inStock = reader.ReadBoolean();

                // Lee la longitud del string y luego el propio string
                var nombreLength = reader.ReadInt32();
                var nombreBytes = reader.ReadBytes(nombreLength);
                loadedDonut.nombre = Encoding.UTF8.GetString(nombreBytes);

                var imagenLength = reader.ReadInt32();
                var imagenBytes = reader.ReadBytes(imagenLength);
                loadedDonut.imagen = ByteArrayToAvaloniaBitmap(imagenBytes);

                loadedDonut.precio = reader.ReadDouble();
                loadedDonut.calidad = reader.ReadChar();

                loadedDonuts.Add(loadedDonut);
            }
        }

        return loadedDonuts;
    }

    private static Bitmap ByteArrayToAvaloniaBitmap(byte[] byteArray)
    {
        using (var stream = new MemoryStream(byteArray))
        {
            return new Bitmap(stream);
        }
    }
}