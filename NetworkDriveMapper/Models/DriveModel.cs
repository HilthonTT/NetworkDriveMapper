using SQLite;

namespace NetworkDriveMapper.Models;

public class DriveModel : INotifyPropertyChanged
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Letter { get; set; }
    public string Address { get; set; }
    public string DriveName { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
    public bool IsConnected { get; set; } = false;

    private string _buttonColor;
    public string ButtonColor
    {
        get { return _buttonColor; }
        set 
        {
            if (_buttonColor != value)
            {
                _buttonColor = value;
                NotifyPropertyChanged(nameof(ButtonColor));
                NotifyPropertyChanged(nameof(MyButtonColorAsColor));
            } 
        }
    }

    public Color MyButtonColorAsColor => Color.FromArgb(ButtonColor);

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
