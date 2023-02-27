using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Semaphore_Practic;


public partial class MainWindow : Window
{


    Thread Cthread;
    public Semaphore semaphore { get; set; }
    public int countSemaphore { get; set; }
    public int count = 1;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        semaphore = new Semaphore(1, 1, "SEMAPHORE");

        for (int i = 0; i < 3; i++)
        {
            Cthread = new Thread(() => Simulation(semaphore));
            ++count;
            Cthread.Name = $"Thread -> {count}";
            Dispatcher.Invoke(() => CreatedListbox.Items.Add(Cthread));
        }
    }


    private void Simulation(Semaphore semaphore)
    {
        bool st = false;
        while (!st)
        {
            if (semaphore.WaitOne(1))
            {
                try
                {
                    var t = Thread.CurrentThread;
                    Dispatcher.Invoke(() => WorkingListbox.Items.Add(t));
                    Dispatcher.Invoke(() => WaitingListbox.Items.Remove(t));
                    Thread.Sleep(3000);
                }
                finally
                {
                    st = true;
                    var t = Thread.CurrentThread;

                    Dispatcher.Invoke(() =>
                    {
                        WorkingListbox.Items.Remove(t);
                        WaitingListbox.Items.Remove(t);
                    });

                    semaphore.Release();
                }
            }
            else
            {
                var t = Thread.CurrentThread;

                Dispatcher.Invoke(() =>
                {
                    if (!WaitingListbox.Items.Contains(t))
                        WaitingListbox.Items.Add(t);
                });
            }
        }
    }

    private void CreatedListbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        try
        {
            var t = CreatedListbox.SelectedItem as Thread;
            t?.Start();
            CreatedListbox.Items.Remove(t);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
