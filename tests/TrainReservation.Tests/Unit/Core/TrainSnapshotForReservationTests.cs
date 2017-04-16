using NFluent;
using NUnit.Framework;
using TrainReservation.Mocks;

namespace TrainReservation.Tests.Unit.Core
{
    [TestFixture]
    public class TrainSnapshotForReservationTests
    {
        [Test]
        public void Should_be_able_to_reserve_70_percent_of_overall_train_capacity()
        {
            var train = TrainProviderHelper.GetTrainWith1CoachAnd10SeatsAvailable("trainId42");

            Check.That(train.OverallTrainCapacity).IsEqualTo(10);
            var option = train.FindReservationOption(7);
            Check.That(option.IsFullfiled).IsTrue();
        }

        [Test]
        public void Should_return_70_percent_of_OverallTrainCapacity_as_MaxReservableSeatsFollowingThePolicy()
        {
            var train = TrainProviderHelper.GetTrainWith1CoachAnd10SeatsAvailable("trainId42");
            Check.That(train.OverallTrainCapacity).IsEqualTo(10);
            Check.That(train.MaxReservableSeatsFollowingThePolicy).IsEqualTo(7);
        }

        [Test]
        public void Should_return_the_number_of_coach()
        {
            var train = TrainProviderHelper.GetTrainWith2CoachesAnd2IndividualSeatsAvailable("train43");

            Check.That(train.CoachCount).IsEqualTo(2);
        }

        [Test]
        public void Should_be_a_value_type()
        {
            var firstInstance = TrainProviderHelper.GetTrainWith1CoachAnd10SeatsAvailable("trainId42");
            var secondInstance = TrainProviderHelper.GetTrainWith1CoachAnd10SeatsAvailable("trainId42");
            Check.That(firstInstance).IsEqualTo(secondInstance);
        }
    }
}