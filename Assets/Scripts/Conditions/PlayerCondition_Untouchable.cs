public class PlayerCondition_Untouchable : PlayerConditionBase
{
    protected override void OnEnterCondition() {

        // 指定された以外のシンボルを非表示にする
        //SymbolManager.instance.SwitchActivateExceptSymbols(false, (int)conditionValue);

        base.OnEnterCondition();
    }

    protected override void OnExitCondition() {

        // 終了時の演出

        // シンボルを表示する
        //SymbolManager.instance.SwitchActivateExceptSymbols(true, (int)conditionValue);

        base.OnExitCondition();
    }
}