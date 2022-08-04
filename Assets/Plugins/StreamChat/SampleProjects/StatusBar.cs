using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Plugins.StreamChat.SampleProjects
{
    class StatusBar : VisualElement
    {
        public StatusBar()
        {
            m_Status = String.Empty;
        }

        string m_Status;
        public string status { get; set; }

        public new class UxmlFactory : UxmlFactory<StatusBar> {}

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Status = new UxmlStringAttributeDescription { name = "status" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ((StatusBar)ve).status = m_Status.GetValueFromBag(bag, cc);
            }
        }

        // ...
    }

}