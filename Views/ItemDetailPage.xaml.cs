using System.Globalization;
using ProyectoWishList.Data;
using ProyectoWishList.Models;
namespace ProyectoWishList.Views;


[QueryProperty(nameof(WishlistId), "WishlistId")]
[QueryProperty(nameof(ItemId), "ItemId")]
public partial class ItemDetailPage : ContentPage
{
    

    private int _wishlistId;
    private int _itemId;
    private bool _isEditMode = false;

    public int WishlistId
    {
        get => _wishlistId;
        set => _wishlistId = value;
    }

    public int ItemId
    {
        get => _itemId;
        set
        {
            _itemId = value;
            if (_itemId > 0)
            {
                _isEditMode = true;
                // Cargar datos del item para editar
                Task.Run(() => CargarDatosItem());
            }
        }
    }
    public ItemDetailPage()
	{
		InitializeComponent();
        pickerPriority.SelectedItem = 2;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Cargar el picker de Tiendas
        await CargarPickers();
    }

    private async Task CargarPickers()
    {
        try
        {
            var tiendas = await StoreService.ObtenerStores(); // Asumo que creaste este método
            pickerStore.ItemsSource = tiendas;
            pickerStore.ItemDisplayBinding = new Binding("Name");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se cargaron las tiendas: {ex.Message}", "OK");
        }
        // Aquí también cargarías el picker de Categorías si fuera necesario
    }

    private async Task CargarDatosItem()
    {
        try
        {
            var item = await ItemService.ObtenerItemPorId(ItemId); // Necesitarás este método
            if (item == null) return;

            // Esperar a que los pickers se carguen
            await CargarPickers();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Title = "Editar Artículo";
                txtProductURL.Text = item.ProductURL;
                txtItemName.Text = item.Name;
                txtPrice.Text = item.Price?.ToString("N2", CultureInfo.InvariantCulture) ?? string.Empty;
                txtImageURL.Text = item.ImageURL;
                pickerPriority.SelectedItem = item.Priority;

                // Seleccionar la tienda en el Picker
                if (item.StoreId.HasValue && pickerStore.ItemsSource is List<Store> tiendas)
                {
                    pickerStore.SelectedItem = tiendas.FirstOrDefault(t => t.StoreId == item.StoreId.Value);
                }
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
                DisplayAlert("Error", $"Error al cargar item: {ex.Message}", "OK"));
        }
    }


    private async void btnGuardar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtItemName.Text) || WishlistId == 0)
        {
            await DisplayAlert("Alerta", "Nombre y Lista son requeridos.", "OK");
            return;
        }

        decimal? precio = null;
        if (decimal.TryParse(txtPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal p))
        {
            precio = p;
        }

        var item = new Item
        {
            ItemId = _itemId, // Será 0 si es nuevo
            WishlistId = _wishlistId,
            Name = txtItemName.Text,
            ProductURL = txtProductURL.Text,
            ImageURL = txtImageURL.Text,
            Price = precio,
            Priority = (int)pickerPriority.SelectedItem,
            StoreId = (pickerStore.SelectedItem as Store)?.StoreId
        };

        try
        {
            bool exito;
            if (_isEditMode)
            {
                exito = await ItemService.EditarItem(item);
            }
            else
            {
                exito = await ItemService.AgregarItem(item);
            }

            if (exito)
            {
                await Shell.Current.GoToAsync(".."); // Regresar a la página anterior
            }
            else
            {
                await DisplayAlert("Error", "No se pudo guardar el artículo.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void btnPegarYAnalizar_Clicked(object sender, EventArgs e)
    {
        // Esta es la lógica del PASO 6.
        // Lo implementaremos después.
        DisplayAlert("Pendiente", "La función de scraping se implementará aquí.", "OK");
    }
}