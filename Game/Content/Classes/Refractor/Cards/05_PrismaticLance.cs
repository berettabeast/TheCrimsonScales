using System.Collections.Generic;
using System.Linq;
using Fractural.Tasks;
using Godot;

public class PrismaticLance : RefractorCardModel<PrismaticLance.CardTop, PrismaticLance.CardBottom>
{
	public override string Name => "Prismatic Lance";
	public override int Level => 1;
	public override int Initiative => 47;
	protected override int AtlasIndex => 5;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RefractingAttackAbility.Builder()
				.WithDamage(3)
				.WithRange(2)
				.WithDuringAttackSubscription(
					ScenarioEvents.DuringAttack.Subscription.ConsumeElement(Element.Light,
						_ => true,
						async parameters =>
						{
							parameters.AbilityState.AbilitySetAOEPattern(new AOEPattern([
								new AOEHex(Vector2I.Zero, AOEHexType.Red), 
								new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red)
								]));

							await AbilityCmd.GainXP(parameters.Performer, 1);
						}))
				.WithAfterTargetConfirmedSubscription(
					ScenarioEvents.AttackAfterTargetConfirmed.Subscription.New(
						parameters => DidRefract(parameters.AbilityState),
						async parameters =>
						{
							await AbilityCmd.InfuseElement(parameters.AbilityState, Element.Light, parameters.Performer);
						}
					)
				)
				.Build())
		];
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(4)
				.Build())
		];
	}
}