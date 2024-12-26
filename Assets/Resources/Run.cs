using UnityEngine;
using R3;  // Для работы с ReactiveProperty

public class Run : MonoBehaviour
{
    // Публичное поле для скорости бега
    [SerializeField] private float runSpeed = 6f;

    // Публикация изменений состояния бега
    public ReactiveProperty<bool> IsRunning { get; private set; } = new ReactiveProperty<bool>();

    private Move move;

    private void Awake()
    {
        // Получаем компонент Move
        move = GetComponent<Move>();

        // // Подписка на изменения флага IsRunning и обновление скорости
        // IsRunning.Subscribe(isRunning =>
        // {
        //     move.Speed.Value = isRunning ? runSpeed : 0f;
        // }).AddTo(this);  // Добавляем подписку к жизненному циклу объекта
    }
}

// using UnityEngine;
// using R3;

// public class Run : MonoBehaviour
// {
//     private Move move;
//     [SerializeField] private float runSpeed = 10f;

//     private void Awake()
//     {
//         move = GetComponent<Move>();
//     }

//     private void Start()
//     {
//         Observable.EveryUpdate()
//             .Select(_ => Input.GetKey(KeyCode.LeftShift)) // Проверяем, нажата ли клавиша Shift
//             .DistinctUntilChanged() // Избегаем повторных вызовов при одинаковых состояниях
//             .Subscribe(isRunning =>
//             {
//                 move.Speed.Value = isRunning ? runSpeed : move.Speed.Value; // Устанавливаем скорость бега
//             });
//     }
// }