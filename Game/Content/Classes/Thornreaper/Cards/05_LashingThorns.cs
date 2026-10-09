using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class LashingThorns : ThornreaperCardModel<LashingThorns.CardTop, LashingThorns.CardBottom>
{
	public override string Name => "Inner Reflection";
	public override int Level => 1;
	public override int Initiative => 53;
	protected override int AtlasIndex => 29 - 5;

	public class CardTop : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(2)
				.Build())
		];
	}

	public class CardBottom : ThornreaperCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2)
				.Build())
		];
	}
}