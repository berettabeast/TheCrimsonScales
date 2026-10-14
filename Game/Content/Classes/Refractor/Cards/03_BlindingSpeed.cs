using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class BlindingSpeed : RefractorCardModel<BlindingSpeed.CardTop, BlindingSpeed.CardBottom>
{
	public override string Name => "Extended Branch";
	public override int Level => 1;
	public override int Initiative => 61;
	protected override int AtlasIndex => 3;

	public class CardTop : RefractorCardSide
	{
		string _hintText = $"{Icons.HintText(Icons.Attack)}2";

		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.Build())
		];
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Subscribe(abilityState, this,
						canApplyParameters =>
							abilityState.Performer.AlliedWith(canApplyParameters.Performer) &&
							(canApplyParameters.Hex.HasHexObjectOfType<HazardousTerrain>() && canApplyParameters.Hex.Creator == abilityState.Performer),
						applyParameters =>
						{
							if(applyParameters.Hex.HasHexObjectOfType<HazardousTerrain>())
							{
								applyParameters.SetAffectedByNegativeHex(false);
							}
						});

					ScenarioEvents.HazardousTerrainTriggeredEvent.Subscribe(abilityState, this,
						canApplyParameters => abilityState.Performer.AlliedWith(canApplyParameters.PotentialAbilityState?.Performer) && canApplyParameters.Hex.Creator == abilityState.Performer,
						applyParameters =>
						{
							applyParameters.SetAffectedByHazardousTerrain(false);
							return GDTask.CompletedTask;
						});

					await GDTask.CompletedTask;
				})
				.WithOnDeactivate(async abilityState =>
				{
					ScenarioCheckEvents.MoveCheckEvent.Unsubscribe(abilityState, this);
					ScenarioEvents.HazardousTerrainTriggeredEvent.Unsubscribe(abilityState, this);

					await GDTask.CompletedTask;
				})
				.Build())
		];
		public override bool Loss => true;
		public override int XP => 1;
		public override bool Persistent => true;
	}
}