using OnlineJobs.Application.Factories;
using OnlineJobs.Domain.Flyweights;
using OnlineJobs.Domain.ValueObjects;
using Xunit;

namespace OnlineJobs.Tests.Patterns
{

    public class FlyweightPatternTests
    {
        [Fact]
        public void SkillFactory_ShouldReturnSameInstanceForSameSkill()
        {
            var factory = new SkillFlyweightFactory();

            // Act
            var skill1 = factory.GetSkill("C#", "Programming");
            var skill2 = factory.GetSkill("C#", "Programming");

            // Assert
            Assert.Same(skill1, skill2); // Same reference
        }

        [Fact]
        public void SkillFactory_ShouldReturnDifferentInstancesForDifferentSkills()
        {
            // Arrange
            var factory = new SkillFlyweightFactory();

            // Act
            var csharp = factory.GetSkill("C#", "Programming");
            var java = factory.GetSkill("Java", "Programming");

            // Assert
            Assert.NotSame(csharp, java);
        }

        [Fact]
        public void SkillFactory_ShouldTrackPoolSize()
        {
            // Arrange
            var factory = new SkillFlyweightFactory();

            // Act
            factory.GetSkill("C#", "Programming");
            factory.GetSkill("JavaScript", "Programming");
            factory.GetSkill("C#", "Programming"); // Reuse

            // Assert
            Assert.Equal(2, factory.GetPoolSize()); // Only 2 unique skills
        }

        [Fact]
        public void JobSkillRequirement_ShouldCombineIntrinsicAndExtrinsicState()
        {
            // Arrange
            var factory = new SkillFlyweightFactory();
            var sharedSkill = factory.GetSkill("Python", "Programming");

            // Act
            var requirement1 = new JobSkillRequirement(sharedSkill, SkillProficiency.Expert, 5, true);
            var requirement2 = new JobSkillRequirement(sharedSkill, SkillProficiency.Intermediate, 2, false);

            // Assert
            Assert.Same(sharedSkill, requirement1.Skill); // Shared flyweight
            Assert.Same(sharedSkill, requirement2.Skill); // Shared flyweight
            Assert.NotEqual(requirement1.RequiredProficiency, requirement2.RequiredProficiency); // Different extrinsic
        }

        [Fact]
        public void SkillFlyweight_ShouldBeImmutable()
        {
            // Arrange & Act
            var skill = new SkillFlyweight("C#", "Programming");

            // Assert
            Assert.Equal("C#", skill.Name);
            Assert.Equal("Programming", skill.Category);
        }

        [Fact]
        public void SkillFactory_ClearShouldEmptyPool()
        {
            // Arrange
            var factory = new SkillFlyweightFactory();
            factory.GetSkill("C#", "Programming");
            factory.GetSkill("Java", "Programming");

            // Act
            factory.Clear();

            // Assert
            Assert.Equal(0, factory.GetPoolSize());
        }
    }
}
