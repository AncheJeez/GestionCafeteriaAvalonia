using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;

namespace GestionDonutsAJSG;

public partial class MainWindow : Window
{
    //@juangarmun
    /*
     * TODO:
     * A la imagen le paso un BitMap, que viene de un Array de Bits de la imagen.
     * Guardamos a cada donut un array de bits, pillando la ruta.
     * ImagenAMostrar.Source = Bitmap.FromStream();
     *
     *
     */

    private bool create_state;
    private bool delete_state;
    private bool exit_state;
    private bool fileChooserActivated = false;
    private int index;
    private string last_file_chosen = "";

    private List<Donut> listaDeDonuts;

    public MainWindow()
    {
        InitializeComponent();
        emptyBoxes();
        listaDeDonuts = ObtenerListaDeDonuts();
        PanelDialogo.IsVisible = false;
        // con estas dos lineas hacemos el setup de los botones de forma base
        BtnAnterior.IsEnabled = false;
        BtnSiguiente.IsEnabled = false;
        if (listaDeDonuts.Count > 0) BtnSiguiente.IsEnabled = true;

        setNumberDonutsBox(listaDeDonuts.Count);
        fillBoxes(listaDeDonuts[index]);
    }

    public List<Donut> ObtenerListaDeDonuts()
    {
        // metodo para meter datos predeterminados a la lista


        var donutsToSave = new List<Donut>
        {
            new(1, true, "Fondat", fromPathToBitMap("Donuts\\donuts_choco1.png"), 1.20, 'B'),
            new(2, false, "Glacé", fromPathToBitMap("Donuts\\donuts_classic1.png"), 1.80, 'A'),
            new(3, true, "Bombón", fromPathToBitMap("Donuts\\donuts_foundan2.png"), 3.50, 'S'),
            new(4, false, "Nocilla", fromPathToBitMap("Donuts\\donuts_nocilla.png"), 1.10, 'C'),
            new(5, true, "Galleta cacao", fromPathToBitMap("Donuts\\galleta_cacao.png"), 2.80, 'S'),
            new(6, true, "Galleta fina", fromPathToBitMap("Donuts\\galleta_fina.png"), 0.50, 'D'),
            new(7, false, "Galleta rellena", fromPathToBitMap("Donuts\\galleta_rellena.png"), 2.00, 'A')
        };
        return donutsToSave;
    }

    public void emptyBoxes()
    {
        txBxId.Text = "";
        txBxNombre.Text = "";
        txBxPrecio.Text = "";
        txBxCalidad.Text = "";
        chBxStock.IsChecked = false;
        Image.Source = fromPathToBitMap("Donuts\\\\default.png");
    }

    public void fillBoxes(Donut donut)
    {
        txBxId.Text = donut.id.ToString();
        txBxNombre.Text = donut.nombre;
        txBxPrecio.Text = donut.precio.ToString();
        txBxCalidad.Text = donut.calidad.ToString();
        chBxStock.IsChecked = donut.inStock;
        Image.Source = donut.imagen;
    }

    public void setNumberDonutsBox(int num)
    {
        txBoxNumDonuts.Text = num.ToString();
    }

