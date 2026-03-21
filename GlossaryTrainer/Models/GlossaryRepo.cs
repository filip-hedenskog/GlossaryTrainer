namespace GlossaryTrainer.Models;

internal static class GlossaryRepo
{
    public static List<Glossary> Load()
    =>
    [
        new("Relevant",[
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
