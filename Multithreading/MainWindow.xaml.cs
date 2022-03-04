using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Multithreading
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        public BackgroundWorker backgroundWorker;

        public event PropertyChangedEventHandler PropertyChanged;

        public ulong label { get; set; }

        private bool pause;
        private bool reset;

        public MainWindow()
        {
            InitializeComponent();

            //Initialize defaults
            Initialize();
        }

        void Initialize()
        {
            //Initialize BackgroundWorker for running parallel thread
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += UpdateAsync;
            backgroundWorker.RunWorkerCompleted += UpdateAsyncCompleted;

            //Initialize Button states
            Play.IsEnabled = true;
            Pause.IsEnabled = false;
            Stop.IsEnabled = false;
            Reset.IsEnabled = false;
        }

        // Function running in Parallel thread
        private void UpdateAsync(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if(!backgroundWorker.CancellationPending)
                {
                    Update();
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        //Function running infinitely in Parallel thread
        private void Update()
        {
            label++;
            OnPropertyChanged(nameof(label));
            Thread.Sleep(10);
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Thread Busy");
            }

            //Update button states
            Play.IsEnabled = false;
            Pause.IsEnabled = true;
            Stop.IsEnabled = true;
            Reset.IsEnabled = true;
        }

        private void Pause_Button_Click(object sender, RoutedEventArgs e)
        {
            pause = true;
            if (!backgroundWorker.CancellationPending)
            {
                backgroundWorker.CancelAsync();
            }

            //Update button states
            Play.IsEnabled = true;
            Pause.IsEnabled = false;
            Stop.IsEnabled = true;
            Reset.IsEnabled = true;
        }

        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!backgroundWorker.CancellationPending)
            {
                backgroundWorker.CancelAsync();
            }
            label = 0;
            OnPropertyChanged(nameof(label));

            //Update button states
            Play.IsEnabled = true;
            Pause.IsEnabled = false;
            Stop.IsEnabled = false;
            Reset.IsEnabled = false;
        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            reset = true;
            if (!backgroundWorker.CancellationPending)
            {
                backgroundWorker.CancelAsync();
            }

            //Update button states
            Play.IsEnabled = false;
            Pause.IsEnabled = true;
            Stop.IsEnabled = true;
            Reset.IsEnabled = true;
        }

        private void UpdateAsyncCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled && reset==true)
            {
                reset = false;
                label = 0;
                OnPropertyChanged(nameof(label));
                backgroundWorker.RunWorkerAsync();
            }
            else if(e.Cancelled && pause==true)
            {
                pause = false;
            }
            else if(e.Cancelled)
            {
                label = 0;
                OnPropertyChanged(nameof(label));
            }
        }

        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
