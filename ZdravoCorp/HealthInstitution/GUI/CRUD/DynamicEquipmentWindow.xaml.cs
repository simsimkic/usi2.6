using EquipmentServicesNS;
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
using System.Windows.Shapes;
using ZdravoCorp.HealthInstitution.Equipment;

namespace ZdravoCorp.HealthInstitution.GUI.CRUD
{
    /// <summary>
    /// Interaction logic for DynamicEquipmentWindow.xaml
    /// </summary>
    public partial class DynamicEquipmentWindow : Window
    {
        public int RoomId;
        public DynamicEquipmentWindow(int roomId)
        {
            RoomId=roomId;
            InitializeComponent();
            InitializeEquipmentData();
        }

        private void InitializeEquipmentData()
        {
            List<EquipmentStorageItem> roomEquipment = DoctorEquipment.GetRoomEquipment(RoomId);
            List<string> equipmentNames = roomEquipment.Select(x => x.StoredEquipmentName).ToList();

            equipmentDataGrid.ItemsSource = roomEquipment;
            equipmentCmb.ItemsSource=equipmentNames;
        }

        private void BtnEquipmentClick(object sender, RoutedEventArgs e)
        {
            if(equipmentCmb.SelectedItem!=null && txtQuantity.Text != "")
            {
                DoctorEquipment.EnterEquipment(equipmentCmb.SelectedItem.ToString(),int.Parse(txtQuantity.Text),RoomId);
                InitializeEquipmentData();
            }
            else
            {
                MessageBox.Show("Enter the values", "Warning");
            }
        }

        private void BtnExitEquipmentClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
