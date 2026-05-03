namespace GlossaryTrainer.Models;

public class GlossaryItem
{
    public GlossaryItem(string word, List<string> validTranslations, bool useJapaneseFont = false)
    {
        Word = word;
        ValidTranslations = validTranslations;
        UseJapaneseFont = useJapaneseFont;
    }
    public string Word { get; }
    public bool UseJapaneseFont { get; }
    public List<string> ValidTranslations { get; init; } = new();
}