using UnityEngine;

namespace Umbra.Prototype
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        void Interact(GameObject interactor);
    }
}
