using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VisualCommandsController : MonoBehaviour
{
    [Header("Card Prefab")]
    [SerializeField] private GameObject cardPrefab;

    [Header("Canvas Refs")]
    [SerializeField] private Transform cardsListTrasform;
    [SerializeField] private Button runQueueButton;

    [SerializeField] private TextMeshProUGUI actionCounterText;
    private const int maxActionsPerTurn = 5;

    private Dictionary<CardVisualCommand, Command> currentCards = new Dictionary<CardVisualCommand, Command>();

    private void Start()
    {
        runQueueButton.onClick.AddListener(CommandManager.Instance.RunQueue);
    }

    public void CreateVisualCard(Command command)
    {
        CardVisualCommand card = Instantiate(cardPrefab, cardsListTrasform).GetComponent<CardVisualCommand>();
        card.SetupCard(command);
        card.transform.SetAsFirstSibling();

        currentCards.Add(card, command);
        UpdateCounterText();
    }

    public void RemoveVisualCard(Command command)
    {
        CardVisualCommand cardToRemove;
        cardToRemove = currentCards.FirstOrDefault(x => x.Value == command).Key;
        currentCards.Remove(cardToRemove);
        Destroy(cardToRemove.gameObject);
        UpdateCounterText();
    }

    private void UpdateCounterText()
    {
        actionCounterText.text = $"{currentCards.Count}/{maxActionsPerTurn}";
    }

}
