namespace NetworkDriveMapper.Views;

public partial class AddDrivePage : ContentPage
{
	public AddDrivePage(AddViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}