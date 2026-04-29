using Avalonia.Controls;
using System.Collections.Specialized;
using Avalonia.Threading; // 이 줄이 새로 추가되었습니다!
using csharp.ViewModels;

namespace csharp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        this.DataContextChanged += (sender, e) =>
        {
            if (this.DataContext is MainWindowViewModel vm)
            {
                vm.LogMessages.CollectionChanged += (s, args) =>
                {
                    if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            LogListBox.ScrollIntoView(args.NewItems[0]);
                        });
                    }
                };
            }
        };
    }
}