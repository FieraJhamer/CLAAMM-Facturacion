using System.Windows;
using ClammApp.UI.Views;

namespace ClammApp.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MostrarPresupuestos();
    }

    private void BtnPresupuestos_Click(object sender, RoutedEventArgs e) => MostrarPresupuestos();

    private void BtnItems_Click(object sender, RoutedEventArgs e) => MostrarItems();

    private void BtnRubros_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new RubrosWindow { Owner = this };
        ventana.ShowDialog();
    }

    private void BtnConfig_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new ConfiguracionWindow { Owner = this };
        ventana.ShowDialog();
    }

    private void MostrarPresupuestos()
    {
        Contenido.Content = new PresupuestosView();
    }

    private void MostrarItems()
    {
        Contenido.Content = new ItemsView();
    }
}
