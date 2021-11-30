using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;

namespace pp_lab_4
{
    /// <summary>
    /// Логика взаимодействия для TableView.xaml
    /// </summary>
    public partial class TableView : Window
    {
        string table;
        List<TextBox> tbs = new List<TextBox>();
        List<string> columnNames = new List<string>();
        Dictionary<int, string> cars = new Dictionary<int, string>();
        Dictionary<int, string> drivers = new Dictionary<int, string>();
        Dictionary<int, string> carsCBOrder = new Dictionary<int, string>();
        Dictionary<int, string> driverCBOrder = new Dictionary<int, string>();
        bool saveState = false;

        public TableView(string table)
        {
            InitializeComponent();

            this.table = table;
            Title = $"Таблица: {table}";

            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.IsReadOnly = true;
            dataGridView1.SelectionMode = DataGridSelectionMode.Single;
            this.Language = XmlLanguage.GetLanguage("ru-RU");

            carsGroupBox.Visibility = Visibility.Hidden;
            driverGroupBox.Visibility = Visibility.Hidden;
            scheduleGroupBox.Visibility = Visibility.Hidden;
            switch (table)
            {
                case "Cars":
                    carsGroupBox.Visibility = Visibility.Visible;
                    break;
                case "Driver":
                    driverGroupBox.Visibility = Visibility.Visible;
                    break;
                case "Schedule":
                    scheduleGroupBox.Visibility = Visibility.Visible;
                    LoadCarsAndDrivers();
                    break;
            }

            GetData();
            GetTextBoxes();
        }
        void LoadCarsAndDrivers()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load($@"XMLs\Cars.xml");
            XmlNode root = doc.ChildNodes[1].ChildNodes[1];
            foreach (XmlNode line in root.ChildNodes)
            {
                cars.Add(int.Parse(line.ChildNodes[0].InnerText), line.ChildNodes[1].InnerText);
            }

            doc = new XmlDocument();
            doc.Load($@"XMLs\Driver.xml");
            root = doc.ChildNodes[1].ChildNodes[1];
            foreach (XmlNode line in root.ChildNodes)
            {
                drivers.Add(int.Parse(line.ChildNodes[0].InnerText), 
                    $"{line.ChildNodes[2].InnerText} {line.ChildNodes[1].InnerText} {line.ChildNodes[3].InnerText}");
            }

            int i = 0;
            if(cars.Count > 0)
            {
                foreach (string car in cars.Values)
                {
                    schCar.Items.Add(car);
                    carsCBOrder.Add(i,car);
                    i++;
                }
            }
            else schCar.Items.Add("<пусто>");
            schCar.SelectedIndex = 0;
            i = 0;
            if (drivers.Count > 0)
            {
                foreach (string driver in drivers.Values)
                {
                    schDriver.Items.Add(driver);
                    driverCBOrder.Add(i, driver);
                    i++;
                }
            }
            else schDriver.Items.Add("<пусто>");
            schDriver.SelectedIndex = 0;

