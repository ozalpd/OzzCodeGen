using OzzCodeGen.Definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OzzCodeGen.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for AddProperties.xaml
    /// </summary>
    public partial class AddProperties : Window, INotifyPropertyChanged
    {
        public AddProperties()
        {
            InitializeComponent();

            AllStringsNullable = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public bool AllStringsNullable
        {
            get
            {
                return _stringsNullable;
            }
            set
            {
                _stringsNullable = value;
                RaisePropertyChanged("AllStringsNullable");
            }
        }
        private bool _stringsNullable;

        public bool IncludeComplexProperties
        {
            get
            {
                return _inclComplexProperties;
            }
            set
            {
                _inclComplexProperties = value;
                RaisePropertyChanged("IncludeComplexProperties");
            }
        }
        private bool _inclComplexProperties;


        public int? MaxStringLenght
        {
            get
            {
                return _maxLenght;
            }
            set
            {
                if (_maxLenght == value) return;
                _maxLenght = value;
                RaisePropertyChanged("MaxStringLenght");
            }
        }
        private int? _maxLenght;

        public EntityPropertyList ParsedProperties
        {
            set
            {
                _parsedProperties = value;
                RaisePropertyChanged("ParsedProperties");
            }
            get
            {
                if (_parsedProperties == null)
                    _parsedProperties = new EntityPropertyList();
                return _parsedProperties;
            }
        }
        private EntityPropertyList _parsedProperties;

        public EntityDefinition SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                if (_selectedEntity == value)
                    return;
                _selectedEntity = value;
                RaisePropertyChanged("SelectedEntity");
            }
        }
        private EntityDefinition _selectedEntity;

        public string TextForParsing
        {
            get { return _textForParsing; }
            set
            {
                _textForParsing = value;
                RaisePropertyChanged("TextForParsing");
            }
        }
        string _textForParsing;

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ParsedProperties)
            {
                SelectedEntity.Properties.Add(item, true);
            }
            DialogResult = true;
        }

        private void btnParseText_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Add different text formats like C# class or XML or JSON
            var text = TextForParsing;
            var lines = Regex.Split(text, "\r\n|\r|\n");
            var properties = new EntityPropertyList();
            foreach (var item in lines)
            {
                var p = ParseSimpleLine(item);
                if (p != null && (IncludeComplexProperties || p is SimpleProperty || p is StringProperty))
                    properties.Add(p, true);
            }

            ParsedProperties = properties;
        }

        private BaseProperty ParseSimpleLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return null;
            Regex regex = new Regex("\t|[ ]{2,}");
            var parts = regex.Replace(line, " ").Split(' ');

            var p = BaseProperty.CreatePropertyDefinition(parts[0], parts[1]);
            p.UiVisible = true;
            p.Editable = true;

            if(p is StringProperty)
            {
                ((StringProperty)p).IsNullable = AllStringsNullable;
                if (MaxStringLenght.HasValue)
                    ((StringProperty)p).MaxLenght = MaxStringLenght.Value;
            }

            return p;
        }
    }
}
