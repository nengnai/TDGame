using NUnit.Framework;

namespace TestScript
{
    [TestFixture]
    public class GameTag
    {
        // 到字符串
        [Test]
        public void ToString()
        {
            FGameTag TempGameTag = new("State.Debuff.眩晕");
            
            Assert.That(TempGameTag.IsValid(), Is.True);
            Assert.That(TempGameTag.ToString(), Is.EqualTo("State.Debuff.眩晕"));
        }

        // 判断 Tag 是否相等
        [Test]
        public void Equals()
        {
            FGameTag GameTagA = new("State.Debuff.眩晕");
            FGameTag GameTagB = new("State.Debuff.眩晕");
            FGameTag GameTagC = new("State.Debuff.沉默");
            
            Assert.That(GameTagA == GameTagB, Is.True);
            Assert.That(GameTagA == GameTagC, Is.False);
        }
        
        // 容器匹配接口：全部匹配 / 任意匹配(含继承)
        [Test]
        public void Container_MatchContainer_WithInheritance()
        {
            GameTagContainer Owned = new GameTagContainer();
            Owned.AddTag(new FGameTag("Gameplay.技能.技能A"));

            GameTagContainer Required = new GameTagContainer();
            Required.AddTag(new FGameTag("Gameplay"));
            Required.AddTag(new FGameTag("Gameplay.技能"));

            Assert.That(Owned.MatchAllContainer(Required), Is.True, "两个父标签都应该被继承命中");
            Assert.That(Owned.MatchAnyContainer(Required), Is.True);

            GameTagContainer Missing = new GameTagContainer();
            Missing.AddTag(new FGameTag("Gameplay.技能.技能B"));
            Assert.That(Owned.MatchAllContainer(Missing), Is.False);
            Assert.That(Owned.MatchAnyContainer(Missing), Is.False);
        }
        
        // 同一路径的 Tag 共享同一个节点(引用比较的前提)
        [Test]
        public void Tag_SamePath_SharesNode()
        {
            FGameTag A = new("Shared.Path.Node");
            FGameTag B = new("Shared.Path.Node");

            Assert.That(A == B, Is.True);
            Assert.That(A.GetHashCode(), Is.EqualTo(B.GetHashCode()));

            // 同一路径共享同一个字符串实例(内存里不产生重复字符串)
            Assert.That(ReferenceEquals(A.ToString(), B.ToString()), Is.True);
        }
        
        // 重复添加后再移除一次，标签必须彻底消失(不能残留父级计数)
        [Test]
        public void Container_DuplicateAdd_RemoveOnce_ClearsTag()
        {
            GameTagContainer Container = new();
            Container.AddTag(new FGameTag("A.B.C"));
            Container.AddTag(new FGameTag("A.B.C"));

            Assert.That(Container.Count, Is.EqualTo(1), "重复添加不应该产生两份");

            Container.RemoveTag(new FGameTag("A.B.C"));

            Assert.That(Container.Count, Is.EqualTo(0));
            Assert.That(Container.HasTagExact(new FGameTag("A.B.C")), Is.False, "精确匹配应该已经移除");
            Assert.That(Container.HasTag(new FGameTag("A.B")), Is.False, "父级计数不应该残留");
            Assert.That(Container.HasTag(new FGameTag("A")), Is.False, "父级计数不应该残留");
        }
        
        // 只拥有父标签时，子标签不命中
        [Test]
        public void Container_ParentTag_DoesNotMatchChild()
        {
            GameTagContainer Container = new();
            Container.AddTag(new FGameTag("Gameplay.技能"));

            Assert.That(Container.HasTag(new FGameTag("Gameplay.技能")), Is.True);
            Assert.That(Container.HasTag(new FGameTag("Gameplay.技能.技能A")), Is.False);
        }
    }
}