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
using UnityEngine.UI;

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

        // define flags for reflection
        public BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        // set up Awake in order to use hooks and initial setup
        internal void Awake()
        {
            // change instance reference ?
            Instance = this;

            //identify class to override with hook (controls for max mana)
            On.CharacterStats.RefreshVitalMaxStat += RefreshVitalMaxHook;

            //identify class to override with hook (display for mana unlock at ley line)
            On.ManaIncreaseMenu.OnManaSelectorChanged += OnManaSelectorChangedHook;
        }


        // Define hook function that redoes mana
        private void RefreshVitalMaxHook(On.CharacterStats.orig_RefreshVitalMaxStat orig, CharacterStats self, bool _updateNeeds = false)
        {
            // run original code
            orig(self,_updateNeeds);

            // code copied from Sinai
            // get the character value needed for health bonuses (.Inventory.Equipment.GetMaxHealthBonus)
            var m_character = self.GetComponent<Character>();
            // get the mana points needed for mana bonuses
            var m_manaPoint = self.ManaPoint;

            // make sure to check for null
            if ((typeof(CharacterStats).GetField("m_manaHealthReduction", flags).GetValue(self) as StatStack) != null)
            {
                // get the new character value for health at ley line
                (typeof(CharacterStats).GetField("m_manaHealthReduction", flags).GetValue(self) as StatStack)
                    .Refresh((float)m_manaPoint * -5f + m_character.Inventory.Equipment.GetMaxHealthBonus());
            }
            if ((typeof(CharacterStats).GetField("m_manaStaminaReduction", flags).GetValue(self) as StatStack) != null)
            {
                // get the new character value for stamina at ley line
                (typeof(CharacterStats).GetField("m_manaStaminaReduction", flags).GetValue(self) as StatStack)
                    .Refresh((float)m_manaPoint * -5f);
            }
            if ((typeof(CharacterStats).GetField("m_manaAugmentation", flags).GetValue(self) as StatStack) != null)
            {
                // get the new character value for mana at ley line
                (typeof(CharacterStats).GetField("m_manaAugmentation", flags).GetValue(self) as StatStack)
                    .Refresh((float)m_manaPoint * 5f);
            }
            

            // update these values
            (typeof(CharacterStats).GetField("m_maxHealthStat", flags).GetValue(self) as Stat).Update();
            (typeof(CharacterStats).GetField("m_maxStamina", flags).GetValue(self) as Stat).Update();
            (typeof(CharacterStats).GetField("m_maxManaStat", flags).GetValue(self) as Stat).Update();

        }

        // define the hook for the mana changed class
        private void OnManaSelectorChangedHook(On.ManaIncreaseMenu.orig_OnManaSelectorChanged orig, ManaIncreaseMenu self, int _value)
        {
            // run original function
            orig(self, _value);

            // use reflection to get numbers
            float num = (float)(typeof(ManaIncreaseMenu).GetField("m_manaIncreaseSelector", flags).GetValue(self) as IntSelector).Value * 5f;
            float num2 = (float)(typeof(ManaIncreaseMenu).GetField("m_manaIncreaseSelector", flags).GetValue(self) as IntSelector).Value * -5f;
            float num3 = (float)(typeof(ManaIncreaseMenu).GetField("m_manaIncreaseSelector", flags).GetValue(self) as IntSelector).Value * -5f;


            // check if null and then display new numbers
            if (typeof(ManaIncreaseMenu).GetField("m_lblResultMana", flags).GetValue(self) as Text)
            {
                (typeof(ManaIncreaseMenu).GetField("m_lblResultMana", flags).GetValue(self) as Text).text = (self.LocalCharacter.Stats.MaxMana + num).ToString("0.#");
                (typeof(ManaIncreaseMenu).GetField("m_lblResultMana", flags).GetValue(self) as Text).color = ((num <= 0f) ? Global.WHITE_GRAY : Global.LIGHT_GREEN);
            }
            if (typeof(ManaIncreaseMenu).GetField("m_lblResultHealth", flags).GetValue(self) as Text)
            {
                (typeof(ManaIncreaseMenu).GetField("m_lblResultHealth", flags).GetValue(self) as Text).text = ((typeof(CharacterStats).GetField("m_maxHealthStat", flags).GetValue(self.LocalCharacter.Stats) as Stat).CurrentValue + num2).ToString("0.#");
                (typeof(ManaIncreaseMenu).GetField("m_lblResultHealth", flags).GetValue(self) as Text).color = ((num2 >= 0f) ? Global.WHITE_GRAY : Global.LIGHT_RED);
            }
            if (typeof(ManaIncreaseMenu).GetField("m_lblresultStamina", flags).GetValue(self) as Text)
            {
                (typeof(ManaIncreaseMenu).GetField("m_lblresultStamina", flags).GetValue(self) as Text).text = ((typeof(CharacterStats).GetField("m_maxStamina", flags).GetValue(self.LocalCharacter.Stats) as Stat).CurrentValue + num3).ToString("0.#");
                (typeof(ManaIncreaseMenu).GetField("m_lblresultStamina", flags).GetValue(self) as Text).color = ((num3 >= 0f) ? Global.WHITE_GRAY : Global.LIGHT_RED);
            }
        }
    }
}