using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// SkillEffectYpe と効果を自動登録(紐づけ)し、取り出すクラス
/// </summary>
public static class SkillEffectHandlerFactory {
    private static readonly Dictionary<SkillEffectType, SkillEffectHandlerBase> handlers;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeSkillFactory() {
        // 呼び出された時点で static コンストラクタが走る
        _ = handlers;
    }

    static SkillEffectHandlerFactory() {
        handlers = new Dictionary<SkillEffectType, SkillEffectHandlerBase>();

        // リフレクションで全アセンブリを走査して、SkillEffectHandlerBase を実装したクラスを探す
        var handlerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(SkillEffectHandlerBase).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

        DebugLogger.Log($"handlerTypes : {handlerTypes.Count()}");

        // 見つかった ISkillEffectHandler を順番に取り出して処理する
        foreach (var type in handlerTypes) {
            // 各クラスが「どの効果タイプに対応しているか」を調べる
            SkillEffectAttribute attribute = type.GetCustomAttribute<SkillEffectAttribute>();
            if (attribute != null) {
                // 1回インスタンス化して辞書に登録
                SkillEffectHandlerBase instance = (SkillEffectHandlerBase)Activator.CreateInstance(type);
                handlers[attribute.EffectType] = instance;
                DebugLogger.Log($"handlers[attribute.EffectType] : {handlers[attribute.EffectType]}");
            }
        }

        DebugLogger.Log($"SkillEffectHandlerFactory: Registered {handlers.Count} handlers.");
    }

    /// <summary>
    /// Enum で指定した効果を適用する
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static SkillEffectHandlerBase GetHandler(SkillEffectType type) {
        if (handlers.TryGetValue(type, out SkillEffectHandlerBase handler)) {
            return handler;
        }

        DebugLogger.Log($"Handler not found for {type}");
        return null;
    }
}