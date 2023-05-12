using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using ZdravoCorp.HealthInstitution.Users;
using ZdravoCorp.HealthInstitution.Services.Login;
using ZdravoCorp;
using Newtonsoft.Json;
using RoomsNS;
using System.IO;
using EquipmentServicesNS;
using RoomServicesNS;
using System.Drawing;
using System.Xml.Linq;
using Color = System.Windows.Media.Color;
using EquipmentMovingNS;
using ZdravoCorp.HealthInstitution.Controllers;
using System;
using System.Threading;

namespace ZdravoCorp.HealthInstitution.GUI
{
    /// <summary>
    /// Interaction logic for ManagerMainWindow.xaml
    /// </summary>
    public partial class ManagerMainWindow : Window
    {
        Thread updatingThread;
        public ManagerMainWindow(string username)
        {
            updatingThread = new Thread(new ThreadStart(CheckForEquipmentUpdates));
            updatingThread.IsBackground = true;
            updatingThread.Start();
            InitializeComponent();
            Manager loggedInManager = Manager.FindManagerByUsername(username);
            InitializeProfile(loggedInManager);
            InitializeEquipmentTable();
            InitializeOrderingTable();
            InitializeEquipmentOrderingTab();
            InitializeEquipmentMoveTab();
        }

        public void CheckForEquipmentUpdates()
        {
            while (true)
            {
                bool updateNeeded = EquipmentMovingService.IsUpdateNeeded() || EquipmentStorageService.IsChanged;
                if (updateNeeded)
                {
                    EquipmentMovingService.UpdateMoveRequests();
                    EquipmentStorageService.IsChanged = false;
                    this.Dispatcher.Invoke(() =>
                    {
                        InitializeEquipmentTable();
                        UpdateEquipmentMoveTable();
                        InitializeOrderingTable();
                    });
                }
                Thread.Sleep(200);      //check 5 times in a second
            }
        }

        public void InitializeOrderingTable()
        {
            Dictionary<string, int> items = EquipmentFilteringService.GetDynamicEquipmentSumItems();
            foreach (var item in items)
            {
                TableRowGroup rowGroup = EquipmentGuiUtils.GetRowGroup(EquipmentOrderingTable);

                if (rowGroup == null) return;
                TableRow row = OrderEquipmentController.MakeRowForEquipmentDisplay(item);
                rowGroup.Rows.Add(row);
            }
        }

        public void InitializeEquipmentOrderingTab()
        {
            List<string> allEquipmentTypes = EquipmentService.GetAllEquipmentTypes();
            EquipmentGuiUtils.SetEquipmentComboBox(OrderEquipmentTypeComboBox, ref allEquipmentTypes, true);
        }

        public static void ClearTable(Table table)
        {
            try
            {
                table.RowGroups.RemoveAt(1);
            }
            catch { }
        }

        public void SetElementsEquipmentTable(List<EquipmentStorageItem> elements)
        {
            foreach (EquipmentStorageItem item in elements)
            {
                TableRowGroup rowGroup = EquipmentGuiUtils.GetRowGroup(EquipmentTableXaml);

                if (rowGroup == null) return;
                TableRow row = EquipmentFilterSearchController.MakeRowForEquipmentDisplay(item);
                rowGroup.Rows.Add(row);

            }
        }

        public void InitializeEquipmentTable()
        {
            var jsontext = File.ReadAllText("../../../Data/EquipmentStorage/EquipmentStorage.json");
            List<EquipmentStorageItem> items = JsonConvert.DeserializeObject<List<EquipmentStorageItem>>(jsontext)!;

            SetElementsEquipmentTable(items);
        }

        public void RefreshEquipmentTable(object sender, RoutedEventArgs e)
        {
            ClearTable(EquipmentTableXaml);
            InitializeEquipmentTable();
        }

        public void InitializeEquipmentMoveTab()
        {
            //initialization of equipment combo box
            List<string> allEquipmentNames = EquipmentService.GetAllEquipmentTypes();
            EquipmentGuiUtils.SetEquipmentComboBox(MovingSelectEquipmentCombo, ref allEquipmentNames, false);

            //initialization of room id combo box
            List<int> allRoomIds = RoomSevice.GetAllRoomIds();
            foreach (int roomId in allRoomIds)
            {
                FromRoomCombo.Items.Add(roomId);
                ToRoomCombo.Items.Add(roomId);
            }

            //equipment move table
            UpdateEquipmentMoveTable();
        }

