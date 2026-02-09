namespace GlossaryTrainer.Models;

public class GlossaryItem
{
    public string Word { get; init; }

    // Allow multiple valid translations
    public List<string> ValidTranslations { get; init; } = new();
}