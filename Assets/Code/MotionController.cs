﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Code
{

    [RequireComponent(typeof(BoxCollider2D))]
    class MotionController : RaycastController
    {

        public float maxClimbAngle = 140f;

        Vector3 GetMoveVector(Vector3 normal, Vector3 velocity)
        {
            var g1 = new Vector3(-normal.y, normal.x);
            var g2 = g1 * -1;
            if (Vector3.Dot(velocity, g1) == 0) return Vector3.zero;
            return (Vector3.Dot(velocity, g1) >= 0 ? g1 : g2).normalized * velocity.magnitude;
        }

        float ClampAngle(float angle)
        {
            if (angle <= 180) return angle;
            return 360 - angle;
        }

        float ReflectAngle(float angle)
        {
            if (angle < 90)
            {
                return angle - (angle - 90) * 2;
            }
            return angle + (90 - angle) * 2;
        }

        CollisionInfo Travel(Vector3 velocity, Vector3 down, Vector3 original)
        {
            var hit = BoxCast(boxCollider.bounds, velocity, velocity.magnitude + skinWidth, solidLayer);
            if (hit)
            {
                var travel = velocity.normalized * (Mathf.Max(hit.distance - skinWidth, 0));
                transform.position += travel;
                var rem = velocity - travel;
                var rot = GetMoveVector(hit.normal, rem).normalized;
                if (Vector3.Distance(velocity.normalized, down.normalized) < skinWidth) // moving directly downwards
                {
                    if (ClampAngle(Vector3.Angle(down, rot)) >= ReflectAngle(maxClimbAngle)) // standing on slope steeper than max climb angle
                    {
                        return new CollisionInfo();
                    }
                }
                else if (ClampAngle(Vector3.Angle(down, rot)) > maxClimbAngle && Vector3.Dot(down, original) >= 0) // trying to move up a wall while originally not moving upwards
                {
                    return new CollisionInfo();
                }
                if (rem.magnitude < skinWidth) return new CollisionInfo();
                return Move(rot * Vector3.Dot(rem, rot), down);
            }
            transform.position += velocity;
            return new CollisionInfo();
        }

        public CollisionInfo Move(Vector3 velocity, Vector3 down)
        {
            return Move_Impl(velocity, down, velocity);
        }

        CollisionInfo Move_Impl(Vector3 velocity, Vector3 down, Vector3 original)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                print("boop");
            }
            RaycastHit2D hit;
            hit = BoxCast(boxCollider.bounds, down, skinWidth * 2, solidLayer);
            if (hit && Vector3.Dot(velocity, down) > 0)
            {
                var info = new CollisionInfo { Below = true };
                var move = GetMoveVector(hit.normal, velocity);

                if (ClampAngle(Vector3.Angle(down, move)) > maxClimbAngle) return info;

                if (Vector3.Distance(velocity.normalized, down.normalized) < skinWidth) // we are moving directly downwards
                {
                    if (ClampAngle(Vector3.Angle(down, move)) >= ReflectAngle(maxClimbAngle)) // we aren't standing on a steep slope, slide down
                    {
                        transform.position += velocity.normalized * (hit.distance - skinWidth);
                        return info;
                    }
                    else
                    {
                        info.Below = false;
                    }
                }
                return info.Or(Travel(move, down, original));
            }
            return Travel(velocity, down, original);
        }

    }

    struct CollisionInfo
    {
        public bool Below { get; set; }
        public bool Above { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }

        public CollisionInfo Or(CollisionInfo info)
        {
            return new CollisionInfo()
            {
                Below = info.Below || Below,
                Above = info.Above || Above,
                Left = info.Left || Left,
                Right = info.Right || Right,
            };
        }
    }

}
