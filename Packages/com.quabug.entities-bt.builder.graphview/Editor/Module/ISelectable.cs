using System;

namespace EntitiesBT.Editor
{
    public interface ISelectable
    {
        bool IsSelected { set; }
        event Action OnSelected;
    }
}