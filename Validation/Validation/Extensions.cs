using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace Validation
{
    public static class BindingExtensions
    {
        public static Binding GetBinding(this BindableObject self, BindableProperty property)
        {
            var methodInfo = typeof(BindableObject).GetTypeInfo().GetDeclaredMethod("GetContext");
            var context = methodInfo?.Invoke(self, new[] { property });
            var propertyInfo = context?.GetType().GetTypeInfo().GetDeclaredField("Binding");
            return propertyInfo?.GetValue(context) as Binding;
        }
    }
}
