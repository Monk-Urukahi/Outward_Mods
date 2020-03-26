using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
// this namespace is used for modloader
using Partiality.Modloader;
using UnityEngine;
using Localizer;

// the name of this mod
namespace ElementalDischargeRemoval
{
    // the name of this mod's class
    public class ElementalDischargeRemovalMod : PartialityMod
    {
        // set standards
        public GameObject _obj = null;
        public double version = 1.0;

        // init of class
        public ElementalDischargeRemovalMod()
        {
            this.ModID = "ElementalDischargeRemoval";
            this.Version = "1.0";
            this.author = "Monk_Urukhai";
        }

        // rewrite mod beginning
        public override void OnEnable()
        {
            // enable the mod
            base.OnEnable();
            // check to make sure we don't overwrite
            if (_obj == null)
            {
                // get new game object
                _obj = new GameObject(this.ModID);
                // protext game object during scene changes
                GameObject.DontDestroyOnLoad(_obj);

                // add a custom monobehaviour script
                _obj.AddComponent<ElementalDischargeRemovalScript>();
            }
        }
        public override void OnDisable()
        {
            // disable the mod
            base.OnDisable();
        }

        // declare script running this mod
        public class ElementalDischargeRemovalScript : MonoBehaviour
        {
            // set up the instance
            public static ElementalDischargeRemovalScript Instance;

            // define flags for reflection
            public BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

            // set up Awake for prefab loader
            internal void Awake()
            {
                StartCoroutine(SetupCoroutine());
            }

            private IEnumerator SetupCoroutine()
            {
                while (!ResourcesPrefabManager.Instance.Loaded)
                {
                    yield return null;
                }

                // Prefabs are loaded

                // Elemental Discharge Object ID 8200310
                var _elementalDischargeID = 8200310;

                // check to make sure it is a skill
                if (ResourcesPrefabManager.Instance.GetItemPrefab(_elementalDischargeID) is Skill _skill)
                {
                    // maintain compatibility with other mods by checking if unqique effects added
                    if (_skill.transform.Find("Effects") is Transform effects)
                    {
                        // add component to child
                        effects.gameObject.AddComponent<RemoveImbueEffects>();
                    }
                    else
                    {
                        // create new child
                        var removeImbueEffect = new GameObject("Effects");
                        removeImbueEffect.transform.parent = _skill.transform;
                        // add component to the child
                        removeImbueEffect.AddComponent<RemoveImbueEffects>();
                    }

                    // change item description (copied from sinai's side loader)
                   // string name = "Elemental Discharge";
                    string desc = String.Format("Required: Infused Weapon\n\nThrust your weapon forward, removing the elemental infusion to shoot a projectile of that element.");

                    ItemLocalization loc = new ItemLocalization(name, desc);

                    //typeof(Item).GetField("m_name",flags).SetValue(_skill, name);

                    if (typeof(LocalizationManager).GetField("m_itemLocalization", flags).GetValue(LocalizationManager.Instance) is Dictionary<int, ItemLocalization> dict)
                    {
                        if (dict.ContainsKey(_skill.ItemID))
                        {
                            dict[_skill.ItemID] = loc;
                        }
                        else
                        {
                            dict.Add(_skill.ItemID, loc);
                        }
                        typeof(LocalizationManager).GetField("m_itemLocalization", flags).SetValue(LocalizationManager.Instance, dict);
                    }
                }
            }
        }
    }
}
