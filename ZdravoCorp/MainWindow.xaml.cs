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
using Newtonsoft.Json;
using System.IO;
using ZdravoCorp.HealthInstitution.Users;
using ZdravoCorp.HealthInstitution.GUI;
using System.Xaml;
using ZdravoCorp.HealthInstitution.Services.Login;
using EquipmentMovingNS;

namespace ZdravoCorp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeEquipment();
        }

        private void LogInClick(object sender, RoutedEventArgs e)
        {
            string fileName = "../../../Data/Login/LoginData.json";
            string username = usernameTextBox.Text;
            string password = passwordBox.Password.ToString();
            bool loggedIn = Login.LoginLogic(username, password, fileName);
            if (loggedIn)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Username and password are not valid, please try again!");
                usernameTextBox.Text = "";
                passwordBox.Password = "";
            }
        }

        private void InitializeEquipment()
        {
            EquipmentOrderNS.EquipmentOrderingService.UpdateDeliveredOrders();
            EquipmentMovingService.UpdateMoveRequests();
        }

    }
}