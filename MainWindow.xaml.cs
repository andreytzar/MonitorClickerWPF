using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MonitorClickerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private CancellationToken _tokenRecMouse;
        private CancellationToken _tokenMainTask;

        public MainWindow()
        {
            InitializeComponent();
            _tokenRecMouse = cancelTokenSource.Token;
            _tokenMainTask = cancelTokenSource.Token;
            Context.Load();
            RefreshListMouseRecs();
            this.DataContext = Context.Settings;
            RecsList.ItemsSource = Context.Settings.Points;
            StartStopMainTask();
        }
        private void StartStopTask_Click(object sender, RoutedEventArgs e)
        {
            StartStopMainTask();
        }
        bool _maintaskrun = false;
        public async void StartStopMainTask()
        {
            if (_maintaskrun)
            {
                _maintaskrun = false;
                await Task.Delay(1000, _tokenMainTask);
                StartStopTask.Content = "START Task";
                return;
            }
            StartStopTask.Content = "STOP Task";
            _maintaskrun = true;
            await MainTask();
        }

        public async Task MainTask()
        {
            var NextStart = DateTime.Now.AddMinutes(Context.Settings.ClickAveryMin);
            while (_maintaskrun && !_tokenMainTask.IsCancellationRequested)
            {
                if (NextStart < DateTime.Now)
                {
                    NextStart = DateTime.Now.AddMinutes(Context.Settings.ClickAveryMin);
                    await Clicker();
                }
                try
                {
                    await Task.Delay(1000, _tokenMainTask);
                    var lefttime = NextStart.Subtract(DateTime.Now);
                    Dispatcher.Invoke(() =>
                    {
                        LBLTimeLeft.Content = $"Time left for Next Clicking secs: {(int)lefttime.TotalSeconds}";
                    });
                }
                catch (Exception ex) { var e = ex; _maintaskrun = false; return; }
            }
        }

        public async Task Clicker()
        {
            foreach (var p in Context.Settings.Points)
            {
                user32imports.SendLeftClick(p);
                try
                {
                    await Task.Delay(Context.Settings.ClickDelay, _tokenMainTask);
                }
                catch (Exception ex) { var e = ex; return; }
            }
        }


        bool _recmouse = false;
        private async void StartRecMouse_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(500, _tokenRecMouse);
            if (_recmouse) { _recmouse = false; await Task.Delay(500, _tokenRecMouse); }
            _recmouse = true;
            await RecordCliclMouse();
        }

        private void RefreshListMouseRecs()
        {
            Dispatcher.Invoke(() =>
            {
                if (Context.Settings.Points.Count > 0)
                    RecsList.ScrollIntoView(Context.Settings.Points[Context.Settings.Points.Count - 1]);
            });
        }

        public async Task RecordCliclMouse()
        {
            while (!_tokenMainTask.IsCancellationRequested && _recmouse)
            {
                if (user32imports.IsLeftMouseBtnPressed())
                {
                    while (!_tokenMainTask.IsCancellationRequested && _recmouse && user32imports.IsLeftMouseBtnPressed())
                    {
                        await Task.Delay(30);
                    }
                    Context.Settings.Points.Add(user32imports.GetCurrentMousePosition());
                    RefreshListMouseRecs();
                }
                await Task.Delay(20);
            }
        }
        private async void StopRecMouse_Click(object sender, RoutedEventArgs e)
        {
            _recmouse = false;
            try
            {
                await Task.Delay(500, _tokenMainTask);
            }
            catch (Exception ex) { return; }
            if (Context.Settings.Points.Count > 0)
                Context.Settings.Points.RemoveAt(Context.Settings.Points.Count - 1);
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cancelTokenSource.Cancel();
            Context.Save();
            await Task.Delay(500);
        }

        private void ClearRecMouse_Click(object sender, RoutedEventArgs e) =>
            Context.Settings.Points.Clear();


        private void SaverRecMouse_Click(object sender, RoutedEventArgs e) =>
            Context.Save();

    }
}