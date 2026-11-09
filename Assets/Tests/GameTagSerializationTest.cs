// 用于测试 GameTag / GameTagContainer 的序列化(存路径字符串)与反序列化(重建节点身份与索引)

using NUnit.Framework;
using UnityEngine;

namespace TestScript
{
    [TestFixture]
    public class GameTagSerializationTest
    {
        [System.Serializable]
        public class TagProbe
        {
            public FGameTag Single;
            public GameTagContainer Container = new GameTagContainer();
        }
        
        private static TagProbe MakeProbe()
        {
            TagProbe Probe = new();
            Probe.Single = new FGameTag("Gameplay.技能.技能A");
            Probe.Container.AddTag(new FGameTag("Gameplay.技能.技能A"));
            Probe.Container.AddTag(new FGameTag("State.眩晕"));
            return Probe;
        }
        
        // 字符串路径到节点
        [Test]
        public void Tag_JsonRoundTrip_KeepsNodeIdentity()
        {
            TagProbe Source = MakeProbe();

            string Json = JsonUtility.ToJson(Source);

            StringAssert.Contains("Gameplay.技能.技能A", Json, "序列化载荷应该是完整路径字符串");

            TagProbe Loaded = JsonUtility.FromJson<TagProbe>(Json);

            Assert.That(Loaded.Single.IsValid(), Is.True, "反序列化后节点应该被重建");
            Assert.That(Loaded.Single.ToString(), Is.EqualTo("Gameplay.技能.技能A"));
            Assert.That(Loaded.Single == new FGameTag("Gameplay.技能.技能A"), Is.True,
                "反序列化得到的 Tag 必须与新建的同名 Tag 相等(节点身份一致)");
            Assert.That(Loaded.Single.GetHashCode(),
                Is.EqualTo(new FGameTag("Gameplay.技能.技能A").GetHashCode()));
        }
        
        // 容器的运行期索引(继承关系)必须被重建
        [Test]
        public void Container_JsonRoundTrip_RebuildsInheritance()
        {
            TagProbe Source = MakeProbe();

            TagProbe Loaded = JsonUtility.FromJson<TagProbe>(JsonUtility.ToJson(Source));

            Assert.That(Loaded.Container.Count, Is.EqualTo(2), "两个 Tag 都应该被读回来");
            Assert.That(Loaded.Container.HasTagExact(new FGameTag("Gameplay.技能.技能A")), Is.True);
            Assert.That(Loaded.Container.HasTag(new FGameTag("Gameplay.技能")), Is.True, "父级标签应该命中");
            Assert.That(Loaded.Container.HasTag(new FGameTag("Gameplay")), Is.True, "根方向的所有父级都应该命中");
            Assert.That(Loaded.Container.HasTag(new FGameTag("Gameplay.其他")), Is.False, "没拥有过的分支不应该命中");
        }
        
        // 空字符串 / default 都是无效标签，且不会污染容器 */
        [Test]
        public void Tag_InvalidValues_AreIgnored()
        {
            Assert.That(new FGameTag("").IsValid(), Is.False);
            Assert.That(default(FGameTag).IsValid(), Is.False);

            GameTagContainer Container = new GameTagContainer();
            Container.AddTag(new FGameTag(""));
            Container.AddTag(default);

            Assert.That(Container.IsEmpty, Is.True);
            Assert.That(Container.HasTag(new FGameTag("")), Is.False);
        }
    }
}
