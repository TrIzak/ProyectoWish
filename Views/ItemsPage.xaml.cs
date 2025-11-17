using ProyectoWishList.Data;
using ProyectoWishList.Models;


namespace ProyectoWishList.Views;

[QueryProperty(nameof(WishlistId), "WishlistId")]
public partial class ItemsPage : ContentPage
{
    private int _wishlistId;
    public int WishlistId
    {
        get => _wishlistId;
        set
        {
            _wishlistId = value;
            // Cargar los artículos cuando el ID es recibido
            Task.Run(() => CargarItems());
        }
    }

    public ItemsPage()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarItems();
    }

    private async Task CargarItems()
    {
        if (WishlistId == 0) return;

        try
        {
            // Opcional: Cargar el nombre de la Wishlist para el título
            var wishlist = await WishlistService.ObtenerWishlistPorId(WishlistId); // Necesitarás este método
            if (wishlist != null)
            {
                // Actualizar la UI (debe hacerse en el hilo principal)
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    lblListaNombre.Text = wishlist.Name;
                });
            }

            // Cargar los items
            var items = await ItemService.ObtenerItems(WishlistId); // El método que ya creaste

            MainThread.BeginInvokeOnMainThread(() =>
            {
                itemsView.ItemsSource = items;
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
                DisplayAlert("Error", $"No se pudieron cargar los artículos: {ex.Message}", "OK"));
        }
    }

    private async void btnAgregarItem_Clicked(object sender, EventArgs e)
    {
        // Navegamos a la página de detalle, pasando el ID de la lista
        // para que el nuevo artículo sepa a qué lista pertenece.
        await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?WishlistId={WishlistId}");
    }

    private async void OnEditarItem(object sender, EventArgs e)
    {
        if (sender is Button boton && boton.CommandParameter is Item item)
        {
            // Navegamos a la página de detalle para *editar*
            // Pasamos tanto el ItemId como el WishlistId
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?ItemId={item.ItemId}&WishlistId={WishlistId}");
        }
    }

    private async void OnEliminarItem(object sender, EventArgs e)
    {
        if (sender is Button boton && boton.CommandParameter is Item item)
        {
            bool confirmar = await DisplayAlert("Confirmar", $"¿Eliminar '{item.Name}'?", "Sí", "No");
            if (!confirmar) return;

            try
            {
                bool exito = await ItemService.EliminarItem(item); // Asumo que creaste este método
                if (exito)
                {
                    await CargarItems(); // Recargar la lista
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

    }}