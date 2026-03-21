namespace GlossaryTrainer.Models;

internal static class GlossaryRepo
{
    public static List<Glossary> Load()
    =>
    [
        new("Section 3, unit 28 - Describe health issues",[

            ]),

        new("Section 3, unit 28 - Describe health issues",[
            new("to put in / to insert / to add (object / reservation)", ["いれます", "入れます"], Tooltip.Iremasu),
            new("What’s wrong? / What happened?", ["どうしましたか"], Tooltip.Doushimashitaka),
            new("It hurts / painful", ["いたい", "痛い"], Tooltip.Itai),
            new("head", ["あたま", "頭"], Tooltip.Atama),
            new("neck", ["くび", "首"], Tooltip.Kubi),
            new("electricity / electric / light", ["でんき", "電気"], Tooltip.Denki),
            new("social media", ["えすえぬえす", "SNS"], Tooltip.SNS),
            new("almost / mostly / nearly all", ["ほとんど"], Tooltip.Hotondo),
            new("arm", ["うで", "腕"], Tooltip.Ude),
            new("to fall / to tumble", ["ころびます", "転びます"], Tooltip.Korobimasu),
            new("to hit / to strike / to bump / to get hit", ["うちます", "打ちます"], Tooltip.Uchimasu),
            new("wound / cut / injury", ["きず", "傷"], Tooltip.Kizu),
            new("pharmacy / drugstore", ["やっきょく", "薬局"], Tooltip.Yakkyoku),
            new("for a while / for some time", ["しばらく"], Tooltip.Shibaraku),
            new("to get better / to recover / to be fixed", ["なおります", "治ります"], Tooltip.Naorimasu),
            new("to paint / to apply / to spread", ["ぬります", "塗ります"], Tooltip.Nurimasu),
        ]),

        new("Section 3, unit 27 - Use past tense adjectives",[
            new("shelf", ["たな", "棚"]),
            new("husband", ["だんなさん", "旦那さん"]),
            new("wife", ["おくさん", "奥さん"]),
            new("entryway", ["げんかん", "玄関"]),
            new("to wear (lower body / feet)", ["はきます", "履きます"]),
            new("to wear (upper body / whole body)", ["きます", "着ます"]),
            new("whose", ["だれの", "誰の"]),
            new("gate", ["もん", "門"]),
            new("furniture", ["かぐ", "家具"]),
            new("between / space / interval / gap", ["あいだ", "間"]),
            new("vase", ["かびん", "花瓶"]),
            new("dining room / dining", ["ダイニング"]),
            new("handmade", ["てづくり", "手作り"]),
            new("wall", ["かべ", "壁"]),
            ]),

        new("Archive",[
            new("onion", ["たまねぎ", "玉ねぎ"]),
            new("sugar", ["さとう", "砂糖"]),
            new("potato", ["じゃがいも", "じゃがいも"]),
            new("wheat flour / flour", ["こむぎこ", "小麦粉"]),

            new("life / daily life", ["せいかつ", "生活"]),
            new("traditional flooring", ["たたみ", "畳"]),
            new("late / slowly", ["おそく", "遅く"]),

            new("to turn on / to attach", ["つけます", "付けます"]),
            new("to arrive / will arrive", ["つきます", "着きます"]),
            new("merch", ["グッズ", "グッズ"]),
            new("to go out / head out (outing/activity)", ["でかけます", "出かけます"]),
            new("to leave / to exit / to come out (general verb for exiting a place)", ["でます", "出ます"]),
            new("pay, going to pay, will pay", ["はらいます", "払います"]),
            new("enter/enroll/join", ["はいります", "入ります"]),

            new("many / numerous / a large number of", ["おおい", "多い"]),
            new("front desk", ["うけつけ", "受付"]),
            ])
    ];
}
