using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace PacketHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _title = "Packet Helper";
        public ObservableCollection<KeyValuePair<string, string>> packets = new ObservableCollection<KeyValuePair<string, string>>();

        public MainWindow()
        {
            InitializeComponent();
            this.Title = _title;
            this.Loaded += Window_Loaded;
            this.ContentRendered += MainWindow_ContentRendered;
        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            packets.Add(new KeyValuePair<string, string>( "Empty", ""));
            cbox_packets.ItemsSource = packets;
            cbox_packets.DisplayMemberPath = "Key";
            cbox_packets.SelectedValuePath = "Value";
            cbox_packets.SelectedIndex = 0;            
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            LoadJson();
        }

        public void LoadJson()
        {
            if (File.Exists("packet.json")) {
                using (StreamReader r = new StreamReader("packet.json"))
                {
                    string json = r.ReadToEnd();
                    dynamic data = JsonConvert.DeserializeObject(json);
                    foreach (var s in data)
                    {
                        //packets.Add(s.Name, s.Value.ToString());
                        packets.Add(new KeyValuePair<string, string>(s.Name, s.Value.ToString()));
                    }
                }
            }
            
        }


        private void text_output_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetClipboard(text_output.Text);
            ToastMessage("Copied");
        }

        private void SetClipboard(string text)
        {
            Clipboard.SetText(text);
        }

        private void ToastMessage(string message)
        {
            label_message.Content = message;

            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, 1);
            DoubleAnimation animation = new DoubleAnimation();

            animation.From = 1.0;
            animation.To = 0.0;
            animation.Duration = new Duration(duration);
            Storyboard.SetTargetName(animation, label_message.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));
            storyboard.Children.Add(animation);

            storyboard.Begin(this);
        }

        private void rtxt_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = new TextRange(rtxt_input.Document.ContentStart, rtxt_input.Document.ContentEnd).Text.Trim('\r', '\n');
            text_output.Text = String.Format("@{0}@{1}#", text, text.Length);
        }

        private void Cbox_packets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string value = (sender as ComboBox).SelectedValue as string;
            rtxt_input.Document.Blocks.Clear();
            rtxt_input.Document.Blocks.Add(new Paragraph(new Run(value)));
        }

    }
}
