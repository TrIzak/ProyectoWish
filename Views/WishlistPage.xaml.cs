using ProyectoWishList.Data; // Tu namespace de servicios
using ProyectoWishList.Models;
namespace ProyectoWishList.Views;

public partial class WishlistPage : ContentPage
{
    private Wishlist wishlistSeleccionada;
    public WishlistPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarWishlists();
    }

    private async Task CargarWishlists()
    {
        try
        {
            var listas = await WishlistService.ObtenerWishlists();
            wishlistsView.ItemsSource = listas;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar las listas: {ex.Message}", "OK");
        }
    }

    private async void btnGuardarWishlist_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtWishlistName.Text))
        {
            await DisplayAlert("Alerta", "Por favor completa el nombre", "OK");
            return;
        }

        try
        {
            var nuevaLista = new Wishlist
            {
                Name = txtWishlistName.Text,
                Description = txtWishlistDesc.Text
            };
            bool exito;

            if (wishlistSeleccionada != null)
            {
                // Editar
                nuevaLista.WishlistId = wishlistSeleccionada.WishlistId;
                exito = await WishlistService.EditarWishlist(nuevaLista); // Asumo que creaste este método
                wishlistSeleccionada = null; // Limpiar selección
            }
            else
            {
                // Agregar
                exito = await WishlistService.AgregarWishlist(nuevaLista);
            }

            if (exito)
            {
                LimpiarFormulario();
                await CargarWishlists();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo guardar la lista", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OnEditarWishlist(object sender, EventArgs e)
    {
        if (sender is Button boton && boton.CommandParameter is Wishlist lista)
        {
            wishlistSeleccionada = lista;
            txtWishlistName.Text = lista.Name;
            txtWishlistDesc.Text = lista.Description;
        }
    }

    private async void OnEliminarWishlist(object sender, EventArgs e)
    {
        if (sender is Button boton && boton.CommandParameter is Wishlist lista)
        {
            bool confirmar = await DisplayAlert("Confirmar", $"¿Eliminar '{lista.Name}'?", "Sí", "No");
            if (!confirmar) return;

            try
            {
                bool exito = await WishlistService.EliminarWishlist(lista); // Asumo que creaste este método
                if (exito)
                {
                    await CargarWishlists();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo eliminar", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    // NAVEGACIÓN: Cuando el usuario toca un item de la lista
    private async void wishlistsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Wishlist lista)
            return;

        // Limpiar la selección
        wishlistsView.SelectedItem = null;

        // Navegar a la página de Items, pasando el ID de la lista
        await Shell.Current.GoToAsync($"{nameof(ItemsPage)}?WishlistId={lista.WishlistId}");
    }

    private void LimpiarFormulario()
    {
        txtWishlistName.Text = string.Empty;
        txtWishlistDesc.Text = string.Empty;
        wishlistSeleccionada = null;
    }
}