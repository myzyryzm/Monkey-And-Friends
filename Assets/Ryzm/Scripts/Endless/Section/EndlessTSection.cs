﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessTSection : EndlessSection
    {
        /// <summary>
        /// direction that the user turned in initially
        /// </summary>
        public Direction turnDirection;

        [Header("Right Transforms")]
        public Transform position3;
        public Transform position4;
        public Transform position5;

        protected override Transform GetPosition(int position)
        {
            switch(position)
            {
                case 0:
                    return turnDirection == Direction.Left ? position0 : position3;
                case 1:
                    return turnDirection == Direction.Left ? position1 : position4;
                case 2:
                    return turnDirection == Direction.Left ? position2 : position5;
                default:
                    return null;
            }
        }


        public void Shift(Direction direction, RunnerController controller, bool turned)
        {
            if(!turned)
            {
                Transform trans = controller.gameObject.transform;
                if(direction == Direction.Left)
                {
                    trans.Rotate(Vector3.up * -90);
                    GenerateWorld.dummyTransform.forward = -trans.forward;
                    GenerateWorld.RunDummy();
                }
                else if(direction == Direction.Right)
                {
                    trans.Rotate(Vector3.up * 90);
                    GenerateWorld.dummyTransform.forward = -trans.forward;
                    GenerateWorld.RunDummy();
                }
                turnDirection = direction;
                float _shiftDistance = GetPosition(1).InverseTransformPoint(trans.position).z;
                trans.Translate(_shiftDistance, 0, 0);
                controller.currentPosition = 1;
            }
            else
            {
                Shift(direction, controller);
            }
        }

        public override void Shift(Direction direction, RunnerController controller)
        {
            Transform trans = controller.gameObject.transform;
            int currentPosition = controller.currentPosition;
            if(direction == Direction.Left && currentPosition > 0)
            {
                Transform pos = GetPosition(currentPosition - 1);
                if(pos != null)
                {
                    float _shiftDistance = pos.InverseTransformPoint(trans.position).z;
                    trans.Translate(_shiftDistance, 0, 0);
                    controller.currentPosition--;
                }
            }
            else if(direction == Direction.Right && currentPosition < 2)
            {
                Transform pos = GetPosition(currentPosition + 1);
                if(pos != null)
                {
                    float _shiftDistance = pos.InverseTransformPoint(trans.position).z;
                    trans.Translate(_shiftDistance, 0, 0);
                    controller.currentPosition++;
                }
            }
        }
    }
}
