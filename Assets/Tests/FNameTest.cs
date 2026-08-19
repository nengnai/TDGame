// 用于测试 FName 系统

using NUnit.Framework;

namespace TestScript
{
    [TestFixture]
    public class FNameTest
    {
        [Test]
        public void CreateName()
        {
            FName Name = new("请输入文本");
            
            Assert.That(Name.IsValid(), Is.True);
            Assert.That(Name.ToString(), Is.EqualTo("请输入文本"));
        }
        
        [Test]
        public void SameName()
        {
            FName A = new("请输入文本");
            FName B = new("请输入文本");
            
            Assert.That(A, Is.EqualTo(B));
            Assert.That(A == B, Is.True);
        }
        
        [Test]
        public void DifferentName()
        {
            FName A = new("请输入文本");
            FName B = new("Enemy");
            
            Assert.That(A == B, Is.False);
        }
        
        [Test]
        public void IsNone()
        {
            FName Name = default;
            
            Assert.That(Name.IsValid(), Is.False);
        }
    }
}