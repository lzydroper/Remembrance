// using SKCell;
// using UnityEngine;
//
// namespace PhaseSystem
// {
//     public class BagUI : SKMonoSingleton<BagUI>
//     {
//         [SerializeField] private GameObject BagUIPanel;
//
//         protected override void Awake()
//         {
//             base.Awake();
//             BagUIPanel.SetActive(false);
//         }
//
//         public void PhaseStart()
//         {
//             BagUIPanel.SetActive(true);
//         }
//
//         public void PhaseEnd()
//         {
//             
//         }
//
//         public void PhaseExit()
//         {
//             BagUIPanel.SetActive(false);
//         }
//     }
// }