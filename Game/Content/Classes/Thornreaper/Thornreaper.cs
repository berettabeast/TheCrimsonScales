using System.Collections.Generic;
using Fractural.Tasks;

public partial class Thornreaper : Character
{
	private ThornreaperModel _ThornreaperModel;

	public override async GDTask OnScenarioSetupCompleted()
	{
		await base.OnScenarioSetupCompleted();

		object subscriber = new object();
	}

	public override async GDTask Spawn(SavedCharacter savedCharacter, int index)
	{
		await base.Spawn(savedCharacter, index);

		_ThornreaperModel = (ThornreaperModel)savedCharacter.ClassModel;
	}
}