using UnityEngine;
using UnityEngine.UI; // Для работы с UI компонентами

public class ButtonHandler : MonoBehaviour
{
    public GameObject figurePrefab;  // Префаб фигурки, которую будем клонировать
    public Transform spawnLocation;  // Место, куда будет спавниться фигурка (например, рука или позиция в пространстве)
    public float spacing = 0.2f;     // Смещение между фигурками (по умолчанию 0.2 единицы)

    private Vector3 lastSpawnPosition;  // Последняя позиция спауна (для отслеживания смещения)

    // Метод, который будет вызываться на событие OnClick кнопки
    public void OnButtonClick()
    {
        // Создаем новый экземпляр фигурки с учетом смещения
        Vector3 spawnPosition = lastSpawnPosition + new Vector3(0, spacing, 0);  // Смещение по оси Y (можно изменить для других направлений)

        // Создаем новый экземпляр фигурки в рассчитанном месте
        Instantiate(figurePrefab, spawnPosition, spawnLocation.rotation);

        // Обновляем последнюю позицию спауна
        lastSpawnPosition = spawnPosition;
    }
}