        public void RefreshMoveTable(object sender, RoutedEventArgs e)
        {
            UpdateEquipmentMoveTable();
        }

        public void UpdateEquipmentMoveTable()
        {
            ClearTable(EquipmentMoveTable);
            var jsontext = File.ReadAllText("../../../Data/EquipmentStorage/EquipmentStorage.json");
            List<EquipmentStorageItem> items = JsonConvert.DeserializeObject<List<EquipmentStorageItem>>(jsontext)!; EquipmentMovingService.UpdateMoveRequests();
            EquipmentMovingService.UpdateMoveRequests();
            jsontext = File.ReadAllText("../../../Data/EquipmentStorage/EquipmentMovingSchedule.json");
            List<EquipmentMovingRequest> movingItems = JsonConvert.DeserializeObject<List<EquipmentMovingRequest>>(jsontext)!;
            SetElementsEquipmentMoveTable(items, movingItems);
        }

        public void SetElementsEquipmentMoveTable(List<EquipmentStorageItem> allEquipment, List<EquipmentMovingRequest> equipmentScheduledForMoving)
        {
            foreach (EquipmentStorageItem item in allEquipment)
            {
                TableRowGroup rowGroup = EquipmentGuiUtils.GetRowGroup(EquipmentMoveTable);

                if (rowGroup == null) return;
                TableRow row = MoveEquipmentController.MakeRowForEquipmentDisplay(item, ref equipmentScheduledForMoving);
                rowGroup.Rows.Add(row);

            }
        }

        public void MoveClick(object sender, RoutedEventArgs e)
        {
            string equipmentName = EquipmentServicesNS.EquipmentService.Format(MovingSelectEquipmentCombo.Text);
            int fromRoom = int.Parse(FromRoomCombo.SelectedItem.ToString());
            int toRoom = int.Parse(ToRoomCombo.SelectedItem.ToString());
            string quantityString = equipmentMoveQuantity.Text;
            string hoursAndMinutes = MoveTimePicker.Text.ToString();
            DateTime moveMoment = new DateTime();
            if (EquipmentService.IsEquipmentDinamicByName(equipmentName))
            {
                moveMoment = DateTime.Now;
            }
            else
            {
                try
                {
                    moveMoment = MoveDatePicker.SelectedDate.Value.Date;
                }
                catch
                {
                    MessageBox.Show("Please input valid date!");
                    return;
                }
            }
            MoveEquipmentController.MoveEquipment(equipmentName, fromRoom, toRoom, quantityString, hoursAndMinutes, moveMoment, equipmentMoveQuantity, FromRoomCombo, ToRoomCombo, MovingSelectEquipmentCombo);
            UpdateEquipmentMoveTable();
        }

        public void MovingEquipmentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedEquipment = (sender as ComboBox).SelectedValue as string;
            if (EquipmentService.IsEquipmentDinamicByName(selectedEquipment))
            {
                MoveDatePicker.Focusable = false;
                MoveDatePicker.IsHitTestVisible = false;
                MoveTimePicker.IsReadOnly = true;
            }
            else
            {
                MoveDatePicker.Focusable = true;
                MoveDatePicker.IsHitTestVisible = true;
                MoveTimePicker.IsReadOnly = false;
            }
            //change room ids that can be selected
            FromRoomCombo.Items.Clear();
            ToRoomCombo.Items.Clear();
            List<Room.Type> possibleRoomTypes = EquipmentService.GetValidRoomTypesForEquipment(selectedEquipment);
            foreach (int id in RoomSevice.GetAllRoomIds())
            {
                if (possibleRoomTypes.Contains(RoomSevice.GetRoomTypeById(id)))
                {
                    FromRoomCombo.Items.Add(id);
                    ToRoomCombo.Items.Add(id);
                }
            }
        }

