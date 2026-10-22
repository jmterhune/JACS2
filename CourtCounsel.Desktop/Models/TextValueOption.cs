namespace CourtCounsel.Desktop.Models;

// Plain class (not a ValueTuple) so XAML bindings like SelectedValuePath="Value"
// can find real, reflectable Text/Value properties at runtime.
public class TextValueOption
{
    public string Text { get; }
    public string Value { get; }

    public TextValueOption(string text, string value)
    {
        Text = text;
        Value = value;
    }
}
