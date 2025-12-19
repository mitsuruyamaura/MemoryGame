using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 成長効果の対象を選択するインターフェース
/// </summary>
public interface ITargetSelector {
    IEnumerable<ITarget> SelectTargets(ITarget origin);
}

/// <summary>
/// 単一の対象のみ返すセレクター
/// </summary>
public class SingleTargetSelector : ITargetSelector {
    public IEnumerable<ITarget> SelectTargets(ITarget origin) {
        yield return origin;
    }
}

/// <summary>
/// 同じ種類のアイテムをすべて対象にするセレクター
/// </summary>
public class SameTypeSelector : ITargetSelector {
    private readonly IEnumerable<ITarget> allTargets;

    public SameTypeSelector(IEnumerable<ITarget> allTargets) {
        this.allTargets = allTargets;
    }

    public IEnumerable<ITarget> SelectTargets(ITarget origin) {
        if (origin is BackPackInItem item) {
            return allTargets.Where(t =>
                t is BackPackInItem i && i.itemData.itemType == item.itemData.itemType);
        }
        return Enumerable.Empty<ITarget>();
    }
}