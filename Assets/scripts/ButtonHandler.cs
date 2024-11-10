using UnityEngine;
using UnityEngine.UI; // ��� ������ � UI ������������

public class ButtonHandler : MonoBehaviour
{
    public GameObject figurePrefab;  // ������ �������, ������� ����� �����������
    public Transform spawnLocation;  // �����, ���� ����� ���������� ������� (��������, ���� ��� ������� � ������������)
    public float spacing = 0.2f;     // �������� ����� ��������� (�� ��������� 0.2 �������)

    private Vector3 lastSpawnPosition;  // ��������� ������� ������ (��� ������������ ��������)

    // �����, ������� ����� ���������� �� ������� OnClick ������
    public void OnButtonClick()
    {
        // ������� ����� ��������� ������� � ������ ��������
        Vector3 spawnPosition = lastSpawnPosition + new Vector3(0, spacing, 0);  // �������� �� ��� Y (����� �������� ��� ������ �����������)

        // ������� ����� ��������� ������� � ������������ �����
        Instantiate(figurePrefab, spawnPosition, spawnLocation.rotation);

        // ��������� ��������� ������� ������
        lastSpawnPosition = spawnPosition;
    }
}
