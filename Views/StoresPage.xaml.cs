using ProyectoWishList.Models;
namespace ProyectoWishList.Views;



public  class StoresPage : ContentPage
{
	public Store tiendaSeleccionada;
	public StoresPage()
	{
		InitializeComponent();
	}
    public async void CargarClientes()
    {
        try
        {
            var lista = await CRUDClientes.ObtenerCliente();
            clientesView.ItemsSource = lista;

        }
        catch (Exception ex)
        {

            await DisplayAlert("Error", $"No se Pudieron Cargar los Clientes {ex.Message}", "OK");
        }
    }


    public void OnEditarCliente(object sender, EventArgs e)
    {
        if (sender is Button boton && boton.CommandParameter is Cliente cliente)
        {
            clienteSeleccionado = cliente;
            txtNombre.Text = cliente.Nombre;
            txtCorreo.Text = cliente.Correo;
            txtTelefono.Text = cliente.Telefono;

        }
    }
    public async void OnEliminarCliente(object sender, EventArgs e)
    {
        if (sender is Button boton && boton.CommandParameter is Cliente cliente)
        {
            bool confirmar = await DisplayAlert("Confirmar", "¿Estas seguro?", "Si", "No");
            if (confirmar) return;
            try
            {
                bool exito = await CRUDClientes.EliminarCliente(cliente);
                if (exito)
                {
                    await DisplayAlert("Exito", "Se eliminó correctamente", "Ok");

                }
                else
                {
                    await DisplayAlert("Error", "No se eliminó correctamente", "Ok");

                }
                CargarClientes();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
    }

    private async void btnAgregarCliente_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
          string.IsNullOrWhiteSpace(txtCorreo.Text) ||
          string.IsNullOrWhiteSpace(txtTelefono.Text))
        {
            await DisplayAlert("Alerta", "Por favor completa todos los datos", "OK");
        }


        try
        {
            var nuevoCliente = new Cliente
            {
                Nombre = txtNombre.Text,
                Correo = txtCorreo.Text,
                Telefono = txtTelefono.Text
            };
            bool exito;
            if (clienteSeleccionado != null)
            {
                nuevoCliente.Id = clienteSeleccionado.Id;
                exito = await CRUDClientes.EditarCliente(nuevoCliente);
                if (exito)
                {
                    await DisplayAlert("Exito", "Cliente Actualizado Correctamente", "OK");
                }
                else
                {
                    await DisplayAlert("ERROR", "Cliente NO se pudo Actualizar", "OK");
                }
                clienteSeleccionado = null;
            }
            else
            {
                exito = await CRUDClientes.AgregarCliente(nuevoCliente);
                if (exito)
                {
                    await DisplayAlert("Exito", "Cliente Creado Correctamente", "OK");
                }
                else
                {
                    await DisplayAlert("ERROR", "Cliente NO se pudo Crear", "OK");
                }

            }
            CargarClientes();
        }
        catch (Exception)
        {

            throw;
        }
    }
    }