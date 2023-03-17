using SQLite;
using System.ComponentModel.DataAnnotations;

namespace NetworkDriveMapper.Models;

public partial class DriveModel : ObservableValidator
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [ObservableProperty] 
    private string _letter;

    [ObservableProperty] 
    private string _address;

    [ObservableProperty] 
    private string _driveName;

    [ObservableProperty] 
    private string _password;

    [ObservableProperty]
    private string _userName;

    [ObservableProperty]
    private bool _isConnected = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MyButtonColorAsColor))]

    private string _buttonColor = "#FF0000";

    public Color MyButtonColorAsColor => Color.FromArgb(ButtonColor);
}
