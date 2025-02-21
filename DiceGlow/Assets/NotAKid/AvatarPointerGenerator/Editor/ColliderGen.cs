﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using ABI.CCK.Components;

namespace AvatarPointerGenerator
{
    public static class ColliderGen
    {
        private const string APGTag = "[APG] ";
        private static readonly HumanBodyBones[] HandBones =
        {
            HumanBodyBones.LeftIndexProximal,
            HumanBodyBones.LeftMiddleProximal,
            HumanBodyBones.LeftRingProximal,
            HumanBodyBones.LeftLittleProximal
        };

        public static void GenerateDefaultColliders(CVRAvatar avatar)
        {
            Debug.Log(APGTag + "Attempting autogeneration of default colliders.");

            if (avatar == null)
            {
                Debug.LogError(APGTag + "Error! Avatar is null. Autogeneration will not occur.");
                EditorUtility.DisplayDialog("Error", "Avatar is null. Autogeneration will not occur.", "Okay");
                return;
            }

            Animator animator = avatar.GetComponent<Animator>();
            if (animator == null || animator.avatar == null || !animator.avatar.isHuman)
            {
                Debug.LogError(APGTag + "Error! Avatar is not Humanoid rig. Autogeneration will not occur.");
                EditorUtility.DisplayDialog("Error", "Avatar is not Humanoid rig or Animator Avatar field is empty. Autogeneration will not occur.", "Okay");
                return;
            }

            Debug.Log(APGTag + "Success! Avatar is found to be a Humanoid rig.");

            GenerateHeadCollider(animator, avatar);
            GenerateChestCollider(animator);
            GenerateHandCollider(animator, true);
            GenerateHandCollider(animator, false);
            GenerateFootCollider(animator, true);
            GenerateFootCollider(animator, false);
            Undo.FlushUndoRecordObjects();

            Debug.Log(APGTag + "Success! Colliders have been generated.");
        }

        private static void GenerateFootCollider(Animator animator, bool isLeft)
        {
            Transform footTransform = isLeft ? animator.GetBoneTransform(HumanBodyBones.LeftFoot) : animator.GetBoneTransform(HumanBodyBones.RightFoot);
            Transform toesTransform = isLeft ? animator.GetBoneTransform(HumanBodyBones.LeftToes) : animator.GetBoneTransform(HumanBodyBones.RightToes);

            if (toesTransform == null)
            {
                foreach (Transform bone in footTransform)
                {
                    if (bone.name.ToLowerInvariant().IndexOf("toe") != -1 || bone.name.ToLowerInvariant().IndexOf("_end") != -1)
                    {
                        toesTransform = bone;
                        break;
                    }
                }

                if (toesTransform == null)
                {
                    return;
                }
            }

            float totalLength = Vector3.Distance(footTransform.position, toesTransform.position);
            CreatePointerObject(footTransform, isLeft ? "LeftFoot" : "RightFoot", totalLength / 2f, Vector3.zero);
        }

        private static void GenerateHandCollider(Animator animator, bool isLeft)
        {
            HumanBodyBones[] bones = isLeft ? HandBones : HandBones.Select(x => x + (HumanBodyBones.RightIndexProximal - HumanBodyBones.LeftIndexProximal)).ToArray();

            Transform handTransform = isLeft ? animator.GetBoneTransform(HumanBodyBones.LeftHand) : animator.GetBoneTransform(HumanBodyBones.RightHand);

            Transform[] fingerTransforms = bones.Select(bone => animator.GetBoneTransform(bone)).ToArray();
            float totalLength = fingerTransforms.Sum(finger => Vector3.Distance(finger.position, handTransform.position)) / fingerTransforms.Length;
            CreatePointerObject(handTransform, isLeft ? "LeftHand" : "RightHand", totalLength / 2f, Vector3.zero);
        }

        private static void GenerateChestCollider(Animator animator)
        {
            Transform hipsTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
            Transform chestTransform = animator.GetBoneTransform(HumanBodyBones.UpperChest) ?? animator.GetBoneTransform(HumanBodyBones.Chest) ?? animator.GetBoneTransform(HumanBodyBones.Spine);

            float totalLength = 0;
            Vector3[] positions = new Vector3[]
            {
                animator.GetBoneTransform(HumanBodyBones.Neck)?.position ?? Vector3.zero,
                animator.GetBoneTransform(HumanBodyBones.UpperChest)?.position ?? Vector3.zero,
                animator.GetBoneTransform(HumanBodyBones.Chest)?.position ?? Vector3.zero,
                animator.GetBoneTransform(HumanBodyBones.Spine)?.position ?? Vector3.zero
            };

            int totalCount = 0;
            foreach (Vector3 pos in positions)
            {
                if (pos != null && hipsTransform.position.y < pos.y)
                {
                    totalLength += Vector3.Distance(hipsTransform.position, pos);
                    totalCount++;
                }
            }

            float distance = Vector3.Dot(positions[0] - hipsTransform.position, chestTransform.forward);
            Vector3 offset = chestTransform.forward * distance;

            totalLength /= totalCount;
            CreatePointerObject(chestTransform, "Chest", totalLength / 2f, offset);
        }

        private static void GenerateHeadCollider(Animator animator, CVRAvatar avatar)
        {
            Transform headTransform = animator.GetBoneTransform(HumanBodyBones.Head);

            float totalLength = 0;
            Vector3[] positions = new Vector3[]
            {
                animator.GetBoneTransform(HumanBodyBones.LeftEye)?.position ?? Vector3.zero,
                animator.GetBoneTransform(HumanBodyBones.RightEye)?.position ?? Vector3.zero,
                avatar.transform?.TransformPoint(avatar.viewPosition) ?? Vector3.zero,
                avatar.transform?.TransformPoint(avatar.voicePosition) ?? Vector3.zero
            };

            int totalCount = 0;
            foreach (Vector3 pos in positions)
            {
                if (pos != null && headTransform.position.y < pos.y)
                {
                    totalLength += Vector3.Distance(headTransform.position, pos);
                    totalCount++;
                }
            }

            totalLength /= totalCount;
            CreatePointerObject(headTransform, "Head", totalLength, Vector3.zero);
        }

        private static void CreatePointerObject(Transform parent, string name, float radius, Vector3 offset)
        {
            foreach (Transform t in parent)
            {
                CVRPointer pointer = t.GetComponent<CVRPointer>();
                if (pointer != null && pointer.type == name)
                {
                    Debug.Log(APGTag + "Warning! Existing pointer found. Ignoring autogeneration.");
                    return;
                }
            }

            GameObject pointerObject = new GameObject(APGTag + name, typeof(CVRPointer), typeof(SphereCollider));
            Undo.RegisterCreatedObjectUndo(pointerObject, APGTag + "Added pointer object.");

            CVRPointer pointerScript = pointerObject.GetComponent<CVRPointer>();
            SphereCollider collider = pointerObject.GetComponent<SphereCollider>();
            //main transform
            pointerObject.transform.parent = parent;
            pointerObject.transform.localPosition = new Vector3(0f, radius*0.75f, 0f) + offset;
            pointerObject.transform.localRotation = Quaternion.identity;
            pointerObject.transform.localScale = Vector3.one;
            //collider
            collider.radius = radius;
            collider.isTrigger = true;
            //pointer
            pointerScript.type = name;
        }
    }
}