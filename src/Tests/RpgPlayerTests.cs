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
            var itemsService = new Items(engine);
            Item item = ItemBuilder.Build;

            itemsService.Inventory.Should().BeEmpty();

            // Act
            itemsService.PickUpItem(item);

            // Assert
            itemsService.Inventory.Should().Contain(item);
        }

        [Fact]
        public void PickUpItem_ThatGivesHealth_HealthIsIncreaseAndItemIsNotAddedToInventory()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var itemsService = new Items(engine)
            {
                MaxHealth = 100,
                Health = 10
            };

            Item healthPotion = ItemBuilder.Build.WithHeal(100);

            // Act
            itemsService.PickUpItem(healthPotion);

            // Assert
            itemsService.Inventory.Should().BeEmpty();
            itemsService.Health.Should().Be(100);
        }

        [Fact]
        public void PickUpItem_ThatGivesHealth_HealthDoesNotExceedMaxHealth()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var itemsService = new Items(engine)
            {
                MaxHealth = 50,
                Health = 10
            };

            Item healthPotion = ItemBuilder.Build.WithHeal(100);

            // Act
            itemsService.PickUpItem(healthPotion);

            // Assert
            itemsService.Inventory.Should().BeEmpty();
            itemsService.Health.Should().Be(50);
        }

        [Fact]
        public void PickUpItem_ThatIsRare_ASpecialEffectShouldBePlayed()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var itemsService = new Items(engine);
            Item rareItem = ItemBuilder.Build.IsRare(true);

            // Act
            itemsService.PickUpItem(rareItem);

            // Assert
            engine.Received().PlaySpecialEffect("cool_swirly_particles");
        }

        [Fact]
        public void PickUpItem_ThatIsUnique_ItShouldNotBePickedUpIfThePlayerAlreadyHasIt()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var itemsService = new Items(engine);
            itemsService.PickUpItem(ItemBuilder.Build.WithId(100));

            Item uniqueItem = ItemBuilder.Build.WithId(100).IsUnique(true);

            // Act
            var result = itemsService.PickUpItem(uniqueItem);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void PickUpItem_ThatDoesMoreThan500Healing_AnExtraGreenSwirlyEffectOccurs()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var itemsService = new Items(engine);
            Item xPotion = ItemBuilder.Build.WithHeal(501);

            // Act
            itemsService.PickUpItem(xPotion);

            // Assert
            engine.Received().PlaySpecialEffect("green_swirly");
        }

        [Fact]
        public void PickUpItem_ThatGivesArmour_ThePlayersArmourValueShouldBeIncreased()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var player = new RpgPlayer(engine);    
            var itemsService = new Items(engine);

            player.Armour.Should().Be(0);

            Item armour = ItemBuilder.Build.WithArmour(100);

            // Act
            itemsService.PickUpItem(armour);

            // Assert
            itemsService.Armour.Should().Be(100);
        }

        [Fact]
        public void PickUpItem_ThatIsTooHeavy_TheItemIsNotPickedUp()
        {
            // Arrange
            var engine = Substitute.For<IGameEngine>();
            var itemsService = new Items(engine);
            Item heavyItem = ItemBuilder.Build.WithWeight(itemsService.CarryingCapacity + 1);

            // Act
            var result = itemsService.PickUpItem(heavyItem);

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
            var player = new RpgPlayer(engine) {Health = 200, Armour = 50 };

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
            var player = new RpgPlayer(engine) {Health = 200, Armour = 2000};

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
            var enemy = Substitute.For<IEnemy>();

            var itemsService = new Items(engine);

            Item item = ItemBuilder.Build.WithName("Stink Bomb");
            engine.GetEnemiesNear(itemsService).Returns(new List<IEnemy> {enemy});

            // Act
            itemsService.UseItem(item);

            // Assert
            enemy.Received().TakeDamage(100);
        }
    }
}