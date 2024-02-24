﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Flow.Launcher.Infrastructure.Logger;
using Flow.Launcher.ViewModel;

namespace Flow.Launcher.Converters
{
    public class QuerySuggestionBoxConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is not [TextBox queryTextBox, ResultViewModel selectedItem, string queryText] ||
                string.IsNullOrEmpty(queryText))
                return string.Empty;

            try
            {

                var selectedResult = selectedItem.Result;
                var selectedResultActionKeyword = string.IsNullOrEmpty(selectedResult.ActionKeywordAssigned) ? "" : selectedResult.ActionKeywordAssigned + " ";
                var selectedResultPossibleSuggestion = selectedResultActionKeyword + selectedResult.Title;

                if (!selectedResultPossibleSuggestion.StartsWith(queryText, StringComparison.CurrentCultureIgnoreCase))
                    return string.Empty;


                // For AutocompleteQueryCommand.
                // When user typed lower case and result title is uppercase, we still want to display suggestion
                selectedItem.QuerySuggestionText = string.Concat(queryText, selectedResultPossibleSuggestion.AsSpan(queryText.Length));

                // Check if Text will be larger than our QueryTextBox
                Typeface typeface = new Typeface(queryTextBox.FontFamily, queryTextBox.FontStyle, queryTextBox.FontWeight, queryTextBox.FontStretch);
                // TODO: Obsolete warning?
                var ft = new FormattedText(queryTextBox.Text, CultureInfo.DefaultThreadCurrentCulture, System.Windows.FlowDirection.LeftToRight, typeface, queryTextBox.FontSize, Brushes.Black);

                var offset = queryTextBox.Padding.Right;

                if (ft.Width + offset > queryTextBox.ActualWidth || queryTextBox.HorizontalOffset != 0)
                    return string.Empty;

                return selectedItem.QuerySuggestionText;
            }
            catch (Exception e)
            {
                Log.Exception(nameof(QuerySuggestionBoxConverter), "fail to convert text for suggestion box", e);
                return string.Empty;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
