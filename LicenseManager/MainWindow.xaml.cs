using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using LicenseManager.Annotations;
using LicenseManager.Models;
using Standard.Licensing;
using Customer = LicenseManager.Models.Customer;
using MessageBox = System.Windows.MessageBox;

namespace LicenseManager
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        readonly LicManager _licManager = new();
        readonly SettingsManager _settingsManager;
        LicenseType _selectedLicenseType;
        int _selectedQuantity; 
        string _customerName;
        string _customerEmail;
        DateTime _expirationDate = DateTime.Now.AddDays(365);
        ObservableCollection<Feature> _selectedFeatures = new();

        public IEnumerable<LicenseType> LicenseTypes =>
            Enum.GetValues(typeof(LicenseType))
                .Cast<LicenseType>();

        public LicenseType SelectedLicenseType
        {
            get => _selectedLicenseType;
            set
            {
                _selectedLicenseType = value;
                OnPropertyChanged(nameof(SelectedLicenseType));
            }
        }

        public ICollection<int> Quantities { get; } = new List<int>() { 1, 5, 10, 50, 100 };

        public int SelectedQuantity
        {
            get => _selectedQuantity;
            set
            {
                _selectedQuantity = value;
                OnPropertyChanged(nameof(SelectedQuantity));
            }
        }

        public string CustomerName
        {
            get => _customerName;
            set
            {
                _customerName = value;
                OnPropertyChanged(nameof(CustomerName));
            }
        }

        public string CustomerEmail
        {
            get => _customerEmail;
            set
            {
                _customerEmail = value;
                OnPropertyChanged(nameof(CustomerEmail));
            }
        }

        public DateTime ExpirationDate
        {
            get => _expirationDate;
            set
            {
                _expirationDate = value;
                OnPropertyChanged(nameof(ExpirationDate));
            }
        }

        public IList<Feature> Features { get; }

        public ObservableCollection<Feature> SelectedFeatures
        {
            get => _selectedFeatures;
            set
            {
                _selectedFeatures = value;
                OnPropertyChanged(nameof(SelectedFeatures));
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            LoadKeys();
            _settingsManager = new SettingsManager();
            OutputPathTextBox.Text = _settingsManager.GetOutputPath();

            Features = new List<Feature>(_settingsManager.Settings.AvailableFeatures);
            LicenseTypeComboBox.ItemsSource = LicenseTypes;
            SelectedLicenseType = LicenseType.Standard;
            QuantityCombobox.ItemsSource = Quantities;
            SelectedQuantity = 1;
            FeaturesListBox.ItemsSource = Features;
        }


        void LoadKeys()
        {
            if (KeyHandler.DoesKeyFileExist())
            {
                var keys = KeyHandler.LoadKeys();
                PrivateKeyTextBox.Text = keys["PrivateKey"];
                PublicKeyTextBox.Text = keys["PublicKey"];
            }
        }

        void CreateLicenseButton_OnClick(object sender, RoutedEventArgs e)
        {
           var licenseModel = new LicenseManager.Models.License()
            {
                Customer = new Customer() { Name = CustomerNameTextBox.Text, Email = CustomerEmailTextBox.Text },
                Expiration = ExpirationDatePicker.SelectedDate.Value,
                Id = null,
                Quantity = Convert.ToInt32(QuantityCombobox.Text),
                Type = SelectedLicenseType,
                ProductFeatures = SelectedFeatures
            };
           
            var createdLicense = _licManager.CreateLicense(KeyPasswordTextBox.Password, 
                PrivateKeyTextBox.Text, licenseModel);

            CreatedLicenseTextBox.Text = createdLicense;

            if (createdLicense.Contains("Error")) return;


            // Save License to output folder
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(createdLicense);
            var root = xmlDoc.DocumentElement;

            var name = root.GetElementsByTagName("Name")[0].InnerText;
            var expirationString = root.GetElementsByTagName("Expiration")[0].InnerText;
            var expiration = DateTime.Parse(expirationString);

            var filePath = _settingsManager.GetOutputPath() + @"\" + expiration.ToString("yyyy-MM-dd")
                           + "-" + name + DateTime.Now.Ticks + ".xml";
            using (var fileStream = File.Create(filePath))
            {
                var info = new UTF8Encoding(true).GetBytes(createdLicense);
                fileStream.Write(info, 0, info.Length);
            }
        }

        void ValidateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var validationResult = _licManager.ValidateLicense(CreatedLicenseTextBox.Text, PublicKeyTextBox.Text);

            ValidateResultTextBox.Text = validationResult;
        }

        void CreateKeysButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(KeyPasswordTextBox.Password)) return;

            if (KeyHandler.DoesKeyFileExist())
            {
                var messageBoxResult = MessageBox.Show(
                    "Already some Keys are existing. Do you really want to overwrite your keys?  " +
                    "All your created licences will become unusable", "", MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);

                if (messageBoxResult == MessageBoxResult.OK) CreateKeysAndSave();
            }
            else
            {
                CreateKeysAndSave();
            }
        }

        void CreateKeysAndSave()
        {
            var keys = _licManager.CreateKeys(KeyPasswordTextBox.Password);
            KeyHandler.SaveKeys(keys);
            LoadKeys();
        }

        void SetNewOutputPathButton_OnClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK) _settingsManager.SetOutputPath(dialog.SelectedPath);
            }
        }

        void OpenOutputPathButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", _settingsManager.GetOutputPath());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void FeaturesListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Feature item in FeaturesListBox.SelectedItems)
            {
                SelectedFeatures.Add(item);
            }
        }
    }
}