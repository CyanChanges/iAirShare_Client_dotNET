using System.Collections.Generic;
using System.Collections.ObjectModel;
using iAirShare_Client.iAirShare;

namespace iAirShare_Client.Models;

public static class MainWindowModel
{
    public static ObservableCollection<ASFile> files = new();
}