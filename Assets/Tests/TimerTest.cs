// 用于测试 Timer 系统

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace TestScript
{
    [TestFixture]
    public class TimerTest
    {
        [UnityTest]
        public IEnumerator TimerTrigger()
        {
            // Arrange
            int CallCount = 0;
            FTimerHandle TimerHandle = TimerSubsystem.GetSubsystem().AddTimer(
                0.8f,
                false,
                false,
                () => { CallCount++; }
            );

            // Act - 等待第一次触发
            yield return new WaitForSeconds(0.85f);

            // Assert
            Assert.That(CallCount, Is.EqualTo(1), "循环定时器应该在0.8秒后第一次触发");

            // Cleanup
            TimerSubsystem.GetSubsystem().RemoveTimer(TimerHandle);
        }

        [UnityTest]
        public IEnumerator LoopTimerTrigger()
        {
            // Arrange
            int CallCount = 0;
            FTimerHandle TimerHandle = TimerSubsystem.GetSubsystem().AddTimer(
                0.8f,
                true,
                false,
                () => { CallCount++; }
            );

            // Act - 等待两次触发
            yield return new WaitForSeconds(0.85f);
            yield return new WaitForSeconds(0.85f);

            // Assert
            Assert.That(CallCount, Is.EqualTo(2), "循环定时器应该触发两次");

            // Cleanup
            TimerSubsystem.GetSubsystem().RemoveTimer(TimerHandle);
        }

        [UnityTest]
        public IEnumerator LoopTimerRemove()
        {
            // Arrange
            int CallCount = 0;
            FTimerHandle TimerHandle = TimerSubsystem.GetSubsystem().AddTimer(
                0.8f,
                true,
                false,
                () => { CallCount++; }
            );

            // 等待第一次触发
            yield return new WaitForSeconds(0.85f);
            int CountBeforeRemoval = CallCount;

            // Act - 删除定时器
            TimerSubsystem.GetSubsystem().RemoveTimer(TimerHandle);

            // 等待，验证不会再触发
            yield return new WaitForSeconds(0.85f);

            // Assert
            Assert.That(CallCount, Is.EqualTo(CountBeforeRemoval), 
                "删除定时器后不应该再触发");
        }
        
        [UnityTest]
        public IEnumerator LoopTimerFastMultipleTrigger()
        {
            // Arrange
            int CallCount = 0;
            FTimerHandle TimerHandle = TimerSubsystem.GetSubsystem().AddTimer(
                0.001f,
                true,
                false,
                () => { CallCount++; }
            );

            // Act - 等待两次触发
            yield return new WaitForSeconds(0.003f);

            // Assert
            Assert.That(CallCount, Is.AtLeast(2), "循环定时器应最少触发两次");

            // Cleanup
            TimerSubsystem.GetSubsystem().RemoveTimer(TimerHandle);
        }
    }
}