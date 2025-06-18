using SeviceLayer.Context;
using SeviceLayer.Models;
using SeviceLayer.Services;
using System.Collections.ObjectModel;
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

namespace PersonalManagment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EmployeeService _employeeService = new();
        public ObservableCollection<Employee> Employees {get;} = new();
        private List<Employee> _allEmployees = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            LoadEmployees();
        }

        public async Task LoadEmployees()
        {
            var employees = await _employeeService.GetBaseInfoAboutEmployess();

            _allEmployees = employees.ToList();
            FilterEmployees("");
        }

        private void FilterEmployees(string searchText)
        {
            Employees.Clear();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                foreach (var employee in _allEmployees)
                {
                    Employees.Add(employee);
                }
                return;
            }

            var searchLower = searchText.ToLower();
            foreach (var employee in _allEmployees)
            {
                if (employee.Surname.ToLower().Contains(searchLower) ||
                    employee.Name.ToLower().Contains(searchLower) ||
                    (employee.Patronymic != null && employee.Patronymic.ToLower().Contains(searchLower)) ||
                    employee.Position.Name.ToLower().Contains(searchLower) ||
                    employee.Department.Name.ToLower().Contains(searchLower))
                {
                    Employees.Add(employee);
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterEmployees(SearchTextBox.Text);
        }

        private async void EmployeesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeesListView.SelectedItem is Employee selectedEmployee)
            {
                var detailsWindow = new EmployeesDetailsWindow(selectedEmployee, _employeeService);
                detailsWindow.ShowDialog();

                if (detailsWindow.DialogResult == true)
                {
                    await LoadEmployees();
                }
                EmployeesListView.SelectedItem = null;
            }
        }
    }
}