namespace GlossaryTrainer.Models;

public class GlossaryItem
{
    public GlossaryItem(string word, List<string> validTranslations)
    {
        Word = word;
        ValidTranslations = validTranslations;
    }
    public string Word { get; init; }

    // Allow multiple valid translations
    public List<string> ValidTranslations { get; init; } = new();
}