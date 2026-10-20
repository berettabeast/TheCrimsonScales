using System.Collections.Generic;
using Fractural.Tasks;

public class RefractorPerks
{
	public abstract class RefractorPerk : PerkModel
	{
	}

	public class ReplaceOneMinusOneWithOnePlusOneIfLightIsStrongOrWaning : RefractorPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusOneAMDCard>(),
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RefractorAMDCards.PlusOneIfLightIsStrongOrWaningRolling>(),
		];
	}

	public class ReplaceOneMinusTwoWithOnePlusZero : RefractorPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<MinusTwoAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RefractorAMDCards.PlusZero>()
		];
	}

	public class ReplaceOnePlusZeroWithOnePlusTwo : RefractorPerk
	{
		public override List<AMDCardModel> CardsToRemove { get; } =
		[
			ModelDB.AMDCard<PlusZeroAMDCard>()
		];

		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RefractorAMDCards.PlusTwo>()
		];
	}

	public class AddThreePlusOneIfLightIsStrongOrWaning : RefractorPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RefractorAMDCards.PlusOneIfLightIsStrongOrWaningRolling>(),
			ModelDB.AMDCard<RefractorAMDCards.PlusOneIfLightIsStrongOrWaningRolling>(),
			ModelDB.AMDCard<RefractorAMDCards.PlusOneIfLightIsStrongOrWaningRolling>()
		];
	}

	public class AddTwoInfuseLightRolling : RefractorPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RefractorAMDCards.PlusZeroLightRolling>(),
			ModelDB.AMDCard<RefractorAMDCards.PlusZeroLightRolling>()
		];
	}

	public class AddThreeInfuseEarthIfLightIsStrongOrWaningRolling : RefractorPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RefractorAMDCards.InfuseEarthIfLightIsStrongOrWaningRolling>(),
			ModelDB.AMDCard<RefractorAMDCards.InfuseEarthIfLightIsStrongOrWaningRolling>(),
			ModelDB.AMDCard<RefractorAMDCards.InfuseEarthIfLightIsStrongOrWaningRolling>(),
		];
	}

	public class AddOneCreateHazardousTerrain : RefractorPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RefractorAMDCards.CreateHazardousTerrain>()
		];
	}

	public class AddOneRetaliateThreeOnHazardousTerrainRolling : RefractorPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RefractorAMDCards.PersistentRetaliateThreeOnNextAdjacentAttackInHazardousTerrain>()
		];
	}

	public class AddOneShieldThreeOnHazardousTerrainRolling : RefractorPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RefractorAMDCards.PersistentShieldThreeOnNextAttackDamageInHazardousTerrain>(),
		];
	}

	public class IgnoreItemEffectsAndAddOnePlusOneIfLightIsStrongOrWaningRolling : RefractorPerk
	{
		public override List<AMDCardModel> CardsToAdd { get; } =
		[
			ModelDB.AMDCard<RefractorAMDCards.PlusOneIfLightIsStrongOrWaningRolling>()
		];

		public override bool IgnoreItemMinusOneEffects => true;

	}

	public class BrambleBulwark : RefractorPerk, IEventSubscriber
	{
		protected override string Title => "Bramble Bulwark";
		public override int PerkBoxCount => 2;

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Gain {Icons.Inline(Icons.Shield, richTextParameters)}1 while you occupy hazardous terrain";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			bool shielded = false;

			ScenarioEvents.FigureEnteredHexEvent.Subscribe(this,
				parameters => parameters.Figure == character,
				async parameters =>
				{
					bool hazardous = parameters.Hex.HasHexObjectOfType<HazardousTerrain>();

					if (hazardous && !shielded)
					{
						await AbilityCmd.AddShield(character, this, 1);
						shielded = true;
					}
					else if (!hazardous && shielded)
					{
						AbilityCmd.RemoveShield(character, this);
						shielded = false;
					}
				});
			
			if (character.Hex.HasHexObjectOfType<HazardousTerrain>())
			{
				await AbilityCmd.AddShield(character, this, 1);
				shielded = true;
			}
		}
	}

	public class RiseAndShine : RefractorPerk, IEventSubscriber
	{
		protected override string Title => "Rise and Shine";

		public override string GetNonAMDDescription(RichTextParameters richTextParameters) =>
			$"Whenever you long rest, {Icons.InlineElement(Element.Light, richTextParameters)}";

		public override async GDTask OnScenarioSetupPhaseCompleted(Character character)
		{
			await base.OnScenarioSetupPhaseCompleted(character);

			ScenarioEvents.LongRestEndedEvent.Subscribe(this,
				parameters => parameters.Character == character,
				async parameters =>
				{
					await AbilityCmd.InfuseElement(null, Element.Light, character);
				});
		}

	}
}