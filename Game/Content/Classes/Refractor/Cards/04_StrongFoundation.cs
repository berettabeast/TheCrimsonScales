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
				.WithPerformAbility(async state =>
				{
					Hex selectedHex = await AbilityCmd.SelectHex(state, list =>
					{
						foreach (Hex possibleHex in RangeHelper.GetHexesInRange(state.Performer.Hex, 2, true, true))
						{
							list.Add(possibleHex);
						}
					}, false, $"Create one 1-hex Crystal Prism obstacle in one empty hex within {Icons.Inline(Icons.Range)}2");

					await CreateCrystalPrismObstacle(selectedHex, state.Performer);
				})
				.Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
				{
					Hex selectedHex = await AbilityCmd.SelectHex(state, list =>
					{
						foreach (Hex possibleHex in RangeHelper.GetHexesInRange(state.Performer.Hex, 1, true, true))
						{
							list.Add(possibleHex);
						}
					}, false, $"Create one 1-hex Crystal Prism obstacle in one adjacent empty hex");

					await CreateCrystalPrismObstacle(selectedHex, state.Performer);
				})
				.Build())
		];
	}
}