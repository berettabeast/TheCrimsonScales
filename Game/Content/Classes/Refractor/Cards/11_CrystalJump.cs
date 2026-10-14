using System.Collections.Generic;
using Fractural.Tasks;
using Godot;

public class CrystalJump : RefractorCardModel<CrystalJump.CardTop, CrystalJump.CardBottom>
{
	public override string Name => "Superradiance";
	public override int Level => 1;
	public override int Initiative => 48;
	protected override int AtlasIndex => 11;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(PushAbility.Builder()
				.WithPush(1)
				.WithTarget(Target.Enemies | Target.TargetAll)
				.WithRange(3)
				.WithConditions(Conditions.Muddle)
				.Build())
		];
		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
		public override int XP => 1;
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherAbility.Builder()
				.WithPerformAbility(async state =>
					{
						
					})
				.Build()),

			new AbilityCardAbility(MoveAbility.Builder()
				.WithDistance(2, new MoveSquare(this, new Vector2(0.2f, 0.3f)))
				.Build()),

			new AbilityCardAbility(HealAbility.Builder()
				.WithHealValue(1)
				.WithRange(2, new RangeSquare(this, new Vector2(0.2f, 0.3f)))
				.Build())
		];
	}
}