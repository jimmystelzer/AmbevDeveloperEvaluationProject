// /home/jimmy/Downloads/Ambev/backend/tests/Ambev.DeveloperEvaluation.Unit/Domain/Specifications/CompositeSpecificationTests.cs
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Domain.Specifications;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications;

public class CompositeSpecificationTests
{
    [Fact]
    public void And_BothSpecificationsSatisfied_ReturnsTrue()
    {
        // Arrange
        var entity = new object();
        var left = Substitute.For<ISpecification<object>>();
        left.IsSatisfiedBy(entity).Returns(true);
        var right = Substitute.For<ISpecification<object>>();
        right.IsSatisfiedBy(entity).Returns(true);

        var andSpecification = new AndSpecification<object>(left, right);

        // Act
        var result = andSpecification.IsSatisfiedBy(entity);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void And_OneSpecificationNotSatisfied_ReturnsFalse()
    {
        // Arrange
        var entity = new object();
        var left = Substitute.For<ISpecification<object>>();
        left.IsSatisfiedBy(entity).Returns(true);
        var right = Substitute.For<ISpecification<object>>();
        right.IsSatisfiedBy(entity).Returns(false);

        var andSpecification = new AndSpecification<object>(left, right);

        // Act
        var result = andSpecification.IsSatisfiedBy(entity);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Or_OneSpecificationSatisfied_ReturnsTrue()
    {
        // Arrange
        var entity = new object();
        var left = Substitute.For<ISpecification<object>>();
        left.IsSatisfiedBy(entity).Returns(true);
        var right = Substitute.For<ISpecification<object>>();
        right.IsSatisfiedBy(entity).Returns(false);

        var orSpecification = new OrSpecification<object>(left, right);

        // Act
        var result = orSpecification.IsSatisfiedBy(entity);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Or_BothSpecificationsNotSatisfied_ReturnsFalse()
    {
        // Arrange
        var entity = new object();
        var left = Substitute.For<ISpecification<object>>();
        left.IsSatisfiedBy(entity).Returns(false);
        var right = Substitute.For<ISpecification<object>>();
        right.IsSatisfiedBy(entity).Returns(false);

        var orSpecification = new OrSpecification<object>(left, right);

        // Act
        var result = orSpecification.IsSatisfiedBy(entity);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Not_SpecificationSatisfied_ReturnsFalse()
    {
        // Arrange
        var entity = new object();
        var specification = Substitute.For<ISpecification<object>>();
        specification.IsSatisfiedBy(entity).Returns(true);

        var notSpecification = new NotSpecification<object>(specification);

        // Act
        var result = notSpecification.IsSatisfiedBy(entity);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Not_SpecificationNotSatisfied_ReturnsTrue()
    {
        // Arrange
        var entity = new object();
        var specification = Substitute.For<ISpecification<object>>();
        specification.IsSatisfiedBy(entity).Returns(false);

        var notSpecification = new NotSpecification<object>(specification);

        // Act
        var result = notSpecification.IsSatisfiedBy(entity);

        // Assert
        Assert.True(result);
    }
}
