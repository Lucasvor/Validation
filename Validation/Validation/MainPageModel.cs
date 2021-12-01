using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Validation
{
    public class MainPageModel : BasePageModel
    {

        public MainPageModel(MainPageModelValidator validator) : base(validator)
        {
            SaveCommand = new Command(p => { }, p => !HasErrors);
            
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value, propertyChanged: () => ValidateProperty());
        }


        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, propertyChanged: () => ValidateProperty());
        }

        public Command SaveCommand { get; }
    }
}
