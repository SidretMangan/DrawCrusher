using System.Reflection;
using UnityEngine;

namespace GSL.InputCustomGUIBehaviour
{
    public class CustomButton : CustomGUIBehaviour
    {
        [HideInInspector] public GameObject TargetSceneGameObject;
        [HideInInspector] public MonoBehaviour SelectedMono;
        [HideInInspector] public MethodInfo m_methode;

        [Header("Sound")]
        public AudioClip ClickSound;

        AudioSource source;
        protected override void Init()
        {
            if (ClickSound == null) ClickSound = Resources.Load<AudioClip>("click_sound");
            source=GetComponent<AudioSource>();
        }

        protected override void ClickAction()
        {
            source.PlayOneShot(ClickSound);
            m_methode?.Invoke(SelectedMono, null);
        }

    }
}
