using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// this namespace is used for getting private vars
using System.Reflection;
// this namespace is used for ?
using Partiality.Modloader;
// this namespace is used for ?
using UnityEngine;

// The name of this mod
namespace ManaOverhaul
{
    // the name of this mods class
    public class ManaOverhaulMod : PartialityMod
    {
        // set standards
        public GameObject _obj = null;
        public double version = 1.0;

        // declaration of class
        public ManaOverhaulMod()
        {
            this.ModID = "ManaOverhaul";
            this.Version = "1.0";
            this.author = "Monk_Urukhai";
        }
        // declaration to read my class first I think
        public override void OnEnable()
        {
            // enable the mod
            base.OnEnable();
            // check to make sure we don't overwrite
            if (_obj == null)
            {
                // declare game object
                _obj = new GameObject(this.ModID);
                // protect game object during scene changes
                GameObject.DontDestroyOnLoad(_obj);

                // add a custom MonoBehaviour script
                _obj.AddComponent<ManaOverhaulScript>();
            }
        }
        public override void OnDisable()
        {
            // disable the mod
            base.OnDisable();
        }
    }
    // declare script running the mod
    public class ManaOverhaulScript : MonoBehaviour
    {
        // setup the instance ?
        public static ManaOverhaulScript Instance;

        // set up Awake in order to use hooks and initial setup
        internal void Awake()
        {
            // change instance reference ?
            Instance = this;

            //identify class to override with hook (controls for max mana)
            On.CharacterStats.RefreshVitalMaxStat += new On.CharacterStats.hook_RefreshVitalMaxStat(RefreshVitalMaxHook);
        }

        // Define hook function that redoes mana
        private void RefreshVitalMaxHook(On.CharacterStats.orig_RefreshVitalMaxStat orig, CharacterStats self, bool _updateNeeds)
        {
            

            // reflect the m_manaAugmentation private variable
            StatStack m_manaAugmentation = typeof(CharacterStats).GetField("m_manaAugmentation", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this) as StatStack;
            // reflect the m_manaPoint private variable
            FieldInfo manaPointField = typeof(CharacterStats).GetField("m_manaPoint", BindingFlags.Instance | BindingFlags.NonPublic);
            var m_manaPoint = manaPointField.GetValue(this);

            // change mana points gained to 5 instead of 20
            if (m_manaAugmentation != null)
            {
                m_manaAugmentation.Refresh((float)m_manaPoint * 5f);
            }

            // get maximum mana stat with reflection
            Stat m_maxManaStat = typeof(CharacterStats).GetField("m_maxManaStat", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this) as Stat;
            // update the maximum mana stat
            m_maxManaStat.Update();

            // have the code run after the original function so it overwrites properly
            orig(self, _updateNeeds);
        }
    }
}