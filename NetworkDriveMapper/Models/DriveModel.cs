﻿using SQLite;
using System.ComponentModel.DataAnnotations;

namespace NetworkDriveMapper.Models;

public partial class DriveModel : ObservableValidator
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    private string _letter;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    private string _address;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    private string _driveName;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    private string _password;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    private string _userName;

    [ObservableProperty]
    private bool _isConnected = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MyButtonColorAsColor))]

    private string _buttonColor = "#FF0000";

    public Color MyButtonColorAsColor => Color.FromArgb(ButtonColor);
}
