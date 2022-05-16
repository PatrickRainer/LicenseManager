using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using LicenseManager.Models;

namespace LicenseManager
{
    public partial class MultiselectComboBox : UserControl
    {
        
        public string DisplayMemberPath { get; set; } = "Name";

        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register(
            nameof(ItemSource), typeof(IEnumerable), typeof(MultiselectComboBox));

        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }
        public MultiselectComboBox()
        {
            InitializeComponent();

        }
    }
}