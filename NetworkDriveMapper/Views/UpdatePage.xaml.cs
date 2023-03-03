namespace NetworkDriveMapper.Views;

public partial class UpdatePage : ContentPage
{
	public UpdatePage(UpdateViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}