    public Bitmap fromPathToBitMap(string relativeImagePath)
    {
        var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), relativeImagePath);
        var imageBytes = File.ReadAllBytes(absolutePath);
        return arrayBitmap(imageBytes);
    }

    public Bitmap arrayBitmap(byte[] arr)
    {
        using (var ms = new MemoryStream(arr))
        {
            var img = new Bitmap(ms);
            return img;
        }
    }

    public void editableButtons(bool editable)
    {
        if (editable)
        {
            if (index + 1 >= listaDeDonuts.Count)
                BtnSiguiente.IsEnabled = false;
            else
                BtnSiguiente.IsEnabled = true;

            if (index <= 0)
                BtnAnterior.IsEnabled = false;
            else
                BtnAnterior.IsEnabled = true;
        }
        else
        {
            BtnSiguiente.IsEnabled = false;
            BtnAnterior.IsEnabled = false;
        }

        BtnCrear.IsEnabled = editable;
        BtnModificar.IsEnabled = editable;
        BtnEliminar.IsEnabled = editable;
        BtnGuardar.IsEnabled = editable;
        BtnCargar.IsEnabled = editable;
        btnFileChooser.IsEnabled = editable;
    }

    public void setUpDialog(bool doWePlaceCancelButn)
    {
        if (doWePlaceCancelButn)
            BtnCancelar.IsVisible = true;
        else
            BtnCancelar.IsVisible = false;
        PanelDialogo.IsVisible = true;
        editableButtons(false);
    }

    public void btnSiguienteClick(object sender, RoutedEventArgs args)
    {
        if (index < listaDeDonuts.Count)
        {
            index += 1;
            fillBoxes(listaDeDonuts[index]);
            BtnAnterior.IsEnabled = true;
            last_file_chosen = "";
            if (index + 1 >= listaDeDonuts.Count) BtnSiguiente.IsEnabled = false;
        }
    }

    public void btnAnteriorClick(object sender, RoutedEventArgs args)
    {
        if (index > 0)
        {
            index -= 1;
            fillBoxes(listaDeDonuts[index]);
            BtnSiguiente.IsEnabled = true;
            last_file_chosen = "";
            if (index <= 0) BtnAnterior.IsEnabled = false;
        }
    }

    public void btnCrear(object sender, RoutedEventArgs args)
    {
        if (!create_state)
        {
            create_state = true;
            last_file_chosen = "";
            setUpDialog(true);
            LblDialogo.Content = "¿Desea crear un nuevo Donut?";
        }
        else
        {
            var oldId = index + 1;
            double oldPrecio = 0;
            var oldCalidad = 'Z';

            // recogemos los nuevos datos, si es erroneo el dato, se pone uno por defecto
            var Id = int.TryParse(txBxId.Text, out var value) ? value : oldId;
            var Nombre = txBxNombre.Text;
            var Precio = double.TryParse(txBxPrecio.Text, out var val) ? val : oldPrecio;
            var Calidad = char.TryParse(txBxCalidad.Text, out var valu) ? valu : oldCalidad;
            var Stock = chBxStock.IsChecked.GetValueOrDefault();
            Bitmap imagen;

            if (!last_file_chosen.Equals(""))
            {
                imagen = fromPathToBitMap(last_file_chosen);
                last_file_chosen = "";
            }
            else
            {
                imagen = fromPathToBitMap("Donuts\\\\default.png");
            }

            listaDeDonuts.Add(new Donut(Id, Stock, Nombre, imagen, Precio, Calidad));
            setNumberDonutsBox(listaDeDonuts.Count);

            create_state = false;
            exit_state = true;
            setUpDialog(false);
            LblDialogo.Content = "Nuevo Donut creado";
        }
    }

    public void btnModificar(object sender, RoutedEventArgs args)
    {
        var oldId = index + 1;
        double oldPrecio = 0;
        var oldCalidad = 'Z';

        // recogemos los nuevos datos, si es erroneo el dato, se pone uno por defecto
        var Id = int.TryParse(txBxId.Text, out var value) ? value : oldId;
        var Nombre = txBxNombre.Text;
        var Precio = double.TryParse(txBxPrecio.Text, out var val) ? val : oldPrecio;
        var Calidad = char.TryParse(txBxCalidad.Text, out var valu) ? valu : oldCalidad;
        var Stock = chBxStock.IsChecked.GetValueOrDefault();

        // introducimos los nuevos datos
        listaDeDonuts[index].id = Id;
        listaDeDonuts[index].nombre = Nombre;
        listaDeDonuts[index].precio = Precio;
        listaDeDonuts[index].calidad = Calidad;
        listaDeDonuts[index].inStock = Stock;

        //listaDeDonuts[index].imagen = Image.Source;
        if (!last_file_chosen.Equals(""))
        {
            listaDeDonuts[index].imagen = fromPathToBitMap(last_file_chosen);
            last_file_chosen = "";
        }

        exit_state = true;
        setUpDialog(false);
        LblDialogo.Content = $"Se ha modificado el donut de Id:{listaDeDonuts[index].id} ";

        fillBoxes(listaDeDonuts[0]);
        index = 0;
    }

    public void btnEliminar(object sender, RoutedEventArgs args)
    {
        if (listaDeDonuts.Count <= 0)
        {
            setUpDialog(false);
            LblDialogo.Content = "No hay donuts para eliminar";
        }
        else
        {
            setUpDialog(true);
            LblDialogo.Content = $"¿Desea eliminar el Donut de Id:{listaDeDonuts[index].id} ";
            delete_state = true;
        }
    }

    public async void btnGuardar(object sender, RoutedEventArgs args)
    {
        if (listaDeDonuts == null || listaDeDonuts.Count == 0)
        {
            exit_state = true;
            setUpDialog(false);
            LblDialogo.Content = "No hay donuts para guardar";
        }
        else
        {
            // Save the donuts to the selected file
            var dlg = new SaveFileDialog();
            var result = await dlg.ShowAsync(this);
            var path = string.Join("", result); // con esto convertimos string[]? a string

            var donutInstance = new Donut(); // es necesario instanciarlo, por si esta vacio
            donutInstance.SaveToFile(path, listaDeDonuts);
        }
    }

    public async void btnCargar(object sender, RoutedEventArgs args)
    {
        var dlg = new OpenFileDialog();
        var result = await dlg.ShowAsync(this);
        var path = string.Join("", result); // con esto convertimos string[]? a string
        listaDeDonuts = Donut.LoadFromFile(path);
        index = 0;
        fillBoxes(listaDeDonuts[0]);
        setNumberDonutsBox(listaDeDonuts.Count);
        editableButtons(true);
    }

    public async void fileChooser(object sender, RoutedEventArgs args)
    {
        var dlg = new OpenFileDialog();
        dlg.Filters!.Add(new FileDialogFilter { Name = "Archivos de imagen png", Extensions = { "png" } });
        dlg.Filters!.Add(new FileDialogFilter { Name = "Archivos de texto", Extensions = { "txt" } });
        dlg.Filters!.Add(new FileDialogFilter { Name = "Archivos de audio", Extensions = { "wav" } });
        dlg.Filters!.Add(new FileDialogFilter { Name = "Todos los archivos", Extensions = { "*" } });
        dlg.AllowMultiple = false;

        var result = await dlg.ShowAsync(this);
        if (result != null)
        {
            //LblRuta.Content = result[0];
            //Image.Source = donut.imagen;
            last_file_chosen = result[0];
            Image.Source = fromPathToBitMap(result[0]);
        }
    }

    public void btnAceptar(object sender, RoutedEventArgs args)
    {
        // BORRAR
        if (delete_state)
        {
            listaDeDonuts.RemoveAt(index);
            // volvemos a mostrar desde el principio
            if (listaDeDonuts.Count > 0)
            {
                fillBoxes(listaDeDonuts[0]);
                BtnSiguiente.IsEnabled = true;
            }
            else
            {
                emptyBoxes();
            }

            setNumberDonutsBox(listaDeDonuts.Count);
            index = 0;

            PanelDialogo.IsVisible = false;
            editableButtons(true);
            delete_state = false;
        }

        if (create_state)
        {
            BtnCrear.IsEnabled = true;
            btnFileChooser.IsEnabled = true;
            BtnModificar.IsEnabled = false;
            BtnEliminar.IsEnabled = false;
            BtnGuardar.IsEnabled = false;
            BtnCargar.IsEnabled = false;
            emptyBoxes();
            // asignamos cuales van a ser visibles y cuales no


            PanelDialogo.IsVisible = false;
        }

        if (exit_state)
        {
            fillBoxes(listaDeDonuts[0]);
            BtnSiguiente.IsEnabled = true;

            PanelDialogo.IsVisible = false;
            editableButtons(true);
            exit_state = false;
        }
    }

    public void btnCancelar(object sender, RoutedEventArgs args)
    {
        PanelDialogo.IsVisible = false;
        editableButtons(true);
    }
}