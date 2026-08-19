using System.Windows;
using ClaammApp.Domain.Entities;

namespace ClaammApp.UI.Views;

public partial class ConfiguracionWindow : Window
{
    public ConfiguracionWindow()
    {
        InitializeComponent();
        Ventanas.AjustarAlAreaTrabajo(this);

        var empresa = Composicion.Configuracion.ObtenerEmpresa();
        TxtResponsable.Text = empresa.Responsable;
        TxtCuit.Text = empresa.Cuit;
        TxtDireccion.Text = empresa.Direccion;
        TxtUbicacion.Text = empresa.Ubicacion;
        TxtTelefono.Text = empresa.Telefono;
        TxtEmail.Text = empresa.Email;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        var empresa = new ConfiguracionEmpresa
        {
            Responsable = TxtResponsable.Text.Trim(),
            Cuit = TxtCuit.Text.Trim(),
            Direccion = TxtDireccion.Text.Trim(),
            Ubicacion = TxtUbicacion.Text.Trim(),
            Telefono = TxtTelefono.Text.Trim(),
            Email = TxtEmail.Text.Trim()
        };

        Composicion.Configuracion.GuardarEmpresa(empresa);
        DialogResult = true;
    }
}