        public void LogOutClick(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        public void InitializeProfile(Manager loggedInManager)
        {
            List<Label> profileLabels = new List<Label>();

            Profile.Background = new ImageBrush(new BitmapImage(new Uri("../../../" + loggedInManager.Image, UriKind.Relative)));
            int nameSpaces = 50 - loggedInManager.Name.Length - name.Content.ToString().Length;
            for (int i = 0; i < nameSpaces + 5; i++)
            {
                name.Content += " ";
            }
            name.Content += loggedInManager.Name;

            int surnameSpaces = 52 - loggedInManager.Surname.Length - surname.Content.ToString().Length;
            for (int i = 0; i < surnameSpaces; i++)
            {
                surname.Content += " ";
            }
            surname.Content += loggedInManager.Surname;

            int usernameSpaces = 48 - loggedInManager.Username.Length - username.Content.ToString().Length;
            for (int i = 0; i < usernameSpaces; i++)
            {
                username.Content += " ";
            }
            username.Content += loggedInManager.Username;

            int passwordSpaces = 49 - loggedInManager.Password.Length - password.Content.ToString().Length;
            for (int i = 0; i < passwordSpaces; i++)
            {
                password.Content += " ";
            }
            password.Content += loggedInManager.Password;


            int birthdaySpaces = 50 - loggedInManager.Birthday.Length - birthday.Content.ToString().Length;
            for (int i = 0; i < birthdaySpaces; i++)
            {
                birthday.Content += " ";
            }
            birthday.Content += loggedInManager.Birthday;


            int emailSpaces = 45 - loggedInManager.Email.Length - email.Content.ToString().Length;
            for (int i = 0; i < emailSpaces; i++)
            {
                email.Content += " ";
            }
            email.Content += loggedInManager.Email;

            int roleSpaces = 54 - "Manager".Length - role.Content.ToString().Length;
            for (int i = 0; i < roleSpaces; i++)
            {
                role.Content += " ";
            }
            role.Content += "Manager";
        }

        public void ApplyFilters(object sender, RoutedEventArgs e)
        {
            List<EquipmentStorageItem> items = GetEquipmentFiltered();
            ClearTable(EquipmentTableXaml);
            SetElementsEquipmentTable(items);
        }

        public List<EquipmentStorageItem> GetEquipmentFiltered()
        {
            //geting room type filters
            bool warehouseChecked = FilterRoomWarehouse.IsChecked ?? false;
            bool waitingChecked = FilterRoomWaiting.IsChecked ?? false;
            bool consultingChecked = FilterRoomConsulting.IsChecked ?? false;
            bool patientCareChecked = FilterRoomPatientCare.IsChecked ?? false;
            bool operatingChecked = FilterRoomOperating.IsChecked ?? false;


            //geting equipment type filters
            bool staticChecked = FilterEquipmentTypeStatic.IsChecked ?? false;
            bool dynamicChecked = FilterEquipmentTypeDynamic.IsChecked ?? false;

            //geting quantity filters
            bool moreThanTenChecked = FilterByQuantityMoreThanTen.IsChecked ?? false;
            bool upToTenChecked = FilterByQuantityUpToTen.IsChecked ?? false;
            bool zeroChecked = FilterQuantityZero.IsChecked ?? false;

            return EquipmentFilterSearchController.ApplyFilters(warehouseChecked, waitingChecked, consultingChecked, patientCareChecked, operatingChecked,
                staticChecked, dynamicChecked, moreThanTenChecked, upToTenChecked, zeroChecked);
        }

        public void ApplySearch(object sender, RoutedEventArgs e)
        {
            //take all items that pass the filters and search them
            //this allows for combined filtering and search
            List<EquipmentStorageItem> adequateItems = GetEquipmentFiltered();

            adequateItems = EquipmentSearchService.SearchEquipment(SearchTextBox.Text, ref adequateItems);
            SearchTextBox.Text = "";
            ClearTable(EquipmentTableXaml);
            SetElementsEquipmentTable(adequateItems);
        }

        public void OrderClick(object sender, RoutedEventArgs e)
        {
            string equipmentName = EquipmentServicesNS.EquipmentService.Format(OrderEquipmentTypeComboBox.Text);
            string quantityBoxText = equipmentOrderQuantity.Text;
            OrderEquipmentController.PlaceOrder(equipmentName, quantityBoxText, equipmentOrderQuantity, OrderEquipmentTypeComboBox);
        }
    }
}