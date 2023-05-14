// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using Kinect = Windows.Kinect;
// using Windows.Kinect;
// using System;

// public class SpeedUp: MonoBehaviour
// {
//     public GameObject MyBodySourceManager;

//     private MyBodySourceManager _BodyManager;
//     public float turnThreshold = 0.5f;

//     public bool Sp_up;
//     public bool speed_up = false;
//     public event Action OnSpeed_up;
//     public event Action StopSpeed_up;

//     private bool crossedThresholdSpU= false;

//     void Start(){
//         if (MyBodySourceManager == null) return;

//         _BodyManager = MyBodySourceManager.GetComponent<MyBodySourceManager>();
//         if (_BodyManager == null) return;
//     }
//     private bool calculatePosition_head(Vector3 head, Vector3 point1, Vector3 point2,Vector3 point1_s, Vector3 point2_s){
//         // Calcular la diferencia en las coordenadas y respecto a la cabeza
//         bool diff1h = head.y < point1.y;
//         bool diff2h = head.y < point2.y;
//         //Asegurar que no estan en la inclinacion de los giros
//         float angleRadians = Mathf.Atan2((point2.y - point1.y), (point2.x - point1.x));
//         bool inclinacionL = (angleRadians * Mathf.Rad2Deg) < turnThreshold;
//         bool inclinacionR = (angleRadians * Mathf.Rad2Deg) > (turnThreshold* -1);
//         // Aseggurar que los brasos esten abiertos y no solo hacia arriba
//         bool diff1s = point1.x < point1_s.x;
//         bool diff2s = point1.x > point1_s.x;
//         //Si todas las caracteristicas se cumplen esta acelerando
//         return (diff1h&&diff2h&&inclinacionL&&inclinacionR&&diff1s&&diff2s);
//     }
//     void detectSpeed_up(ref bool spd_up) {
//         if (!_BodyManager.isThereBody()) return;
//         //Debug.Log("No detecto algun cuerpo");
//         else if (Enum.IsDefined(typeof(Kinect.JointType), JointType.Head) &&
//                  Enum.IsDefined(typeof(Kinect.JointType), JointType.WristLeft) &&
//                  Enum.IsDefined(typeof(Kinect.JointType), JointType.WristRight)&&
//                  Enum.IsDefined(typeof(Kinect.JointType), JointType.ShoulderLeft) &&
//                  Enum.IsDefined(typeof(Kinect.JointType), JointType.ShoulderRight))
//         {

//             Sp_up = calculatePosition_head(_BodyManager.GetBodyJointPos(JointType.Head),
//                 _BodyManager.GetBodyJointPos(JointType.WristLeft),
//                 _BodyManager.GetBodyJointPos(JointType.WristRight),
//                 _BodyManager.GetBodyJointPos(JointType.ShoulderLeft),
//                 _BodyManager.GetBodyJointPos(JointType.ShoulderRight));

//             if (Sp_up)
//             {
//                 spd_up = true;
//                 return;
//             }
//         }
//     }
//     void Update(){
//         bool spd_up = false;
//         detectSpeed_up(ref spd_up);

//         //Right
//         bool wasCrossed = crossedThresholdSpU;
//         crossedThresholdSpU = spd_up;

//         if (crossedThresholdSpU && !wasCrossed){
//             speed_up = true;
//             OnSpeed_up?.Invoke();
//             Debug.Log("Comenzo a acelerar");
//         }
//         else if (!crossedThresholdSpU && wasCrossed){
//             speed_up = false;
//             StopSpeed_up?.Invoke();
//             Debug.Log("Termino de acelerar");
//         }
//     }
// }
