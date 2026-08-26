using NUnit.Framework;

namespace TestScript
{
    [TestFixture]
    public class GameTag
    {
        [Test]
        public void ToString()
        {
            FGameTag TempGameTag = new("State.Debuff.眩晕");
            
            Assert.That(TempGameTag.IsValid(), Is.True);
            Assert.That(TempGameTag.ToString(), Is.EqualTo("State.Debuff.眩晕"));
        }

        [Test]
        public void Equals()
        {
            FGameTag GameTagA = new("State.Debuff.眩晕");
            FGameTag GameTagB = new("State.Debuff.眩晕");
            FGameTag GameTagC = new("State.Debuff.沉默");
            
            Assert.That(GameTagA == GameTagB, Is.True);
            Assert.That(GameTagA == GameTagC, Is.False);
        }
        
    }
}