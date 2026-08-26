using System.Windows;
using ClaammApp.Domain.Entities;

namespace ClaammApp.UI.Views;

public partial class ConfiguracionWindow : Window
{
    private string _responsableOriginal = string.Empty;
    private string _cuitOriginal = string.Empty;
    private string _direccionOriginal = string.Empty;
    private string _ubicacionOriginal = string.Empty;
    private string _telefonoOriginal = string.Empty;
    private string _emailOriginal = string.Empty;

    public ConfiguracionWindow()
    {
        InitializeComponent();
        Ventanas.AjustarAlAreaTrabajo(this);
        Ventanas.HabilitarCierreConEscape(this);

        var empresa = Composicion.Configuracion.ObtenerEmpresa();
        TxtResponsable.Text = empresa.Responsable;
        TxtCuit.Text = empresa.Cuit;
        TxtDireccion.Text = empresa.Direccion;
        TxtUbicacion.Text = empresa.Ubicacion;
        TxtTelefono.Text = empresa.Telefono;
        TxtEmail.Text = empresa.Email;

        _responsableOriginal = TxtResponsable.Text;
        _cuitOriginal = TxtCuit.Text;
        _direccionOriginal = TxtDireccion.Text;
        _ubicacionOriginal = TxtUbicacion.Text;
        _telefonoOriginal = TxtTelefono.Text;
        _emailOriginal = TxtEmail.Text;

        Ventanas.ConfirmarSalidaSinGuardar(this, HayCambiosSinGuardar);
    }

    private bool HayCambiosSinGuardar()
        => TxtResponsable.Text != _responsableOriginal
           || TxtCuit.Text != _cuitOriginal
           || TxtDireccion.Text != _direccionOriginal
           || TxtUbicacion.Text != _ubicacionOriginal
           || TxtTelefono.Text != _telefonoOriginal
           || TxtEmail.Text != _emailOriginal;

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
