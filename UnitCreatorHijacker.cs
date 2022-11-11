using UnityEngine;
using UnityEngine.SceneManagement;
using Landfall.TABS;
using System.Linq;
using Landfall.TABS.UnitEditor;
using System.Collections.Generic;
using Landfall.TABS.AI.Components.Modifiers;

namespace Creative
{
    public class UnitCreatorHijacker : MonoBehaviour
    {
        public UnitCreatorHijacker()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        public void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "UnitCreator_GamepadUI")
            {
                var manager = scene.GetRootGameObjects().ToList().Find(x => x.GetComponent<UnitEditorManager>()).GetComponent<UnitEditorManager>();
                var movement = new List<UnitEditorManager.MovementTypeWrapper>(manager.MovementTypes);
                movement.Add(new UnitEditorManager.MovementTypeWrapper() { DisplayName = "Keep Running" });
                manager.MovementTypes = movement.ToArray();
                var components = new List<IMovementComponent>(manager.MovementTypesComponents);
                components.Add(default(NeverStopRunning));
                manager.MovementTypesComponents = components;
                var bases = new List<UnitEditorManager.UnitBaseWrapper>(manager.UnitBases);
                foreach (var b in CRMain.creative.LoadAllAssets<UnitBlueprint>())
                {
                    var wrapper = new UnitEditorManager.UnitBaseWrapper
                    {
                        BaseDisplayName = b.Entity.Name,
                        UnitBaseBlueprint = b,
                        UnitBaseRestriction = CharacterItem.UnitBaseRestrictions.None
                    };
                    b.Entity.GetSpriteIconAsync(sprite =>
                    {
                        wrapper.BaseIcon = sprite;
                    });
                    bases.Add(wrapper);
                }
                manager.UnitBases = bases.ToArray();
            }
        }
    }
}
