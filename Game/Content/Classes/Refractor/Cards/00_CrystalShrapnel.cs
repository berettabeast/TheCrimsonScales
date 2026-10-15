using System.Collections.Generic;
using Godot;
using Fractural.Tasks;
using System.Data;

public class CrystalShrapnel : RefractorCardModel<CrystalShrapnel.CardTop, CrystalShrapnel.CardBottom>
{
	public override string Name => "Crystal Shrapnel";
	public override int Level => 1;
	public override int Initiative => 18;
	protected override int AtlasIndex => 0;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(RefractingAttack(2).Build())
		];

		public override int XP => 1;
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(MoveAbility.Builder()
			.WithDistance(2, new MoveSquare(this, new Vector2(0.3f, 0.5f)))
			.Build()),

			new AbilityCardAbility(ShieldAbility.Builder()
			.WithShieldValue(1)
			.Build()),

			new AbilityCardAbility(OtherAbility.Builder()
			.WithPerformAbility(state => AbilityCmd.InfuseElement(state, Element.Earth))
			.WithConditionalAbilityCheck(state => AbilityCmd.AskConsumeElement(state.Performer, Element.Earth))
			.Build()
			)
		];
		public override bool Round => true;
	}
}