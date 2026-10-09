using System.Collections.Generic;
using System.Data;
using Fractural.Tasks;
using Godot;

public class ShrewdOvergrowth : ThornreaperCardModel<ShrewdOvergrowth.CardTop, ShrewdOvergrowth.CardBottom>
{
	public override string Name => "Shrewd Overgrowth";
	public override int Level => 1;
	public override int Initiative => 92;
	protected override int AtlasIndex => 29 - 8;

	public class CardTop : ThornreaperCardSide
	{
		bool roundPlayed = true;
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async abilityState =>
				{
					Hex hex = abilityState.Performer.Hex;

					if (hex.IsFeatureless())
					{
						List<Hex> selectedHexes =
							await AbilityCmd.SelectHexes(abilityState, list => list.Add(hex), 0, 1, true, "Create hazardous terrain?");

						foreach (Hex selectedHex in selectedHexes)
						{
							await CreateHazardousTerrain(selectedHex);
							abilityState.SetPerformed();
						}
					}
				})
				.Build()),

			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async abilityState =>
				{
					ScenarioEvents.RoundEndedEvent.Subscribe(abilityState, this, parameters => true,
						async parameters =>
						{
							if (roundPlayed)
							{
								await AbilityCmd.AddRetaliate(abilityState.Performer, this, 2, 1);
								await AbilityCmd.AddShield(abilityState.Performer, this, 2);
								ScenarioCheckEvents.PotentialTargetCheckEvent.Subscribe(abilityState, this,
								parameters => parameters.PotentialTarget == abilityState.Performer,
								parameters =>
								{
									parameters.AdjustTargetSortingInitiative(1);
								});
								roundPlayed = false;
							}
							else
							{
								await abilityState.RemoveFromActive();
							}
							await GDTask.CompletedTask;
						});
					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async state =>
				{
					ScenarioEvents.RoundEndedEvent.Unsubscribe(state, this);
					ScenarioCheckEvents.PotentialTargetCheckEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				})
				.Build())
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
						if (GameController.Instance.ElementManager.GetState(Element.Light) is ElementState.Strong or ElementState.Waning)
						{
							await AbilityCmd.InfuseElement(state, Element.Earth);
						}
					})
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
				.WithMoveType(MoveType.Jump)
				.Build())
		];
	}
}