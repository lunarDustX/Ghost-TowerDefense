using UnityEngine;
using UnityEngine.EventSystems;

public class SkillBranchHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public SkillTreeUI skillTreeUI;
    public HeroUpgradeBranch branch;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skillTreeUI != null)
        {
            skillTreeUI.ShowTooltip(branch, eventData.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillTreeUI != null)
        {
            skillTreeUI.HideTooltip();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (skillTreeUI != null)
        {
            skillTreeUI.MoveTooltip(eventData.position);
        }
    }
}
