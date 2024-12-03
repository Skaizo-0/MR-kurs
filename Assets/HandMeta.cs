using Blocks.Builder;
using Blocks;
using System;
using System.Linq;
using UnityEngine;

namespace Sandbox.Controller
{
    public class HandMeta : MonoBehaviour
    {
        private bool isGrabbing;  // Флаг, указывающий на то, что объект в данный момент захвачен
        private Chunk chunkHeld;  // Текущий захваченный чанк
        [SerializeField] private ChunkSpawner chunkSpawner;  // Спавнер чанков
        private Helper helper;  // Вспомогательный компонент для работы с контроллерами

        // Перечисление состояний, используем только Grab и Disconnect
        public enum State
        {
            Grab, Disconnect
        }

        [SerializeField] private State state;  // Текущее состояние

        // Метод, вызываемый при запуске объекта
        private void Awake()
        {
            // В этом случае нет необходимости в контроллерах, настраиваем всё для отслеживания только движения руки
            helper = gameObject.AddComponent<Helper>();  // Добавляем компонент Helper для управления контроллерами
        }

        // Метод для обновления состояния каждый кадр
        private void Update()
        {
            // Проверяем, захвачен ли чанк
            if (chunkHeld)
            {
                var component = chunkHeld.GetComponent<Rigidbody>();  // Получаем Rigidbody для управления физикой объекта
                component.isKinematic = true;  // Отключаем физику для удержания объекта
                component.transform.position = transform.position;  // Привязываем объект к руке
                component.transform.rotation = transform.rotation;  // Привязываем поворот объекта к повороту руки
            }

            // Проверка на приближение руки к объекту для захвата
            if (IsHandNearObject())
            {
                TryGrabObject();
            }

            // Проверка на выполнение жеста "отпустить" (например, рука распрямляется или отходит от объекта)
            if (isGrabbing && !IsHandNearObject())
            {
                ReleaseObject();
            }

            UpdateState();  // Обновляем состояние в зависимости от действий
        }

        // Проверка, находится ли рука рядом с объектом, который можно захватить
        private bool IsHandNearObject()
        {
            // Проверяем все объекты в радиусе 0.05 от позиции руки
            var nearbyObjects = Physics.OverlapSphere(transform.position, 0.05f);
            return nearbyObjects.Any(c => c.GetComponent<Block>());  // Если есть хотя бы один объект типа Block, считаем, что рука рядом
        }

        // Попытка захвата объекта, если рука рядом с ним
        private void TryGrabObject()
        {
            if (isGrabbing || chunkHeld != null) return;  // Если уже захвачен объект, ничего не делаем

            var blockCandidate = CheckForChunk();  // Ищем подходящий объект для захвата

            if (blockCandidate)
            {
                chunkHeld = blockCandidate;  // Присваиваем найденный чанк
                chunkHeld.GetComponent<BuildPreviewManager>().StartPreview();  // Запускаем предварительный просмотр
                isGrabbing = true;  // Обновляем флаг захвата
            }
        }

        // Метод для завершения захвата
        private void ReleaseObject()
        {
            if (chunkHeld)
            {
                chunkHeld.GetComponent<Rigidbody>().isKinematic = false;  // Включаем физику для объекта
                chunkHeld.GetComponent<BuildPreviewManager>().StopPreview();  // Останавливаем предварительный просмотр
            }

            chunkHeld = null;  // Сбрасываем захваченный чанк
            isGrabbing = false;  // Сбрасываем флаг захвата
        }

        // Метод для обновления состояния
        private void UpdateState()
        {
            switch (state)
            {
                case State.Grab:
                    // Логика для состояния "Grab" (захват)
                    break;
                case State.Disconnect:
                    // Логика для состояния "Disconnect" (отключение)
                    break;
                default:
                    throw new ArgumentOutOfRangeException();  // Ошибка при неизвестном состоянии
            }
        }

        // Метод для поиска ближайшего чанка для захвата
        private Chunk CheckForChunk()
        {
            // Ищем все объекты в радиусе 0.05 от руки
            var blockCandidate = Physics.OverlapSphere(transform.position, 0.05f)
                .Where(c => c.GetComponent<Block>())  // Ищем только объекты с компонентом Block
                .Where(c => c.GetComponent<Block>().IsAnchored == false)  // Исключаем анкклированные блоки
                .OrderBy(c => Vector3.Distance(c.transform.position, transform.position))  // Сортируем по ближайшим к руке
                .FirstOrDefault();  // Берем первый (самый близкий)

            if (blockCandidate)
            {
                var chunk = blockCandidate.GetComponentInParent<Chunk>();  // Получаем чанк
                if (chunk.GetComponent<Rigidbody>().isKinematic == false)  // Проверяем, не является ли чанк кинематическим
                {
                    return chunk;  // Возвращаем чанк
                }
            }

            return null;  // Если чанк не найден, возвращаем null
        }
    }
}
