using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Fractural.Tasks;
using Godot;

public class SunBurn : RefractorCardModel<SunBurn.CardTop, SunBurn.CardBottom>
{
	public override string Name => "Encased in Thorns";
	public override int Level => 1;
	public override int Initiative => 22;
	protected override int AtlasIndex => 2;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RetaliateAbility.Builder()
				.WithRetaliateValue(1)
				.WithRange(2)
				.WithOnAbilityStarted(async abilityState =>
					{
						if (abilityState.Performer.Hex.HasHexObjectOfType<HazardousTerrain>())
						{
							abilityState.AdjustRetaliateValue(1);
							await AbilityCmd.GainXP(abilityState.Performer, 1);
						}
					})
				.Build()),
		];
		public override bool Round => true;
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(3, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
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
			new AbilityCardAbility(ShieldAbility.Builder()
				.WithShieldValue(1)
				.WithConditionalAbilityCheck(async state => 
					{
						await GDTask.CompletedTask;
						return state.Performer.Hex.HasHexObjectOfType<HazardousTerrain>();
					}
				).Build()),
		];
		public override bool Round => true;
	}
}