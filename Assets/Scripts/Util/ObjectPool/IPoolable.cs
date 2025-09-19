/// <summary>
/// オブジェクトプールを利用して再利用されるクラスに実装するインターフェース
/// 使用用途としては、エフェクト、敵、弾、ドロップアイテム、フロート表示など
/// </summary>
public interface IPoolable {
    void Release();
}