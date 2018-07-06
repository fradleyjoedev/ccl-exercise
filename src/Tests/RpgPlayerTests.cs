using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using SrpTask;
using Xunit;

namespace Tests
{
    public class RpgPlayerTests
    {
        [Fact]
        public void PickUpItem_ThatCanBePickedUp_ItIsAddedToTheInventory()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine);
            Item item = ItemBuilder.Build;

            player.Inventory.Should().BeEmpty();

            // Act
            player.PickUpItem(item);

            // Assert
            player.Inventory.Should().Contain(item);
        }

        [Fact]
        public void PickUpItem_ThatGivesHealth_HealthIsIncreaseAndItemIsNotAddedToInventory()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine)
            {
                MaxHealth = 100,
                Health = 10
            };

            Item healthPotion = ItemBuilder.Build.WithHeal(100);

            // Act
            player.PickUpItem(healthPotion);

            // Assert
            player.Inventory.Should().BeEmpty();
            player.Health.Should().Be(100);
        }

        [Fact]
        public void PickUpItem_ThatGivesHealth_HealthDoesNotExceedMaxHealth()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine)
            {
                MaxHealth = 50,
                Health = 10
            };

            Item healthPotion = ItemBuilder.Build.WithHeal(100);

            // Act
            player.PickUpItem(healthPotion);

            // Assert
            player.Inventory.Should().BeEmpty();
            player.Health.Should().Be(50);
        }

        [Fact]
        public void PickUpItem_ThatIsRare_ASpecialEffectShouldBePlayed()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine);
            Item rareItem = ItemBuilder.Build.IsRare(true);

            // Act
            player.PickUpItem(rareItem);

            // Assert
            engine.Received().PlaySpecialEffect("cool_swirly_particles");
        }

        [Fact]
        public void PickUpItem_ThatIsUnique_ItShouldNotBePickedUpIfThePlayerAlreadyHasIt()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine);
            player.PickUpItem(ItemBuilder.Build.WithId(100));

            Item uniqueItem = ItemBuilder.Build.WithId(100).IsUnique(true);

            // Act
            var result = player.PickUpItem(uniqueItem);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void PickUpItem_ThatDoesMoreThan500Healing_AnExtraGreenSwirlyEffectOccurs()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine);
            Item xPotion = ItemBuilder.Build.WithHeal(501);

            // Act
            player.PickUpItem(xPotion);

            // Assert
            engine.Received().PlaySpecialEffect("green_swirly");
        }

        [Fact]
        public void PickUpItem_ThatGivesArmour_ThePlayersArmourValueShouldBeIncreased()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine);
            player.Armour.Should().Be(0);

            Item armour = ItemBuilder.Build.WithArmour(100);

            // Act
            player.PickUpItem(armour);

            // Assert
            player.Armour.Should().Be(100);
        }

        [Fact]
        public void PickUpItem_ThatIsTooHeavy_TheItemIsNotPickedUp()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine);
            Item heavyItem = ItemBuilder.Build.WithWeight(player.CarryingCapacity + 1);

            // Act
            var result = player.PickUpItem(heavyItem);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TakeDamage_WithNoArmour_HealthIsReducedAndParticleEffectIsShown()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine)
            {
                Health = 200
            };

            // Act
            player.TakeDamage(100);

            // Assert
            player.Health.Should().Be(100);
            engine.Received().PlaySpecialEffect("lots_of_gore");
        }

        [Fact]
        public void TakeDamage_With50Armour_DamageIsReducedBy50AndParticleEffectIsShown()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine) {Health = 200};
            player.PickUpItem(ItemBuilder.Build.WithArmour(50));

            // Act
            player.TakeDamage(100);

            // Assert
            player.Health.Should().Be(150);
            engine.Received().PlaySpecialEffect("lots_of_gore");
        }

        [Fact]
        public void TakeDamage_WithMoreArmourThanDamage_NoDamageIsDealtAndParryEffectIsPlayed()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine) {Health = 200};
            player.PickUpItem(ItemBuilder.Build.WithArmour(2000));

            // Act
            player.TakeDamage(100);

            // Assert
            player.Health.Should().Be(200);
            engine.Received().PlaySpecialEffect("parry");
        }

        [Fact]
        public void UseItem_StinkBomb_AllEnemiesNearThePlayerAreDamaged()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine);
            var enemy = Substitute.For<IEnemy>();

            Item item = ItemBuilder.Build.WithName("Stink Bomb");
            engine.GetEnemiesNear(player).Returns(new List<IEnemy> {enemy});

            // Act
            player.UseItem(item);

            // Assert
            enemy.Received().TakeDamage(100);
        }
    }
}