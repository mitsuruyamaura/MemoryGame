public class PlayerCondition_HideSymbol : PlayerConditionBase
{
    protected override void OnEnterCondition() {

        // シンボルの画像を非表示にする
        //SymbolManager.instance.SwitchDisplayAllSymbols(false);
 
        base.OnEnterCondition();
    }

    protected override void OnExitCondition() {

        // 終了時の演出

        // シンボルの画像を表示する
        //SymbolManager.instance.SwitchDisplayAllSymbols(true);

        base.OnExitCondition();
    }
}