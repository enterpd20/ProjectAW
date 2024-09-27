using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DockDetailUI_Character : MonoBehaviour
{
    public Text character_ShipType;                       // �Ϸ���Ʈ ���� ǥ���� ĳ������ ����
    [SerializeField] private Image character_Image;       // ĳ���� �������� ��� ĳ���� �Ϸ���Ʈ
    [SerializeField] private Text character_Name;         // �Ϸ���Ʈ ���� ǥ���� ĳ������ �̸�
    //public Text Character_ShipName;     

    // ĳ���� �Ϸ���Ʈ �������� ����â�� ǥ�õ� ���� = ĳ���� �⺻ ���� + ���� ĳ���Ͱ� ������ ��� ����
    [SerializeField] private Text finalCharacterStats_HP;
    [SerializeField] private Text finalCharacterStats_FP;
    [SerializeField] private Text finalCharacterStats_TRP;
    [SerializeField] private Text finalCharacterStats_AVI;
    [SerializeField] private Text finalCharacterStats_AA;
    [SerializeField] private Text finalCharacterStats_SPD;

    [SerializeField] private Image backgroundImage;

    // UI ǥ���� ����
    [SerializeField] private RectTransform Rect_StatusUI;

    private void Start()
    {
        LoadCharacterData();
    }

    public void LoadCharacterData()
    {
        Character character = Player.Instance.GetSelectedCharacter_DockUI();

        if (character != null)
        {
            // UI�� ĳ���� ���� ǥ��
            character_Image.sprite = Resources.Load<Sprite>($"Images_Character/{character.imageName}");
            character_Name.text = character.name;
            character_ShipType.text = character.shipType;

            backgroundImage.sprite = GetRarityBackground(character.rarity);

            // ��� ���� ��������
            List<Gear> equippedGears = GetEquippedGears(character);

            // ĳ���� ���� ������Ʈ �� ���� ������ PlayerData�� ����
            UpdateCharacterStats(character, equippedGears);
        }
        else
        {
            Debug.LogWarning("No character selected.");
        }
    }

    Sprite GetRarityBackground(string rarity)
    {
        switch (rarity)
        {
            case "SSR": return Resources.Load<Sprite>("RarityBG/BG_SSR");
            case "SR": return Resources.Load<Sprite>("RarityBG/BG_SR");
            case "R": return Resources.Load<Sprite>("RarityBG/BG_R");
            case "N": return Resources.Load<Sprite>("RarityBG/BG_N");
            default: return Resources.Load<Sprite>("RarityBG/DefaultBackground");
        }
    }

    private List<Gear> GetEquippedGears(/*string shipType*/ Character character)
    {
        List<Gear> equippedGears = new List<Gear>();

        foreach (string gearName in character.eqiuppedGears)
        {
            Gear matchingGear = GearDataLoader.GetGearByName(gearName);
            if (matchingGear != null)
            {
                equippedGears.Add(matchingGear);
            }
            else
            {
                Debug.LogWarning($"No matching gear foun for {gearName}");
            }
        }
        return equippedGears;
    }

    public void UpdateCharacterStats(Character character, List<Gear> equippedGears)
    {
        CharacterStats finalStats = new CharacterStats
        {
            HP = character.stats.HP,
            FP = character.stats.FP,
            TRP = character.stats.TRP,
            AVI = character.stats.AVI,
            AA = character.stats.AA,
            SPD = character.stats.SPD
        };

        foreach (var gear in equippedGears)
        {
            Gear.GearStats stats = gear.stats;

            // ��� �װ����� ���, HP�� SPD�� �����ϰ� ���
            if (gear.gearType == "Torpedo Bomber" || gear.gearType == "Dive Bomber")
            {
                finalStats.FP += stats.FP;
                finalStats.TRP += stats.TRP;
                finalStats.AVI += stats.AVI;
                finalStats.AA += stats.AA;
            }
            else
            {
                // ��ȿ�� ���ȸ� ���͸��Ͽ� ���ϱ�
                finalStats.HP += stats.HP;
                finalStats.FP += stats.FP;
                finalStats.TRP += stats.TRP;
                finalStats.AVI += stats.AVI;
                finalStats.AA += stats.AA;
                finalStats.SPD += stats.SPD;
            }
        }

        finalCharacterStats_HP.text = finalStats.HP.ToString();
        finalCharacterStats_FP.text = finalStats.FP.ToString();
        finalCharacterStats_TRP.text = finalStats.TRP.ToString();
        finalCharacterStats_AVI.text = finalStats.AVI.ToString();
        finalCharacterStats_AA.text = finalStats.AA.ToString();
        finalCharacterStats_SPD.text = finalStats.SPD.ToString();

        Player.Instance.finalCharacterStats = finalStats;
        Player.Instance.SavePlayerData();
    }

    public void Gear_MoveChracterIllust()   // Gear ��ư ������ �Ϸ���Ʈ�� �߾����� �̵��ϴ� �޼���
    {
        StartCoroutine(MoveCharacterImage_co
            (new Vector3(105, character_Image.rectTransform.anchoredPosition.y, 0), 1.0f));
    }

    public void Info_MoveChracterIllust()   // Info ��ư ������ �Ϸ���Ʈ�� �ٽ� �������� �̵��ϴ� �޼���
    {
        StartCoroutine(MoveCharacterImage_co
            (new Vector3(-365, character_Image.rectTransform.anchoredPosition.y, 0), 1.0f));
    }

    private IEnumerator MoveCharacterImage_co(Vector3 targetPosition, float duration)
    {
        // ���� ��ġ ����
        Vector3 startPosition = character_Image.rectTransform.anchoredPosition;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            // ���� �ð��� ����ؼ� ��ġ ������Ʈ
            character_Image.rectTransform.anchoredPosition =
                Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // �̵��� ���� �� ���� ��ġ ����
        character_Image.rectTransform.anchoredPosition = targetPosition;
    }

    public void Appear_StatusUI()
    {
        Rect_StatusUI.localScale = Vector3.one;
    }

    public void Disappear_StatusUI()
    {
        Rect_StatusUI.localScale = Vector3.zero;
    }
}