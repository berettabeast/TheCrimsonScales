using System.Collections.Generic;
using System.Data;
using Fractural.Tasks;
using Godot;

public class FocusedRay : RefractorCardModel<FocusedRay.CardTop, FocusedRay.CardBottom>
{
	public override string Name => "Photosynthesis";
	public override int Level => 1;
	public override int Initiative => 35;
	protected override int AtlasIndex => 7;

	public class CardTop : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(AttackAbility.Builder()
				.WithDamage(3, new AttackSquare(this, new Vector2(0.2f, 0.3f)))
				.WithRange(3, new RangeSquare(this, new Vector2(0.2f, 0.3f)))
				.Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
	}

	public class CardBottom : RefractorCardSide
	{
		protected override List<AbilityCardAbility> GetAbilities() =>
		[
			new AbilityCardAbility(OtherActiveAbility.Builder()
				.WithOnActivate(async state =>
				{
				}
				).WithOnDeactivate(async state =>
				{
					ScenarioEvents.FigureTurnStartedEvent.Unsubscribe(state, this);
					await GDTask.CompletedTask;
				}).Build())
		];

		public override IEnumerable<CardElementInfusion> Elements => [CardElementInfusion.Infuse(Element.Light)];
		public override int XP => 2;
		public override bool Persistent => true;
		public override bool Loss => true;
	}
}