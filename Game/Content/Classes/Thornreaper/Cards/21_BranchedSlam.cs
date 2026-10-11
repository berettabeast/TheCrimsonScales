using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BranchedSlam : ThornreaperLevelUpCardModel<BranchedSlam.CardTop, BranchedSlam.CardBottom>
{
	public override string Name => "Branched Slam";
	public override int Level => 5;
	public override int Initiative => 45;
	protected override int AtlasIndex => 29 - 21;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
					ScenarioEvents.AttackAfterTargetConfirmedEvent.Subscribe(state, this, 
						parameters => parameters.Performer == state.Performer &&
						parameters.AbilityState.AbilityRangeType == RangeType.Melee &&
						parameters.AbilityState.GetRedAOEHexes() != null,
						async parameters => 
						{
							var originalHexes = parameters.AbilityState.GetRedAOEHexes();
							var pivot = await AbilityCmd.SelectHex(parameters.AbilityState, list =>
							{
								foreach (Hex hex in originalHexes)
								{
									list.Add(hex);
								}
							}, true, "Select black hex for Branched Slam");

							var selectionAbility = AttackAbility.Builder()
								.WithDamage(null)
								.WithNoAMDCards()
								.WithCustomGetPerformHex(_ => pivot)
								.WithAOEPattern(new AOEPattern([
								new AOEHex(Vector2I.Zero, AOEHexType.Yellow),
								new AOEHex(Vector2I.Zero.Add(Direction.NorthEast), AOEHexType.Red),
								new AOEHex(Vector2I.Zero.Add(Direction.NorthEast).Add(Direction.East), AOEHexType.Red)
							]))
								.Build();

							await selectionAbility.Perform(state.ActionState);

							var addedHexes = state.ActionState.GetAbilityState<AttackAbility.State>(0)
								?.GetRedAOEHexes() ?? new List<Hex>();
								
							foreach (var hex in addedHexes)
							{
								parameters.AbilityState.AbilityAddAOEHex(new AOEHex(hex.Coords, AOEHexType.Red));
							}
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					await GDTask.CompletedTask;
				})
				.Build()),
		];

		public override int XP => 2;
		public override bool Loss => true;
		public override bool Persistent => true;
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					if (GetEarthElementState() == ElementState.Waning)
					{
						await AbilityCmd.InfuseElement(state, Element.Earth);
					}
					state.SetPerformed();
				})
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
				.Build())
		];
	}
}
