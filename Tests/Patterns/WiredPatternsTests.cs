using Moq;
using OnlineJobs.Application.Commands;
using OnlineJobs.Application.Commands.ApplicationCommands;
using OnlineJobs.Application.Decorators;
using OnlineJobs.Application.Factories;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Notifications;
using OnlineJobs.Application.Observers;
using OnlineJobs.Application.States.ApplicationStates;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.Enums;
using Xunit;

namespace OnlineJobs.Tests.Patterns
{
    /// <summary>
    /// Tests covering the patterns that were wired into real flows during the refactor:
    /// Command (undo/redo without reflection), Observer→in-app store, Decorator in-app
    /// channel, Memento store, Prototype clone safety, Flyweight sharing, State guards.
    /// </summary>
    public class WiredPatternsTests
    {
        // ---------------- Command: execute + undo restores previous status ----------
        [Fact]
        public async Task ApproveCommand_Undo_RestoresPreviousStatus()
        {
            var app = new JobApplication(Guid.NewGuid(), Guid.NewGuid(), "cover letter")
            {
                Status = ApplicationStatus.Interviewing
            };
            var repo = new Mock<IRepository<JobApplication>>();
            repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(app);
            repo.Setup(r => r.UpdateAsync(It.IsAny<JobApplication>())).Returns(Task.CompletedTask);

            var command = new ApproveApplicationCommand(repo.Object, app.Id);
            await command.ExecuteAsync();
            Assert.Equal(ApplicationStatus.Accepted, app.Status);

            await command.UndoAsync();
            Assert.Equal(ApplicationStatus.Interviewing, app.Status);
        }

        [Fact]
        public async Task CommandHistory_Undo_Then_Redo_Works()
        {
            var history = new CommandHistory();
            var invoker = new CommandInvoker(history);
            var app = new JobApplication(Guid.NewGuid(), Guid.NewGuid(), "cover")
            {
                Status = ApplicationStatus.Interviewing
            };
            var repo = new Mock<IRepository<JobApplication>>();
            repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(app);
            repo.Setup(r => r.UpdateAsync(It.IsAny<JobApplication>())).Returns(Task.CompletedTask);

            await invoker.ExecuteAsync(new ApproveApplicationCommand(repo.Object, app.Id));
            Assert.True(invoker.CanUndo());

            Assert.True(await invoker.UndoAsync());
            Assert.Equal(ApplicationStatus.Interviewing, app.Status);
            Assert.True(invoker.CanRedo());

            Assert.True(await invoker.RedoAsync());
            Assert.Equal(ApplicationStatus.Accepted, app.Status);
        }

        [Fact]
        public async Task UserCommandHistoryStore_KeepsHistoryPerUser()
        {
            var store = new UserCommandHistoryStore();
            var userA = Guid.NewGuid();
            var userB = Guid.NewGuid();
            Assert.Same(store.GetForUser(userA), store.GetForUser(userA)); // same instance across requests
            Assert.NotSame(store.GetForUser(userA), store.GetForUser(userB));
            await Task.CompletedTask;
        }

        // ---------------- Observer → in-app notification store -----------------------
        [Fact]
        public async Task StatusSubject_Notifies_DashboardObserver_WritesToStore()
        {
            var store = new NotificationStore();
            var seekerId = Guid.NewGuid();
            var subject = new ApplicationStatusSubject(
                new EmailAlertObserver(),
                new DashboardNotificationObserver(store),
                new AuditLogObserver(),
                new StatisticsObserver());

            var application = new JobApplication(Guid.NewGuid(), seekerId, "cover")
            {
                Status = ApplicationStatus.Accepted
            };
            await subject.NotifyAsync(application);

            Assert.Equal(1, store.UnreadCount(seekerId));
            Assert.Contains(store.GetForUser(seekerId), n => n.Message.Contains("Accepted"));
        }

        // ---------------- Decorator: in-app channel delivers to the store ------------
        [Fact]
        public async Task NotificationService_FullChain_DeliversInAppNotification()
        {
            var store = new NotificationStore();
            var service = new OnlineJobs.Application.Services.NotificationService(store);
            var seekerId = Guid.NewGuid();

            await service.SendApplicationConfirmationAsync(seekerId, "Senior Developer");

            Assert.Equal(1, store.UnreadCount(seekerId));
            Assert.Contains(store.GetForUser(seekerId), n => n.Title == "Application submitted");
        }

        [Fact]
        public async Task NotificationStore_MarkAllRead_ClearsUnread()
        {
            var store = new NotificationStore();
            var user = Guid.NewGuid();
            store.Add(user, "t", "m");
            Assert.Equal(1, store.UnreadCount(user));
            store.MarkAllRead(user);
            Assert.Equal(0, store.UnreadCount(user));
            await Task.CompletedTask;
        }

        // ---------------- Prototype: Clone is a fresh draft, ShallowCopy is safe -----
        [Fact]
        public void JobPosting_Clone_IsFreshDraftWithNewId()
        {
            var original = new JobPosting("Backend Engineer", "Build APIs", Guid.NewGuid(), Guid.NewGuid())
            {
                Status = JobStatus.Active
            };
            original.Applications.Add(new JobApplication(Guid.NewGuid(), Guid.NewGuid(), "x"));

            var clone = original.Clone();

            Assert.NotEqual(original.Id, clone.Id);
            Assert.Equal(JobStatus.Draft, clone.Status);
            Assert.Empty(clone.Applications);                 // doesn't carry applications
            Assert.Equal(original.Title, clone.Title);
        }

        [Fact]
        public void JobPosting_ShallowCopy_DoesNotShareApplications()
        {
            var original = new JobPosting("Backend Role", "Description here", Guid.NewGuid(), Guid.NewGuid());
            var copy = original.ShallowCopy();
            copy.Applications.Add(new JobApplication(Guid.NewGuid(), Guid.NewGuid(), "x"));
            Assert.Empty(original.Applications);              // original not corrupted
        }

        // ---------------- Flyweight: shared instances + interning -------------------
        [Fact]
        public void SkillFlyweightFactory_SharesInstances()
        {
            var factory = new SkillFlyweightFactory();
            var a = factory.GetSkill("C#", "Programming");
            var b = factory.GetSkill("c#", "programming"); // case-insensitive
            Assert.Same(a, b);
            Assert.Equal(1, factory.GetPoolSize());

            var skills = factory.GetSkills(new[] { "C#", "SQL", " sql ", "" }, "Programming");
            Assert.Equal(2, factory.GetPoolSize());          // C# already pooled, SQL added once
            Assert.Equal(3, skills.Count);                   // C#, SQL, sql(->same SQL); blank skipped
            Assert.Same(skills[1], skills[2]);               // "SQL" and " sql " share one flyweight
        }

        // ---------------- State: transition guards -----------------------------------
        [Fact]
        public void State_Submitted_AllowsReviewButNotAccept()
        {
            var state = ApplicationStateContext.GetStateFromStatus(ApplicationStatus.Submitted);
            Assert.True(state.CanTransitionTo("UnderReview"));
            Assert.False(state.CanTransitionTo("Accepted"));
        }
    }
}
