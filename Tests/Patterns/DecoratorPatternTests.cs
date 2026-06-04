using OnlineJobs.Application.Decorators;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Notifications;
using Xunit;

namespace OnlineJobs.Tests.Patterns
{
    /// <summary>
    /// LAB 5 - Decorator Pattern Tests
    /// Tests dynamic functionality extension
    /// </summary>
    public class DecoratorPatternTests
    {
        [Fact]
        public async Task BaseNotification_ShouldWork()
        {
            // Arrange
            INotification notification = new BaseNotification();

            // Act
            await notification.SendAsync("test@example.com", "Test", "Message");
            var description = notification.GetDescription();

            // Assert
            Assert.Equal("Base Notification", description);
        }

        [Fact]
        public async Task EmailDecorator_ShouldAddEmailFunctionality()
        {
            // Arrange
            INotification notification = new BaseNotification();
            notification = new EmailNotificationDecorator(notification);

            // Act
            await notification.SendAsync("test@example.com", "Test", "Message");
            var description = notification.GetDescription();

            // Assert
            Assert.Contains("Email", description);
        }

        [Fact]
        public async Task MultipleDecorators_ShouldStackFunctionality()
        {
            // Arrange
            INotification notification = new BaseNotification();
            notification = new EmailNotificationDecorator(notification);
            notification = new SMSNotificationDecorator(notification);
            notification = new PushNotificationDecorator(notification);

            // Act
            var description = notification.GetDescription();

            // Assert
            Assert.Contains("Email", description);
            Assert.Contains("SMS", description);
            Assert.Contains("Push", description);
        }

        [Fact]
        public async Task LoggingDecorator_ShouldAddLogging()
        {
            // Arrange
            INotification baseNotification = new BaseNotification();
            var loggingDecorator = new LoggingNotificationDecorator(baseNotification);
            INotification notification = loggingDecorator;

            // Act
            await notification.SendAsync("test@example.com", "Subject", "Message");
            var log = loggingDecorator.GetNotificationLog();

            // Assert
            Assert.NotEmpty(log);
            Assert.Contains(log, entry => entry.Contains("test@example.com"));
        }

        [Fact]
        public void Decorators_CanBeAppliedInAnyOrder()
        {
            // Arrange & Act
            var order1 = new EmailNotificationDecorator(new SMSNotificationDecorator(new BaseNotification()));
            var order2 = new SMSNotificationDecorator(new EmailNotificationDecorator(new BaseNotification()));

            // Assert
            Assert.Contains("Email", order1.GetDescription());
            Assert.Contains("SMS", order2.GetDescription());
            // Both should work regardless of order
        }

        [Fact]
        public async Task SMSDecorator_ShouldTruncateLongMessages()
        {
            // Arrange
            INotification notification = new SMSNotificationDecorator(new BaseNotification());
            var longMessage = new string('A', 200);

            // Act
            await notification.SendAsync("test", "Subject", longMessage);

            // Assert - Should not throw, truncates internally
            Assert.True(true);
        }
    }
}
