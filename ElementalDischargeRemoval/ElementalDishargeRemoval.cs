using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
// this namespace is used for modloader
using Partiality.Modloader;
using UnityEngine;

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
            }
        }
    }
}
