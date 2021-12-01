using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Validation
{
    public class BasePageModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        protected readonly Dictionary<string, IList<string>> Errors = new Dictionary<string, IList<string>>();
        private readonly IValidator _validator;

        protected BasePageModel(IValidator validator)
        {
            _validator = validator;
        }

        protected BasePageModel()
        {
            // setup anything else
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            Errors.TryGetValue(propertyName, out var errorsForName);
            return errorsForName;
        }

        public bool HasErrors => Errors.Any(kv => kv.Value != null && kv.Value.Count > 0);
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual ValidationResult ValidateProperty([CallerMemberName] string propertyName = null)
        {
            if (_validator == null)
                throw new NullReferenceException("An instance of IValidator must be passed into the constructor of your ViewModel in order to call ValidateProperty");

            var teste = new MainPageModel(new MainPageModelValidator());

            var vt = typeof(AbstractValidator<>);
            var et = this.GetType();
            var evt = vt.MakeGenericType(et);
            var validatorType = FindValidatorType(Assembly.GetExecutingAssembly(), evt);

            var validatorInstance = (IValidator)Activator.CreateInstance(validatorType);
            // return validatorInstance.Validate((object)this);

            var val = new MainPageModelValidator();
            var context = new ValidationContext<BasePageModel>(this);
            

            var result = _validator.Validate(context);
            HandleValidationResultForProperty(result, propertyName);
            return result;
        }
        private Type FindValidatorType(Assembly assembly, Type evt)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            if (evt == null) throw new ArgumentNullException("evt");
            return assembly.GetTypes().FirstOrDefault(t => t.IsSubclassOf(evt));
        }
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = null, Action propertyChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;
            backingStore = value;
            propertyChanged?.Invoke();
            RaisePropertyChanged(propertyName);
            return true;
        }

        private void HandleValidationResultForProperty(ValidationResult result, string propertyName)
        {
            var parts = propertyName.Split('.');
            var validationPropertyName = parts.Length < 2 ? propertyName : string.Join(".", parts.Skip(1));
            var isPropertyValid = result.Errors.All(err => err.PropertyName != validationPropertyName);
            if (!isPropertyValid)
            {
                var errors = result.Errors.Where(e => e.PropertyName == validationPropertyName).Select(error => error.ErrorMessage).ToList();
                Errors[propertyName] = errors;
            }
            else
            {
                if (Errors.ContainsKey(propertyName))
                    Errors.Remove(propertyName);
            }

            RaiseErrorsChanged(propertyName);
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            RaisePropertyChanged(nameof(HasErrors));
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
