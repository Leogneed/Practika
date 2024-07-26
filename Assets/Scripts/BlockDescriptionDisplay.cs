using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BlockDescriptionDisplay : MonoBehaviour
{
    public TerrainGenerator terrainGenerator;
    public TMP_Text descriptionText;

    // ����� ��� ���������� �������� �����
    public void UpdateBlockDescription(BlockType blockType)
    {
        BlockInfo blockInfo = terrainGenerator.GetBlockInfo(blockType);
        if (blockInfo != null)
        {
            descriptionText.text = blockInfo.Description;
        }
        else
        {
            descriptionText.text = "Block description not found.";
        }
    }
}
