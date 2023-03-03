namespace NetworkDriveMapper.View;

public partial class AddDrivePage : ContentPage
{
	public AddDrivePage(AddViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}