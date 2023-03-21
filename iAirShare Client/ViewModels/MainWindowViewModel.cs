using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using iAirShare_Client.iAirShare;
using iAirShare_Client.Models;

namespace iAirShare_Client.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ItemsSourceView myListView = new ItemsSourceView(MainWindowModel.files);

    public ObservableCollection<ASFile> FileList
    {
        get => MainWindowModel.files;
        set
        {
            MainWindowModel.files = value;
            RaiseEvent();
        }
    }

    public ItemsSourceView MyFileList
    {
        get => myListView;
        set
        {
            myListView = value;
            RaiseEvent();
        }
    }

    public void SortByName()
    {
        MyFileList = new ItemsSourceView(FileList.OrderBy(p => p.file_name));
    }

    public void SortBySize()
    {
        MyFileList = new ItemsSourceView(FileList.OrderBy(p => p.file_property?.file_size));
    }
    

    public void SortByTime()
    {
        MyFileList = new ItemsSourceView(FileList.OrderBy(p => p.file_property?.last_update));
    }
}