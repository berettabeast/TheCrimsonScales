using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class StrongFoundation : RefractorCardModel<StrongFoundation.CardTop, StrongFoundation.CardBottom>
{
	public override string Name => "Jagged Clutch";
	public override int Level => 1;
	public override int Initiative => 32;
	protected override int AtlasIndex => 4;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
				
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async parameters =>
				{
					await AbilityCmd.SelectHex(parameters, list =>
					{
						foreach (Hex hex in RangeHelper.GetHexesInRange(parameters.Performer.Hex, 2, false, true))
						{
							if (hex.IsEmpty())
							{
								list.Add(hex);
							}
						}
					}, false);
				}).Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(CreatePrismObstacleBuilder().WithRange(3).WithObstacleCount(2).Build())
		];
	}
}