            startHour.SelectedIndex = 0;
            startMinute.SelectedIndex = 0;
            endHour.SelectedIndex = 0;
            endMinute.SelectedIndex = 0;
            startDate.SelectedDate = DateTime.Now;
            endDate.SelectedDate = DateTime.Now;
        }
        public struct Cars
        {
            public int id { get; set; }
            public string car_name { get; set; }
            public int year { get; set; }
            public int mileage { get; set; }
            public int tank_volume { get; set; }
            public int seats { get; set; }
        }
        public struct Driver
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string second_name { get; set; }
            public DateTime date_of_birth { get; set; }
            public int driving_exp { get; set; }
        }
        public struct Schedule
        {
            public int id { get; set; }
            public string car_id { get; set; }
            public string driver_id { get; set; }
            public DateTime start_date { get; set; }
            public DateTime end_date { get; set; }
        }
        void GetData()
        {
            dataGridView1.Items.Clear();
            dataGridView1.Columns.Clear();

            XmlDocument doc = new XmlDocument();
            doc.Load($@"XMLs\{table}.xml");
            XmlNode root = doc.ChildNodes[1].ChildNodes[0];
            foreach(XmlNode line in root)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Header = line.InnerText;
                textColumn.Binding = new Binding(line.Name);
                dataGridView1.Columns.Add(textColumn);
                columnNames.Add(line.Name);
            }
            root = doc.ChildNodes[1].ChildNodes[1];
            foreach (XmlNode line in root.ChildNodes)
            {
                switch (table)
                {
                    case "Cars":
                        dataGridView1.Items.Add(new Cars()
                        {
                            id = int.Parse(line.ChildNodes[0].InnerText),
                            car_name = line.ChildNodes[1].InnerText,
                            year = int.Parse(line.ChildNodes[2].InnerText),
                            mileage = int.Parse(line.ChildNodes[3].InnerText),
                            tank_volume = int.Parse(line.ChildNodes[4].InnerText),
                            seats = int.Parse(line.ChildNodes[5].InnerText)
                        });
                        break;
                    case "Driver":
                        dataGridView1.Items.Add(new Driver()
                        {
                            id = int.Parse(line.ChildNodes[0].InnerText),
                            first_name = line.ChildNodes[1].InnerText,
                            last_name = line.ChildNodes[2].InnerText,
                            second_name = line.ChildNodes[3].InnerText,
                            date_of_birth = DateTime.Parse(line.ChildNodes[4].InnerText),
                            driving_exp = int.Parse(line.ChildNodes[5].InnerText)
                        });
                        break;
                    case "Schedule":
                        int carId = int.Parse(line.ChildNodes[1].InnerText);
                        int driverId = int.Parse(line.ChildNodes[2].InnerText);
                        dataGridView1.Items.Add(new Schedule()
                        {
                            id = int.Parse(line.ChildNodes[0].InnerText),
                            car_id = cars.ContainsKey(carId) ? cars[carId] : "<пусто>",
                            driver_id = drivers.ContainsKey(driverId) ? drivers[driverId] : "<пусто>",
                            start_date = DateTime.Parse(line.ChildNodes[3].InnerText),
                            end_date = DateTime.Parse(line.ChildNodes[4].InnerText)
                        });
                        break;
                }
            }
        }
        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }

        void GetTextBoxes()
        {
            switch (table)
            {
                case "Cars":
                    tbs.Add(cartext1);
                    tbs.Add(cartext2);
                    tbs.Add(cartext3);
                    tbs.Add(cartext4);
                    tbs.Add(cartext5);
                    break;
                case "Driver":
                    tbs.Add(drivertext1);
                    tbs.Add(drivertext2);
                    tbs.Add(drivertext3);
                    tbs.Add(drivertext4);
                    break;
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                saveState = true;

                XmlDocument doc = new XmlDocument();
                doc.Load($@"XMLs\{table}.xml");
                XmlNode root = doc.ChildNodes[1].ChildNodes[1];
                root.RemoveAll();
                for(int i = 0; i < dataGridView1.Items.Count; i++)
                {
                    object item = GetRow(i).Item;
                    XmlElement line = doc.CreateElement("line");

                    for (int j = 0; j < dataGridView1.Columns.Count; j++)
                    {
                        string value = "";

                        switch (table)
                        {
                            case "Schedule":
                                switch (j)
                                {
                                    case 1: 
                                        value = cars.First(x => x.Value == (dataGridView1.Columns[j].GetCellContent(item) as TextBlock).Text).Key.ToString();
                                        break;
                                    case 2: 
                                        value = drivers.First(x => x.Value == (dataGridView1.Columns[j].GetCellContent(item) as TextBlock).Text).Key.ToString();
                                        break;
                                    default: 
                                        value = (dataGridView1.Columns[j].GetCellContent(item) as TextBlock).Text;
                                        break;
                                }
                                break;
                            default:
                                value = (dataGridView1.Columns[j].GetCellContent(item) as TextBlock).Text;
                                break;
                        }

                        string atributeName = columnNames[j];
                        XmlElement el = doc.CreateElement(atributeName);
                        XmlText text = doc.CreateTextNode(value);
                        el.AppendChild(text);
                        line.AppendChild(el);
                    }
                    root.AppendChild(line);
                }
                doc.Save($@"XMLs\{table}.xml");

                GetData();

                saveState = false;
                MessageBox.Show("Сохранено!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);

                MessageBox.Show("Произошла ошибка! Наведите на красный круг для получения информации, либо нажмите на 🔄 для возвращения таблицы в актуальный вид");
            }
        }
        public DataGridRow GetRow(int index)
        {
            DataGridRow row = (DataGridRow)dataGridView1.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                dataGridView1.UpdateLayout();
                dataGridView1.ScrollIntoView(dataGridView1.Items[index]);
                row = (DataGridRow)dataGridView1.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedItems.Count > 0)
                {
                    foreach (TextBox tb in tbs)
                        tb.Text = "";

                    int index = dataGridView1.SelectedIndex;
                    dataGridView1.Items.RemoveAt(index);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void dataGridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (saveState) return;

            object item = dataGridView1.SelectedItem;
            switch (table)
            {
                case "Cars":
                    tbs[0].Text = (dataGridView1.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
                    tbs[1].Text = (dataGridView1.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
                    tbs[2].Text = (dataGridView1.SelectedCells[3].Column.GetCellContent(item) as TextBlock).Text;
                    tbs[3].Text = (dataGridView1.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text;
                    tbs[4].Text = (dataGridView1.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text;
                    break;
                case "Driver":
                    tbs[0].Text = (dataGridView1.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
                    tbs[1].Text = (dataGridView1.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
                    tbs[2].Text = (dataGridView1.SelectedCells[3].Column.GetCellContent(item) as TextBlock).Text;
                    driverDoB.SelectedDate = DateTime.Parse((dataGridView1.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text);
                    tbs[3].Text = (dataGridView1.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text;
                    break;
                case "Schedule":
                    string carName = (dataGridView1.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
                    string driverName = (dataGridView1.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text;
                    string[] start = (dataGridView1.SelectedCells[3].Column.GetCellContent(item) as TextBlock).Text.Split(' ');
                    string[] end = (dataGridView1.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text.Split(' ');

                    if (carName != "<пусто>")
                        schCar.SelectedIndex = carsCBOrder.First(x => x.Value == carName).Key;
                    if (driverName != "<пусто>")
                        schDriver.SelectedIndex = driverCBOrder.First(x => x.Value == driverName).Key;
                    startDate.SelectedDate = DateTime.Parse(start[0]);
                    endDate.SelectedDate = DateTime.Parse(end[0]);

                    string[] startHoursMinutes = start[1].Split(':');
                    string[] endHoursMinutes = end[1].Split(':');
                    startHour.SelectedIndex = int.Parse(startHoursMinutes[0]);
                    endHour.SelectedIndex = int.Parse(endHoursMinutes[0]);
                    startMinute.SelectedIndex = int.Parse(startHoursMinutes[1]) / 10;
                    endMinute.SelectedIndex = int.Parse(endHoursMinutes[1]) / 10;
                    break;
            }
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            object item = dataGridView1.SelectedItem;
            switch (table)
            {
                case "Cars":
                    for (int i = 0; i < tbs.Count; i++)
                    {
                        (dataGridView1.SelectedCells[i + 1].Column.GetCellContent(item) as TextBlock).Text = tbs[i].Text;
                    }
                    break;
                case "Driver":
                    (dataGridView1.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text = tbs[0].Text;
                    (dataGridView1.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text = tbs[1].Text;
                    (dataGridView1.SelectedCells[3].Column.GetCellContent(item) as TextBlock).Text = tbs[2].Text;
                    (dataGridView1.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text = driverDoB.SelectedDate.ToString();
                    (dataGridView1.SelectedCells[5].Column.GetCellContent(item) as TextBlock).Text = tbs[3].Text;
                    break;
                case "Schedule":
                    int startH = int.Parse(startHour.Text);
                    int endH = int.Parse(endHour.Text);
                    int startM = int.Parse(startMinute.Text);
                    int endM = int.Parse(endMinute.Text);
                    DateTime startDT = startDate.SelectedDate.Value.AddHours(startH).AddMinutes(startM);
                    DateTime endDT = endDate.SelectedDate.Value.AddHours(endH).AddMinutes(endM);

                    (dataGridView1.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text = schCar.Text;
                    (dataGridView1.SelectedCells[2].Column.GetCellContent(item) as TextBlock).Text = schDriver.Text;
                    (dataGridView1.SelectedCells[3].Column.GetCellContent(item) as TextBlock).Text = startDT.ToString();
                    (dataGridView1.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text = endDT.ToString();
                    break;
            }
        }

        private void clearAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TextBox tb in tbs)
                tb.Text = "";

            driverDoB.SelectedDate = DateTime.Now;

            schCar.SelectedIndex = 0;
            schDriver.SelectedIndex = 0;
            startHour.SelectedIndex = 0;
            startMinute.SelectedIndex = 0;
            endHour.SelectedIndex = 0;
            endMinute.SelectedIndex = 0;
            startDate.SelectedDate = DateTime.Now;
            endDate.SelectedDate = DateTime.Now;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            object item = null;
            if (dataGridView1.Items.Count > 0)
                item = dataGridView1.Items[dataGridView1.Items.Count - 1];
            int lastId = item != null ? int.Parse((dataGridView1.Columns[0].GetCellContent(item) as TextBlock).Text) : -1;

            try
            {
                switch (table)
                {
                    case "Cars":
                        dataGridView1.Items.Add(new Cars()
                        {
                            id = lastId + 1,
                            car_name = tbs[0].Text,
                            year = int.Parse(tbs[1].Text),
                            mileage = int.Parse(tbs[2].Text),
                            tank_volume = int.Parse(tbs[3].Text),
                            seats = int.Parse(tbs[4].Text)
                        });
                        break;
                    case "Driver":
                        dataGridView1.Items.Add(new Driver()
                        {
                            id = lastId + 1,
                            first_name = tbs[0].Text,
                            last_name = tbs[1].Text,
                            second_name = tbs[2].Text,
                            date_of_birth = (DateTime)driverDoB.SelectedDate,
                            driving_exp = int.Parse(tbs[3].Text)
                        });
                        break;
                    case "Schedule":
                        int startH = int.Parse(startHour.Text);
                        int endH = int.Parse(endHour.Text);
                        int startM = int.Parse(startMinute.Text);
                        int endM = int.Parse(endMinute.Text);
                        DateTime startDT = startDate.SelectedDate.Value.AddHours(startH).AddMinutes(startM);
                        DateTime endDT = endDate.SelectedDate.Value.AddHours(endH).AddMinutes(endM);

                        dataGridView1.Items.Add(new Schedule()
                        {
                            id = lastId + 1,
                            car_id = schCar.Text,
                            driver_id = schDriver.Text,
                            start_date = startDT,
                            end_date = endDT
                        });
                        break;
                }
            }
            catch(FormatException)
            {
                MessageBox.Show("Неверный тип поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
