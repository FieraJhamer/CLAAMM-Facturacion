using System.IO;
using System.Windows;
using ClaammApp.Domain.Entities;
using Microsoft.Win32;

namespace ClaammApp.UI.Views;

public partial class ConfiguracionWindow : Window
{
    public ConfiguracionWindow()
    {
        InitializeComponent();
        Ventanas.AjustarAlAreaTrabajo(this);

        var empresa = Composicion.Configuracion.ObtenerEmpresa();
        TxtRazonSocial.Text = empresa.RazonSocial;
        TxtCuit.Text = empresa.Cuit;
        TxtDireccion.Text = empresa.Direccion;
        TxtTelefono.Text = empresa.Telefono;
        TxtEmail.Text = empresa.Email;
        TxtLogo.Text = empresa.LogoRuta;
    }

    private void BtnExaminarLogo_Click(object sender, RoutedEventArgs e)
    {
        var dialogo = new OpenFileDialog
        {
            Title = "Seleccionar logo",
            Filter = "Imágenes (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp"
        };

        if (dialogo.ShowDialog(this) == true)
            TxtLogo.Text = dialogo.FileName;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        var empresa = new ConfiguracionEmpresa
        {
            RazonSocial = TxtRazonSocial.Text.Trim(),
            Cuit = TxtCuit.Text.Trim(),
            Direccion = TxtDireccion.Text.Trim(),
            Telefono = TxtTelefono.Text.Trim(),
            Email = TxtEmail.Text.Trim(),
            LogoRuta = TxtLogo.Text.Trim()
        };

        if (!string.IsNullOrWhiteSpace(empresa.LogoRuta) && !File.Exists(empresa.LogoRuta))
        {
            MessageBox.Show(this, "No se encontró el archivo de logo seleccionado.", "CLAAMM", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Composicion.Configuracion.GuardarEmpresa(empresa);
        DialogResult = true;
    }
}
