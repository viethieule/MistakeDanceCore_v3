namespace Application.Common.Dropdowns;

public class DropdownOptionDTO
{
    public DropdownOptionDTO(string value, string text)
    {
        Value = value;
        Text = text;
    }
    public string Value { get; set; }
    public string Text { get; set; }
}