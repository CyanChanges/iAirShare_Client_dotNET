using System.Collections.Generic;
using iAirShare_Client.iAirShare;
using iAirShare_Client.Models;

namespace iAirShare_Client.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public List<ASFile> FileList
    {
        get => MainWindowModel.FileList;
        set
        {
            MainWindowModel.FileList = value;
            RaiseEvent();
        }
    }
}