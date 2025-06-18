using SeviceLayer.Models;
using SeviceLayer.Services;
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

namespace PersonalManagment
{
    /// <summary>
    /// Логика взаимодействия для EmployeesDetailsWindow.xaml
    /// </summary>
    public partial class EmployeesDetailsWindow : Window
    {
        private readonly EmployeeService _employeeService = new();
        private bool _isEditMode = false;

        public EmployeesDetailsWindow(Employee employee, EmployeeService employeeService)
        {
            InitializeComponent();
            _employeeService = employeeService;
            DataContext = employee;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = true;
            UpdateEditState();
        }

        private void UpdateEditState()
        {
            SurnameBox.IsReadOnly = !_isEditMode;
            NameBox.IsReadOnly = !_isEditMode;
            PatronymicBox.IsReadOnly = !_isEditMode;
            BirthdayPicker.IsEnabled = _isEditMode;
            PhoneBox.IsReadOnly = !_isEditMode;
            EmailBox.IsReadOnly = !_isEditMode;

            EditButton.Visibility = _isEditMode ? Visibility.Collapsed : Visibility.Visible;
            SaveButton.Visibility = _isEditMode ? Visibility.Visible : Visibility.Collapsed;
            ErrorText.Text = "";
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData())
                return;

            var employee = (Employee)DataContext;
            var result = await _employeeService.UpdateEmployee(employee);

            if (result)
            {
                MessageBox.Show("Данные сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _isEditMode = false;
                UpdateEditState();
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateData()
        {
            var employee = (Employee)DataContext;
            var errors = new List<string>();

            // Проверка ФИО
            if (string.IsNullOrWhiteSpace(employee.Surname))
                errors.Add("Фамилия обязательна для заполнения");
            if (string.IsNullOrWhiteSpace(employee.Name))
                errors.Add("Имя обязательно для заполнения");

            // Проверка телефона
            if (!string.IsNullOrEmpty(employee.Phone) && !System.Text.RegularExpressions.Regex.IsMatch(employee.Phone, @"^\d+$"))
                errors.Add("Телефон должен содержать только цифры");

            // Проверка email
            if (string.IsNullOrWhiteSpace(employee.Email))
                errors.Add("Email обязателен для заполнения");
            else if (!IsValidEmail(employee.Email))
                errors.Add("Некорректный формат email");

            if (errors.Count > 0)
            {
                ErrorText.Text = string.Join("\n", errors);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            var errors = new List<string>();

            // Создаем временный объект для валидации
            var tempEmployee = new Employee
            {
                Surname = ((Employee)DataContext).Surname,
                Name = ((Employee)DataContext).Name,
                Patronymic = ((Employee)DataContext).Patronymic,
                Phone = ((Employee)DataContext).Phone,
                Email = ((Employee)DataContext).Email,
                Birthday = ((Employee)DataContext).Birthday
            };

            // Проверка ФИО
            if (string.IsNullOrWhiteSpace(tempEmployee.Surname))
                errors.Add("Фамилия обязательна для заполнения");

            if (string.IsNullOrWhiteSpace(tempEmployee.Name))
                errors.Add("Имя обязательно для заполнения");

            // Проверка телефона
            if (!string.IsNullOrEmpty(tempEmployee.Phone))
            {
                var phoneDigits = new string(tempEmployee.Phone.Where(char.IsDigit).ToArray());
                if (tempEmployee.Phone != phoneDigits)
                    errors.Add("Телефон должен содержать только цифры");
            }

            // Проверка email
            if (string.IsNullOrWhiteSpace(tempEmployee.Email))
            {
                errors.Add("Email обязателен для заполнения");
            }
            else
            {
                try
                {
                    var mailAddress = new System.Net.Mail.MailAddress(tempEmployee.Email);
                    if (mailAddress.Address != tempEmployee.Email)
                        errors.Add("Некорректный формат email");
                }
                catch
                {
                    errors.Add("Некорректный формат email");
                }
            }

            if (errors.Count > 0)
            {
                ErrorText.Text = string.Join("\n", errors);
                return false;
            }

            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
