namespace GlossaryTrainer.Models;

public class GlossaryItem
{
    public GlossaryItem(string word, List<string> validTranslations, string tooltip = "")
    {
        Word = word;
        ValidTranslations = validTranslations;
        Tooltip = tooltip;
    }
    public string Word { get; }
    public string Tooltip { get; }

    public List<string> ValidTranslations { get; init; } = new();
}