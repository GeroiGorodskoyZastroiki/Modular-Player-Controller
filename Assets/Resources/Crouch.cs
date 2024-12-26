using UnityEngine;
using R3;  // Для работы с ReactiveProperty

public class Crouch : MonoBehaviour
{
    // Публичное поле для скорости приседания
    [SerializeField] private float crouchSpeed = 2f;

    // Публикация изменений состояния приседания
    public ReactiveProperty<bool> IsCrouching { get; private set; } = new ReactiveProperty<bool>();

    private Move move;

    private void Awake()
    {
        // Получаем компонент Move
        move = GetComponent<Move>();

        // // Подписка на изменения флага IsCrouching и изменение скорости
        // IsCrouching.Subscribe(isCrouching =>
        // {
        //     move.Speed.Value = isCrouching ? crouchSpeed : 0f;
        // }).AddTo(this);  // Добавляем подписку к жизненному циклу объекта
    }
}


// using UnityEngine;
// using R3;

// public class Crouch : MonoBehaviour
// {
//     private Move move;
//     [SerializeField] private float crouchSpeed = 2f;

//     private void Awake()
//     {
//         move = GetComponent<Move>();
//     }

//     private void Start()
//     {
//         Observable.EveryUpdate()
//             .Select(_ => Input.GetKey(KeyCode.LeftControl)) // Проверяем, нажата ли клавиша Ctrl
//             .DistinctUntilChanged() // Избегаем повторных вызовов при одинаковых состояниях
//             .Subscribe(isCrouching =>
//             {
//                 move.Speed.Value = isCrouching ? crouchSpeed : move.Speed.Value; // Устанавливаем скорость приседания
//             });
//     }
// }