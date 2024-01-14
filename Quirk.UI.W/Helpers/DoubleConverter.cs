using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace Quirk.UI.W.Helpers;

public class DoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double originalHeight && parameter is double adjustment)
        {
            // Adjust the original height by the specified amount
            var adjustedHeight = originalHeight + adjustment;

            // Ensure the height is not negative
            return adjustedHeight >= 0 ? adjustedHeight : 0;
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
