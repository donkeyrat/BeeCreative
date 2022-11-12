using BepInEx;
using UnityEngine;

namespace Creative
{
	[BepInPlugin("teamgrad.beecreative", "BeeCreative", "1.1.3")]
	public class CRLauncher : BaseUnityPlugin
	{
		public CRLauncher()
		{
			CRBinder.UnitGlad();
		}
	}
}
