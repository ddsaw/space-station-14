using Content.Shared._RedTruce;
using NUnit.Framework;
using Robust.Shared.Random;

namespace Content.Tests.Shared._RedTruce.Rpg;

[TestFixture]
[TestOf(typeof(RTDicePoolResolver))]
public sealed class RTDicePoolResolverTests : ContentUnitTest
{
    [Test]
    public void Roll_ZeroPool_ReturnsNoFacesAndNoSuccesses()
    {
        var random = new RobustRandom();
        random.SetSeed(1);

        var result = RTDicePoolResolver.Roll(random, new RTDiceRollSpec(0, 10, 5));

        Assert.That(result.Successes, Is.EqualTo(0));
        Assert.That(result.Faces.Count, Is.EqualTo(0));
    }

    [Test]
    public void Roll_Seeded_IsDeterministic()
    {
        const int seed = 1337;
        var spec = new RTDiceRollSpec(8, 10, 6);

        var random = new RobustRandom();
        random.SetSeed(seed);
        var actual = RTDicePoolResolver.Roll(random, spec);

        var expected = ManualRoll(seed, spec);

        Assert.That(actual.Successes, Is.EqualTo(expected.Successes));
        Assert.That(actual.Faces, Is.EqualTo(expected.Faces));
    }

    [Test]
    public void Roll_TargetAboveSides_HasNoSuccesses()
    {
        var random = new RobustRandom();
        random.SetSeed(44);

        var result = RTDicePoolResolver.Roll(random, new RTDiceRollSpec(12, 10, 11));

        Assert.That(result.Successes, Is.EqualTo(0));
        Assert.That(result.Faces.Count, Is.EqualTo(12));
        Assert.That(result.Faces, Has.All.InRange(1, 10));
    }

    private static RTDiceRollResult ManualRoll(int seed, RTDiceRollSpec spec)
    {
        var random = new RobustRandom();
        random.SetSeed(seed);

        var result = new RTDiceRollResult();
        for (var i = 0; i < spec.Pool; i++)
        {
            var face = random.Next(1, spec.DiceSides + 1);
            result.Faces.Add(face);
            if (face >= spec.TargetNumber)
                result.Successes++;
        }

        return result;
    }
}
