using System;
using System.Windows;

namespace pp_lab_4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="|DataDirectory|\PP Lab 2 (2).mdf";Integrated Security=True
        private void startup(object sender, EventArgs e)
        {
            listBox.Items.Add("Cars");
            listBox.Items.Add("Driver");
            listBox.Items.Add("Schedule");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;
            TableView tv = new TableView(listBox.SelectedItem.ToString());
            tv.ShowDialog();
            button.IsEnabled = true;
        }
    }
}
