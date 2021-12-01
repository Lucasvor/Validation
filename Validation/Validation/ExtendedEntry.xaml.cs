using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Validation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExtendedEntry : ContentView
    {
        public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(nameof(ErrorText),
     typeof(string),
     typeof(ExtendedEntry),
     default(string),
     BindingMode.TwoWay);

        public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(nameof(HorizontalTextAlignment),
            typeof(TextAlignment),
            typeof(ExtendedEntry),
            TextAlignment.Start);

        public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(nameof(Keyboard),
            typeof(Keyboard),
            typeof(ExtendedEntry),
            Keyboard.Default);

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text),
            typeof(string),
            typeof(ExtendedEntry),
            default(string),
            BindingMode.TwoWay);

        public static readonly BindableProperty ValidatesOnDataErrorsProperty = BindableProperty.Create(nameof(ValidatesOnDataErrors),
            typeof(bool),
            typeof(ExtendedEntry),
            default(bool), propertyChanged: OnValidatesOnDataErrorsPropertyChanged);

        private Binding _binding;

        public ExtendedEntry()
        {
            InitializeComponent();
        }

        public string ErrorText
        {
            get => (string)GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }

        public TextAlignment HorizontalTextAlignment
        {
            get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
            set => SetValue(HorizontalTextAlignmentProperty, value);
        }

        /// <summary>
        ///     Keyboard summary. This is a bindable property.
        /// </summary>
        public Keyboard Keyboard
        {
            get => (Keyboard)GetValue(KeyboardProperty);
            set => SetValue(KeyboardProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool ValidatesOnDataErrors
        {
            get => (bool)GetValue(ValidatesOnDataErrorsProperty);
            set => SetValue(ValidatesOnDataErrorsProperty, value);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            OnValidatesOnDataErrorsPropertyChanged(this, null, ValidatesOnDataErrors);
        }

        private static void OnValidatesOnDataErrorsPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var self = (ExtendedEntry)bindable;
            if (oldvalue == newvalue) return;
            if (!(self.BindingContext is INotifyDataErrorInfo vm)) return;
            vm.ErrorsChanged -= self.VmOnErrorsChanged;
            if (bool.TryParse(newvalue.ToString(), out var validatesOnDataErrors) && validatesOnDataErrors) vm.ErrorsChanged += self.VmOnErrorsChanged;
        }

        private void VmOnErrorsChanged(object sender, DataErrorsChangedEventArgs args)
        {
            var model = (INotifyDataErrorInfo)sender;
            _binding = _binding ?? this.GetBinding(TextProperty);
            if (string.IsNullOrWhiteSpace(_binding.Path)) throw new ArgumentNullException($"{nameof(_binding.Path)} cannot be null");
            if (args.PropertyName != _binding.Path) return;
            var error = model.GetErrors(args.PropertyName)?.Cast<string>().First();
            ErrorText = error;
        }
    }
}