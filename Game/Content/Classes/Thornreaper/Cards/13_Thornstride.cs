using System.Collections.Generic;
using System.Security.Cryptography;
using Fractural.Tasks;
using Godot;

public class Thornstride : ThornreaperCardModel<Thornstride.CardTop, Thornstride.CardBottom>
{
	public override string Name => "Thornstride";
	public override int Level => 1;
	public override int Initiative => 24;
	protected override int AtlasIndex => 29 - 13;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithAOEPattern(new AOEPattern([
					new AOEHex(Vector2I.Zero,AOEHexType.Gray),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
					new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast), AOEHexType.Red),
				]),
				new AOEHexMark(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.NorthEast).Add(Direction.NorthEast), this, new Vector2(0.5f, 0.4f)),
				new AOEHexMark(Vector2I.Zero.Add(Direction.SouthWest), this, new Vector2(0.5f, 0.4f)))
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.New(
						parameters => parameters.AbilityState.Performer.Hex.HasHexObjectOfType<HazardousTerrain>(),
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustPierce(1);
							await AbilityCmd.GainXP(parameters.Performer, 1);
						})
					)
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => parameters.AbilityState.Target.Hex.HasHexObjectOfType<HazardousTerrain>(),
						async parameters =>
						{
							parameters.AbilityState.AbilityAdjustAttackValue(1);
							await GDTask.CompletedTask;
						}
					)
				)
				.Build())
		];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.WithOnAbilityStarted(async abilityState =>
				{
					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(abilityState, this,
						canApplyParameters => canApplyParameters.PotentialAbilityState?.Performer == abilityState.Performer,
						applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							return GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnAbilityEnded(async abilityState =>
				{
					ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);

					await GDTask.CompletedTask;
				})
				.Build()),
		];
	}
}