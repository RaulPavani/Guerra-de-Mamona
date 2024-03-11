using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardVisualCommand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Command command;

    [SerializeField] private Image characterLeftImage;
    [SerializeField] private Image characterRightImage;
    [SerializeField] private Image commandTypeImage;
    [SerializeField] private Button undoButton;

    [SerializeField] private Sprite moveTypeSprite;
    [SerializeField] private Sprite attackTypeSprite;

    [SerializeField] private Sprite tileSprite;

    private Vector3Int? tileStartPos = null;

    public void SetupCard(Command command)
    {
        this.command = command;
        SetCommandType(command.GetType());
        SetCharacter(command);

        undoButton.onClick.AddListener(UndoCard);
    }

    private void SetCommandType(Type commandType)
    {
        if (commandType == typeof(MoveCommand))
        {
            commandTypeImage.sprite = moveTypeSprite;
            tileStartPos = EntitiesController.Instance.GetTileCell((command as MoveCommand).GetStartPosition());
        }

        if (commandType == typeof(AttackCommand))
        {
            commandTypeImage.sprite = attackTypeSprite;
        }
    }

    private void SetCharacter(Command command)
    {
        characterLeftImage.sprite = command.GetPortraitSelectedCharacter();
        characterLeftImage.SetNativeSize();

        Sprite rightPortrait = command.GetPortraitTargetCharacter();
        characterRightImage.sprite = rightPortrait != null ? rightPortrait : tileSprite;
        characterRightImage.SetNativeSize();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        command.GetSelectedCharacter().Highlight();

        if (command.GetType() == typeof(AttackCommand))
        {
            command.GetTargetCharacter()?.Highlight();
        }

        if (tileStartPos != null)
        {
            EntitiesController.Instance.ShowSelectedTile(tileStartPos);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        command.GetSelectedCharacter().RemoveHighlight();
        command.GetTargetCharacter()?.RemoveHighlight();

        if (tileStartPos != null)
        {
            EntitiesController.Instance.ShowSelectedTile(tileStartPos, false);
        }
    }

    private void UndoCard()
    {
        command.GetSelectedCharacter().RemoveHighlight();
        command.GetTargetCharacter()?.RemoveHighlight();

        if (tileStartPos != null)
        {
            EntitiesController.Instance.ShowSelectedTile(tileStartPos, false);
        }

        CommandManager.Instance.UndoCommand(command);
    }
}
