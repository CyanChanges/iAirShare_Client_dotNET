using System;
using System.Collections.ObjectModel;
using System.Net;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DynamicData.Kernel;
using iAirShare_Client.iAirShare;
using iAirShare_Client.Models;
using iAirShare_Client.ViewModels;
using Button = FluentAvalonia.UI.Controls.Button;

namespace iAirShare_Client.Views;

public partial class MainWindow : Window
{
    private readonly ASClient client;

    public MainWindow()
    {
        InitializeComponent();
        client = new ASClient(new Uri("http://127.0.0.1:10000/api"), "tests");
        var asFiles = client.ListDirectory();
        foreach (var asFile in asFiles)
            if (asFile.file_type != ASFileType.Null)
                MainWindowModel.files.Add(asFile);
    }

    private void InputElement_OnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (((sender as ListBox)!).SelectedIndex == -1) return;
        var selectedItem = (sender as ListBox)?.SelectedItem as ASFile?;
        if (selectedItem?.file_type == ASFileType.directory)
        {
            if (selectedItem?.file_name != null) client.ChangeDirectory(selectedItem?.file_name);
            else return;
            (DataContext as MainWindowViewModel)!.FileList = new ObservableCollection<ASFile>(client.ListDirectory());
        }
        else
        {
            // throw new NotImplementedException("Open File Not Implemented");

            var webClient = new WebClient();

            webClient.DownloadProgressChanged += (o, args) =>
            {
                Console.WriteLine($"Downloaded {args.BytesReceived} of {args.TotalBytesToReceive} " +
                                  $"({args.BytesReceived / args.TotalBytesToReceive})...");
            };
            webClient.DownloadDataCompleted += (o, args) =>
            {
                Console.WriteLine($"Download Complete!");
            };

            var fileName = ((sender as ListBox)!.SelectedItem as ASFile?)?.file_name;
            if (fileName != null)
                webClient.DownloadFileAsync(
                    this.client.GetFileUriBuilder(fileName).Uri,
                    fileName);
        }
    }

    private void InputElement_OnTapped(object? sender, RoutedEventArgs e)
    {
        (sender as Button)!.IsEnabled = false;
        (DataContext as MainWindowViewModel)!.FileList = new ObservableCollection<ASFile>(client.ListDirectory());
        (sender as Button)!.IsEnabled = true;
    }
}